using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  // Use this for initialization
  public bool KeyboardControl = true;
  public int PlayerNumber;
  public float Speed = 0f;

  private int timesDead = 0;
  private Vector3 initialPosition;
  private bool isPressingAttack = false;
  private bool isBlocking = false;

  void Start() {
    timesDead = 0;
    initialPosition = gameObject.transform.position;
  }

  // Update is called once per frame
  void FixedUpdate() { 
    //if player is 1 the execute the joystick movement else do keyboard and mouse
    if (KeyboardControl) {
      GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("HorizontalMovementK") * Speed, Input.GetAxis("VerticalMovementK") * Speed);
      executeMouseRotation();
    }
    else {
      GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("HorizontalMovementJ1") * Speed, -Input.GetAxis("VerticalMovementJ1") * Speed);
      executeJoyStickRotation();
    }

    processAttackInput();
    processBlockInput();
  }

  public bool IsBlocking() {
    return isBlocking;
  }

  public void Kill() {
    timesDead++;
    transform.position = initialPosition;
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
    //Vector3 that says where the joystick is
    Vector3 joyStickLocation = new Vector3(Input.GetAxis("HorizontalRotationJ1"), Input.GetAxis("VerticalRotationJ1"));
    if (joyStickLocation.magnitude != 0) { //this wont reset your rotation if you randomly let go off the controller
      joyStickLocation.Normalize();
      float angleToStick = (Mathf.Atan2(joyStickLocation.x, joyStickLocation.y) * Mathf.Rad2Deg);
      transform.rotation = Quaternion.Euler(0f, 0f, angleToStick);
    }
  }

  private void processAttackInput() {
    // Perform an attack if the player is pressing an attack key
    if (Input.GetAxis("Slash") != 0) {
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
    } else if (Input.GetAxis("Slash") == 0) {
      isPressingAttack = false;
    }
  }

  private void processBlockInput() {
    // Check if a player is trying to block
    if (Input.GetAxis("Block") != 0) {
      isBlocking = true;
      print("blocking");
    } else if (Input.GetAxis("Block") == 0) {
      isBlocking = false;
    }
  }
}
