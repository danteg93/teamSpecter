using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour {

  //This was created because it would have been weird to pass parameters
  //to the menu manager for it to know whether it had been previously initiliazed after a game had been played
  //We might also want to add more stuff between the start screen and the game set up.
  public bool DisplayMouse = true;

  void Start() {
    Cursor.visible = DisplayMouse;
  }
  public void openMenu() {
    SceneManager.LoadScene(2);
  }
}
