using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProgressBar;

public class GUI : MonoBehaviour {

	private GameController gameController;
	private MothershipController mothershipController;

	public CNAbstractController MovementJoystick;

	public bool isCommandModeON;
	public bool hasGameEnded;

	public GameObject GUIPanel;
	public GameObject CommandPanel;
	public GameObject InfoPanel;
	public GameObject Joystick;
	public GameObject GameOver;

	public Text WhoWon;
	public Text PlayerHPACCStatus;
	public Text Credits;
	public Text TimeToSpawn;

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
		InfoPanel.SetActive(false);
		GameOver.SetActive(false);
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		mothershipController = gameController.getMothership().GetComponent<MothershipController>();
		hasGameEnded = false;
    }

	public void FocusCameraOnMothership() {
		this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		if (this.gameController != null)
		{
			if (this.gameController.getMothership() != null)
			{
				this.gameController.moveCamera(
					gameController.getMothership().transform.position
					+
					new Vector3(0,120,0)
				);
			} 
		}
	}

	public void Fire() {
		//FocusCameraOnMothership();
		mothershipController.fire(1); //TODO change attack nr
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

	private void DrawPlayerStats() {
		PlayerHPACCStatus.text = "HP: " + mothershipController.getHitPoints().ToString() +
			" ACC: ";
		Credits.text = "Credits: " + gameController.getPlayerCredits(GameController.Who.Player).ToString();
		TimeToSpawn.text = "Time to spawn: " + gameController.TimeToSpawn[(int) GameController.Who.Player].ToString();
		
	}
	
	private void DrawScore() {
		PlayerProgress.Value = (gameController.getPlayerPoints(GameController.Who.Player) / gameController.getWinPoints()) * 100.0f;
		EnemyProgress.Value = (gameController.getPlayerPoints(GameController.Who.Enemy) / gameController.getWinPoints()) * 100.0f;	
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

	public void BuildUnit(int i) {
		gameController.buildUnit(GameController.Who.Player, (GameController.UnitType) i);
	}

	private void checkIfGameOver() {

		if (!hasGameEnded) {
		/* Assuming that There won't be any Gamestate.Paused since I'm just slowing down time to 0 */
			if (gameController.getGameState() != GameController.GameState.Playing) {
				GameOver.SetActive(true);
				hasGameEnded = true;
				if (gameController.getGameState() == GameController.GameState.PlayerWon) {
					WhoWon.text = "You have won!";
				} else if (gameController.getGameState() == GameController.GameState.EnemyWon) {
					WhoWon.text = "Enemy has won!";
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		HandleJoystick();
		DrawPlayerStats();
		DrawScore();
		checkIfGameOver();
	
	}
}
