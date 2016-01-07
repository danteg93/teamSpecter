using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  // Use this for initialization
  public float PlayerNumber = 1;
  public float Speed = 0f;
  public float AttackPower = 0f;

  private int timesDead = 0;
  private Vector3 initialPosition;
  private bool m_isAttacking = false;
  private bool m_isBlocking = false;

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
      if (!m_isAttacking) {
        m_isAttacking = true;

        // Find all objects in range of the player, and push other player objects away
        // TODO: This needs to only select things in front of the player
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1);
        int i = 0;
        while (i < hitColliders.Length) {
          if (hitColliders[i].gameObject.GetComponent<PlayerController>() != null) {
            PlayerController player = hitColliders[i].gameObject.GetComponent<PlayerController>();
            if (player != this && !player.isBlocking()) {
              player.GetComponent<Rigidbody2D>().AddForce(transform.right * AttackPower);
            }
          }
          i++;
        }
      }
    } else if (Input.GetAxis("Attack") == 0) {
      m_isAttacking = false;
    }

    // Check if a player is trying to block
    if (Input.GetAxis("Block") != 0) {
      m_isBlocking = true;
    } else if (Input.GetAxis("Block") != 0) {
      m_isBlocking = false;
    }
  }

  public bool isBlocking() {
    return m_isBlocking;
  }

  public void kill() {
    timesDead++;
    transform.position = initialPosition;
    Debug.Log(timesDead);
  }
}
