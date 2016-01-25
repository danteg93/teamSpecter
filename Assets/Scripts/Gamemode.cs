using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Gamemode : MonoBehaviour {
  private bool gameOverOn = false;

  // Update is called once per frame
  void Update() {
    PlayerController[] players = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    if (players.Length <= 1) {
      gameOverOn = true;
    }
  }

  void OnTriggerExit2D(Collider2D coll) {
    if (coll.tag == "Player") {
      coll.transform.GetComponent<PlayerController>().Kill();
    }
  }

  void OnGUI() {
    if (gameOverOn) {
      displayGameOverGUI();
    }
  }

  private void displayGameOverGUI() {
    // Find the still living player to display who won, or make the text displayed
    // say there was a tie
    PlayerController winningPlayer = FindObjectOfType(typeof(PlayerController)) as PlayerController;
    string winText = "Game Over. ";
    if (winningPlayer) {
      winText += "Player " + winningPlayer.PlayerNumber + " wins!";
    } else {
      winText += "The game ended in a tie!";
    }
    winText += "\nPress \'Start\' to Restart";

    // Display the game over text and a button to restart the level
    GUI.Box(new Rect(0, 0, Screen.width, Screen.height), winText);
    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "Restart") || Input.GetAxis("GeneralPause") != 0) {
      gameOverOn = false;
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }
}
