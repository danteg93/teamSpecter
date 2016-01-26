using UnityEngine;

public class ReflectiveShield : AbstractAbility {

  public float ShieldActiveTime;

  private PlayerController player;

  // Decrease the shield's active time and remove it if either
  // the time is out or the player goes away (potentially from
  // dieing).
  void Update() {
    if (ShieldActiveTime <= 0 || !player) {
      Destroy(gameObject);
      player.SetShieldOn(false);
    }
    ShieldActiveTime -= Time.deltaTime;
    if (player) { transform.position = player.transform.position; }
  }

  // Instantiate a new shield prefab and set the player.
  public override void Cast(PlayerController player) {
    GameObject shield = Instantiate(gameObject, player.transform.position, player.transform.rotation) as GameObject;
    shield.GetComponent<ReflectiveShield>().player = player;
    player.SetShieldOn(true);
  }
}
