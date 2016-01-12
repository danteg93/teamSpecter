using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour {
	public float Velocity = 1;
	private float selfDestroyTimer = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = this.transform.position;
		position.y--;
		this.transform.position = position;

		selfDestroyTimer -= Time.deltaTime;

		if (selfDestroyTimer <= 0) {
			Destroy (gameObject);
		}
		}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "Player")
		{
			Debug.Log ("destroyed player");
			Destroy(col.gameObject);
			Destroy (gameObject);
		}
	}


}
