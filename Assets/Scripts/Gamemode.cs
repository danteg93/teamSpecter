using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
  private bool roundStarted = false;
  private bool roundOver = false;
  private int roundWinnerNumber = 0;
  private bool roundSetUp = false;

  //Cleaning variables
  private bool cleaning;

  //Pause realated variables
  private bool gamePaused = false;

  //UI variables
  public Text playerOneScoreText;
  public Text playerTwoScoreText;
  public Text playerThreeScoreText;
  public Text playerFourScoreText;
  public GameObject Scoreboard;
  public GameObject PauseMenu;
  public Text Countdown;
  public Button GameOverButton;
  public Button RoundOverButton;
  public Button ResumeGameButton;
  public Text ScoreboardTitle;

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
    //Set up initial score UI
    setScoreText();

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
    //Started work for pause menu
    if (roundStarted && !gamePaused && checkPause()) {
      gamePaused = true;
      PauseGame();
    }
    //Encapsulated the win condition function for further customization
    checkWinCondition();
  }
  //This gets called instead of start when level reloads
  void OnLevelWasLoaded(int level) {
    cleaning = false;
    Scoreboard.SetActive(false);
    findPlayers();
    setUpRound();
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
        setScoreText();
      }
      else {
        //If you commited suicide then you lose a point?
        scores[killedBy - 1] -= 1;
        setScoreText();
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
      //Update score
      setScoreText();
      showScoreboard();
    }
    //If there is a tie then no one wins
    else if (numberOfDeadPlayers == 4) {
      roundOver = true;
      roundSetUp = false;
      roundWinnerNumber = 0;
      showScoreboard();
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
      showScoreboard();
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
      showScoreboard();
    }
  }
  //=================================================================
  //==================Misc Helper Fucntions =========================
  // Check controller input for anyone pressing the options button.
  private bool checkPause() {
    string[] joystikcsConnected = InputController.inputController.GetPlayerMappings();
    for (int i = 0; i < joystikcsConnected.Length; i++) {
      if (joystikcsConnected[i] != "k" && Input.GetAxis(joystikcsConnected[i] + "_Pause") != 0) {
        return true;
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
  public void ReloadLevel() {
    cleanAndLoadScene(SceneManager.GetActiveScene().name);
  }
  // Destroy the Gamemode since it will be remade on the menu,
  // and move back to the main menu.
  public void EndGame() {
    Time.timeScale = 1.0F;
    Destroy(this.gameObject);
    cleanAndLoadScene("Menu");
  }

  public void PauseGame() {
    if (!Scoreboard.activeSelf) {
      Time.timeScale = 0.0F;
      PauseMenu.SetActive(true);
      EventSystem.current.SetSelectedGameObject(RoundOverButton.gameObject);
      EventSystem.current.SetSelectedGameObject(ResumeGameButton.gameObject);
    }
  }
  public void ResumeGame() {
    if (PauseMenu.activeSelf) {
      StartCoroutine(pauseCoolDown());
    }
  }
  //=================================================================
  //======================== GUI Functions ==========================
  // Display a countdown timer before each round starts.
  IEnumerator displayCountDown() {
    Countdown.gameObject.SetActive(true);
    AudioClip countdownSound = Resources.Load<AudioClip>("Audio/SFX/Misc/Countdown");
    GetComponent<AudioSource>().PlayOneShot(countdownSound, 0.5f);
    for (int i = 3; i >= 0; i--) {
      if (i == 0) {
        Countdown.text = "GO!";
        AudioClip roundStartSound = Resources.Load<AudioClip>("Audio/SFX/Misc/RoundStartHorn");
        GetComponent<AudioSource>().PlayOneShot(roundStartSound, 0.5f);
      }
      else {
        Countdown.text = i.ToString();
      }
      yield return new WaitForSeconds(1);
    }
    Countdown.gameObject.SetActive(false);
    roundStarted = true;
    setAllPlayersMoveAndShoot(true);
    setAllPlayersInvincible(false);
  }
  // Pause Cooldown
  IEnumerator pauseCoolDown() {
    Time.timeScale = 1.0F;
    PauseMenu.SetActive(false);
    yield return new WaitForSeconds(0.5f);
    gamePaused = false;
  }
  // Sets/updates score UI
  void setScoreText() {
    playerOneScoreText.text = scores[0].ToString();
    playerTwoScoreText.text = scores[1].ToString();
    playerThreeScoreText.text = scores[2].ToString();
    playerFourScoreText.text = scores[3].ToString();
    Text[] scoreboardText = Scoreboard.GetComponentsInChildren<Text>();
    for (int i = 0; i < scoreboardText.Length; i++) {
      switch (scoreboardText[i].name) {
        case "RedPlayerScore":
          scoreboardText[i].text = scores[0].ToString();
          break;
        case "BluePlayerScore":
          scoreboardText[i].text = scores[1].ToString();
          break;
        case "GreenPlayerScore":
          scoreboardText[i].text = scores[2].ToString();
          break;
        case "YellowPlayerScore":
          scoreboardText[i].text = scores[3].ToString();
          break;
      }
    }
  }

  private void showScoreboard() {
    if (!Scoreboard.activeSelf) {
      Scoreboard.SetActive(true);
      if (gameOver) {
        ScoreboardTitle.text = "Game Over\n Final Scores";
        GameOverButton.gameObject.SetActive(true);
        RoundOverButton.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(GameOverButton.gameObject);
      }
      else {
        ScoreboardTitle.text = "Round Over";
        GameOverButton.gameObject.SetActive(false);
        RoundOverButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(RoundOverButton.gameObject);
      }
    }
  }
  //=================================================================
}
