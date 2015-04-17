using UnityEngine;
using System.Collections;

public class GameC : MonoBehaviour {

	void Start () {
		this.navigateTo (GameObject.FindGameObjectsWithTag ("Unit"), new Vector3 (100, 0, 100));
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Moving one unit into position.
	public void navigateTo (GameObject unit, Vector3 pos)
	{
		NavMeshAgent navAgent;
		navAgent = unit.GetComponent<NavMeshAgent> ();
		navAgent.SetDestination (pos);

	}

	// Moving many units to position.
	public void navigateTo (GameObject[] units, Vector3 pos)
	{
		foreach (GameObject unit in units) {
			navigateTo(unit, pos);
		}
	}
}
