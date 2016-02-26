using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

  public GameObject LoadingImage;

  private GameObject mainMenuGUI;

  void Start() {
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
    //if (sceneNumber == 3 || sceneNumber == 4 || sceneNumber == 5) {
    //  StartCoroutine(TutorialLoad(5, sceneNumber));
    //}
    //else {
    //  SceneManager.LoadScene(sceneNumber);
    //}
    StartCoroutine(TutorialLoad(5, sceneNumber));
  }
  //Called by the canvas Buttons
  public void Quit() { Application.Quit(); }
}
