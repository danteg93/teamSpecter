using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  // Use this for initialization
  public bool UseKeyboardControl = false;
  public int PlayerNumber = 0;
  public float ProjectileCooldown = 1.0f;
  public float Speed = 0f;
  public GameObject projectile;
  public float AccelerationFactor = 0.55f;
  
  private bool isPressingAttack = false;
  private bool isBlocking = false;
  private float projectileCooldownTimer;
  private float previousVelocityMagnitude;

  void Start() {
    projectileCooldownTimer = ProjectileCooldown;
    previousVelocityMagnitude = 0.0f;
  }

  // Update is called once per frame
  void Update() {
    processShootBullet();
    processBlockInput();
  }

  void FixedUpdate() {
    //Execute movement of the player. Code was way too similar, with the exception of one variable
    //so I left it as one function :P
    executeMovement();
    // If the player is using the keyboard and mouse, execute that movement. Otherwise,
    // find the appropriate joystick for the player
    if (UseKeyboardControl) {
      executeMouseRotation();
    }
    else {
      executeJoyStickRotation();
    }
  }

  public bool IsBlocking() {
    return isBlocking;
  }

  public void Kill() {
    Cameraman.cameraman.CameraShake(0.5f, 0.1f);
    Destroy(this.gameObject);
  }

  private void executeMovement() {
    //Vector calculated from the input, it gives a direction of where to move next.
    Vector2 newVelocity;
    if (UseKeyboardControl) {
      //Getting the axis raw (lawl) stops unity from smoothing out key presses (reason why we had sliding with keyboard and not with sticks)
      newVelocity = new Vector2(Input.GetAxisRaw("HorizontalMovementK") * Speed, Input.GetAxisRaw("VerticalMovementK") * Speed);
    }
    else {
      newVelocity = new Vector2(Input.GetAxis("HorizontalMovementJ" + PlayerNumber) * Speed, -Input.GetAxis("VerticalMovementJ" + PlayerNumber) * Speed);
    }
    //if the direction of the movement has changed and you are not static then you will want to "deaccelerate"
    if (newVelocity.magnitude - previousVelocityMagnitude < 0.0f && GetComponent<Rigidbody2D>().velocity.magnitude != 0.0f) {
      newVelocity = Vector2.zero;
    }
    //change the direction towards the direction of the new velocity
    GetComponent<Rigidbody2D>().velocity = Vector2.MoveTowards(GetComponent<Rigidbody2D>().velocity, newVelocity, AccelerationFactor);
    //set previous velocity for delta calculations;
    previousVelocityMagnitude = GetComponent<Rigidbody2D>().velocity.magnitude;
  }

  private void executeMouseRotation() {
    //Vector3 from the object to the mouse
    Vector3 objectToMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    objectToMouse.Normalize();
    //Good old arctan2 lmao
    float angleToMouse = (Mathf.Atan2(objectToMouse.y, objectToMouse.x) * Mathf.Rad2Deg) + 90.0f; //The 90 assumes that the front of the sprite is the bottom part.  
    //Do you even quaternion bro?
    transform.rotation = Quaternion.Euler(0f, 0f, angleToMouse);
  }

  private void executeJoyStickRotation() {
    //Vector2 that says where the joystick is
    Vector2 joyStickLocation = new Vector2(Input.GetAxis("HorizontalRotationJ" + PlayerNumber), Input.GetAxis("VerticalRotationJ" + PlayerNumber));
    if (joyStickLocation.magnitude != 0) { //this wont reset your rotation if you randomly let go off the controller
      joyStickLocation.Normalize();
      float angleToStick = (Mathf.Atan2(joyStickLocation.x, joyStickLocation.y) * Mathf.Rad2Deg);
      transform.rotation = Quaternion.Euler(0f, 0f, angleToStick);
    }
  }

  // Removed until our first character is complete.
  private void processSlashInput() {
    float slashInput;
    if (UseKeyboardControl) {
      slashInput = Input.GetAxis("SlashK");
    } else {
      slashInput = Input.GetAxis("SlashJ" + PlayerNumber);
    }
 
    // Perform an attack if the player is pressing an attack key
    if (slashInput != 0) {
      // Prevent the attack from occurring multiple times in one key press and
      // the player from using multiple abilities at once (i.e. attack & block)
      if (!isPressingAttack) {
        isPressingAttack = true;

        // Find all objects in range of the player, and push other player objects away
        // TODO: This needs to only select things in front of the player
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1);
        int i = 0;
        while (i < hitColliders.Length) {
          if (hitColliders[i].GetComponent<PlayerController>() != null) {
            PlayerController player = hitColliders[i].GetComponent<PlayerController>();
            if (player != this && !player.IsBlocking()) {
              player.Kill();
            }
          }
          i++;
        }
      }
    } else if (slashInput == 0) {
      isPressingAttack = false;
    }
  }

  private void processBlockInput() {
    // Check if a player is trying to block
    if (UseKeyboardControl && Input.GetAxis("BlockK") != 0 || !UseKeyboardControl && Input.GetAxis("BlockJ" + PlayerNumber) != 0) {
      isBlocking = true;
    } else if (UseKeyboardControl && Input.GetAxis("BlockK") == 0 || !UseKeyboardControl && Input.GetAxis("BlockJ" + PlayerNumber) != 0) {
      isBlocking = false;
    }
  }

  private void processShootBullet() {
    //If player is using keyboard controls then listen for keyboard press
    //else, listen for joystick press
    //also, projectileCooldown needs to be reset
    if (((!UseKeyboardControl && Input.GetAxis("ShootProjectileJ" + PlayerNumber) != 0) || (UseKeyboardControl && Input.GetAxis("ShootProjectileK") != 0)) && projectileCooldownTimer == ProjectileCooldown) {
      ProjectileController temp = ((GameObject)Instantiate(projectile, transform.position, transform.rotation)).GetComponent<ProjectileController>();
      temp.SetOwnerAndShoot(transform.gameObject.name);
      projectileCooldownTimer -= Time.deltaTime;
    }
    else if (projectileCooldownTimer < ProjectileCooldown && projectileCooldownTimer > 0) {
      projectileCooldownTimer -= Time.deltaTime;
    }
    else if (projectileCooldownTimer <= 0) {
      projectileCooldownTimer = ProjectileCooldown;
    }
  }
}
