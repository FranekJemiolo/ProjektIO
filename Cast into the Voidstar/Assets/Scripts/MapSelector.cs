using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class MapSelector : MonoBehaviour {

	public GameObject MapSelectButton;
	public GameObject Panel;

	private int numberOfLevels = 16;
	private int buttonPositionX = 250;
	private int buttonSpacingY = 65;
	private int panelWidth = 300;

	private void CreateButton(int i) {
		GameObject b = Instantiate(MapSelectButton, new Vector3 (buttonPositionX, i * buttonSpacingY, 0), Quaternion.identity) as GameObject;
		b.transform.SetParent(Panel.transform);
		// TODO:
		// connect method here
    }

	private void SetHeightOfPanel() {
		RectTransform rt = Panel.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(panelWidth, numberOfLevels * buttonSpacingY); 
	}

	// Use this for initialization
	void Start () {
		SetHeightOfPanel();
		for (int i = 0; i <= numberOfLevels; i++) {
			CreateButton(i);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
