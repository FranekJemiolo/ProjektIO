using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidController : MonoBehaviour {

	public float timeCapped;
	// The range from which we can capture an asteroid, square size.
	public float captureRangeSqr = 4900.0f;


	private List<GameObject> units;
	private List<GameObject> enemies;

	private enum Master{Player, Enemy, None};

	// Who owns the asteroid.
	private Master belongsTo;
	// Who is currently capping.
	private Master capping;


	// Use this for initialization
	void Start () 
	{
		belongsTo = Master.None;
		capping = Master.None;
		units = new List<GameObject> ();
		enemies = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnTriggerEnter (Collider other) 
	{
		Debug.Log (other + "Entering" + other.gameObject.tag);
		Master whoAmI = Master.None;
		if (other.gameObject.tag == "Enemy") 
		{
			enemies.Add (other.gameObject);
			whoAmI = Master.Enemy;
		} 
		else if (other.gameObject.tag == "Unit") 
		{
			units.Add (other.gameObject);
			whoAmI = Master.Player;
		}
		// If no one holds the asteroid. Also TO DO: time passed to cap etc.
		if (belongsTo == Master.None)
		{
			// If no one currently capping the asteroid.
			if (capping == Master.None)
			{
				capping = whoAmI;
			}
			else if (capping != whoAmI)
			{
				capping = Master.None;
			}
			// Else do nothing because capping == whoAmI;
		}
		else if (belongsTo != whoAmI)
		{
			if (whoAmI == Master.Player)
			{
				// No enemies we can convert for us.
				if (enemies.Count == 0)
				{
					capping = whoAmI;
				}
				// else we must kill them.
			}
			else if (whoAmI == Master.Enemy)
			{
				// No enemies we can convert for us.
				if (enemies.Count == 0)
				{
					capping = whoAmI;
				}
				// else we must kill them.
			}
		}
		// Else do nothing we already have it.
	}
	void OnTriggerStay (Collider other) 
	{
		//Debug.Log ("Staying" + units);
	}
	void OnTriggerExit (Collider other) 
	{
		Debug.Log (other + "Leaving" + other.gameObject.tag);
		if (other.gameObject.tag == "Enemy") 
		{
			enemies.Remove (other.gameObject);
		} 
		else if (other.gameObject.tag == "Unit") 
		{
			units.Remove (other.gameObject);
		}
	}
}
