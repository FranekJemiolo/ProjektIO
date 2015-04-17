using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scr : MonoBehaviour {

	private Button mybut;
	// Use this for initialization
	void Start () {
		mybut = this.GetComponent<Button> ();
		GameController gc = FindObjectOfType<GameController> ();
		mybut.onClick.AddListener (() => {
			gc.navigateTo (GameObject.FindGameObjectWithTag ("Unit"), new Vector3 (50, 0, 0));});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
