using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

  public static GameManager gameManager;

  enum gameMode { Default, ThreeRounds, FiveRounds };
  private gameMode currentGameMode = gameMode.Default;

  void Awake() {
    if (gameManager == null) {
      DontDestroyOnLoad(gameObject);
      gameManager = this;
    }
    else if (gameManager != this) {
      Destroy(gameObject);
    }
  }

  void OnLevelWasLoaded(int level) {
    if (level == 2) { return; }
    setUpGame();
  }

  public void setGameMode(int selectedGameMode) {
    switch (selectedGameMode) {
      case 0: 
        currentGameMode = gameMode.ThreeRounds;
        break;
      case 1:
        currentGameMode = gameMode.FiveRounds;
        break;
      default:
        currentGameMode = gameMode.Default;
        break;

    }
  }

  public void kill() {
    Destroy(this.gameObject);
  }

  private void setUpGame() {
    switch (currentGameMode) {
      case gameMode.ThreeRounds:
        Gamemode.gamemode.setUpRoundNumbers(3);
        break;
      case gameMode.FiveRounds:
        Gamemode.gamemode.setUpRoundNumbers(5);
        break;
      case gameMode.Default:
        break;
    }
  }
}
