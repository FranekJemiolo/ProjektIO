using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidController : MonoBehaviour {

	public float timeCapped;
	// The range from which we can capture an asteroid, square size.
	public float captureRangeSqr = 4900.0f;


	private List<GameObject> units;
	private List<GameObject> enemies;


	// Use this for initialization
	void Start () 
	{
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
		if (other.gameObject.tag == "Enemy") 
		{
			enemies.Add (other.gameObject);
		} 
		else if (other.gameObject.tag = "Unit") 
		{
			units.Add (other.gameObject);
		}
	}
	void OnTriggerStay (Collider other) 
	{
		//Debug.Log (other + "Staying" + other.gameObject.tag);
	}
	void OnTriggerExit (Collider other) 
	{
		Debug.Log (other + "Leaving" + other.gameObject.tag);
		if (other.gameObject.tag == "Enemy") 
		{
			enemies.Remove (other.gameObject);
		} 
		else if (other.gameObject.tag = "Unit") 
		{
			units.Remove (other.gameObject);
		}
	}
}
