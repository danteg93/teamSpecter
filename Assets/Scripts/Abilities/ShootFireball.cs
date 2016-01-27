using UnityEngine;

public class ShootFireball : AbstractAbility {

  public float TimeToLive;
  public float Speed;

  void Start() {
    GetComponent<Rigidbody2D>().AddForce(transform.up * -Speed);
  }

  // Decrease the time to live every frame for the bullet and
  // destroy it if it's time to live runs out.
  void Update() {
    if (TimeToLive <= 0) { Destroy(gameObject); }
    TimeToLive -= Time.deltaTime;
  }

  // On collision with an object
  // Kill players that are not currently blocking, or bounce
  // in the opposite direction if they are.
  void OnTriggerEnter2D(Collider2D col) {
    if (col.GetComponent<PlayerController>()) {
      if (col.gameObject.GetComponent<PlayerController>().IsShieldOn()) {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        Speed = -Speed;
        GetComponent<Rigidbody2D>().AddForce(transform.up * -Speed);
      } else {
        col.transform.gameObject.GetComponent<PlayerController>().Kill();
        Destroy(gameObject);
      }
    }
  }

  // Instantiate the bullet prefab.
  public override GameObject Cast(PlayerController player) {
    return Instantiate(gameObject, player.transform.position + (-player.transform.up * 1), player.transform.rotation) as GameObject;
  }
}
