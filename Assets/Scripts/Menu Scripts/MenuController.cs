using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

  public GameObject LoadingImage;
  public bool DisplayMouse = true;

  private GameObject mainMenuGUI;
  private GameObject selectGameModeGUI;
  //public GUIElement titleLabel;

  void Start() {
    Cursor.visible = DisplayMouse;
    mainMenuGUI = GameObject.FindGameObjectWithTag("MainMenuGUI");
    selectGameModeGUI = GameObject.FindGameObjectWithTag("SelectGameModeGUI");
    selectGameModeGUI.SetActive(true);
    mainMenuGUI.SetActive(false);
  }
  //Times the controller image
  IEnumerator TutorialLoad(float loadTime, int sceneNumber) {
    LoadingImage.SetActive(true);
    yield return new WaitForSeconds(loadTime);
    SceneManager.LoadScene(sceneNumber);
  }
  public void setGameMode(int gameMode) {
    GameManager.gameManager.setGameMode(gameMode);
    ShowLevelSelect();
  }
  public void ShowSelectGameMode(){
    selectGameModeGUI.SetActive(true);
    mainMenuGUI.SetActive(false);
    EventSystem.current.SetSelectedGameObject(selectGameModeGUI.transform.GetChild(0).gameObject);
  }
  private void ShowLevelSelect() {
    selectGameModeGUI.SetActive(false);
    mainMenuGUI.SetActive(true);
    EventSystem.current.SetSelectedGameObject(mainMenuGUI.transform.GetChild(0).gameObject);
  }
  //Called by the canvas Buttons
  public void LoadScene(int sceneNumber) {
    StartCoroutine(TutorialLoad(5, sceneNumber));
  }
  //Called by the canvas Buttons
  public void Quit() { Application.Quit(); }
}
