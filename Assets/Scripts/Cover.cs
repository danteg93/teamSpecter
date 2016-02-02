using UnityEngine;
using System.Collections;

public class Cover : MonoBehaviour {

  public bool IsBreakable = false;
  public bool IsReflecting = false;

  public void Break() {
    if (IsBreakable) { Destroy(gameObject); }
  }
}
