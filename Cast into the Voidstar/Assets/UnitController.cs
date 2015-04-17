using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {

	// The firing range of our unit. Range = 50.0f
	public float rangeSqr = 2500.0f;

	// The firing rate of our unit. How many seconds between attacks.
	public float firingRate = 2.0f;


	// Time needed for rotation so the unit faces enemy.
	public float damping = 2.0f;

	// The targeted unit which unit wants to attack.
	private GameObject target;


	// Unit navigation agent.
	private NavMeshAgent navAgent;

	// Use this for initialization
	void Start () 
	{
		//this.target = GameObject.FindGameObjectWithTag ("Cube");//GameObject.FindGameObjectWithTag ("Unit");
		this.navAgent = this.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void FixedUpdate ()
	{
		// We have to call an attack function - we must follow unit and attack it repeatedly.
		if (target) 
		{
			attack (target);
		}
	}

	public void setTarget (GameObject go)
	{
		this.target = go;
	}

	// Moves unit into desired position.
	public void moveTo (Vector3 pos)
	{
		navAgent.SetDestination (pos);
	}

	public void attack (GameObject go)
	{
		float distanceSqr = (this.transform.position - go.transform.position).sqrMagnitude;
		// Enemy is in the firing range.
		if (distanceSqr < this.rangeSqr) 
		{
			this.navAgent.ResetPath ();
			// Rotate towards enemy.
			Quaternion lookAtRotation = Quaternion.LookRotation
				(go.transform.position - this.transform.position);
			this.transform.rotation = Quaternion.Slerp
				(this.transform.rotation, lookAtRotation, Time.deltaTime/damping);
			// Alternative is:
			//this.transform.LookAt(go.transform.position, Vector3.back);

			// Do some firing.

		}
		// Follow the enemy!
		else
		{
			this.moveTo(go.transform.position);
		}

	}
}
