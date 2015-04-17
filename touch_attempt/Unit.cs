using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	private string a; 
	public Unit(string name) {
		this.a = name;
	}

	public string Name() {
		return this.a;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
