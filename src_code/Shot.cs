using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {

	public float fireForce = 1000.0f;
	public float damage = 1.0f;


	public void fire ()
	{
		Debug.Log (this.transform.forward);
		this.GetComponent<Rigidbody>().AddForce(this.transform.forward * fireForce);
	}

	// Use this for initialization
	void Start () {
	}


	// Update is called once per frame
	void Update () {

	}
}
