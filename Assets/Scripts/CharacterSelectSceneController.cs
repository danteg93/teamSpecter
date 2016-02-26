using UnityEngine;
using System.Collections;

public class CharacterSelectSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	  PlayerController[] players = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    for (int i = 0; i < players.Length; i++) {
      players[i].SetPlayerInvincibility(true);
    }
  }
	
	// Update is called once per frame
	void Update () {
	
	}
}
