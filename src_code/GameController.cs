using UnityEngine;
using System.Collections;
using System.Collections.Generic;






// This script holds the main game api and contains main game loop.
public class GameController : MonoBehaviour 
{


	public enum UnitType {Mothership = 0, Destroyer, Cruiser, Fighter, Transporter, Shuttle, Unknown};
	
	public enum GameState {NotStarted, Playing, Paused, PlayerWon, EnemyWon, Draw};



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


	// End of game variables.


	private bool first = false;
	private float ten = 0.0f;
	void Start () 
	{
		players = new Player[numberOfPlayers];
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
	}
	
	// Update is called once per frame
	void Update () 
	{
		timePassed += Time.deltaTime;
		if (this.timePassed > this.updateRate)
		{
			this.timePassed = 0.0f;
			this.handleGame();
		}
		//Debug.Log (timePassed);
		//Debug.Log ("Player points is :" + players[0].getPoints());
		//Debug.Log ("Enemy points is :" + players[1].getPoints());
		// Debug test.
		if (!first)
		{
			//this.navigateTo (GameObject.FindGameObjectWithTag ("Unit"), new Vector3 (50, 0, 100));
			List<GameObject> lista = new List<GameObject>();
			lista.Add(GameObject.FindGameObjectWithTag ("Player"));

			this.attackUnit(lista[0], GameObject.FindGameObjectWithTag("Cube"));
			first = true;
		}
		ten++;
		//this.moveCamera(new Vector3(this.transform.position.x+(ten/10),this.transform.position.y,this.transform.position.z+(ten/10)));
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
		this.transform.position = pos;
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

	public void handleAsteroid (GameObject asteroid)
	{
		AsteroidController asteroidController = asteroid.GetComponent<AsteroidController>();
		// Handle points for holding the asteroid.
		float morePoints = pointsForAsteroid;
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
		}
		else if (gameState == GameState.EnemyWon)
		{
		}
		else if (gameState == GameState.Draw)
		{
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


}
