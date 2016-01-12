using UnityEngine;
using System.Collections;

public class Cameraman : MonoBehaviour {

  private float shakeTimer;
  private float shakeAmount;

  // Use this for initialization
  void Start() {
    transform.position = new Vector3(0, 0, -10);
  }

  // Update is called once per frame
  void Update() {
    if (shakeTimer >= 0) {
      executeCameraShake();
    }

    if (Input.GetKey("=")) {
      CameraShake(1, 0.1f);
    }

  }

  private void executeCameraShake() {
    Vector2 ShakePos = Random.insideUnitCircle * shakeAmount;
    transform.position = new Vector3(transform.position.x + ShakePos.x, transform.position.y, transform.position.z);
    shakeTimer -= Time.deltaTime;
  }
  public void CameraShake(float shakeDuration, float shakePower) {
    shakeAmount = shakePower;
    shakeTimer = shakeDuration;
  }
}
