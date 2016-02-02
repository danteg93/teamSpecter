using UnityEngine;
using System.Collections;

public class ShrinkingArena : MonoBehaviour {

  public int StartTime;
  public int Interval;

  private float targetScale;
  private bool shrinking;

  void Update() {
    if (transform.localScale.x <= 1.5) { return; }

    if (!shrinking && (int)Time.time >= StartTime && (int)Time.time % Interval == 0) {
      targetScale = transform.localScale.x - 0.2f;
      shrinking = true;
    }

    if (shrinking) {
	  transform.localScale -= new Vector3(0.01f, 0.01f, 1);
      // Cameraman.cameraman.shrinkCamera(0.8F, 1);
	}

    if (targetScale <= transform.localScale.x) { shrinking = false; }
  }

  void OnTriggerExit2D(Collider2D coll) {
    print(coll.tag);
    if (coll.tag == "Player") {
      coll.transform.GetComponent<PlayerController>().Kill();
    } else if (coll.tag == "Cover") {
      print("here");
      Destroy(coll);
    }
  }
}
