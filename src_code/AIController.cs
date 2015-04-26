using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
	private List<AIState> knowledgeBase;
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

	// The list of units which we control.
	private List<GameObject> units;
	// The list of enemy objects we see, or we have seen.
	// AI is very intelligent - once it seen an object it 
	// tracks it till the end.
	private HashSet<GameObject> enemies;
	// The list of asteroids we control.
	private List<GameObject> asteroids;
	// Our command center - mothership.
	private GameObject mothership;
	// The rate of recording knowledge.
	private float gatherRate = 2.0f;
	// How many time passed from last update.
	private float timePassed = 0.0f;
	// The rate for updating parameters.
	private float updateRate = 10.0f;
	// How many time passed since last update.
	private float timeFromUpdate = 0.0f;



	// This function records current gameState.
	public void recordKnowledge ()
	{
		AIState state = new AIState (this.aggresivness,
		     this.defensivness, this.units.Count,
			 this.enemies.Count, this.points,
			 this.enemyPoints, this.asteroids.Count,
			 this.credits);
		this.knowledgeBase.Add(state);

	}

	public void reportedEnemy (GameObject go)
	{
		this.enemies.Add(go);
	}

	// Do some mutation on load.
	public void loadParams (AIState state)
	{
		this.aggresivness = state.aggresivness + Random.Range(0.0f, 1.0f);
		this.defensivness = state.defensivness + Random.Range(0.0f, 1.0f);
	}

	// This function should be called not so often.
	// It will go through game states and rethink it's actions.
	public void rethinkMyActions ()
	{
		AIState bestState = null;
		float max = 0.0f;
		float current = 0.0f;
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
		if (max > 0.0f)
		{
			this.loadParams (bestState);
		}
	}


	void Start () 
	{
		units = new List<GameObject>();
		enemies = new HashSet<GameObject>();
		asteroids = new List<GameObject>();
		knowledgeBase = new List<AIState>();
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
}
