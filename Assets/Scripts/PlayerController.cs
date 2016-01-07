using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  // Use this for initialization
  public float Speed = 0f;
  public float playerNumber = 1;

  private int timesDead = 0;
  private Vector3 initialPosition;

  void Start() {
    timesDead = 0;
    initialPosition = gameObject.transform.position;
  }

  // Update is called once per frame
  void FixedUpdate() {
    GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, Input.GetAxis("Vertical") * Speed);
  }

  public void reSpawn() {
    timesDead++;
    gameObject.transform.position = initialPosition;
    Debug.Log(timesDead);
  }
}
