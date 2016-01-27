using UnityEngine;

public abstract class AbstractAbility : MonoBehaviour {

  // All abilities must specify some length of time for
  // a cooldown. The default is 0, or no cooldown.
  public float Cooldown = 0;

  // Abilities can be "cast" by a player. This should
  // initialize prefabs or perform actions based on
  // the ability.
  public virtual GameObject Cast(PlayerController player) {
    Debug.Log("Seems you fogot to define Cast(PlayerController player) in your ability!");
    return new GameObject();
  }

  // Some abilities have a second effect when their
  // activate button is pressed again.
  public virtual void Uncast() {
    Debug.Log("Seems you forgot to define Uncast(PlayerController player) in your ability!");
  }
}
