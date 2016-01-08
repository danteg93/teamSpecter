﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
  // Use this for initialization
  public bool UseKeyboardControl = true;
  public int PlayerNumber = 0;
  public float Speed = 0f;
  public float AttackPower = 0f;

  private int timesDead = 0;
  private Vector3 initialPosition;
  private bool isAttacking = false;
  private bool isBlocking = false;

  void Start() {
    initialPosition = gameObject.transform.position;
  }

  // Update is called once per frame
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

    // Perform an attack if the player is pressing an attack key
    if (Input.GetAxis("Attack") != 0) {
      // Prevent the attack from occurring multiple times in one key press and
      // the player from using multiple abilities at once (i.e. attack & block)
      if (!isAttacking) {
        isAttacking = true;

        // Find all objects in range of the player, and push other player objects away
        // TODO: This needs to only select things in front of the player
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1);
        int i = 0;
        while (i < hitColliders.Length) {
          if (hitColliders[i].GetComponent<PlayerController>() != null) {
            PlayerController player = hitColliders[i].GetComponent<PlayerController>();
            if (player != this && !player.IsBlocking()) {
              player.GetComponent<Rigidbody2D>().AddForce(transform.right * AttackPower);
            }
          }
          i++;
        }
      }
    } else if (Input.GetAxis("Attack") == 0) {
      isAttacking = false;
    }

    // Check if a player is trying to block
    if (Input.GetAxis("Block") != 0) {
      isBlocking = true;
    } else if (Input.GetAxis("Block") != 0) {
      isBlocking = false;
    }

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
    //Vector2 that says where the joystick is
    Vector2 joyStickLocation = new Vector2(Input.GetAxis("HorizontalRotationJ" + PlayerNumber), Input.GetAxis("VerticalRotationJ" + PlayerNumber));
    if (joyStickLocation.magnitude != 0) { //this wont reset your rotation if you randomly let go off the controller
      joyStickLocation.Normalize();
      float angleToStick = (Mathf.Atan2(joyStickLocation.x, joyStickLocation.y) * Mathf.Rad2Deg);
      transform.rotation = Quaternion.Euler(0f, 0f, angleToStick);
    }
  }
}
