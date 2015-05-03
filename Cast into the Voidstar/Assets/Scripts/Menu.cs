using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {
	
	public GameObject QuitPrompt;
	public GameObject OptionsPrompt;

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

	// Use this for initialization
	void Start () {
		QuitPrompt.SetActive(false);
		OptionsPrompt.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
