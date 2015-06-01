using UnityEngine;
using System.Collections;

public class Android : MonoBehaviour {

	private bool useVirtalJoystick;

	public bool IsJoystickEnabled() {
		return useVirtalJoystick;
	}

	public void SetJoystickStatus(bool enable) {
		useVirtalJoystick = enable;
	}

	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		useVirtalJoystick = true;
	}

	void Awake() {
		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
