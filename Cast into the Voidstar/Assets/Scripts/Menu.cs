using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {

	private Android android;
	
	public GameObject QuitPrompt;
	public GameObject OptionsPrompt;
	public Toggle JoystickToggle;

	public void Exit() {
		Debug.Log ("Quit");
		Application.Quit();
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
        
    }
    
    public void startDebugLevel() {
		Application.LoadLevel("Battleground");
	}

	public void loadSkirmish() {
		Application.LoadLevel("SkirmishOptions");
	}

	public void selectMission() {
		Application.LoadLevel("MissionSelector");
	}


	public void showQuitPrompt() {
		QuitPrompt.SetActive(true);
	}

	public void showOptionsPrompt() {
		OptionsPrompt.SetActive(true);
	}

	public void closeOptionsPrompt() {
		OptionsPrompt.SetActive(false);
	} 

	public void closeQuitPrompt() {
		QuitPrompt.SetActive(false);

	}

	public void OptionsChanged() {
		android.SetJoystickStatus(JoystickToggle.isOn);
	}

	// Use this for initialization
	void Start () {
		QuitPrompt.SetActive(false);
		OptionsPrompt.SetActive(false);
		android = GameObject.Find("Android").GetComponent<Android>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
