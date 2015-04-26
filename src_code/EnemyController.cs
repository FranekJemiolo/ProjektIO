using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This script handles the logic for individual ai <enemy> unit.
public class EnemyController : MonoBehaviour 
{

	public class Agent
	{
		private List<GameObject> allies;
		private List<GameObject> enemies;

		public Agent ()
		{
			this.allies = new List<GameObject>();
			this.enemies = new List<GameObject>();
		}

		public void addEnemy (GameObject enemy)
		{
			enemies.Add(enemy);
		}

		public void removeEnemy (GameObject enemy)
		{
			enemies.Remove(enemy);
		}

		public void addAlly (GameObject ally)
		{
			allies.Add(ally);
		}

		public void removeAlly (GameObject ally)
		{
			allies.Remove(ally);
		}

	}

	private UnitController unitController;
	private string myTag;
	private string enemyTag;
	private Agent agent;

	
	void Start () 
	{
		// Initialize the unit controller.
		unitController = this.transform.parent.gameObject.GetComponent<UnitController>();
		myTag = this.transform.parent.gameObject.tag;
		if (myTag == "Enemy")
		{
			enemyTag = "Player";
		}
		else if (myTag == "Player")
		{
			enemyTag = "Enemy";
		}
		agent = new Agent();
	}

	void Update () 
	{
	
	}

	void OnTriggerEnter (Collider other) 
	{
		// If an ally come in our area.
		if (other.gameObject.tag == myTag)
		{
			agent.addAlly(other.gameObject);
		}
		// Check if an enemy has come to our area?
		else if (other.gameObject.tag == enemyTag)
		{
			agent.addEnemy(other.gameObject);
		}
		// The agent has to think again probably in update.
		// Else do nothing. We are not interested in other things.
	}


	// I don't know yet what will be necessary here.
	void OnTriggerStay (Collider other) 
	{
	}

	void OnTriggerExit (Collider other) 
	{
		// If an ally leaves our area.
		if (other.gameObject.tag == myTag)
		{
			agent.removeAlly(other.gameObject);
		}
		// Check if an enemy leaves our area?
		else if (other.gameObject.tag == enemyTag)
		{
			agent.removeEnemy(other.gameObject);
		}
		// The agent has to think again probably in update.
		// Else do nothing. We are not interested in other things.
	}
}
