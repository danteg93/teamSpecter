using UnityEngine;
using System.Collections;

public class playerMovement : MonoBehaviour {

	// Use this for initialization
	public float movementOffset = 0.5f;

	private int timesDead = 0;

	public float playerNumber = 1;

	private Vector3 initialPosition;
	void Start () {
		timesDead = 0;
		initialPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("w") && playerNumber == 1){
			gameObject.transform.Translate(Vector3.up * movementOffset);
		}
		if(Input.GetKeyDown("a") && playerNumber == 1){
			gameObject.transform.Translate(Vector3.left * movementOffset);
		}
		if(Input.GetKeyDown("s") && playerNumber == 1){
			gameObject.transform.Translate(Vector3.down * movementOffset);
		}
		if(Input.GetKeyDown("d") && playerNumber == 1){
			gameObject.transform.Translate(Vector3.right * movementOffset);

		}

		if(Input.GetKeyDown(KeyCode.UpArrow) && playerNumber != 1){
			gameObject.transform.Translate(Vector3.up * movementOffset);
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow) && playerNumber != 1){
			gameObject.transform.Translate(Vector3.left * movementOffset);
		}
		if(Input.GetKeyDown(KeyCode.DownArrow) && playerNumber != 1){
			gameObject.transform.Translate(Vector3.down * movementOffset);
		}
		if(Input.GetKeyDown(KeyCode.RightArrow) && playerNumber != 1){
			gameObject.transform.Translate(Vector3.right * movementOffset);

		}
	}
	public void reSpawn(){
		timesDead++;
		gameObject.transform.position = initialPosition;
		Debug.Log(timesDead);
	}
}
