﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class MissionSelector : MonoBehaviour {

	private string gameDataPath = "/gameData.dat";
	private GameController.GameData gameData;
	private bool[] unlocked;

	public GameObject MissionPanel;
	public GameObject Button;
	public GameObject LoadingScreen;

	private const int NumberOfMissions = 16;


	private void loadData ()
	{
		if (File.Exists(Application.persistentDataPath + gameDataPath))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + gameDataPath,
			                              FileMode.Open, FileAccess.Read);
			System.Object obj = (GameController.GameData)binaryFormatter.Deserialize(stream);
			gameData = (GameController.GameData)obj; 
			stream.Close();
			GameController.GameData.ProgressTree.MissionNode[] nodes = gameData.getProgressTree().getNodes();

			for (int i = 0; i < NumberOfMissions; i++)
				unlocked[i] = nodes[i].isUnlocked();
		}
		else
		{
			unlocked[0] = true;
			for (int i = 1; i < NumberOfMissions; i++)
				unlocked[i] = false;
		}
	}

	private void resizeMissionPanel(int x, int y) {
		MissionPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
		//MissionPanel.GetComponent<RectTransform>().right = new Vector3(-x, 0, 0);
	}

	public void LoadLevel(int i) {
		LoadingScreen.SetActive(true);
		Application.LoadLevel("0");
	}

	public void returnToMenu() {
		Application.LoadLevel("Menu");
	}

	private void DetermineButtonState() {
		for (int i = 0; i < NumberOfMissions; i++) {
			GameObject a;
			a = GameObject.Find("MapSelectButton " + i.ToString());

			if (!unlocked[i]) {
				a.GetComponent<Button>().interactable = false;
				a.GetComponentInChildren<Text>().text = "Locked";
				a.GetComponentInChildren<Text>().color = Color.gray;
			}

		}
	}

	// Use this for initialization
	void Start () {
		unlocked = new bool[NumberOfMissions + 1];
		loadData();
		resizeMissionPanel(1000, 500);
		DetermineButtonState();
		LoadingScreen.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
