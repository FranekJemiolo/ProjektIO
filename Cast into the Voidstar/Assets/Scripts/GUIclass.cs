using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProgressBar;
using System.Threading;

public class GUIclass : MonoBehaviour {

	private GameController gameController;
	private MothershipController mothershipController;
	private TouchScript touchScript;
	private AsteroidController asteroidController;
	private GameObject asteroid;
	private Android android;

	public CNAbstractController MovementJoystick;

	public bool isCommandModeON;
	private bool hasGameEnded;
	private bool MoveUp;
	private bool MoveDown;
	private bool MoveLeft;
	private bool MoveRight;
	private bool CameraMinus;
	private bool CameraPlus;
	private bool gamePaused;
	//private bool ableToBuildMine;

	public GameObject LeftButton;
	public GameObject RightButton;
	public GameObject JoystickSystem;
	public GameObject GUIPanel;
	public GameObject CommandPanel;
	public GameObject InfoPanel;
	public GameObject Joystick;
	public GameObject GameOver;
	public GameObject deselect;
	public GameObject buildMineOnAsteroid;

	public Text WhoWon;
	public Text PlayerHPACCStatus;
	public Text Credits;
	public Text TimeToSpawn;
	public Text buildMineOnAsteroidText;
	public Text NumberOfAsteroids;
	public Text PauseGameText;

	public Button switcher;
	public Button fire;
	public Button up;
	public Button back;
	public Button menu;

	public Button exit;
	public Button resume;

	public ProgressRadialBehaviour PlayerProgress;
	public ProgressRadialBehaviour EnemyProgress;

	public float ratio = 0.2f;

	
	public void AllowToBuild(string Info, bool Allow) {
		buildMineOnAsteroid.SetActive(Allow);
		buildMineOnAsteroidText.text = Info;
	}
    
	public void ShowGameOverScreen(string Info) {
		GameOver.SetActive(true);
		WhoWon.text = Info;
	}

    // Use this for initialization
	void Start () {
		switchToGui ();

		InfoPanel.SetActive(false);
		GameOver.SetActive(false);
		buildMineOnAsteroid.SetActive(false);

		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		mothershipController = gameController.getMothership().GetComponent<MothershipController>();
		touchScript = GameObject.FindGameObjectWithTag("TouchScript").GetComponent<TouchScript>();
		GameObject androidalke = GameObject.Find ("Android");
		if (androidalke != null)
			android = androidalke.GetComponent<Android>();

		hasGameEnded = false;
		MoveUp = false;
		gamePaused = false;

		CameraMinus = false;
		CameraPlus = false;

		SetupControls();
    }

	private void SetupControls() {

		bool joystickActive = true;

		if (android != null)
			joystickActive = android.IsJoystickEnabled();
		JoystickSystem.SetActive(joystickActive);
		LeftButton.SetActive(!joystickActive);
		RightButton.SetActive(!joystickActive);
	}

	private void UpdateAsteroidsInfo() {
		NumberOfAsteroids.text = "Info:\nAsteroid controlled:\n" +
			gameController.getPlayerAsteroids(GameController.Who.Player).ToString() +
			"\nEnemy Asteroid controlled: \n" + 
			gameController.getPlayerAsteroids(GameController.Who.Enemy).ToString();
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
					new Vector3(0,
				    	GameObject.FindGameObjectWithTag("MainCamera").transform.position.y,
				        0
				    )
				);
			} 
		}
	}

	public void PauseGame() {
		if (!gamePaused) {
			gamePaused = true;
			Time.timeScale = 0.0f;
			PauseGameText.text = "Resume";
		} else {
			gamePaused = false;
			Time.timeScale = 1.0f;
			PauseGameText.text = "Pause";
		}
	}

	public void Fire(int i) {
		//FocusCameraOnMothership();
		mothershipController.fire(i); //TODO change attack nr
	}
	
	public void RotateLeft() {
		FocusCameraOnMothership();
		mothershipController.rotateLeft(Time.deltaTime);
	}

	public void RotateRight() {
		FocusCameraOnMothership();
		mothershipController.rotateRight(Time.deltaTime);
	}

	public void MoveThread() {
		while (MoveUp) {
			FocusCameraOnMothership();
			Debug.LogError("WHILED");
			mothershipController.moveForward(Time.deltaTime);
            break;
        }
	}
	public void MoveForward() {
		MoveUp = true;
    }

	public void RelaseMoveForward() {
		MoveUp = false;
	}

	public void MoveBackward() {
		MoveDown = true;
	}

	public void RelaseMoveBackward() {
		MoveDown = false;
	}

	public void RotateLeftButton() {
		MoveLeft = true;
	}

	public void RelaseRotateLeftButton() {
		MoveLeft = false;
	}

	public void RotateRightButton() {
		MoveRight = true;
	}

	public void RelaseRotateRightButton() {
		MoveRight = false;
	}

	private void DrawPlayerStats() {
        // PlayerHPACCStatus.text = "HP: " + mothershipController.getHitPoints().ToString();
		Credits.text = "Credits: " + gameController.getPlayerCredits(GameController.Who.Player).ToString();
		TimeToSpawn.text = "Time to spawn: " + gameController.playerTimeLeftToBuild(GameController.Who.Player).ToString();
		
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
		if (!gamePaused)
			Time.timeScale = 1.0f;

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
		UpdateAsteroidsInfo();
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
		Application.LoadLevel ("Menu");
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

	public void Deselect() {
		GameObject.FindGameObjectWithTag("TouchScript").GetComponent<TouchScript>().DeselectUnits();
		//touchScript.Deselect();
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

	private void HandleMovement() {
		if (MoveDown) {
			FocusCameraOnMothership();
			mothershipController.moveBackward(Time.deltaTime);
		}
		if (MoveUp) {
			FocusCameraOnMothership();
			mothershipController.moveForward(Time.deltaTime);
        }
		if (MoveLeft) {
			FocusCameraOnMothership();
			mothershipController.rotateLeft(Time.deltaTime);
		}
		if (MoveRight) {
			FocusCameraOnMothership();
			mothershipController.rotateRight(Time.deltaTime);
		}
    }

	private void DrawDeselection() {
		if (!touchScript.isSelectionEmpty()) {
			deselect.SetActive(true);
		} else {
			deselect.SetActive(false);
		}
	}

	public void DrawBuildMine(string Info, bool Active) {
		buildMineOnAsteroid.SetActive(Active);
		buildMineOnAsteroidText.text = Info;   
    }

	public void BuildOnAsteroid() {
		touchScript.createBuilding();
	}

	public void ActivatePlusCamera() {
		CameraPlus = true;
	}

	public void RelasePlusCamera() {
		CameraPlus = false;
	}

	public void ActivateMinusCamera() {
		CameraMinus = true;
	}

	public void RelaseMinusCamera() {
		CameraMinus = false;
	}

	private void HandleCamera() {
		if (CameraMinus) {
			Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
			gameController.moveCamera(new Vector3(cam.transform.position.x, cam.transform.position.y * (1 - (ratio * Time.deltaTime)), cam.transform.position.z));
		}
		if (CameraPlus) {
			Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
			gameController.moveCamera(new Vector3(cam.transform.position.x, cam.transform.position.y * (1 + (ratio * Time.deltaTime)), cam.transform.position.z));
		}
	}

	// Update is called once per frame
	void Update () {
		HandleJoystick();
		DrawPlayerStats();
		DrawScore();
		HandleMovement();
		HandleCamera();
		DrawDeselection();
	}
}
