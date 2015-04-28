using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ui : MonoBehaviour {

	public CNAbstractController MovementJoystick;
	private Text hullStatus;
	private Movement mv;

	// Use this for initialization
	void Start () {
	
	}
	
	private void drawHullStatus() {
		hullStatus.text = "100" + "%";
	}

	private void handleJoystickMovement() {
		mv = MonoBehaviour.FindObjectOfType<Movement>();

		if (MovementJoystick.GetAxis("Horizontal") > 0.5) {
			mv.rotateRight();
		} else if (MovementJoystick.GetAxis("Horizontal") < - 0.5) {
			mv.rotateLeft();
		} else if (MovementJoystick.GetAxis("Vertical") > 0.5) {
			mv.goForward();
		} else if (MovementJoystick.GetAxis("Vertical") < -0.5) {
			mv.goBack();
		}

	}
	// Update is called once per frame
	void Update () {
		handleJoystickMovement();
		drawHullStatus();
	
	}
}
