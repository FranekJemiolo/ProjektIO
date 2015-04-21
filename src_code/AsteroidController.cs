using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AsteroidController : MonoBehaviour {

	public enum Master{Player, Enemy, None};

	public Master getPlayer(GameObject ob)
	{
		if (ob.tag == "Player")
		{
			return Master.Player;
		}
		else if (ob.tag == "Enemy")
		{
			return Master.Enemy;
		}
		else
		{
			return Master.None;
		}
	}

	public class Building
	{
		// Reference to GameObject of the building.
		private GameObject building;


		// Building's life points. Some size.
		private float hitPoints;
		
		// Who controlls our building.
		private Master who;
		
		// Creates the building we want to build.
		Building (Master who, GameObject b)
		{
			this.building = b;
			this.who = who;
			this.hitPoints = 100.0f;
		}
		
		public void destroyBuilding()
		{
			// Calls destructor and create some form of explosion.
		}
		
		public GameObject getBuilding()
		{
			return this.building;
		}
		
		public Master whoControlls()
		{
			return this.who;
		}

		public float getHP ()
		{
			return this.hitPoints;
		}

		public void setHP (float hp)
		{
			this.hitPoints = hp;
		}
	}



	// The range from which we can capture an asteroid, square size.
	public float captureRangeSqr = 4900.0f;

	private Building building;

	private List<GameObject> units;
	private List<GameObject> enemies;

	// Who owns the asteroid.
	private Master belongsTo;
	// Who is currently capping.
	private Master capping;

	// Time from cap start.
	private float timeCapped = 0.0f;
	public float cappingTime = 5.0f;


	// Use this for initialization
	void Start () 
	{
		building = null;
		belongsTo = Master.None;
		capping = Master.None;
		units = new List<GameObject> ();
		enemies = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Check if someone is capping? Set to none if no one.
		if ((units.Count == 0) && (enemies.Count == 0))
		{
			capping = Master.None;
			timeCapped = 0.0f;
		}
		handleBuilding();

	}

	// When unit enters asteroid vicinity.	
	void OnTriggerEnter (Collider other) 
	{
		Debug.Log (other + "Entering" + other.gameObject.tag);
		if (other.gameObject.tag == "Enemy") 
		{
			enemies.Add (other.gameObject);
		} 
		else if (other.gameObject.tag == "Player") 
		{
			units.Add (other.gameObject);
		}

	}

	// When unit is in asteroid vicinity.
	void OnTriggerStay (Collider other) 
	{
		Master whoAmI = Master.None;
		if (other.gameObject.tag == "Enemy") 
		{
			whoAmI = Master.Enemy;
		} 
		else if (other.gameObject.tag == "Player") 
		{
			whoAmI = Master.Player;
		}
		// If no one holds the asteroid.
		if (timeCapped > cappingTime)
		{
			if ((capping == whoAmI) && (capping != Master.None))
			{
				belongsTo = whoAmI;
			}
		}
		if (belongsTo == Master.None)
		{
			// If no one currently capping the asteroid.
			if (capping == Master.None)
			{
				capping = whoAmI;
				timeCapped += Time.deltaTime;
			}
			else if (capping != whoAmI)
			{
				capping = Master.None;
				timeCapped = 0.0f;
			}
			// Else do nothing because capping == whoAmI.
			else
			{
				timeCapped += Time.deltaTime;
			}
			
		}
		else if (belongsTo != whoAmI)
		{
			if (whoAmI == Master.Player)
			{
				// No enemies, we can convert astero for us.
				if (enemies.Count == 0)
				{
					capping = whoAmI;
					timeCapped += Time.deltaTime;
				}
				// else we must kill them. No cap.
				else
				{
					capping = Master.None;
					timeCapped = 0.0f;
				}
			}
			else if (whoAmI == Master.Enemy)
			{
				// No enemies(units), we can convert for us.
				if (units.Count == 0)
				{
					capping = whoAmI;
					timeCapped += Time.deltaTime;
				}
				// else we must kill them. No cap.
				else
				{
					capping = Master.None;
					timeCapped = 0.0f;
				}
			}
		}
		Debug.Log (capping);
		Debug.Log(belongsTo);
		// Else do nothing we already have it.
	}

	// When object leaves asteroid field.
	void OnTriggerExit (Collider other) 
	{
		Debug.Log (other + "Leaving" + other.gameObject.tag);
		if (other.gameObject.tag == "Enemy") 
		{
			enemies.Remove (other.gameObject);
			/*if (enemies.Count == 0)
			{
				capping = Master.None;
				timeCapped = 0.0f;
			}*/
		} 
		else if (other.gameObject.tag == "Player") 
		{
			units.Remove (other.gameObject);
			/*if (units.Count == 0)
			{
				capping = Master.None;
				timeCapped = 0.0f;
			}*/
		}
	}

	public bool createBuilding(Master player)
	{
		// If no building.
		if (building == null)
		{
			// If we capped asteroid.
			if (belongsTo == player)
			{
				//building = Building(player, GameObject someModel);
				return true;
			}
			else
			{
				return false;
			}
		}
		else 
		{
			return false;
		}
	}


	// Attack the building on asteroid by player with his unit.
	public bool attackBuilding(GameObject unit)
	{
		// If there is building to attack.
		if (building != null)
		{
			// If it belongs to our enemy.
			if (getPlayer(unit) != building.whoControlls())
			{
				unit.GetComponent<UnitController> ().setTarget(building.getBuilding());
				return true;
			}

		}
		return false;
	}

	// Attack the building on asteroid by player with his units.
	public bool attackBuilding(List<GameObject> units)
	{
		// If there is building to attack.
		if (building != null)
		{
				
			foreach(GameObject unit in units)
			{
				if (this.getPlayer(unit) != building.whoControlls())
				{
				unit.GetComponent<UnitController> ().setTarget(building.getBuilding());
				}
			
			}
			return true;
		}
		return false;
	}

	// On building destruction.
	public bool destroyBuilding()
	{
		if (building != null)
		{
			building.destroyBuilding();
			building = null;
			return true;
		}
		return false;
	}

	// In this function we check the status of the building.
	private void handleBuilding()
	{
		if (building != null)
		{
			// If no life left.
			if (building.getHP() < 0.0f)
			{
				this.destroyBuilding();
			}
		}
	}
	
	
}
