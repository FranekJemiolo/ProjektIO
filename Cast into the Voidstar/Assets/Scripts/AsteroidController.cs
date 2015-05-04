using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// This script describes behaviour of an asteroid in-game.
public class AsteroidController : MonoBehaviour 
{

	public enum Master{Player, Enemy, None};

	public static Master getPlayer(GameObject ob)
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

		// Where the building should be spawned?
		private Transform where;


		// Building's life points. Some size.
		private float hitPoints;
		
		// Who controlls our building.
		private Master who;

		// How long it takes to build it.
		private float buildTime = 10.0f;

		// Timer
		private float timePassed;

		// Is actively builded?
		private bool builded;

		// If built set to true. Can destroy after.
		private bool built;

		
		// Creates the building we want to build.
		public Building (Master who, GameObject b)
		{
			this.building = b;
			this.who = who;
			this.hitPoints = 100.0f;
			this.timePassed = 0.0f;
			this.builded = false;
			this.built = false;
		}


		// Called after building was created - for the model to be build.
		public void updateOnCreation (float t)
		{
			this.timePassed += t;
			// Now instantiate.
			if (this.timePassed > this.buildTime)
			{
				Debug.Log ("Building has been built");
				this.building = Instantiate (this.building, where.position, Quaternion.identity) as GameObject;
				this.building.transform.SetParent (where);
				//this.building
				this.builded = false;
				this.built = true;
			}
		}
		
		public void destroyBuilding ()
		{
			// Calls destructor and create some form of explosion.
			Destroy(this.building);
		}
		
		public GameObject getBuilding ()
		{
			return this.building;
		}
		
		public Master whoControlls ()
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

		public bool isBuilded ()
		{
			return this.builded;
		}

		public bool isBuilt ()
		{
			return this.built;
		}
	}



	// The range from which we can capture an asteroid, square size.
	public float captureRangeSqr = 4900.0f;

	// The class used to represent building data.
	private Building building;
	// Insert prefab model here for building.
	public GameObject prefabBuildingModel;

	public GameObject prefabEnemyBuildingModel;


	// List to represent gameobject in vicinity.
	private List<GameObject> units;
	private List<GameObject> enemies;

	// Who owns the asteroid.
	public Master belongsTo;
	// Who is currently capping.
	private Master capping;

	// Time from cap start.
	private float timeCapped = 0.0f;
	private float cappingTime = 5.0f;


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
			this.capping = Master.None;
			this.timeCapped = 0.0f;
		}
		this.handleBuilding();

	}

	// When unit enters asteroid vicinity.	
	void OnTriggerEnter (Collider other) 
	{
		Debug.Log (other + "Entering" + other.gameObject.tag);
		if (other.gameObject.tag == "Enemy") 
		{
			this.enemies.Add (other.gameObject);
		} 
		else if (other.gameObject.tag == "Player") 
		{
			this.units.Add (other.gameObject);
		}

	}

	// When unit is in asteroid vicinity.
	void OnTriggerStay (Collider other) 
	{
		Master whoAmI = this.getPlayer(other.gameObject);
		if (whoAmI == Master.None)
		{
			return;
		}
		// If no one holds the asteroid.
		if (this.timeCapped > this.cappingTime)
		{
			if ((this.capping == whoAmI) && (this.capping != Master.None))
			{
				this.belongsTo = whoAmI;
			}
		}
		if (this.belongsTo == Master.None)
		{
			// If no one currently capping the asteroid.
			if (this.capping == Master.None)
			{
				this.capping = whoAmI;
				this.timeCapped += Time.deltaTime;
			}
			else if (capping != whoAmI)
			{
				this.capping = Master.None;
				this.timeCapped = 0.0f;
			}
			// Else do nothing because capping == whoAmI.
			else
			{
				timeCapped += Time.deltaTime;
			}
			
		}
		else if (this.belongsTo != whoAmI)
		{
			if (whoAmI == Master.Player)
			{
				// No enemies, we can convert astero for us.
				if (this.enemies.Count == 0)
				{
					this.capping = whoAmI;
					this.timeCapped += Time.deltaTime;
				}
				// else we must kill them. No cap.
				else
				{
					this.capping = Master.None;
					this.timeCapped = 0.0f;
				}
			}
			else if (whoAmI == Master.Enemy)
			{
				// No enemies(units), we can convert for us.
				if (this.units.Count == 0)
				{
					this.capping = whoAmI;
					this.timeCapped += Time.deltaTime;
				}
				// else we must kill them. No cap.
				else
				{
					this.capping = Master.None;
					this.timeCapped = 0.0f;
				}
			}
		}
		//Debug.Log ("Capping:" + capping);
		Debug.Log("Belongs to:" + belongsTo);
		// Else do nothing we already have it.
	}

	// When object leaves asteroid field.
	void OnTriggerExit (Collider other) 
	{
		Debug.Log (other + "Leaving" + other.gameObject.tag);
		if (other.gameObject.tag == "Enemy") 
		{
			this.enemies.Remove (other.gameObject);
			/*if (enemies.Count == 0)
			{
				capping = Master.None;
				timeCapped = 0.0f;
			}*/
		} 
		else if (other.gameObject.tag == "Player") 
		{
			this.units.Remove (other.gameObject);
			/*if (units.Count == 0)
			{
				capping = Master.None;
				timeCapped = 0.0f;
			}*/
		}
	}

	public bool createBuilding (Master player)
	{
		// If no building.
		if (building == null)
		{
			// If we capped asteroid.
			if (belongsTo == player)
			{
				//building = Building(player, GameObject someModel);
				if (player == Master.Player)
				{
					this.building = new Building(player, this.prefabBuildingModel);
				}
				else if (player == Master.Enemy)
				{
					this.building = new Building(player, this.prefabEnemyBuildingModel);
				}
				else 
				{
					return false;
				}
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
	public bool attackBuilding (GameObject unit)
	{
		// If there is building to attack.
		if (this.building != null)
		{
			// If it belongs to our enemy.
			if (this.getPlayer(unit) != this.building.whoControlls())
			{
				unit.GetComponent<UnitController> ().setTarget(building.getBuilding());
				return true;
			}

		}
		return false;
	}

	// Attack the building on asteroid by player with his units.
	public bool attackBuilding (List<GameObject> units)
	{
		// If there is building to attack.
		if (this.building != null)
		{
				
			foreach(GameObject unit in units)
			{
				if (this.getPlayer(unit) != this.building.whoControlls())
				{
					unit.GetComponent<UnitController> ().setTarget(this.building.getBuilding());
				}
			
			}
			return true;
		}
		return false;
	}

	// On building destruction.
	public bool destroyBuilding ()
	{
		if (this.building != null)
		{
			this.building.destroyBuilding();
			this.building = null;
			return true;
		}
		return false;
	}

	// In this function we check the status of the building.
	private void handleBuilding ()
	{
		if (this.building != null)
		{
			// Check if building is being built?
			if (this.building.isBuilded())
			{
				this.building.updateOnCreation (Time.deltaTime);
			}
			else if (this.building.isBuilt())
			{
				// If no life left.
				if (this.building.getHP() < 0.0f)
				{
					this.destroyBuilding();
				}
			}
		}
	}

	public void unitKilled (GameObject unit)
	{
		if (unit.tag == "Player")
		{
			this.units.Remove(unit);
		}
		else if (unit.tag == "Enemy")
		{
			this.enemies.Remove(unit);
		}
	}


	public Building getBuilding ()
	{
		return this.building;
	}
	
}
