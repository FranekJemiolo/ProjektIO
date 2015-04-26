using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// This script handles the logic for individual ai <enemy> unit.
public class EnemyController : MonoBehaviour 
{

	public class Agent
	{
		private Transform myTransform;

		private List<GameObject> allies;
		private List<GameObject> enemies;

		private float groupStrength;
		private float enemiesStrength;
		private float myStrength;

		private GameObject target;

		public Agent ()
		{
			this.allies = new List<GameObject>();
			this.enemies = new List<GameObject>();
			this.myStrength = 0.0f;
			this.groupStrength = myStrength;
			this.enemiesStrength = 0.0f;
			target = null;
			myTransform = null;
		}

		public void setStrength (float f)
		{
			this.myStrength = f;
		}

		public void setGroupStrength (float f)
		{
			this.groupStrength = f;
		}

		public void setTarget (GameObject go)
		{
			this.target = go;
		}

		public void setTransform (Transform t)
		{
			this.myTransform = t;
		}

		public void addEnemy (GameObject enemy)
		{
			this.enemies.Add(enemy);
			this.enemiesStrength += enemy.GetComponent<UnitController>().getAttackForce();
		}

		public void removeEnemy (GameObject enemy)
		{
			this.enemies.Remove(enemy);
			this.enemiesStrength -= enemy.GetComponent<UnitController>().getAttackForce();
		}

		public void addAlly (GameObject ally)
		{
			this.allies.Add(ally);
			this.groupStrength += ally.GetComponent<UnitController>().getAttackForce();
		}

		public void removeAlly (GameObject ally)
		{
			this.allies.Remove(ally);
			this.groupStrength -= ally.GetComponent<UnitController>().getAttackForce();
		}


		// A unit will flee if opponent has too much units or when it has too low health.
		public void flee ()
		{
			if (enemies.Count > 0)
			{
				Transform enemyPosition = enemies.First().transform;
				Vector3 runVector = this.myTransform.parent.position - enemyPosition.position;
				this.myTransform.parent.GetComponent<UnitController>().moveTo (runVector);
			}
		}


		// Called when support is needed to attack opponent.
		public void callForBackup ()
		{

		}

		// This method is called on update and forces unit to think.
		public void think ()
		{
			if (this.enemiesStrength > (2.0f * this.groupStrength))
			{
				// Flee!!!
				this.flee();
			}
			else
			{
				if (this.enemiesStrength > this.groupStrength)
				{
					this.callForBackup();
				}

			}
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
			this.reportEnemy();
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


	public void reportEnemy (GameObject go)
	{
		GetComponent<AIController>().reportedEnemy(go);
	}
}
