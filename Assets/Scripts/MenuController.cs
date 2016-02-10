using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour {

  GameObject mainMenuGUI;
  GameObject gameOverGUI;

  void Start() {
    mainMenuGUI = GameObject.FindGameObjectWithTag("MainMenuGUI");
    gameOverGUI = GameObject.FindGameObjectWithTag("GameOverGUI");
  }

  public void LoadScene(int sceneNumber) {
    SceneManager.LoadScene(sceneNumber);
  }

  public void Quit() {
    Application.Quit();
  }
}
