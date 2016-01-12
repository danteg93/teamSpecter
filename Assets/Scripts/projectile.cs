using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

  private float selfDestroyTimer = 1;
  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    Vector3 position = this.transform.position; // testing purposes only
    position.y--;						// testing purposes only
    this.transform.position = position;

    selfDestroyTimer -= Time.deltaTime;

    if (selfDestroyTimer <= 0) {
      Destroy(gameObject);
    }
  }

  void OnCollisionEnter2D(Collision2D col) {
    if (col.gameObject.tag == "Player") {
      Debug.Log("destroyed player");
      col.transform.gameObject.GetComponent<PlayerController>().Kill(); //call player kill function
      Destroy(gameObject);
    }
  }


}
