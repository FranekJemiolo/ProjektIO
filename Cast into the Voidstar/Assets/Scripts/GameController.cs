using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;






// This script holds the main game api and contains main game loop.
public class GameController : MonoBehaviour 
{


	public enum UnitType {Mothership = 0, Destroyer, Cruiser, Fighter, Transporter, Shuttle, Unknown};
	
	public enum GameState {NotStarted, Playing, Paused, PlayerWon, EnemyWon, Draw};

	public enum Who {Player = 0, Enemy};

	// Unit type translation.
	public static UnitType getUnitType (GameObject ob)
	{
		if (ob == null)
		{
			return UnitType.Unknown;
		}
		if (ob.name == "Mothership")
		{
			return UnitType.Mothership;
		}
		else if (ob.name == "Destroyer")
		{
			return UnitType.Destroyer;
		}
		else if (ob.name == "Cruiser")
		{
			return UnitType.Cruiser;
		}
		else if (ob.name == "Fighter")
		{
			return UnitType.Fighter;
		}
		else if (ob.name == "Transporter")
		{
			return UnitType.Transporter;
		}
		else if (ob.name == "Shuttle")
		{
			return UnitType.Shuttle;
		}
		else
		{
			return UnitType.Unknown;
		}
	}

	// This class is used to represent players in-game variables.
	public class Player
	{
		// Unit type translation.
		public static UnitType getUnitType (GameObject ob)
		{
			if (ob == null)
			{
				return UnitType.Unknown;
			}
			if (ob.name == "Mothership")
			{
				return UnitType.Mothership;
			}
			else if (ob.name == "Destroyer")
			{
				return UnitType.Destroyer;
			}
			else if (ob.name == "Cruiser")
			{
				return UnitType.Cruiser;
			}
			else if (ob.name == "Fighter")
			{
				return UnitType.Fighter;
			}
			else if (ob.name == "Transporter")
			{
				return UnitType.Transporter;
			}
			else if (ob.name == "Shuttle")
			{
				return UnitType.Shuttle;
			}
			else
			{
				return UnitType.Unknown;
			}
		}
		// Points to victory of a player.
		private float points;

		// Credits for buying units.
		private float credits;
		
		// Now will be the list of units of some types.
		
		private const int typeOfUnits = 7;
		private List<GameObject>[] units;
		
		public Player () 
		{
			this.units = new List<GameObject>[typeOfUnits];
			for (int i = 0; i < typeOfUnits; i++)
			{
				this.units[i] = new List<GameObject>();
			}
		}
		
		
		
		// Some methods for accessing fields.
		
		// Should be called when creating new object for a player.
		public void addUnit(GameObject ob)
		{
			if (ob != null)
			{
				UnitType t = getUnitType(ob); 
				this.units[(int)t].Add(ob);
			}
		}
		
		// Should be called on destroying an object.
		public void removeUnit(GameObject ob)
		{
			if (ob != null)
			{
				UnitType t = getUnitType(ob); 
				this.units[(int)t].Remove(ob);
			}
		}

		public int getUnitCount (UnitType type)
		{
			return this.units[(int) type].Count;
		}

		
		public void setPoints (float p)
		{
			this.points = p;
		}
		
		public float getPoints ()
		{
			return this.points;
		}

		public void setCredits (float c)
		{
			this.credits = c;
		}

		public float getCredits ()
		{
			return this.credits;
		}

		
	}
	// Space for game variables.
	// The amount of credits that every player gets on start.
	private const float START_CREDITS = 1000.0f;
	// How many points player gets for holding an asteroid.
	private const float pointsForAsteroid = 1.0f;
	// How many points player gets for killing enemy units.
	private const float pointsForKilling = 1.0f;
	// How many credits player gets for building on asteroid.
	private const float creditsPerBuilding = 1.0f;
	// How many points one has to get to win.
	private const float winPoints = 1000.0f;
	// Array of players. Player[0] - is our player. Player[1] - enemy.
	// This is only for know, because in future it will be possible to have
	// many players at once.
	private const int numberOfPlayers = 2;
	public Player[] players;
	// Array of Asteroids.
	public GameObject[] asteroids;
	// In which state of game are we?
	private GameState gameState = GameState.NotStarted;

	// How many times we update the values?
	private float updateRate = 1.0f;
	private float timePassed = 0.0f;

	// Array of unit prefabs - gameObject to instantiate.
	public const int typeOfUnits = 7;
	public float[] unitCosts = new float[typeOfUnits];
	public float[] buildTime = new float[typeOfUnits];
	public GameObject[] unitPrefabs = new GameObject[typeOfUnits];
	public GameObject[] enemyPrefabs = new GameObject[typeOfUnits];

	// What are actually selected units? Are they a group?
	// We know that only player can select units for now
	// so any portability features will not be considered for now.
	public enum Selected {None, Single, Group}
	private List<GameObject> selectedUnits;
	private Selected selected;

	// This variables are the spawn points for the players.
	public GameObject[] massRelays = new GameObject[numberOfPlayers];
	// How much time is used for building?
	private float[] timeToSpawn = new float[numberOfPlayers];
	// This will help the gui, are the player building a unit?
	private bool[] isBuilded = new bool[numberOfPlayers];
	// What units are builded?
	private UnitType[] builded = new UnitType[numberOfPlayers];
	// The constraints of map.
	private float minX = 0.0f;
	private float maxX = 400.0f;
	private float minZ = 0.0f;
	private float maxZ = 400.0f;

	// Just for getting the mothership of a player.
	public GameObject mothership;

	public GameState getGameState() {
		return gameState;
	}

	public float[] TimeToSpawn {
		get { return timeToSpawn; }
	}


	private GUI gui;

	// End of game variables.


    // DEBUG VARS

	private bool first = false;
	private float ten = 0.0f;

    // END OF DEBUG VARS


	void Start () 
	{
		players = new Player[numberOfPlayers];
		timeToSpawn[0] = 0.0f;
		timeToSpawn[1] = 0.0f;
		isBuilded[0] = false;
		isBuilded[1] = false;
		// Initialize players variables.
		players[0] = new Player();
		players[1] = new Player();
		players[0].setPoints(0.0f);
		players[1].setPoints(0.0f);
		players[0].setCredits(START_CREDITS);
		players[1].setCredits(START_CREDITS);
		// Get all the asteroids so we can handle them.
		asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
		// Set the game state to playing.
		gameState = GameState.Playing;

		gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUI>();
		if (gui == null) {
			Debug.LogError("NOGUI");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		timePassed += Time.deltaTime;
		if (this.timePassed > this.updateRate)
		{
			this.handleGame();
			this.timePassed = 0.0f;
			Debug.Log ("Player points is :" + players[0].getPoints());
			Debug.Log ("Enemy points is :" + players[1].getPoints());
		}

		//Debug.Log (timePassed);
		//Debug.Log ("Player points is :" + players[0].getPoints());
		//Debug.Log ("Enemy points is :" + players[1].getPoints());
		// Debug test.
		//if (!first)
		//{
			//this.navigateTo (GameObject.FindGameObjectWithTag ("Unit"), new Vector3 (50, 0, 100));
			//List<GameObject> lista = new List<GameObject>();
			//lista.Add(GameObject.FindGameObjectWithTag ("Player"));

			//this.buildUnit(Who.Player, UnitType.Mothership);

			//this.attackUnit(lista[0], GameObject.FindGameObjectWithTag("Cube"));
			//first = true;
		//}
		//ten++;
		//this.moveCamera(new Vector3(this.transform.position.x+(ten/10),50.0f,this.transform.position.z+(ten/10)));
		// End of debug test.
	}

	// Moving one unit into position.
	public void navigateTo (GameObject unit, Vector3 pos)
	{
		unit.GetComponent<UnitController> ().moveTo (pos);
	}

	// Moving many units to position.
	public void navigateTo (List<GameObject> units, Vector3 pos)
	{
		foreach (GameObject unit in units) 
		{
			this.navigateTo(unit, pos);
		}
	}

	// Unit attacks enemy unit.
	public void attackUnit (GameObject unit, GameObject enemy)
	{
		unit.GetComponent<UnitController> ().setTarget (enemy);
	}

	// Many units attack enemy unit.
	public void attackUnit (List<GameObject> units, GameObject enemy)
	{
		foreach (GameObject unit in units) 
		{
			this.attackUnit (unit, enemy);
		}
	}

	// Moves main camera into desired position.
	public void moveCamera(Vector3 pos)
	{
		GameObject.FindGameObjectWithTag("MainCamera").transform.position = pos;
	}

	// Should be called before the destruction of enemy unit.
	public void givePointsForKilling (GameObject unit)
	{
		if (unit.tag == "Enemy")
		{
			players[0].setPoints(players[0].getPoints() + pointsForKilling);
		}
		else if (unit.tag == "Player")
		{
			players[1].setPoints(players[1].getPoints() + pointsForKilling);
		}
	}

	public void handleBuilding (AsteroidController ac)
	{
		AsteroidController.Building bld = ac.getBuilding();
		if (bld != null)
		{
			if (bld.whoControlls() == AsteroidController.Master.Player)
			{
				players[0].setCredits(players[0].getCredits() + (creditsPerBuilding * this.timePassed));
			}
			else if (bld.whoControlls() == AsteroidController.Master.Enemy)
			{
				players[1].setCredits(players[1].getCredits() + (creditsPerBuilding * this.timePassed));
			}
		}
	}


	public void handleAsteroid (GameObject asteroid)
	{
		AsteroidController asteroidController = asteroid.GetComponent<AsteroidController>();
		// Handle points for holding the asteroid.
		float morePoints = pointsForAsteroid * this.timePassed;
		if (asteroidController.belongsTo == AsteroidController.Master.Player)
		{
			//Debug.Log ("Here");
			players[0].setPoints(players[0].getPoints() + morePoints);
		}
		else if (asteroidController.belongsTo == AsteroidController.Master.Enemy)
		{
			players[1].setPoints(players[1].getPoints() + morePoints);
		}
		// Handle resources gained from asteroid.
		this.handleBuilding(asteroidController);
	}

	public void handleGame ()
	{
		if (gameState == GameState.Playing)
		{
			// Handle points for asteroids.
			// Every asteroid gives some points for winning.
			foreach (GameObject Asteroid in this.asteroids)
			{
				this.handleAsteroid(Asteroid);
			}

			// Handle builded unit.
			this.handleBuildedUnits();

			// Check if player has won the game.
			if ((players[0].getPoints() >= winPoints) && (players[1].getPoints() < players[0].getPoints()))
			{
				// Yaaay player has won the game. Set the game state to player won and announce it.
				gameState = GameState.PlayerWon;
			}
			// Check if enemy has won the game.
			else if ((players[1].getPoints() >= winPoints) && (players[0].getPoints() < players[1].getPoints()))
			{
				// :< Player has lost. Setting the game state and do something with it.
				gameState = GameState.EnemyWon;
			}
			// Check if the game result is draw.
			else if ((players[0].getPoints() >= winPoints) && (players[1].getPoints() >= winPoints))
			{
				// The game ended in draw. Do something about it.
				gameState = GameState.Draw;
			}
			// Else we are playing the game - no need to change the game state.
		}
		// Stop game and show some screens.
		else if (gameState == GameState.PlayerWon)
		{
			gui.ShowGameOverScreen("You have won!");
		}
		else if (gameState == GameState.EnemyWon)
		{
			gui.ShowGameOverScreen("Enemy has won!");
		}
		else if (gameState == GameState.Draw)
		{
			gui.ShowGameOverScreen("Draw!");
		}
		// The game is paused.
		else
		{
			// Do something or nothing.
		}


	}

	// Handles unit deletion from every list and set that it would be.
	public void deleteUnit (GameObject unit)
	{
		if (unit.tag == "Enemy")
		{
			this.players[1].removeUnit(unit);
		}
		else if (unit.tag == "Player")
		{
			this.players[0].removeUnit(unit);
		}
		foreach (GameObject astero in asteroids)
		{
			astero.GetComponent<AsteroidController>().unitKilled(unit);
		}
		GameObject[] AIs = GameObject.FindGameObjectsWithTag("AICore");
		foreach (GameObject ai in AIs)
		{
			ai.GetComponent<AIController>().deleteUnit(unit);
		}
		GameObject[] agents = GameObject.FindGameObjectsWithTag("EnemyAgent");
		foreach (GameObject agent in agents)
		{
			agent.GetComponent<EnemyController>().removeUnit(unit);
		}
	}

	// Returns vector around some vector.
	private Vector3 getVectorAround (Vector3 vec)
	{
		Vector3 ret = new Vector3 (vec.x + Random.Range(-50.0f, 50.0f), vec.y, vec.z + Random.Range(-50.0f, 50.0f));
		if (ret.x < this.minX)
		{
			ret.x = minX;
		}
		else if (ret.x > this.maxX)
		{
			ret.x = maxX;
		}

		if (ret.z < this.minZ)
		{
			ret.z = minZ;
		}
		else if (ret.z > this.maxZ)
		{
			ret.z = maxZ;
		}
		return ret;
	}

	// Spawn units near it's mass relay.
	private void spawnUnit (int i)
	{
		timeToSpawn[i] = 0.0f;
		isBuilded[i] = false;
		GameObject unit;
		// If a player?
		if (i == 0)
		{
			unit = Instantiate (unitPrefabs[(int) builded[i]], 
			                    this.getVectorAround(massRelays[i].transform.position),
			                    Quaternion.identity) as GameObject;
			players[i].addUnit(unit);
		}
		// If an enemy?
		else if (i == 1)
		{
			unit = Instantiate (enemyPrefabs[(int) builded[i]], 
			                    this.getVectorAround(massRelays[i].transform.position),
			                    Quaternion.identity) as GameObject;
			players[i].addUnit(unit);
			GameObject.FindGameObjectWithTag("AICore").GetComponent<AIController>().units.Add(unit);
		}
	}

	/*
	private void fallingDownTemporaryFix() {
		if (mothership.transform.position.y != 0) {
			Debug.LogError("Falling down");
			mothership.transform.Translate(mothership.transform.position.x, 0, mothership.transform.position.z, Space.World);
			mothership.transform.Rotate (0, mothership.transform.rotation.x, 0);
		}
	}
	*/

	// We are handling here the updates on building units.
	private void handleBuildedUnits ()
	{
		for (int i = 0; i < numberOfPlayers; i++)
		{
			// Check if a player is building?
			if (isBuilded[i])
			{
				timeToSpawn[i] += this.timePassed;
				if (timeToSpawn[i] > buildTime[(int) builded[i]])
				{
					this.spawnUnit(i);
				}
			}
		}
	}

	// Builds a unit in desired position and of desired type of unit.
	public bool buildUnit (Who who, UnitType typeOfUnit)
	{
		if ((who == Who.Player) || (who == Who.Enemy))
		{
			if ((!isBuilded[(int) who]) && (players[(int) who].getCredits() >= unitCosts[(int) typeOfUnit]))
			{
				players[(int) who].setCredits(players[(int) who].getCredits() - unitCosts[(int) typeOfUnit]);
				// Set building to wanted unit and start counting down.
				timeToSpawn[(int) who] = 0.0f;
				isBuilded[(int) who] = true;
				builded[(int) who] = typeOfUnit;
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;

	}


	// Deselects all currenlty selected units.
	public void deselectAllUnits ()
	{
		this.selectedUnits.Clear ();
		this.selected = Selected.None;
	}


	// Deselect given unit.
	public void deselectUnit (GameObject unit)
	{
		this.selectedUnits.Remove (unit);
		if (selectedUnits.Count == 0)
		{
			this.selected = Selected.None;
		}
		else if (selectedUnits.Count == 1)
		{
			this.selected = Selected.Single;
		}
		// Else still group selected.
	}


	// Selects one unit, deselects if any other units were currently selected.
	public void selectUnit (GameObject unit)
	{
		// We are discarding all currently selected units.
		this.deselectAllUnits ();
		this.selectedUnits.Add(unit);
		this.selected = Selected.Single;
	}

	// Adds to a group of selected units.
	public void addToGroup (GameObject unit)
	{
		this.selectedUnits.Add(unit);
		if (this.selectedUnits.Count == 1)
		{
			this.selected = Selected.Single;
		}
		else if (this.selectedUnits.Count > 1)
		{
			this.selected = Selected.Group;
		}
	}


	// Returns selected unit, if only one unit is selected.
	// Else it returns null.
	public GameObject getSelectedUnit ()
	{
		if (this.selected == Selected.None)
		{
			return null;
		}
		else if (this.selected == Selected.Single)
		{
			return this.selectedUnits.First();
		}
		else
		{
			// This is not included in Selected.None because maybe
			// in future we would want the first unit to get Selected.
			return null;
		}
	}

	// Returns list of all selected units.
	public List<GameObject> getSelectedGroup ()
	{
		return this.selectedUnits;
	}

	// You can ask if a player is building a unit
	public bool isPlayerBuilding (Who who)
	{
		return isBuilded[(int) who];
	}

	// You can get the unit he is building.
	public UnitType whatPlayerBuilds (Who who)
	{
		return builded[(int) who];
	}

	// You can get his time left to build.
	public float playerTimeLeftToBuild (Who who)
	{
		float res = buildTime[(int) builded[(int) who]] - timeToSpawn[(int) who];
		if (res < 0.0f)
		{
			res = 0.0f;
		}
		return res;
	}

	// This functions returns player's points
	public float getPlayerPoints (Who who)
	{
		return this.players[(int) who].getPoints();
	}

	// This functions returns player's credits
	public float getPlayerCredits (Who who)
	{
		return this.players[(int) who].getCredits();
	}

	// Returns the points for victory.
	public float getWinPoints ()
	{
		return winPoints;
	}

	// If mothership was killed - the player has ultimately lost.
	public void mothershipKilled(Who who)
	{
		if (who == Who.Enemy)
		{
			this.gameState = GameState.PlayerWon;
		}
		else if (who == Who.Player)
		{
			this.gameState = GameState.EnemyWon;
		}
	}

	public GameObject getMothership()
	{
		return this.mothership;
	}

}
