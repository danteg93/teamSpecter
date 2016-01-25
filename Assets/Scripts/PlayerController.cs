using UnityEngine;
using System.Collections;

//TODO dash CD

public class PlayerController : MonoBehaviour {

  // Use this for initialization
  public bool UseKeyboardControl = false;
  public int PlayerNumber = 0;
  public float ProjectileCooldown = 1.0f;
  public float DashCooldown = 1.0f;
  public float Speed = 0f;
  public GameObject projectile;
  public float AccelerationFactor = 0.55f;
  public float DashPower = 15.0f;
  
  private bool isPressingAttack = false;
  private bool isPressingDash = false;
  private bool isBlocking = false;
  private float projectileCooldownTimer;
  private float dashCooldownTimer;
  private Vector2 previousVelocity = Vector2.zero; //changed to vector, figured its more valuable than just the magnitude
  private Vector2 movementDirection = Vector2.zero;

  void Start() {
    projectileCooldownTimer = ProjectileCooldown;
    dashCooldownTimer = DashCooldown;
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
    //According to unity docs, all rigidbody calculations should happen on FixedUpdate o.o
    executeDash();
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
    if (newVelocity.magnitude - previousVelocity.magnitude < 0.0f && GetComponent<Rigidbody2D>().velocity.magnitude != 0.0f) {
      newVelocity = Vector2.zero;
    }
    //change the direction towards the direction of the new velocity
    GetComponent<Rigidbody2D>().velocity = Vector2.MoveTowards(GetComponent<Rigidbody2D>().velocity, newVelocity, AccelerationFactor);
    //set previous velocity for delta calculations;
    previousVelocity = GetComponent<Rigidbody2D>().velocity;
    //kept newVeloctiy because these methods are asynced, so other functions that might use movementDirection
    //could potentially access it while being updated. 
    movementDirection = newVelocity.normalized;
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

  private void executeDash() {
    Vector2 dashVector;
    //get the value for the dash input
    float dashInput;
    if (UseKeyboardControl) {
      dashInput = Input.GetAxis("DashK");
    }
    else {
      dashInput = Input.GetAxis("DashJ" + PlayerNumber);
    }
    //makes sure that this doesnt get called a million times
    if (dashInput != 0 && !isPressingDash && dashCooldownTimer == DashCooldown) {
      isPressingDash = true;
      dashCooldownTimer -= Time.deltaTime;
      //movement Direction gets calculated in the executeMovement function.
      //If you are moving, the dash will take you in that direction.
      if (movementDirection.magnitude != 0.0f) {
        //Set the vector of the dash to be the movementDirection (normalized by trig) times the dash factor
        dashVector = movementDirection * DashPower;
      }
      //If you are not moving, the dash will take you in the direction you are facing
      else {
        //Get the adjusted rotation (in rads) of the current rotation of the sprite.
        float adjustedRotationRadians = (transform.rotation.eulerAngles.z - 90.0f) * Mathf.Deg2Rad;
        //Using the magic of trig, calculate a vector (comes out normalized) that will move you in the direction
        //of the angle you are facing. Do you even unit circle bro?
        Vector2 facingDirection = new Vector2(Mathf.Cos(adjustedRotationRadians), Mathf.Sin(adjustedRotationRadians));
        //Multiply it by the dash factor
        dashVector = facingDirection * DashPower;
      }
      //Set the veloctiy
      GetComponent<Rigidbody2D>().velocity += dashVector;
    }
    else if(dashInput == 0) {
      isPressingDash = false;
    }
    //Decrease the timer
    if (dashCooldownTimer < DashCooldown && dashCooldownTimer > 0) {
      dashCooldownTimer -= Time.deltaTime;
    }
    else if (dashCooldownTimer <= 0) {
      dashCooldownTimer = DashCooldown;
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
