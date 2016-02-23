using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour {

  enum Page { Start, Main, GameOver }

  private Page currentPage = Page.Start;
  private GameObject startMenuGUI;
  private GameObject mainMenuGUI;
  private GameObject gameOverGUI;

  void Start() {
    startMenuGUI = GameObject.FindGameObjectWithTag("StartMenuGUI");
    mainMenuGUI = GameObject.FindGameObjectWithTag("MainMenuGUI");
    gameOverGUI = GameObject.FindGameObjectWithTag("GameOverGUI");
    mainMenuGUI.SetActive(false);
    gameOverGUI.SetActive(false);
  }

  void OnGUI() {
    switch (currentPage) {
      case Page.Start: showStartMenu(); break;
      case Page.Main: showMainMenu(); break;
      case Page.GameOver: showGameOver(); break;
    }
  }

  public void StartGame() { currentPage = Page.Main; }

  public void LoadScene(int sceneNumber) { SceneManager.LoadScene(sceneNumber); }

  public void Quit() { Application.Quit(); }

  private void showStartMenu() {
    startMenuGUI.SetActive(true);
    mainMenuGUI.SetActive(false);
    gameOverGUI.SetActive(false);
  }

  private void showMainMenu() {
    startMenuGUI.SetActive(false);
    mainMenuGUI.SetActive(true);
    gameOverGUI.SetActive(false);
  }

  private void showGameOver() {
    startMenuGUI.SetActive(false);
    mainMenuGUI.SetActive(false);
    gameOverGUI.SetActive(true);
  }
}
