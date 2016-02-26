using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

  public GameObject LoadingImage;
  public bool DisplayMouse = true;

  private GameObject mainMenuGUI;

  void Start() {
    Cursor.visible = DisplayMouse;
    mainMenuGUI = GameObject.FindGameObjectWithTag("MainMenuGUI");
    mainMenuGUI.SetActive(true);
  }
  //Times the controller image
  IEnumerator TutorialLoad(float loadTime, int sceneNumber) {
    LoadingImage.SetActive(true);
    yield return new WaitForSeconds(loadTime);
    SceneManager.LoadScene(sceneNumber);
  }
  //Called by the canvas Buttons
  public void LoadScene(int sceneNumber) {
    StartCoroutine(TutorialLoad(5, sceneNumber));
  }
  //Called by the canvas Buttons
  public void Quit() { Application.Quit(); }
}
