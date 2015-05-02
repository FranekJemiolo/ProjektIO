using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// This script is used to control every enemy unit - ai core.
public class AIController : MonoBehaviour 
{
	// This class describes ai actual state - it's knowledge and such.
	public class AIState
	{
		// These two values define how ai controls units.
		// Are they more aggresive or defensive?
		public float aggresivness;
		public float defensivness;

		// How many units ai controlled at that time?
		public int numOfUnits;
		// How many units enemy controlled at that time? -- seen units.
		public int enemyUnits;

		// How many points AI has?
		public float points;
		// How many points enemy has?
		public float enemyPoints;

		// How many asteroid we are holding at that time.
		public float numOfAsteroids;
		// How many credits we have.
		public float credits;

		public AIState (float a, float d, int nu, int eu, 
		                float p, float ep, float na, float c)
		{
			this.aggresivness = a;
			this.defensivness = d;
			this.numOfUnits = nu;
			this.enemyUnits = eu;
			this.points = p;
			this.enemyPoints = ep;
			this.numOfAsteroids = na;
			this.credits = c;
		}


		// Returns the value for the state.
		// Big numbers represent positive state - in which we win
		// Lower numbers worse state.
		public float calculateState ()
		{
			float stateValue = 0.0f;
			float unitCost = 100.0f;
			float pointsToWin = 1000.0f;
			// More aggresive = more units and more attacking.
			stateValue += aggresivness * (numOfUnits - enemyUnits);
			stateValue += aggresivness * (points - enemyPoints);
			stateValue += defensivness * ((points - enemyPoints) / 2);
			stateValue += (credits / unitCost);
			stateValue += aggresivness * (numOfAsteroids);
			stateValue += defensivness * ((numOfAsteroids / 2) + (points - enemyPoints)/2);
			if (points > (pointsToWin / 2))
			{
				if (enemyPoints > points)
				{
					stateValue += aggresivness * (enemyPoints - points);
				}
			}
			return stateValue;

		}
	}

	// Our recorded AIStates. - 
	// We can have many because they contain only simple values.
	private HashSet<AIState> knowledgeBase;
	// Actual factor of aggresivness.
	private float aggresivness;
	// Actual factor of defensivness.
	private float defensivness;
	// How many points we have?
	private float points;
	// How many points enemy have?
	private float enemyPoints;
	// How many credits we have?
	private float credits;
	// What is my tag?
	private string myTag;
	// What is enemy tag?
	private string enemyTag;

	// The set of units which we control.
	public HashSet<GameObject> units;
	// The set of enemy objects we see, or we have seen.
	// AI is very intelligent - once it seen an object it 
	// tracks it till the end.
	public HashSet<GameObject> enemies;
	// The number of asteroids we control.
	private int asteroids;
	// Our command center - mothership.
	public GameObject mothership;
	// The rate of updating values.
	private float valueRate = 1.0f;
	private float lastValuesCheck = 0.0f;
	// The rate of recording knowledge.
	private float gatherRate = 2.0f;
	// How many time passed from last update.
	private float timePassed = 0.0f;
	// The rate for updating parameters.
	private float updateRate = 5.0f;
	// How many time passed since last update.
	private float timeFromUpdate = 0.0f;

	// Game controller will be needed.
	GameController gameController;


	public bool requestedBackup (GameObject go)
	{
		// Find every free unit, if none - return false
		int backup = 0;
		foreach (GameObject unit in units)
		{
			if (unit != null)
			{
				EnemyController uc = unit.GetComponentInChildren<EnemyController>();
				if (uc.agent.getTarget() == null)
				{
					uc.agent.setTarget(go);
					backup++;
				}
			}
		}
		if (backup == 0)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	// This function records current gameState.
	public void recordKnowledge ()
	{
		AIState state = new AIState (this.aggresivness,
		     this.defensivness, this.units.Count,
			 this.enemies.Count, this.points,
			 this.enemyPoints, this.asteroids,
			 this.credits);
		this.knowledgeBase.Add(state);

	}

	public void reportedEnemy (GameObject go)
	{
		this.enemies.Add(go);
	}


	public void changeUnitsBehaviour ()
	{
		foreach (GameObject unit in units)
		{
			unit.GetComponentInChildren<EnemyController>().updateAgent(this.aggresivness, this.defensivness);
		}
	}

	// Do some mutation on load.
	public void loadParams (AIState state)
	{
		this.aggresivness = state.aggresivness + Random.Range(-1.0f, 1.0f);
		this.defensivness = state.defensivness + Random.Range(-1.0f, 1.0f);
	}

	// This function should be called not so often.
	// It will go through game states and rethink it's actions.
	public void rethinkMyActions ()
	{
		Debug.Log("I am thinking");
		AIState bestState = null;
		float current = this.knowledgeBase.Last().calculateState();
		float max = current;
		foreach (AIState state in this.knowledgeBase)
		{
			current = state.calculateState();
			if (current > max)
			{
				max = current;
				bestState = state;
			}
		}
		// Adjust parameters.
		if (max > this.knowledgeBase.Last().calculateState())
		{
			this.loadParams (bestState);
		}
		Debug.Log ("Aggresivnes is " + this.aggresivness);
		Debug.Log ("Defenisvness is " + this.defensivness);
		this.buildUnit();
	}


	void Start () 
	{
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		units = new HashSet<GameObject>();
		enemies = new HashSet<GameObject>();
		asteroids = 0;
		knowledgeBase = new HashSet<AIState>();
		// Set the parameters.
		this.aggresivness = 1.0f;
		this.defensivness = 1.0f;
		this.points = 0.0f;
		this.enemyPoints = 0.0f;
		this.myTag = this.transform.gameObject.tag;
		if (myTag == "Player")
		{
			this.enemyTag = "Enemy";
		}
		else if (myTag == "Enemy")
		{
			this.enemyTag = "Player";
		}
		this.recordKnowledge ();
	}
	

	void Update () 
	{
		this.lastValuesCheck += Time.deltaTime;
		if (this.lastValuesCheck > this.valueRate)
		{
			this.lastValuesCheck = 0.0f;
			this.updateValues();
		}

		this.timePassed += Time.deltaTime;
		if (this.timePassed > this.gatherRate)
		{
			this.timePassed = 0.0f;
			this.recordKnowledge();
		}

		this.timeFromUpdate += Time.deltaTime;
		if (this.timeFromUpdate > this.updateRate)
		{
			this.timeFromUpdate = 0.0f;
			this.rethinkMyActions();
		}
	}

	public void updateValues()
	{
		if (myTag == "Player")
		{
			this.points = gameController.players[0].getPoints();
			this.enemyPoints = gameController.players[1].getPoints();
			this.credits = gameController.players[0].getCredits();
			int count = 0;
			foreach (GameObject astero in gameController.asteroids)
			{
				AsteroidController ac = astero.GetComponent<AsteroidController>();
				if (ac.belongsTo == AsteroidController.Master.Player)
				{
					count++;
				}
			}
			this.asteroids = count;
		}
		else if (myTag == "Enemy")
		{
			this.points = gameController.players[1].getPoints();
			this.enemyPoints = gameController.players[0].getPoints();
			this.credits = gameController.players[1].getCredits();
			int count = 0;
			foreach (GameObject astero in gameController.asteroids)
			{
				AsteroidController ac = astero.GetComponent<AsteroidController>();
				if (ac.belongsTo == AsteroidController.Master.Enemy)
				{
					count++;
				}
			}
			this.asteroids = count;
		}
	}

	public void deleteUnit (GameObject unit)
	{
		if (unit.tag == myTag)
		{
			this.units.Remove(unit);
		}
		else if (unit.tag == enemyTag)
		{
			this.enemies.Remove(unit);
		}
	}
	

	// Here the ai will have to decide which unit it needs the most - and builds it.
	// No time for it now :<
	public void buildUnit ()
	{
		int[] enemyUnits = new int[GameController.typeOfUnits];
		int[] myUnits = new int[GameController.typeOfUnits];
		int myIndex = 0;
		int enemyIndex = 1;
		if (myTag == "Player")
		{
			myIndex = 0;
			enemyIndex = 1;
		}
		else if (myTag == "Enemy")
		{
			myIndex = 1;
			enemyIndex = 0;
		}
		GameController.UnitType t = GameController.UnitType.Mothership;
		for (int i = 0; i < GameController.typeOfUnits-1; i++)
		{
			enemyUnits[i] = this.gameController.players[enemyIndex].getUnitCount(t);
			myUnits[i] = this.gameController.players[myIndex].getUnitCount(t);
			t++;
		}
		// Now decide what to build.
		t = GameController.UnitType.Mothership;
		t++;
		// To be continued.
		this.gameController.buildUnit(GameController.Who.Enemy, 
		                         GameController.UnitType.Mothership);
	}
}
