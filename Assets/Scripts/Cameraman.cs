using UnityEngine;
using System.Collections;

public class Cameraman : MonoBehaviour {

  public static Cameraman cameraman;
  private float shakeTimer;
  private float shakeAmount;

  private float shrinkTimer;
  private float shrinkValue;

  // Use this for initialization
  void Awake() {
    cameraman = this;
  }
  void Start() {
    transform.position = new Vector3(0, 0, -10);
  }

  // Update is called once per frame
  void Update() {
    if (shakeTimer >= 0) {
      executeCameraShake();
    }

    if (shrinkTimer > 0)
    {
      executeCameraShrink();
    }
  }

  public void CameraShake(float shakeDuration, float shakePower) {
    shakeAmount = shakePower;
    shakeTimer = shakeDuration;
  }

  private void executeCameraShake() {
    Vector2 ShakePos = Random.insideUnitCircle * shakeAmount;
    transform.position = new Vector3(transform.position.x + ShakePos.x, transform.position.y, transform.position.z);
    shakeTimer -= Time.deltaTime;
  }

  public void shrinkCamera(float shrinkPower, float shrinkDuration)
  {
    shrinkValue = shrinkPower/100;
    shrinkTimer = shrinkDuration;
  }


  public void executeCameraShrink()
  {
    Camera.main.orthographicSize -= shrinkValue;
    shrinkTimer -= Time.deltaTime;

  }
}
