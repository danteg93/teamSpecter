using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour {

  //This was created because it would have been weird to pass parameters
  //to the menu manager for it to know whether it had been previously initiliazed after a game had been played
  //We might also want to add more stuff between the start screen and the game set up.
  public bool DisplayMouse = true;
  private PlayerController[] players = new PlayerController[4];
  private bool loadingScene = false;

  void Awake() {
    PlayerController[] tempPlayers = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    for (int i = 0; i < tempPlayers.Length; i++) {
      tempPlayers[i].setInitializedByGamemode(true);
      players[tempPlayers[i].PlayerNumber - 1] = tempPlayers[i];
    }
    for (int i = 0; i < players.Length; i++) {
      players[i].SetPlayerMoveAndShoot(false);
    }
    for (int i = 0; i < players.Length; i++) {
      players[i].setInitializedByGamemode(false);
    }
  }
  void Start() {
    Cursor.visible = DisplayMouse;
  }
  public void openMenu() {
    if (!loadingScene) {
      loadingScene = true;
      for (int i = 0; i < players.Length; i++) {
        players[i].Kill();
      }
      StartCoroutine(openScene());
    }
  }
  IEnumerator openScene() {
    //TODO graphical que that dude is invincible
    yield return new WaitForSeconds(2.0f);
    SceneManager.LoadScene(2);
  }
}
