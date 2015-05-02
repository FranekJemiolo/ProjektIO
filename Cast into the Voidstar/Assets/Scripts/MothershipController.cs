using UnityEngine;
using System.Collections;

public class MothershipController : MonoBehaviour {
	
	// Rotation speed in degrees.
	private float rotateSpeed = 10.0f;
	
	// The speed and acceleration
	private float speed = 10.0f;
	private float sqrSpeed = 100.0f;
	private float acceleration = 100.0f;
	
	// The life points of the unit.
	private float hitPoints = 100.0f;
	
	// Number of attacks
	public const int numberOfProjectiles = 2;
	
	// The firing rate of our unit. How many seconds between attacks.
	private float[] firingRate = new float[numberOfProjectiles];
	// How many time passed from last firing.
	private float[] timePassed = new float[numberOfProjectiles];
	
	// The strength of the attack of the unit.
	private float[] attackForce = new float[numberOfProjectiles];
	
	// The gameobject of a shot.
	public GameObject[] projectiles = new GameObject[numberOfProjectiles];
	
	// The transform object where to spawn shots.
	public Transform shotSpawn;
	
	// The rigidbody attached to the unit.
	private Rigidbody myRigidbody;
	
	
	void Start () 
	{
		this.myRigidbody = this.GetComponent<Rigidbody>();
	}
	
	
	void Update () 
	{

	}
	
	void FixedUpdate ()
	{
	}
	
	
	// Rotates mothership to right.
	public void rotateRight (float timeDelta)
	{
		this.transform.Rotate (0.0f, (this.rotateSpeed * timeDelta), 0.0f);
	}
	
	// Rotates mothership to left.
	public void rotateLeft (float timeDelta)
	{
		this.transform.Rotate (0.0f, -(this.rotateSpeed * timeDelta), 0.0f);
	}
	
	// Moves mothership forward.
	public void moveForward (float timeDelta)
	{
		
		//if (this.myRigidbody.velocity.sqrMagnitude < sqrSpeed)
		//{
		Debug.Log(this.transform.forward);
		//Vector3 res = this.transform.position - this.engines.transform.position;
		//this.myRigidbody.AddForceAtPosition(this.transform.forward * this.acceleration * timeDelta, this.engines.transform.position);
		Vector3 relativeVector = new Vector3 (Mathf.Abs(this.transform.forward.x), Mathf.Abs(this.transform.forward.y), Mathf.Abs(this.transform.forward.z));
		this.myRigidbody.AddRelativeForce(relativeVector * this.acceleration * timeDelta);
		//this.myRigidbody.AddForce(this.transform.forward * this.acceleration * timeDelta);
		//}
		// else we already reached max speed.
	}
	
	// Moves mothership backwards.
	public void moveBackward (float timeDelta)
	{
		//if (this.myRigidbody.velocity.sqrMagnitude < sqrSpeed)
		//{
		Vector3 relativeVector = new Vector3 (Mathf.Abs(this.transform.forward.x), Mathf.Abs(this.transform.forward.y), Mathf.Abs(this.transform.forward.z));
		this.myRigidbody.AddRelativeForce(-(relativeVector * this.acceleration * timeDelta));
		//}
		// else we reached max speed.
	}
	
	// Fires one of mothership's attacks.
	public bool fire (int whichAttack)
	{
		if (timePassed[whichAttack] > firingRate[whichAttack])
		{
			GameObject firedShot;
			firedShot = Instantiate(projectiles[whichAttack], shotSpawn.position, this.transform.rotation) as GameObject;
			firedShot.GetComponent<Shot>().setOwner(this.gameObject);
			firedShot.GetComponent<Shot>().fire(this.attackForce[whichAttack]);
			return true;
		}
		else
		{
			return false;
		}
		
	}
	
	// Check if an attack is available?
	public bool attackAvailable (int attackNumber)
	{
		return (timePassed[attackNumber] > firingRate[attackNumber]);
	}
	
	// Returns time to when the wanted attack will be available.
	public float getTimeToAttack (int attackNumber)
	{
		float res = firingRate[attackNumber] - timePassed[attackNumber];
		if (res < 0.0f)
		{
			res = 0.0f;
		}
		return res;
	}
	
	
	public float getAttackForce (int whichAttack)
	{
		return this.attackForce[whichAttack];
	}
	
	public float getHitPoints ()
	{
		return this.hitPoints;
	}
    
    public void setHitPoints (float f)
    {
        this.hitPoints = f;
    }
    
    // Called when unit reach below 0 hp.
    // Destroys object and does some preparation
    // plus explosions later.
    public void die ()
    {
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.givePointsForKilling(this.transform.gameObject);
        gc.deleteUnit(this.transform.gameObject);
        
        // This script is only available for player now.
        gc.mothershipKilled (GameController.Who.Player);
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(this.transform.gameObject);
	}
}
