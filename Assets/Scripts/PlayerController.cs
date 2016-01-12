using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  // Use this for initialization
  public bool UseKeyboardControl = false;
  public int PlayerNumber = 0;
  public float ProjectileCooldown = 1.0f;
  public float Speed = 0f;
  public GameObject projectile;
  
  private bool isPressingAttack = false;
  private bool isBlocking = false;
  private float projectileCooldown;

  void Start() {
    projectileCooldown = ProjectileCooldown;
  }

  // Update is called once per frame
  void Update() {
    processShootBullet();
    processSlashInput();
    processBlockInput();
  }

  void FixedUpdate() {
    // If the player is using the keyboard and mouse, execute that movement. Otherwise,
    // find the appropriate joystick for the player
    if (UseKeyboardControl) {
      GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("HorizontalMovementK") * Speed, Input.GetAxis("VerticalMovementK") * Speed);
      executeMouseRotation();
    }
    else {
      GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("HorizontalMovementJ" + PlayerNumber) * Speed, -Input.GetAxis("VerticalMovementJ" + PlayerNumber) * Speed);
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

  private void processSlashInput() {
    float slashInput;
    if (UseKeyboardControl) {
      slashInput = Input.GetAxis("Slash");
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
    if (Input.GetAxis("Block") != 0) {
      isBlocking = true;
    } else if (Input.GetAxis("Block") == 0) {
      isBlocking = false;
    }
  }

  private void processShootBullet() {
    if (!UseKeyboardControl && Input.GetAxis("ShootProjectileJ" + PlayerNumber) != 0 && projectileCooldown == ProjectileCooldown) {
      ProjectileController temp = ((GameObject)Instantiate(projectile, transform.position, transform.rotation)).GetComponent<ProjectileController>();
      temp.SetOwnerAndShoot(transform.gameObject.name);
      projectileCooldown -= Time.deltaTime;
    }
    else if (UseKeyboardControl && Input.GetAxis("ShootProjectileK") != 0 && projectileCooldown == ProjectileCooldown) {
      ProjectileController temp = ((GameObject)Instantiate(projectile, transform.position, transform.rotation)).GetComponent<ProjectileController>();
      temp.SetOwnerAndShoot(transform.gameObject.name);
      projectileCooldown -= Time.deltaTime;
    }
    else if (projectileCooldown < ProjectileCooldown && projectileCooldown > 0) {
      projectileCooldown -= Time.deltaTime;
    }
    else if(projectileCooldown <= 0){
      projectileCooldown = ProjectileCooldown;
    }
  }
}
