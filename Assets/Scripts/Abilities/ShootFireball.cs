using UnityEngine;

public class ShootFireball : AbstractAbility {

  public float TimeToLive;
  public float Speed;

  private float timeSpawned;

  void Start() {
    GetComponent<Rigidbody2D>().AddForce(transform.up * -Speed);
    timeSpawned = Time.time;
  }

  // Decrease the time to live every frame for the bullet and
  // destroy it if it's time to live runs out.
  void Update() {
    if (TimeToLive <= 0) { Destroy(gameObject); }
    TimeToLive -= Time.deltaTime;
    //This makes it so that if the fireball slows down enough then it blows up.
    //Also, the Time.time > timeSpawned + 0.1f makes it so that the fireball doesnt blow up 
    //before it has a chance to catch some speed. 
    if (Time.time > timeSpawned + 0.1f && GetComponent<Rigidbody2D>().velocity.magnitude < 8.0f) {
      DestroyProjectile();
    }
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
        DestroyProjectile();
      }
    }
    else if (col.gameObject.GetComponent<Cover>()) { // Didnt combine with the first check, to keep it readable.
      if (col.gameObject.GetComponent<Cover>().IsReflecting) {
        reflectFireball(col.contacts[0].normal);
      }
      else if (col.gameObject.GetComponent<Cover>().IsBreakable) {
        col.gameObject.GetComponent<Cover>().Break();
        DestroyProjectile();
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
  public void DestroyProjectile() {
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
