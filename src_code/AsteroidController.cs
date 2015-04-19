using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidController : MonoBehaviour {
	
	// The range from which we can capture an asteroid, square size.
	public float captureRangeSqr = 4900.0f;


	private List<GameObject> units;
	private List<GameObject> enemies;

	private enum Master{Player, Enemy, None};

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

	}

	void OnTriggerEnter (Collider other) 
	{
		Debug.Log (other + "Entering" + other.gameObject.tag);
		if (other.gameObject.tag == "Enemy") 
		{
			enemies.Add (other.gameObject);
		} 
		else if (other.gameObject.tag == "Unit") 
		{
			units.Add (other.gameObject);
		}

	}
	void OnTriggerStay (Collider other) 
	{
		Master whoAmI = Master.None;
		if (other.gameObject.tag == "Enemy") 
		{
			whoAmI = Master.Enemy;
		} 
		else if (other.gameObject.tag == "Unit") 
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
		else if (other.gameObject.tag == "Unit") 
		{
			units.Remove (other.gameObject);
			/*if (units.Count == 0)
			{
				capping = Master.None;
				timeCapped = 0.0f;
			}*/
		}
	}
}
