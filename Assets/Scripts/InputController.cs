using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

  public static InputController inputController;

  private string[] playerMappings = new string[4] {"k","k","k","k"};
	// Use this for initialization
	void Awake () {
    if (inputController == null) {
      DontDestroyOnLoad(gameObject);
      mapConnectedControllers();
      inputController = this;
    }
    else if (inputController != this) {
      Destroy(gameObject);
    }
	}
  void Update() {
    //Debug.Log(Input.GetJoystickNames()[1]);
  }
	// Update is called once per frame
  private void mapConnectedControllers() {
    string[] controllersConnected = Input.GetJoystickNames();

    for (int i = 0; i < controllersConnected.Length; i++) {
      //Debug.Log(controllersConnected[i]);
      if (controllersConnected[i] == "") {
        continue;
      }
      else if (controllersConnected[i].Contains("Xbox") || controllersConnected[i].Contains("XBOX")) {
        playerMappings[i] = ("XBOX_J" + (i + 1));
      }
      else {
        playerMappings[i] = ("PS4_J" + (i + 1));
      }
    }
  }
  public string GetPlayerMapping(int playerNumber) {
    return playerMappings[playerNumber - 1];
  }
  public string[] GetPlayerMappings() {
    return playerMappings;
  }
}
