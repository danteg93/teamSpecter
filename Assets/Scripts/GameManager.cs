using UnityEngine;

public class GameManager : MonoBehaviour {

  public static GameManager gameManager;

  public AudioClip MenuMusic;
  public AudioClip BattleMusic;

  private enum gameModeSetup { Default, LastManStanding, DeathMatch };
  private gameModeSetup currentGameMode = gameModeSetup.Default;
  private AudioSource audioSource;

  void Awake() {
    //This keeps it alive during gameplay.
    if (gameManager == null) {
      DontDestroyOnLoad(gameObject);
      gameManager = this;
    }
    else if (gameManager != this) {
      Destroy(gameObject);
    }
    audioSource = GetComponent<AudioSource>();
  }

  void OnLevelWasLoaded(int level) {
    //If you are loading the menu then dont do anything because gamemode doesnt exist yet
    if (level == 2 || level == 0) {
      if (audioSource.clip != MenuMusic) {
        audioSource.Stop();
        audioSource.clip = MenuMusic;
        audioSource.volume = 1;
        audioSource.Play();
      }
      return;
    }
    if (audioSource.clip != BattleMusic) {
      audioSource.Stop();
      audioSource.clip = BattleMusic;
      audioSource.volume = 0.25f;
      audioSource.Play();
    }
    setUpGame();
  }

  public void setGameMode(int selectedGameMode) {
    //This function gets called from the menu controller and it sets the current game type
    switch (selectedGameMode) {
      case 0:
        currentGameMode = gameModeSetup.LastManStanding;
        break;
      case 1:
        currentGameMode = gameModeSetup.DeathMatch;
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
      case gameModeSetup.LastManStanding:
        Gamemode.gamemode.setScoreType(0);
        Gamemode.gamemode.setUpWinningScore(3);
        break;
      case gameModeSetup.DeathMatch:
        Gamemode.gamemode.setScoreType(1);
        Gamemode.gamemode.setUpMatchTime(60.0f);
        Gamemode.gamemode.setUpWinningScore(15);
        break;
      case gameModeSetup.Default:
        break;
    }
  }
}
