using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ui : MonoBehaviour {

	public Text hullStatus;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		hullStatus.text = "1" + "%";
	
	}
}
