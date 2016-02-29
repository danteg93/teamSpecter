using UnityEngine;

public class ShootFireball : AbstractAbility {

  public float TimeToLive;
  public float Speed;
  public int playerLastInteracted = -1;

  private float timeSpawned;
  // On spawn, launch the fireball away from the player.
  void Start() {
    GetComponent<Rigidbody2D>().AddForce(transform.up * -Speed);
    timeSpawned = Time.time;
    playAudioCast();
  }

  // Decrease the time to live every frame for the bullet and
  // destroy it if it's time to live runs out.
  void Update() {
    if (TimeToLive <= 0) { DestroyProjectile(); }
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
        playerLastInteracted = col.gameObject.GetComponent<PlayerController>().PlayerNumber;
        col.rigidbody.AddForce(-GetComponent<Rigidbody2D>().velocity.normalized * -1500);
        reflectFireball(col.contacts[0].normal);
      } else {
        col.gameObject.GetComponent<PlayerController>().Kill(playerLastInteracted);
        DestroyProjectile();
      }
    } else if (col.gameObject.GetComponent<Cover>()) {
      if (col.gameObject.GetComponent<Cover>().IsReflecting) {
        reflectFireball(col.contacts[0].normal);
      } else if (col.gameObject.GetComponent<Cover>().IsBreakable) {
        col.gameObject.GetComponent<Cover>().Break();
        DestroyProjectile();
      }
    } else if (col.gameObject.GetComponent<ShootFireball>()) { //Reflect fireballs if they collide.
      int temp = col.gameObject.GetComponent<ShootFireball>().playerLastInteracted;
      reflectFireball(col.contacts[0].normal);
      //Here so that there are no weird conflicts when fireballs hit at the same time lol
      playerLastInteracted = temp;
    }
  }

  // Instantiate the bullet prefab.
  public override GameObject Cast(PlayerController player) {
    GameObject fireBall = Instantiate(gameObject, player.transform.position + (-player.transform.up * 0.7f), player.transform.rotation) as GameObject;
    fireBall.GetComponent<ShootFireball>().playerLastInteracted = player.PlayerNumber;
    return fireBall;
  }

  //this is here for the future, when there are aniamtions and other stuff
  public void DestroyProjectile() {
    Destroy(gameObject);
  }

  //This function will take a normal and revert collision based on that normal
  private void reflectFireball(Vector2 collidingSurfaceNormal) {
    GetComponent<Rigidbody2D>().velocity = Vector2.Reflect(GetComponent<Rigidbody2D>().velocity, collidingSurfaceNormal);
  }

  private void playAudioCast() {
    AudioClip[] castSounds = Resources.LoadAll<AudioClip>("Audio/SFX/Fireball/Cast");
    int castIndex = Random.Range(0, castSounds.Length);
    GetComponent<AudioSource>().PlayOneShot(castSounds[castIndex], 0.5f);
  }
}
