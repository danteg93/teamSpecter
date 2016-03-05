using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Gamemode : MonoBehaviour {

  public static Gamemode gamemode;

  public bool DisplayMouse = true;

  //Rules related variables
  private enum scoreType { LMS, DM }
  private scoreType currentScoreType = scoreType.LMS;
  private int winningScore = 1;
  private float matchTime = -1.0f;
  private float roundStartTime;
  private bool[] playersAlive = new bool[4] { true, true, true, true };
  private int[] scores = new int[4] { 0, 0, 0, 0 };
  private bool gameOver = false;

  //players in the scene found in findPlayers()
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
      findPlayers();
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
  //This gets called instead of start when level reloads
  void OnLevelWasLoaded(int level) {
    cleaning = false;
    findPlayers();
    setUpRound();
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
  // ========= Game Set Up Stuff (called by GameManager) ============
  public void setScoreType(int gameType) {
    switch (gameType) {
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
  public void setUpMatchTime(float timeOfMatch) {
    matchTime = timeOfMatch;
  }
  public void setUpWinningScore(int scoreToWin) {
    winningScore = scoreToWin;
  }
  //=================================================================
  //========= Player Death ==========================================
  public void playerDied(int playerNumber) {
    playersAlive[playerNumber - 1] = false;
  }
  public void playerDied(int playerNumber, int killedBy) {
    playersAlive[playerNumber - 1] = false;
    if (currentScoreType == scoreType.DM) {
      if (playerNumber != killedBy) {
        scores[killedBy - 1] += 1;
      }
      else {
        //If you commited suicide then you lose a point?
        scores[killedBy - 1] -= 1;
      }
    }
  }
  //=================================================================
  //==================Win Condition Checks===========================
  private void checkWinCondition() {
    if (!roundStarted || roundOver) { return; }
    switch (currentScoreType) {
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
  //~~~~~~~~~~~ Rules for last man standing ~~~~~~~~~~~~~~
  private void checkLMS() {
    int numberOfDeadPlayers = 0;
    int playerAlive = 0;
    //Loop through the players alive
    //If there are 3 dead players that means someone won
    //playersAlive will contain the winning player if there
    //are less than 3 alive players
    for (int i = 0; i < playersAlive.Length; i++) {
      if (!playersAlive[i]) {
        numberOfDeadPlayers++;
      }
      else {
        playerAlive = i;
      }
    }
    //Set Winners and do GUI stuff if there is one winner
    if (numberOfDeadPlayers == 3) {
      roundOver = true;
      roundSetUp = false;
      roundWinnerNumber = playerAlive + 1;
      scores[playerAlive] += 1;
      if (scores.Contains(winningScore)) { gameOver = true; }
    }
    //If there is a tie then no one wins
    else if (numberOfDeadPlayers == 4) {
      roundOver = true;
      roundSetUp = false;
      roundWinnerNumber = 0;
    }
  }
  //~~~~~~~~~~~ Rules for Death match ~~~~~~~~~~~~~~~~
  private void checkDM() {
    //If someone got the amount of kills then there is a winner
    if (scores.Contains(winningScore)) {
      int winnersFound = 0;
      //Find how many players have the winning socre (for ties)
      for (int i = 0; i < scores.Length; i++) {
        if (scores[i] == winningScore) {
          roundWinnerNumber = i + 1;
          winnersFound++;
        }
      }
      //If more than one player won then there is a tie
      if (winnersFound > 1) {
        roundWinnerNumber = 0;
      }
      setAllPlayersMoveAndShoot(false);
      setAllPlayersInvincible(true);
      roundOver = true;
      roundSetUp = false;
      gameOver = true;
    }
    //This is the match time loop
    //matchTime of -1 means that we want unlimited time
    if (matchTime > 0 || matchTime == -1.0f) {
      //If the match time is -1 then dont decrease it (unlimited time)
      if (matchTime != -1.0f) {
        matchTime -= Time.deltaTime;
      }
      //If a player has died then tell it to respawn and reset its flag
      for (int i = 0; i < playersAlive.Length; i++) {
        if (!playersAlive[i]) {
          players[i].respawn();
          playersAlive[i] = true;
        }
      }
    }
    //if the match time is over then check who has the biggest score
    else {
      //Assume that player 1 has the best score
      int bestScore = scores[0];
      roundWinnerNumber = 1;
      //loop through the other players to see if anyone has
      //a better or equal score
      for (int i = 1; i < scores.Length; i++) {
        //If the score is better then we found a new winner
        if (scores[i] > bestScore) {
          bestScore = scores[i];
          roundWinnerNumber = i + 1;
        }
        //If the score is the same then there is a tie D:
        else if (scores[i] == bestScore) {
          roundWinnerNumber = 0;
        }
      }
      //GUI stuff goes here
      setAllPlayersMoveAndShoot(false);
      setAllPlayersInvincible(true);
      roundOver = true;
      roundSetUp = false;
      gameOver = true;
    }
  }
  //=================================================================
  //==================Misc Helper Fucntions =========================
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
  private void setUpRound() {
    setAllPlayersMoveAndShoot(false);
    setAllPlayersInvincible(true);
    //All players are alive when the round starts
    playersAlive = new bool[4] { true, true, true, true };
    roundStarted = false;
    roundOver = false;
    showCountdown = true;
    StartCoroutine(displayCountDown());
  }
  private void findPlayers() {
    //Map all the players in the scene to the appropriate postion
    //given by their player number
    PlayerController[] tempPlayers = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    for (int i = 0; i < tempPlayers.Length; i++) {
      tempPlayers[i].setInitializedByGamemode(true);
      players[tempPlayers[i].PlayerNumber - 1] = tempPlayers[i];
    }
  }
  private void setAllPlayersMoveAndShoot(bool allowMoveAndShoot) {
    for (int i = 0; i < players.Length; i++) {
      players[i].SetPlayerMoveAndShoot(allowMoveAndShoot);
    }
  }
  private void setAllPlayersInvincible(bool isInvinsible) {
    for (int i = 0; i < players.Length; i++) {
      players[i].SetPlayerInvincibility(isInvinsible);
    }
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
  // Destroy the Gamemode since it will be remade on the menu,
  // and move back to the main menu.
  private void endGame() {
    Destroy(this.gameObject);
    cleanAndLoadScene("Menu");
  }
  //=================================================================
  //======================== GUI Fucntions ==========================
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
  // Display a countdown timer before each round starts.
  IEnumerator displayCountDown() {
    AudioClip countdownSound = Resources.Load<AudioClip>("Audio/SFX/Misc/Countdown");
    GetComponent<AudioSource>().PlayOneShot(countdownSound, 0.5f);
    for (int i = 3; i >= 0; i--) {
      if (i == 0) {
        countdownText = "GO!";
        AudioClip roundStartSound = Resources.Load<AudioClip>("Audio/SFX/Misc/RoundStartHorn");
        GetComponent<AudioSource>().PlayOneShot(roundStartSound, 0.5f);
      } else {
        countdownText = i.ToString();
      }
      yield return new WaitForSeconds(1);
    }
    showCountdown = false;
    roundStarted = true;
    setAllPlayersMoveAndShoot(true);
    setAllPlayersInvincible(false);
  }
  //=================================================================
}
