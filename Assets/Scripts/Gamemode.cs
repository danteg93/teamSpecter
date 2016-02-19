using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Gamemode : MonoBehaviour {

  public static Gamemode gamemode;

  public bool DisplayMouse = true;
  private bool gameOverOn = false;
  private int winningPlayerNumber = 0;
  public bool gameStart = false;
  private string readyTime = "";

  void Awake() {
    gamemode = this;
  }

  void Start() {
    Cursor.visible = DisplayMouse;
    StartCoroutine(displayCountDown());
  }

  void Update() {
    if (gameOverOn) { return; }
    PlayerController[] players = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    if (players.Length == 1) {
      gameOverOn = true;
      winningPlayerNumber = players[0].PlayerNumber;
    } else if (players.Length == 0) {
      gameOverOn = true;
    }
  }

  void OnGUI() {
    if (!gameStart)
    {
      GUI.Box(new Rect((Screen.width / 2) - 25, (Screen.height / 2) - 25, 50, 50), readyTime);
    }
    if (gameOverOn) {
      displayGameOverGUI();
    }
  }

  // Display who won and a button to restart the level.
  private void displayGameOverGUI() {
    string winText = "Game Over. ";
    if (winningPlayerNumber != 0) {
      winText += "Player " + winningPlayerNumber + " wins!";
    } else {
      winText += "The game ended in a tie!";
    }
    winText += "\nPress \'Start\' to Restart";

    GUI.Box(new Rect(0, 0, Screen.width, Screen.height), winText);
    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "Restart") || Input.GetAxis("KB_Pause") != 0 || checkPause()) {
      gameOverOn = false;
      cleanAndLoadScene(SceneManager.GetActiveScene().name);
    } else if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 250, 200, 100), "Level Select")) {
      gameOverOn = false;
      cleanAndLoadScene("Menu");
    }
  }

  //TODO: It's a known issue that pressing the right trigger in the ps4 controller will act as the pause button of an xbox controller
  //I've been trying to fix this issue forawhile but I need to step away and take a breather
  //If anyone wants to see whats up then please do
  private bool checkPause() {
    string[] joystikcsConnected = InputController.inputController.GetPlayerMappings();
    List<int> ps4Controllers = InputController.inputController.GetPS4Controllers();
    for (int i = 0; i < joystikcsConnected.Length; i++) {
      if (joystikcsConnected[i] != "k" && Input.GetAxis(joystikcsConnected[i] + "_Pause") != 0) {
        for (int j = 0; j < ps4Controllers.Count; i++) {
          if (Input.GetAxis(joystikcsConnected[ps4Controllers[j]] + "_Primary") != 1) {
            return true;
          }
        }
      }
    }
      return false;
  }

  private void cleanAndLoadScene(string sceneName) {
    GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
    foreach (GameObject projectile in projectiles) {
      //Changed the function name in case we ever have other projectiles. This might change lat0r.  
      projectile.gameObject.GetComponent<ShootFireball>().DestroyProjectile();
    }
    SceneManager.LoadScene(sceneName);
  }
  
  // Display a countdown timer before game start
  IEnumerator displayCountDown()
  {
    readyTime = "3";
    yield return new WaitForSeconds(1);
    readyTime = "2";
    yield return new WaitForSeconds(1);
    readyTime = "1";
    yield return new WaitForSeconds(1);
    readyTime = "GO!";
    yield return new WaitForSeconds(1);
    gameStart = true;
    // display text, can be replaced by sprites for better visual
  }
}
