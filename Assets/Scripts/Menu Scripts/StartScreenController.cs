using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour {

  //This was created because it would have been weird to pass parameters
  //to the menu manager for it to know whether it had been previously initiliazed after a game had been played
  //We might also want to add more stuff between the start screen and the game set up.
  public bool DisplayMouse = true;
  private PlayerController[] players = new PlayerController[4];

  void Awake() {
    PlayerController[] tempPlayers = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    for (int i = 0; i < tempPlayers.Length; i++) {
      tempPlayers[i].setInitializedByGamemode(true);
      players[tempPlayers[i].PlayerNumber - 1] = tempPlayers[i];
    }
    for (int i = 0; i < players.Length; i++) {
      players[i].SetPlayerInvincibility(true);
    }
    for (int i = 0; i < players.Length; i++) {
      players[i].SetPlayerMoveAndShoot(false);
    }
  }
  void Start() {
    Cursor.visible = DisplayMouse;
  }
  public void openMenu() {
    SceneManager.LoadScene(2);
  }
}
