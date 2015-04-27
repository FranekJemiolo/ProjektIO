using UnityEngine;
using System.Collections;

// This script describes behaviour of single shot.
public class Shot : MonoBehaviour 
{
	// With what force it will be fired?
	public float fireForce = 10000.0f;
	// What is the damage?
	public float damage = 1.0f;
	// How long the shot lives.
	private float lifeSpan = 10.0f;
	private float timeLeft = 0.0f;

	public void fire (float dmg)
	{
		Debug.Log (this.transform.forward);
		this.damage = dmg;
		this.GetComponent<Rigidbody>().AddForce(this.transform.forward * fireForce);
	}

	// Use this for initialization
	void Start () 
	{
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

	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			UnitController uc = collision.gameObject.GetComponent<UnitController>();
			uc.setHitPoints(uc.getHitPoints() - this.damage);
			if (uc.getHitPoints() <= 0.0f)
			{
				uc.die();
			}
		}
		else if (collision.gameObject.tag == "Player")
		{
			UnitController uc = collision.gameObject.GetComponent<UnitController>();
			uc.setHitPoints(uc.getHitPoints() - this.damage);
			if (uc.getHitPoints() <= 0.0f)
			{
				uc.die();
			}
		}
		this.blowUp();
	}
}
