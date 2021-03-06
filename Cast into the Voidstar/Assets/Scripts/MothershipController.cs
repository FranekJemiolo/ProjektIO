﻿using UnityEngine;
using System.Collections;
using ProgressBar;

public class MothershipController : MonoBehaviour {

	// Rotation speed in degrees.
	public float rotateSpeed = 10.0f;
	public float rotationConst = 10.0f;

	// The speed and acceleration
	public float speed = 0.5f;
	public float sqrSpeed = 100.0f;
	public float acceleration = 500.0f;
	
	// The life points of the unit.
	public float hitPoints = 100.0f;

	// Number of attacks
	public const int numberOfProjectiles = 2;

	// The firing rate of our unit. How many seconds between attacks.
	public float[] firingRate = new float[numberOfProjectiles];
	// How many time passed from last firing.
	private float[] timePassed = new float[numberOfProjectiles];

	// The strength of the attack of the unit.
	public float[] attackForce = new float[numberOfProjectiles];

	// The gameobject of a shot.
	public GameObject[] projectiles = new GameObject[numberOfProjectiles];
	
	// The transform object where to spawn shots.
	public Transform shotSpawn;

	// The rigidbody attached to the unit.
	private Rigidbody myRigidbody;

	private AudioSource[] sources;

	private AudioSource explosion_source;

	public GameObject explosion;
	
	// Mothership HP bar
	private ProgressBarBehaviour hpProgressBar;

	private float maxHitPoints;


	void Start () 
	{
		this.myRigidbody = this.GetComponent<Rigidbody>();
		sources = GetComponentsInChildren<AudioSource>();
		explosion_source = this.transform.FindChild("ExplosionAudio").GetComponent<AudioSource>();
		this.hpProgressBar = this.GetComponentInChildren<ProgressBarBehaviour>();
		this.maxHitPoints = hitPoints;
	}


	void Update () 
	{
		for (int i = 0; i < numberOfProjectiles; i++)
		{
			timePassed[i] += Time.deltaTime;
		}
		updateHPBar();
		
	}
	
	void FixedUpdate ()
	{
	}


	// Rotates mothership to right.
	public void rotateRight (float timeDelta)
	{
		this.transform.Rotate (0.0f, (this.rotateSpeed * timeDelta * rotationConst), 0.0f);
	}

	// Rotates mothership to left.
	public void rotateLeft (float timeDelta)
	{
		this.transform.Rotate (0.0f, -(this.rotateSpeed * timeDelta * rotationConst), 0.0f);
	}

	// Moves mothership forward.
	public void moveForward (float timeDelta)
	{

		//if (this.myRigidbody.velocity.sqrMagnitude < sqrSpeed)
		//{
			//Debug.Log(this.transform.forward);
			//Vector3 res = this.transform.position - this.engines.transform.position;
			//this.myRigidbody.AddForceAtPosition(this.transform.forward * this.acceleration * timeDelta, this.engines.transform.position);
		//Vector3 relativeVector = new Vector3 (Mathf.Abs(this.transform.forward.x), Mathf.Abs(this.transform.forward.y), Mathf.Abs(this.transform.forward.z));
		//this.myRigidbody.AddRelativeForce(relativeVector * this.acceleration * timeDelta);
		//this.myRigidbody.AddForce(this.transform.forward * this.acceleration * timeDelta);
		this.transform.position += this.transform.forward * this.speed;
		//}
		// else we already reached max speed.
	}

	// Moves mothership backwards.
	public void moveBackward (float timeDelta)
	{
		//if (this.myRigidbody.velocity.sqrMagnitude < sqrSpeed)
		//{
		//Vector3 relativeVector = new Vector3 (Mathf.Abs(this.transform.forward.x), Mathf.Abs(this.transform.forward.y), Mathf.Abs(this.transform.forward.z));
		//this.myRigidbody.AddRelativeForce((relativeVector * this.acceleration * timeDelta * (-1)));
		this.transform.position -= this.transform.forward * this.speed;
		//}
		// else we reached max speed.
	}

	// Fires one of mothership's attacks.
	public bool fire (int whichAttack)
	{
		if (timePassed[whichAttack] > firingRate[whichAttack])
		{
			timePassed[whichAttack] = 0;
			GameObject firedShot;
			firedShot = Instantiate(projectiles[whichAttack], shotSpawn.position, this.transform.rotation) as GameObject;
			sources[whichAttack].Play();
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
		Instantiate(explosion, this.transform.position, Quaternion.identity);
		foreach (Transform child in this.transform)
		{
			Destroy(child.gameObject);
		}
		Destroy(this.transform.gameObject);
	}

	private void updateHPBar()
	{
		hpProgressBar.Value = (float) ((hitPoints)/ maxHitPoints) * (100.0f);
	}
}
