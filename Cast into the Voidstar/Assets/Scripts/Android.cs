using UnityEngine;
using System.Collections;

public class Android : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Awake() {
		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
