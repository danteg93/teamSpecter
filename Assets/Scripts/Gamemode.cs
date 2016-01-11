using UnityEngine;
using System.Collections;

public class Gamemode : MonoBehaviour {

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  void OnTriggerExit2D(Collider2D coll) {
    if (coll.tag == "Player") {
      coll.transform.GetComponent<PlayerController>().Kill();
    }
  }
}
