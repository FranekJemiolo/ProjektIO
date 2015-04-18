using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	private bool first = false;
	private float ten = 0.0f;
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!first)
		{
			//this.navigateTo (GameObject.FindGameObjectWithTag ("Unit"), new Vector3 (50, 0, 100));
			this.attackUnit(GameObject.FindGameObjectsWithTag ("Unit"), GameObject.FindGameObjectWithTag("Cube"));
			first = true;
		}
		ten++;
		//this.moveCamera(new Vector3(this.transform.position.x+(ten/10),this.transform.position.y,this.transform.position.z+(ten/10)));
	}

	// Moving one unit into position.
	public void navigateTo (GameObject unit, Vector3 pos)
	{
		unit.GetComponent<UnitController> ().moveTo (pos);
	}

	// Moving many units to position.
	public void navigateTo (GameObject[] units, Vector3 pos)
	{
		foreach (GameObject unit in units) 
		{
			this.navigateTo(unit, pos);
		}
	}

	// Unit attacks enemy unit.
	public void attackUnit (GameObject unit, GameObject enemy)
	{
		unit.GetComponent<UnitController> ().setTarget (enemy);
	}

	// Many units attack enemy unit.
	public void attackUnit (GameObject[] units, GameObject enemy)
	{
		foreach (GameObject unit in units) 
		{
			this.attackUnit (unit, enemy);
		}
	}

	public void moveCamera(Vector3 pos)
	{
		this.transform.position = pos;
	}


}
