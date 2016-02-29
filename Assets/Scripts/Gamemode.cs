using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Gamemode : MonoBehaviour {

  public static Gamemode gamemode;

  public bool DisplayMouse = true;

  // Game specific variables that last through all rounds
  private enum scoreType {LMS, DM}
  private scoreType currentScoreType = scoreType.LMS;

  private int winningScore = 1;
  private float matchTime = 20.0f;
  private float roundStartTime;
  private int[] playersAlive = new int[4] { 1, 1, 1, 1 };
  private int[] scores = new int[4] { 0, 0, 0, 0 };
  private bool gameOver = false;

  private PlayerController[] players = new PlayerController[4];

  // Per round variables needed for saving round specific information.
  private bool showCountdown = false;
  private string countdownText = "";
  private bool roundStarted = false;
  private bool roundOver = false;
  private int roundWinnerNumber = 0;
  private bool roundSetUp = false;

  //Cleaning variables
  private bool cleaning;

  // Make the Gamemode accessible from any script and
  // ensure it persists between scene loads.
  // This will get destroyed when endGame() gets called
  void Awake() {
    if (gamemode == null) {
      gamemode = this;
      PlayerController[] tempPlayers = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
      for (int i = 0; i < tempPlayers.Length; i++) {
        players[tempPlayers[i].PlayerNumber - 1] = tempPlayers[i];
      }
      DontDestroyOnLoad(gameObject);
    }
    else if (gamemode != this) {
      Destroy(gameObject);
    }
  }

  void Start() {
    //Turn off the cursor if the editor told us to.
    Cursor.visible = DisplayMouse;
    //This is here in case the level was loaded from the editor
    //(makes it so scene doesn't have to be linked to menu to test)
    if (!roundSetUp) {
      setUpRound();
    }
    cleaning = false;
  }

  // Check every frame of a round to see if there is a winner yet. If there is,
  // end the round and show the end round GUI.
  void Update() {
    //Encapsulated the win condition function for further customization
    checkWinCondition();
  }

  // If the countdown timer should display, show that. Otherwise if the game is over,
  // display the end round GUI.
  void OnGUI() {
    if (showCountdown) { GUI.Box(new Rect((Screen.width / 2) - 25, (Screen.height / 2) - 25, 50, 50), countdownText); }
    if (roundOver) {
      if (gameOver) {
        displayGameOverGUI();
      }
      else {
        displayRoundOverGUI();
      }
    }
  }

  void OnLevelWasLoaded(int level) {
    cleaning = false;
    setUpRound();
  }

  public void setScoreType(int gameType){
    switch(gameType){
      case 0:
        currentScoreType = scoreType.LMS;
        break;
      case 1:
        currentScoreType = scoreType.DM;
        break;
      default:
        currentScoreType = scoreType.LMS;
        break;
    }
  }
  //Set up round number
  public void setUpWinningScore(int scoreToWin) {
    winningScore = scoreToWin;
  }
  public void playerDied(int playerNumber) {
    playersAlive[playerNumber - 1] = 0;
  }
  //We can have this function check parameters set up by the game manager
  //As of now, only number of rounds won (I know its max score) is checked
  private void checkWinCondition(){
    if (!roundStarted || roundOver) { return; }
    switch(currentScoreType){
      case scoreType.LMS:
        checkLMS();
        break;
      case scoreType.DM:
        checkDM();
        break;
      default:
        checkLMS();
        break;
    }
  }

  private void checkLMS(){
    //players = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    if (players.Length == 1) {
      roundOver = true;
      roundSetUp = false;
      roundWinnerNumber = players[0].PlayerNumber;
      scores[roundWinnerNumber - 1] += 1;
      if (scores.Contains(winningScore)) { gameOver = true; }
    }
    else if (players.Length == 0) {
      roundOver = true;
      roundSetUp = false;
    }
  }
  private void checkDM(){
    //Debug.Log(matchTime);
    if (matchTime > 0 ) {
      //matchTime -= Time.deltaTime;
      //players = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
      for (int i = 0; i < playersAlive.Length; i++) {
        if (playersAlive[i] == 0) {
          players[i].respawn();
          playersAlive[i] = 1;
        }
      }
    }
    else {
      roundOver = true;
      roundSetUp = false;
      gameOver = true;
    }
  }
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
    }
    else {
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
    if (!cleaning) {
      cleaning = true;
      GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
      foreach (GameObject projectile in projectiles) {
        //Changed the function name in case we ever have other projectiles. This might change lat0r.
        projectile.gameObject.GetComponent<ShootFireball>().DestroyProjectile();
      }
      SceneManager.LoadScene(sceneName);
    }
  }

  private void setUpRound() {
    setAllPlayersMoveAndShoot(false);

    roundStarted = false;
    roundOver = false;
    // TODO: This should be a prefab we instantiate that kills itself.
    showCountdown = true;
    StartCoroutine(displayCountDown());
  }

  private void setAllPlayersMoveAndShoot(bool allowMoveAndShoot) {
    //players = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    for (int i = 0; i < players.Length; i++) {
      players[i].SetPlayerMoveAndShoot(allowMoveAndShoot);
    }
  }

  // Display a countdown timer before each round starts.
  IEnumerator displayCountDown() {
    for (int i = 3; i >= 0; i--) {
      countdownText = i == 0 ? "GO!" : i.ToString();
      yield return new WaitForSeconds(1);
    }
    showCountdown = false;
    roundStarted = true;
    setAllPlayersMoveAndShoot(true);
  }
}
