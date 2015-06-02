using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitDefault : MonoBehaviour {

	public bool isCommanded;
	private GUIclass gui;
	private bool first = true;
	private GameObject unit;
	private UnitController unitController;
	private List<GameObject> enemies;

	// Use this for initialization
	void Start () {
		isCommanded = false;
		enemies = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (first)
		{
			gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIclass>();
			unit = this.transform.parent.gameObject;
			unitController = unit.GetComponent<UnitController>();
			first = false;
		}
		isCommanded = gui.isCommandModeON;
		if (!isCommanded)
		{
			if (enemies.Count > 0)
			{
				if (unitController.getTarget() == null)
				{
					unitController.setTarget(enemies.First());
				}
			}
		}
	}

	void OnTriggerEnter (Collider other) 
	{
		string enemyTag = "Enemy";
		// Check if an enemy has come to our area?
		if (other.gameObject.tag == enemyTag)
		{
			this.enemies.Add(other.gameObject);
		}
	}

	void OnTriggerExit (Collider other) 
	{
		string enemyTag = "Enemy";
		if (other.gameObject.tag == enemyTag)
		{
			this.enemies.Add(other.gameObject);
			//Debug.Log("Enemy leaves " + other.gameObject);
		}
	}

}
