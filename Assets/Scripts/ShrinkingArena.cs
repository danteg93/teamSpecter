using UnityEngine;
using System.Collections;

public class ShrinkingArena : MonoBehaviour {

  public int ShrinkStartTime;
  public int Interval;

  private float targetScale;
	private bool shrinking = false;
	private int sceneStartingTime;

	void Start(){
		sceneStartingTime = (int)Time.time;
    targetScale = transform.localScale.x - 0.2f;
	}
  void Update() {
		if (transform.localScale.x <= 1.5 || ShrinkStartTime == 0) { return; }

		if (!shrinking && (int)Time.time >= (ShrinkStartTime + sceneStartingTime) && (int)Time.time % Interval == 0) {
      shrinking = true;
    }

    if (shrinking) {
	    transform.localScale -= new Vector3(0.01f, 0.01f, 1);
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
