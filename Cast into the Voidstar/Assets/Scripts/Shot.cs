using UnityEngine;
using System.Collections;

// This script describes behaviour of single shot.
public class Shot : MonoBehaviour 
{
	// With what force it will be fired?
	public float fireForce = 1000.0f;
	// What is the damage?
	public float damage = 50.0f;
	// How long the shot lives.
	private float lifeSpan = 10.0f;
	private float timeLeft = 0.0f;

	private GameObject owner;

	public void fire (float dmg)
	{
		Debug.Log (this.owner);
		//this.damage = dmg;
		this.GetComponent<Rigidbody>().AddForce(this.transform.forward * fireForce);
	}

	// Use this for initialization
	void Start () 
	{
	}

	public void setOwner (GameObject go)
	{
		this.owner = go;
	}


	// Update is called once per frame
	void Update () 
	{
		timeLeft += Time.deltaTime;
		if (timeLeft > lifeSpan)
		{
			this.blowUp();
		}
	}

	private void blowUp ()
	{
		Destroy(this.gameObject);
	}

	void OnTriggerEnter (Collider other)
	{
		Debug.Log(other.name);
		if (other.gameObject == this.owner)
		{
			return;
		}
		//Debug.Log ("Collision with " + other.gameObject);
		if (other.gameObject.tag == "Enemy")
		{
			UnitController uc = other.gameObject.GetComponent<UnitController>();
			uc.setHitPoints(uc.getHitPoints() - this.damage);
			if (uc.getHitPoints() <= 0.0f)
			{
				uc.die();
			}
			this.blowUp();
		}
		else if (other.gameObject.tag == "Player")
		{
			if (other.gameObject.name != "Mothership")
			{
				UnitController uc = other.gameObject.GetComponent<UnitController>();
				uc.setHitPoints(uc.getHitPoints() - this.damage);
				if (uc.getHitPoints() <= 0.0f)
				{
					uc.die();
				}
			}
			else
			{
				MothershipController mc = other.gameObject.GetComponent<MothershipController>();
				mc.setHitPoints(mc.getHitPoints() - this.damage);
				if (mc.getHitPoints() <= 0.0f)
				{
					mc.die();
				}
			}

			this.blowUp();
		}
		else if (other.gameObject.tag == "Building")
		{
			AsteroidController ac = other.gameObject.transform.parent.gameObject.GetComponent<AsteroidController>();
			ac.getBuilding().setHP (ac.getBuilding().getHP() - this.damage);
		}

	}
}
