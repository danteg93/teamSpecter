using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public float FireRate = 0; 
	public LayerMask NotToHit;

	float timeToFire = 0;
	Transform firePoint;
	// Use this for initialization
	void Start () {
		firePoint = transform.FindChild ("FirePoint");
		if (firePoint == null) {
			Debug.LogError ("Something is wrong!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (FireRate == 0) {
			if (Input.GetMouseButtonDown (0)) {
				Shoot ();
			}
		} 
		else {
			if (Input.GetMouseButtonDown (0) && Time.time > timeToFire) {
				timeToFire = Time.time + 1 / FireRate;
				Shoot ();
			}
		}
	}

	void Shoot (){
		Debug.Log ("Test");
		Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);
		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, mousePosition - firePointPosition, 100, NotToHit);
		Debug.DrawLine (firePointPosition, mousePosition);
	}
}
