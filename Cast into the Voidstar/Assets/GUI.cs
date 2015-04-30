using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI : MonoBehaviour {

	bool isCommandModeON;

	public GameObject GUIPanel;
	public GameObject CommandPanel;
	public GameObject InfoPanel;

	public Button switcher;
	public Button fire;
	public Button up;
	public Button back;
	public Button menu;

	public Button exit;
	public Button resume;
	
	// Use this for initialization
	void Start () {
		switchToGui ();
		InfoPanel.SetActive (false);
	}

	void switchToGui() {
		switcher.GetComponentInChildren<Text>().text = "Command Mode";
		isCommandModeON = false;
		CommandPanel.SetActive (false);
		GUIPanel.SetActive (true);
	}

	public void switchButton() {
		if (isCommandModeON) {
			switchToGui();
		} else {
			switchToCommand();
		}
	}

	public void EnterMenu() {
		GUIPanel.SetActive (false);
		CommandPanel.SetActive (false);
		InfoPanel.SetActive (true);
		Time.timeScale = 0.0f;
	}

	public void ResumeGame() {
		InfoPanel.SetActive (false);
		Time.timeScale = 1.0f;

		if (isCommandModeON) {
			switchToCommand ();
		} else {
			switchToGui ();
		}
	}

	public void ExitToMenu() {
		Time.timeScale = 1.0f;
		Application.LoadLevel ("basic_menu");
	}

	void switchToCommand() {
		switcher.GetComponentInChildren<Text>().text = "Control Mode";
		isCommandModeON = true;
		CommandPanel.SetActive (true);
		GUIPanel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
