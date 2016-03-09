using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {

  public static InputController inputController;

  private string[] playerMappings = new string[4] {"k","k","k","k"};
  private int controllerDisconnected = -1;
  private bool controllerDC = false;
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
    //Pause the game
    if (checkForDisconnects()) {
      Time.timeScale = 0;
      controllerDC = true;
    }
    if (controllerDC && !checkForDisconnects()) {
      Time.timeScale = 1;
      controllerDC = false;
    }
  }
  void OnGUI() {
    if (controllerDisconnected > -1) {
      //TODO: Make better GUI
      //This is temporary as well, two boxes so that the damn thing is darker lol. 
      GUI.Box(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 25, 200, 50), ("Player " + (controllerDisconnected + 1) + " controller Disconnected\nPlease Reconnect"));
      GUI.Box(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 25, 200, 50), ("Player " + (controllerDisconnected + 1) + " controller Disconnected\nPlease Reconnect"));
    }
  }
  public List<int> GetPS4Controllers() {
    List<int> ps4Controllers = new List<int>();
    for (int i = 0; i < playerMappings.Length; i++) {
      if (playerMappings[i].Contains("PS4")) {
        ps4Controllers.Add(i);
      }
    }
    return ps4Controllers;
  }
  public string GetPlayerMapping(int playerNumber) {
    return playerMappings[playerNumber - 1];
  }
  public string[] GetPlayerMappings() {
    return playerMappings;
  }
  private void mapConnectedControllers() {
    string[] controllersConnected = Input.GetJoystickNames();

    for (int i = 0; i < controllersConnected.Length; i++) {
      //TODO: talk about this with team
      //This is temporary. If the controller is not connected when the players do their mappings then use keyboard controls
      //I wrote this so that we could test. 
      //Reason is that unity saves what controllers you started with when you booted the editor
      //If you happen to disconnect a controller then the game would stop and ask you to reconnect the controller. 
      //This is really annoying for testing purposes
      if (controllersConnected[i] == "") {
        playerMappings[i] = "k";
      }
      else if (controllersConnected[i].Contains("Xbox") || controllersConnected[i].Contains("XBOX")) {
        playerMappings[i] = ("XBOX_J" + (i + 1));
      }
      else {
        playerMappings[i] = ("PS4_J" + (i + 1));
      }
    }
  }
  private bool checkForDisconnects(){
    string[] controllersConnected = Input.GetJoystickNames();
    for (int i = 0; i < controllersConnected.Length; i++) {
      if(controllersConnected[i] == "" && playerMappings[i] != "k") {
        controllerDisconnected = i;
        return true;
      }
    }
    controllerDisconnected = -1;
    return false;
  }
}
