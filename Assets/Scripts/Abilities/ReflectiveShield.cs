using System.Collections;
using UnityEngine;

public class ReflectiveShield : AbstractAbility {

  public float ShieldActiveTime;

  // Sounds related to this object.
  public AudioClip ShieldActivateSound;
  public AudioClip ShieldLoopSound;
  public AudioClip ShieldDeactivateSound;

  private PlayerController player;
  private bool destroying = false;
  private AudioSource audioSource;

  void Awake() {
    audioSource = GetComponent<AudioSource>();
    StartCoroutine(castAnimation());
  }

  // Decrease the shield's active time and remove it if either
  // the time is out or the player goes away (potentially from
  // dieing).
  void Update() {
    if (ShieldActiveTime <= 0 || !player) { Uncast(); }
    ShieldActiveTime -= Time.deltaTime;
    if (player) { transform.position = player.transform.position; }
  }

  // Instantiate a new shield prefab and set the player.
  public override GameObject Cast(PlayerController player) {
    GameObject shield = Instantiate(gameObject, player.transform.position, player.transform.rotation) as GameObject;
    shield.GetComponent<ReflectiveShield>().player = player;
    return shield;
  }

  IEnumerator castAnimation() {
    audioSource.PlayOneShot(ShieldActivateSound);
    yield return new WaitForSeconds(0.4f);
    audioSource.loop = true;
    audioSource.clip = ShieldLoopSound;
    audioSource.Play();
  }

  // Destroy the shield if the player cancels it.
  public override void Uncast() {
    if (destroying) { return; }
    destroying = true;
    StartCoroutine(destroyAnimation());
  }

  IEnumerator destroyAnimation() {
    audioSource.Stop();
    audioSource.PlayOneShot(ShieldDeactivateSound, 0.5f);
    yield return new WaitForSeconds(0.5f);
    if (player) { player.ShieldDestroyed(); }
    Destroy(gameObject);
  }
}
