using System.Collections;
using UnityEngine;

public class ShootFireball : AbstractAbility {

  public float TimeToLive;
  public float Speed;
  public int PlayerLastInteracted = -1;

  //Smoke Particle
  public GameObject smokeParticle;

  // Sounds related to this object.
  public AudioClip FireballCastSound1;
  public AudioClip FireballCastSound2;
  public AudioClip FireballCastSound3;
  public AudioClip FireballCastSound4;
  public AudioClip FireballBounceSound1;
  public AudioClip FireballBounceSound2;
  public AudioClip FireballBounceSound3;
  public AudioClip FireballBounceSound4;
  public AudioClip FireballDissipateSound;

  private float timeSpawned;
  private bool destroying = false;
  private AudioSource audioSource;

  void Awake() {
    audioSource = GetComponent<AudioSource>();
  }

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
        PlayerLastInteracted = col.gameObject.GetComponent<PlayerController>().PlayerNumber;
        col.rigidbody.AddForce(-GetComponent<Rigidbody2D>().velocity.normalized * -1500);
        reflectFireball(col.contacts[0].normal);
      } else {
        col.gameObject.GetComponent<PlayerController>().Kill(PlayerLastInteracted);
        DestroyProjectileInstant();
      }
    } else if (col.gameObject.GetComponent<Cover>()) {
      if (col.gameObject.GetComponent<Cover>().IsReflecting) {
        reflectFireball(col.contacts[0].normal);
      } else if (col.gameObject.GetComponent<Cover>().IsBreakable) {
        col.gameObject.GetComponent<Cover>().Break();
        DestroyProjectile();
      }
    } else if (col.gameObject.GetComponent<ShootFireball>()) { //Reflect fireballs if they collide.
      int temp = col.gameObject.GetComponent<ShootFireball>().PlayerLastInteracted;
      reflectFireball(col.contacts[0].normal);
      //Here so that there are no weird conflicts when fireballs hit at the same time lol
      PlayerLastInteracted = temp;
    }
  }

  // Instantiate the bullet prefab.
  public override GameObject Cast(PlayerController player) {
    // adjust spawn distance based on ball size
    GameObject fireBall = Instantiate(gameObject, player.transform.position + (-player.transform.up * (0.7f + (PlayerController.ballSize)*2)), player.transform.rotation) as GameObject;
    fireBall.GetComponent<ShootFireball>().PlayerLastInteracted = player.PlayerNumber;
    return fireBall;
  }

  //this is here for the future, when there are aniamtions and other stuff
  public void DestroyProjectile() {
    if (destroying) { return; }
    destroying = true;
    StartCoroutine(destroyAnimation(false));
  }
  //Added it like this because not sure how many external things use DestroyProjectile
  public void DestroyProjectileInstant() {
    if (destroying) { return; }
    destroying = true;
    StartCoroutine(destroyAnimation(true));
  }

  IEnumerator destroyAnimation(bool instantDestroy) {
    if(instantDestroy){
      Destroy(gameObject);
      //Have to return something for IEnumerator
      return true;
    }
    audioSource.PlayOneShot(FireballDissipateSound, 0.5f);
    //Smoke stuff
    GameObject tempSmoke = Instantiate(smokeParticle, transform.position, transform.rotation) as GameObject;
    tempSmoke.transform.parent = transform;
    yield return new WaitForSeconds(0.3f);
    Destroy(gameObject);
  }

  //This function will take a normal and revert collision based on that normal
  private void reflectFireball(Vector2 collidingSurfaceNormal) {
    GetComponent<Rigidbody2D>().velocity = Vector2.Reflect(GetComponent<Rigidbody2D>().velocity, collidingSurfaceNormal);
    playAudioBounce();
  }

  private void playAudioCast() {
    AudioClip[] castSounds = { FireballCastSound1, FireballCastSound2, FireballCastSound3, FireballCastSound4 };
    int castIndex = Random.Range(0, castSounds.Length);
    audioSource.PlayOneShot(castSounds[castIndex], 0.5f);
  }

  private void playAudioBounce() {
    AudioClip[] bounceSounds = { FireballBounceSound1, FireballBounceSound2, FireballBounceSound3, FireballBounceSound4 };
    int bounceIndex = Random.Range(0, bounceSounds.Length);
    audioSource.PlayOneShot(bounceSounds[bounceIndex], 0.5f);
  }
}
