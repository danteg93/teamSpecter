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
  void OnCollisionEnter2D(Collision2D col) {
    if (col.gameObject.GetComponent<PlayerController>()) {
      if (col.gameObject.GetComponent<PlayerController>().IsShieldOn()) {
        reflectFireball(col.contacts[0].normal);
      } else {
        col.transform.gameObject.GetComponent<PlayerController>().Kill();
        DestroyFireball();
      }
    }
    else if (col.gameObject.GetComponent<Cover>()) { // Didnt combine with the first check, to keep it readable.
      if (col.gameObject.GetComponent<Cover>().IsReflecting) {
        reflectFireball(col.contacts[0].normal);
      }
      else if (col.gameObject.GetComponent<Cover>().IsBreakable) {
        col.gameObject.GetComponent<Cover>().Break();
        DestroyFireball();
      }
    }
    else if (col.gameObject.GetComponent<ShootFireball>()) { //Refkect fireballs if they collide.
      reflectFireball(col.contacts[0].normal);
    }
  }
  // Instantiate the bullet prefab.
  public override GameObject Cast(PlayerController player) {
    return Instantiate(gameObject, player.transform.position + (-player.transform.up * 0.7f), player.transform.rotation) as GameObject;
  }
  //this is here for the future, when there are aniamtions and other stuff
  public void DestroyFireball() {
    Destroy(gameObject);
  }
  //made this into a class since it gets used a lot
  private void reflectFireball() {
    GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity * -1;
  }
  //This function will take a normal and revert collision based on that normal
  private void reflectFireball(Vector2 collidingSurfaceNormal) {
    GetComponent<Rigidbody2D>().velocity = Vector2.Reflect(GetComponent<Rigidbody2D>().velocity, collidingSurfaceNormal);
  }
}
