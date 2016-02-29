using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

  public static GameManager gameManager;

  private enum gameModeSetup { Default, ThreeRounds, FiveRounds };
  private gameModeSetup currentGameMode = gameModeSetup.Default;

  void Awake() {
    //This keeps it alive during gameplay.
    if (gameManager == null) {
      DontDestroyOnLoad(gameObject);
      gameManager = this;
    }
    else if (gameManager != this) {
      Destroy(gameObject);
    }
  }

  void OnLevelWasLoaded(int level) {
    //If you are loading the menu then dont do anything because gamemode doesnt exist yet
    if (level == 2) { return; }
    setUpGame();
  }

  public void setGameMode(int selectedGameMode) {
    //This function gets called from the menu controller and it sets the current game type
    switch (selectedGameMode) {
      case 0:
        currentGameMode = gameModeSetup.ThreeRounds;
        break;
      case 1:
        currentGameMode = gameModeSetup.FiveRounds;
        break;
      default:
        currentGameMode = gameModeSetup.Default;
        break;
    }
  }

  public void kill() {
    Destroy(gameObject);
  }

  private void setUpGame() {
    //The magic happens here
    //This is where you tell game mode how to set up its variables
    //At this point in the code, it is guaranteed that game mode exists
    switch (currentGameMode) {
      case gameModeSetup.ThreeRounds:
        Gamemode.gamemode.setScoreType(0);
        Gamemode.gamemode.setUpWinningScore(3);
        break;
      case gameModeSetup.FiveRounds:
        //Gamemode.gamemode.setScoreType(0);
        //Gamemode.gamemode.setUpWinningScore(5);
        Gamemode.gamemode.setScoreType(1);
        Gamemode.gamemode.setUpMatchTime(60.0f);
        Gamemode.gamemode.setUpWinningScore(25);
        break;
      case gameModeSetup.Default:
        break;
    }
  }
}
