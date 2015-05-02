using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProgressBar;

public class GUI : MonoBehaviour {

	private GameController gameController;
	private MothershipController mothershipController;

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
		this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		this.mothershipController = gameController.getMothership().GetComponent<MothershipController>();
    }
    
	public void FocusCameraOnMothership() {
		this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		//if (this.gameController != null)
		//{
			///if (this.gameController.getMothership() != null)
			//{
				this.gameController.moveCamera(gameController.getMothership().transform.position);
			//}
		//}


		//GameObject.FindGameObjectWithTag("MainCamera").transform.position = gameController.mothership.transform.position;
	}

	public void Fire() {
		FocusCameraOnMothership();
	}
	
	public void RotateLeft() {
		FocusCameraOnMothership();
		mothershipController.rotateLeft(Time.deltaTime);
	}

	public void RotateRight() {
		FocusCameraOnMothership();
		mothershipController.rotateRight(Time.deltaTime);
	}

	public void MoveForward() {
		FocusCameraOnMothership();
		mothershipController.moveForward(Time.deltaTime);
	}

	public void MoveBackward() {
		FocusCameraOnMothership();
		mothershipController.moveBackward(Time.deltaTime);
	}

	private void DrawPlayerHP() {
		FocusCameraOnMothership();
		
	}
	
	private void DrawScore() {
		PlayerProgress.Value = gameController.getPlayerPoints(GameController.Who.Player) / gameController.getWinPoints();
		EnemyProgress.Value = gameController.getPlayerPoints(GameController.Who.Enemy) / gameController.getWinPoints();	
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
