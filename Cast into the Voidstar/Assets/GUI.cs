using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProgressBar;

public class GUI : MonoBehaviour {

	private GameController gameController;
	public CNAbstractController MovementJoystick;

	bool isCommandModeON;

	public GameObject GUIPanel;
	public GameObject CommandPanel;
	public GameObject InfoPanel;
	public GameObject Joystick;

	public Button switcher;
	public Button fire;
	public Button up;
	public Button back;
	public Button menu;

	public Button exit;
	public Button resume;

	public ProgressRadialBehaviour PlayerProgress;
	public ProgressRadialBehaviour EnemyProgress;

	// Use this for initialization
	void Start () {
		switchToGui ();
		InfoPanel.SetActive (false);
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    
	public void FocusCameraOnMothership() {
	}

	public void Fire() {
		FocusCameraOnMothership();
	}
	
	public void RotateLeft() {
		FocusCameraOnMothership();
	}

	public void RotateRight() {
		FocusCameraOnMothership();
	}

	public void MoveForward() {
		FocusCameraOnMothership();
	}

	public void MoveBackward() {
		FocusCameraOnMothership();
	}

	private void DrawPlayerHP() {
		FocusCameraOnMothership();
		
	}
	
	private void DrawScore() {
		PlayerProgress.Value = 0.0f; //gameController.getPlayerCredits(1) / gameController.getWiningPoints
		EnemyProgress.Value = 0.0f;
		
	}

	void switchToGui() {
		switcher.GetComponentInChildren<Text>().text = "Command Mode";
		isCommandModeON = false;
		CommandPanel.SetActive (false);
		GUIPanel.SetActive (true);
		Joystick.SetActive(true);
		FocusCameraOnMothership();
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
		Joystick.SetActive(false);
	}

	private void HandleJoystick() {
		if (MovementJoystick.GetAxis("Horizontal") > 0.5) {
			RotateRight();
		} else if (MovementJoystick.GetAxis("Horizontal") < - 0.5) {
			RotateLeft();
		}
	}


	// Update is called once per frame
	void Update () {
		HandleJoystick();
		DrawPlayerHP();
		DrawScore();
	
	}
}
