using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour {

  private bool shieldEnabled = false;
  private MeshRenderer renderer;
  // Update is called once per frame
  void Start() {
    renderer = GetComponent<MeshRenderer>();
  }
  void Update() {
    if (shieldEnabled) {
      renderer.enabled = true;
      transform.Rotate(1.0f, 0.0f, 1.0f);
    }
    else {
      renderer.enabled = false;
    }
  }

  public void ActivateShield() {
    if (!shieldEnabled) {
      shieldEnabled = true;
    }
  }
  public void DeactivateShield() {
    if (shieldEnabled) {
      shieldEnabled = false;
    }
  }
}
