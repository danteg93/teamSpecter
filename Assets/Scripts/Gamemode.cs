using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Gamemode : MonoBehaviour {

  public static Gamemode gamemode;

  public bool DisplayMouse = true;
  // Temporary variable we can change how many rounds we play in testing.
  public int WinningScore = 3;

  // Game specific variables that last through all rounds
  private int[] scores = new int[4] { 0, 0, 0, 0 };
  private bool gameOver = false;

  // Per round variables needed for saving round specific information.
  private bool showCountdown = false;
  private string countdownText = "";
  private bool roundStarted = false;
  private bool roundOver = false;
  private int roundWinnerNumber = 0;

  // Make the Gamemode accessible from any script and
  // ensure it persists between scene loads.
  void Awake() {
    if (gamemode == null) {
      gamemode = this;
      DontDestroyOnLoad(gameObject);
    } else if (gamemode != this) {
      Destroy(gameObject);
    }
  }

  // Turn off the cursor if the editor told us to.
  void Start() { Cursor.visible = DisplayMouse; }

  // Check every frame of a round to see if there is a winner yet. If there is,
  // end the round and show the end round GUI.
  void Update() {
    if (!roundStarted || roundOver) { return; }
    PlayerController[] players = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    if (players.Length == 1) {
      roundOver = true;
      roundWinnerNumber = players[0].PlayerNumber;
      scores[roundWinnerNumber - 1] += 1;
      if (scores.Contains(WinningScore)) { gameOver = true; }
    } else if (players.Length == 0) {
      roundOver = true;
    }
  }

  // If the countdown timer should display, show that. Otherwise if the game is over,
  // display the end round GUI.
  void OnGUI() {
    if (showCountdown) { GUI.Box(new Rect((Screen.width / 2) - 25, (Screen.height / 2) - 25, 50, 50), countdownText); }
    if (roundOver) {
      if (gameOver) {
        displayGameOverGUI();
      } else {
        displayRoundOverGUI();
      }
    }
  }

  // TODO: We can probably do this much better by having a custom event fire
  // when game levels load. Maybe even use this function and just check if
  // PlayerControllers exist in the scene.
  void OnLevelWasLoaded(int level) {
    if (level == 3 || level == 4 || level == 5 || level == 1) {
      roundStarted = false;
      roundOver = false;
      // TODO: This should be a prefab we instantiate that kills itself.
      showCountdown = true;
      StartCoroutine(displayCountDown());
    }
  }

  // Return whether the round has started and if players can move.
  public bool RoundStarted() { return roundStarted && !roundOver; }

  // Destroy the Gamemode since it will be remade on the menu,
  // and move back to the main menu.
  private void endGame() {
    Destroy(this.gameObject);
    cleanAndLoadScene("Menu");
  }

  // Display who won and a button to restart the level.
  private void displayRoundOverGUI() {
    GUI.Box(new Rect(Screen.width / 2 - 200, 100, 400, 300), scoreboardText());
    if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 + 35, 150, 70), "Next Round") || Input.GetAxis("KB_Pause") != 0 || checkPause()) {
      cleanAndLoadScene(SceneManager.GetActiveScene().name);
    }
  }

  // Display the end game scores and a button to quit to the main menu.
  private void displayGameOverGUI() {
    GUI.Box(new Rect(Screen.width / 2 - 200, 100, 400, 300), scoreboardText());
    if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 + 35, 150, 70), "Main Menu") || Input.GetAxis("KB_Pause") != 0 || checkPause()) {
      endGame();
    }
  }

  private string scoreboardText() {
    string scoreboardText = "Game Over. ";
    if (roundWinnerNumber != 0) {
      scoreboardText += "Player " + roundWinnerNumber;
      scoreboardText += gameOver ? " wins the game!" : " wins this round!";
    } else {
      scoreboardText += "This round ended in a tie!";
    }
    scoreboardText += gameOver ? "\n\n Final Scores:\n" : "\n\n Current Scores:\n";
    for (int i = 0; i < 4; i++) {
      scoreboardText += "\nPlayer " + (i + 1) + ": " + scores[i];
    }
    return scoreboardText;
  }

  // Check controller input for anyone pressing the options button.
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

  // Display a countdown timer before each round starts.
  IEnumerator displayCountDown() {
    for (int i = 3; i >= 0; i--) {
      countdownText = i == 0 ? "GO!" : i.ToString();
      yield return new WaitForSeconds(1);
    }
    showCountdown = false;
    roundStarted = true;
  }
}
