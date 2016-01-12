using UnityEngine;
using System.Collections;

public class Cameraman : MonoBehaviour {

    public float shakeTimer;
    public float shakeAmount;

  // Use this for initialization
  void Start()
  {
    transform.position = new Vector3(0,0,-10);
  }

  // Update is called once per frame
  void Update () {
	  if (shakeTimer >= 0)
      {
        Vector2 ShakePos = Random.insideUnitCircle * shakeAmount;
        transform.position = new Vector3(transform.position.x + ShakePos.x, transform.position.y, transform.position.z);
        shakeTimer -= Time.deltaTime;

      }

    if (Input.GetKey("="))
    {
      cameraShake(1, 0.1f);
    }
    
	}

  public void cameraShake(float shakeDuration, float shakePower)
  {
    shakeAmount = shakePower;
    shakeTimer = shakeDuration;
  }
}
