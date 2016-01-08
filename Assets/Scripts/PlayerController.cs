using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  // Use this for initialization
  public float PlayerNumber = 1;
  public float Speed = 0f;
  public float AttackPower = 0f;

  private int timesDead = 0;
  private Vector3 initialPosition;
  private bool isAttacking = false;
  private bool isBlocking = false;

  void Start() {
    timesDead = 0;
    initialPosition = gameObject.transform.position;
  }

  // Update is called once per frame
  void FixedUpdate() {
    // Update movement if the player is pressing any movement key
    GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, Input.GetAxis("Vertical") * Speed);

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
    Debug.Log(timesDead);
  }
}
