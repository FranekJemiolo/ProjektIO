using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// This script handles the logic for individual ai <enemy> unit.
public class EnemyController : MonoBehaviour 
{



	public class Agent
	{
		// Some variables for movement.
		// This is the size of map.
		private float rangeSqr = 25.0f;
		private float minX = 0.0f;
		private float maxX = 500.0f;
		private float minZ = 0.0f;
		private float maxZ = 500.0f;


		private Transform myTransform;

		private List<GameObject> allies;
		private List<GameObject> enemies;

		private float groupStrength;
		private float enemiesStrength;
		private float myStrength;

		private float aggresivness;
		private float defensivness;

		private GameObject target;

		// Where the object should go?
		private Vector3 destination;

		// The values if detected asteroid.
		private GameObject asteroid;

		private float capping;
		private float timePassed;


		public Agent (float agg, float def)
		{
			this.allies = new List<GameObject>();
			this.enemies = new List<GameObject>();
			this.myStrength = 0.0f;
			this.capping = 10.0f;
			this.timePassed = 0.0f;
			this.groupStrength = myStrength;
			this.enemiesStrength = 0.0f;
			this.aggresivness = agg;
			this.defensivness = def;
			this.target = null;
			this.myTransform = null;
			this.asteroid = null;
		}

		public void setAsteroid (GameObject astero)
		{
			this.asteroid = astero;
		}

		public GameObject getAsteroid ()
		{
			return this.asteroid;
		}

		public void setTimePassed (float f)
		{
			this.timePassed = f;
		}

		public float getTimePassed ()
		{
			return this.timePassed;
		}

		public void setCapping (float f)
		{
			this.capping = f;
		}

		public float getCapping ()
		{
			return this.capping;
		}

		public void setDestination (Vector3 pos)
		{
			this.destination = pos;
		}

		public Vector3 getDestination ()
		{
			return this.destination;
		}

		public void setAggresivness (float f)
		{
			this.aggresivness = f;
		}

		public void setDefensivness (float f)
		{
			this.defensivness = f;
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

		public GameObject getTarget ()
		{
			return this.target;
		}

		public void setTransform (Transform t)
		{
			this.myTransform = t;
		}

		public Transform getTransform ()
		{
			return this.myTransform;
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
				this.setTarget(null);
				Transform enemyPosition = enemies.First().transform;
				Vector3 runVector = this.myTransform.parent.position - enemyPosition.position;
				this.myTransform.parent.GetComponent<UnitController>().moveTo(runVector);
			}
			else
			{
				this.scout();
			}
		}


		// If no units in vicinity, we will scout.
		public void scout ()
		{
			Vector3 randomPosition = new Vector3(
				this.myTransform.parent.position.x + (Random.Range(-100.0f, 100.0f)*(this.aggresivness)), 
				this.myTransform.parent.position.y,
				this.myTransform.parent.position.z + (Random.Range(-100.0f, 100.0f)*(this.aggresivness)));
			if (randomPosition.x < minX)
			{
				randomPosition.x = minX;
			}
			if (randomPosition.x > maxX)
			{
				randomPosition.x = maxX;
			}
			if (randomPosition.z < minZ)
			{
				randomPosition.z = minZ;
			}
			if (randomPosition.z > maxZ)
			{
				randomPosition.z = maxZ;
			}
			this.destination = randomPosition;
			this.myTransform.parent.GetComponent<UnitController>().moveTo(destination);
		}


		// Called when support is needed to attack opponent.
		public bool callForBackup ()
		{
			if (target == null)
			{
				if (enemies.Count > 0)
				{
					target = enemies.First();
				}
			}
			if (target != null)
			{
				if (GameObject.FindWithTag("AICore").GetComponent<AIController>().requestedBackup(target))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}

		}

		// This method is called on update and forces unit to think.
		public void think ()
		{
			this.capping = this.defensivness - this.aggresivness + 10.0f;
			Debug.Log ("Aggresivnes is " + this.aggresivness);
			Debug.Log ("Defenisvness is " + this.defensivness);
			if (this.enemiesStrength > ((1.0f + this.aggresivness) * this.groupStrength))
			{
				// Flee!!!
				this.flee();
			}
			else
			{
				bool backup = true;
				if (this.enemiesStrength > this.groupStrength)
				{
					backup = this.callForBackup();
				}
				if (backup)
				{
					if (this.target == null)
					{
						if (enemies.Count > 0)
						{
							this.target = enemies.First();
							this.myTransform.parent.GetComponent<UnitController>().setTarget(target);
						}
					}
					if (target != null)
					{
						this.myTransform.parent.GetComponent<UnitController>().setTarget(target);
					}
					else
					{
						if (this.asteroid != null)
						{
							if (this.asteroid.GetComponent<AsteroidController>().belongsTo != 
							    AsteroidController.getPlayer(myTransform.parent.gameObject))
							{
								this.timePassed = 0.0f;
							}
							else
							{
								// We are leaving anyway.
								if (this.timePassed > this.capping)
								{
									this.timePassed = 0.0f;
									this.asteroid = null;
									this.scout();
								}
								else
								{
									this.destination = this.myTransform.parent.position;
								}
							}
						}
						else if (((this.myTransform.position - this.destination).sqrMagnitude) < rangeSqr)
						{
							this.scout();
						}

					}
				}
				else 
				{
					this.flee();
				}
				Debug.Log (asteroid);
				Debug.Log (destination);
			}
		}

	}

	private UnitController unitController;
	private string myTag;
	private string enemyTag;
	public Agent agent;

	// How often agents think.
	private float thinkRate = 2.0f;
	private float timePassed = 0.0f;

	
	void Start () 
	{
		// Initialize the unit controller.
		this.unitController = this.transform.parent.gameObject.GetComponent<UnitController>();
		this.myTag = this.transform.parent.gameObject.tag;
		if (this.myTag == "Enemy")
		{
			this.enemyTag = "Player";
		}
		else if (this.myTag == "Player")
		{
			this.enemyTag = "Enemy";
		}
		this.agent = new Agent(1.0f, 1.0f);
		this.agent.setTransform(this.transform);
		float s = unitController.getAttackForce();
		this.agent.setStrength(s);
		this.agent.setGroupStrength(s);
		this.agent.setDestination(this.agent.getTransform().position);
	}

	void Update () 
	{
		this.timePassed += Time.deltaTime;
		if (this.timePassed > this.thinkRate)
		{
			this.timePassed = 0.0f;
			this.agent.setTimePassed(this.agent.getTimePassed() + this.thinkRate);
			this.agent.think();
		}
	}

	void OnTriggerEnter (Collider other) 
	{
		// If an ally come in our area.
		if (other.gameObject.tag == myTag)
		{
			this.agent.addAlly(other.gameObject);
		}
		// Check if an enemy has come to our area?
		else if (other.gameObject.tag == enemyTag)
		{
			this.agent.addEnemy(other.gameObject);
			Debug.Log("Enemy enters " + other.gameObject);
			this.reportEnemy(other.gameObject);
		}
		else if (other.gameObject.tag == "Asteroid")
		{
			this.agent.setAsteroid(other.gameObject);
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
			this.agent.removeAlly(other.gameObject);
		}
		// Check if an enemy leaves our area?
		else if (other.gameObject.tag == enemyTag)
		{
			this.agent.removeEnemy(other.gameObject);
			Debug.Log("Enemy leaves " + other.gameObject);
		}
		else if (other.gameObject.tag == "Asteroid")
		{
			this.agent.setAsteroid(null);
		}
		// The agent has to think again probably in update.
		// Else do nothing. We are not interested in other things.
	}


	public void reportEnemy (GameObject go)
	{
		GameObject.FindGameObjectWithTag("AICore").GetComponent<AIController>().reportedEnemy(go);
	}

	public void updateAgent (float agg, float def)
	{
		this.agent.setAggresivness(agg);
		this.agent.setDefensivness(def);
	}

	public void removeUnit (GameObject unit)
	{
		if (unit.tag == myTag)
		{
			this.agent.removeAlly(unit);
		}
		else if (unit.tag == enemyTag)
		{
			this.agent.removeEnemy(unit);
		}
	}

}
