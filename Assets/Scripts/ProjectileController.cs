using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

  private float selfDestroyTimer = 1;
  private float bulletSpeed = 10.0f;
  private Rigidbody2D bulletBody;
  private string bulletOwner;
  private bool bulletFiring = false;
  // Use this for initialization
  void Start() {
    bulletBody = GetComponent<Rigidbody2D>();
  }

  // Update is called once per frame
  void Update() {
    if (bulletFiring) {
      executeBulletFire();
    }
  }

  void OnTriggerEnter2D(Collider2D col) {
    if (col.gameObject.tag == "Player" && col.gameObject.name != bulletOwner && bulletFiring) {
      col.transform.gameObject.GetComponent<PlayerController>().Kill(); //call player kill function
      Destroy(gameObject);
    }
  }

  public void SetOwnerAndShoot(string owner) {
    bulletOwner = owner;
    bulletFiring = true;
  }

  private void executeBulletFire() {
    bulletBody.AddForce(transform.up * -bulletSpeed);
    selfDestroyTimer -= Time.deltaTime;

    if (selfDestroyTimer <= 0) {
      Destroy(gameObject);
    }
  }

}
