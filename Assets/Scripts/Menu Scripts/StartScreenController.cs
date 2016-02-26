using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour {

  public bool DisplayMouse = true;

  void Start() {
    Cursor.visible = DisplayMouse;
  }
  //This was created because it would have been weird to pass parameters
  //to the menu manager for it to know whether it had been previously initiliazed after a game had been played
  public void openMenu() {
    SceneManager.LoadScene(2);
  }
}
