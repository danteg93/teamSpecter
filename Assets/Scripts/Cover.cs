using UnityEngine;
using System.Collections;

public class Cover : MonoBehaviour {

  public bool IsBreakable = false;

  void OnTriggerEnter2D(Collider2D col) {
    if (IsBreakable && col.GetComponent<ShootFireball>()) {
      Destroy(col.gameObject);
      Destroy(gameObject);
    }
  }

  public void Break() {
    if (IsBreakable) { Destroy(gameObject); }
  }

}
