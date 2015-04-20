using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class is used to represent players in-game variables.
public class Player
{
	// Points to victory of a player.
	private float points;

	// Now will be the list of units of some types.
	private enum UnitType {Mothership = 0, Destroyer, Cruiser, Fighter, Transporter, Shuttle, Unknown};
	private const int typeOfUnits = 7;
	private List<GameObject>[] units;

	Player () 
	{
		List<GameObject>[] units = new List<GameObject>[typeOfUnits];
	}

	// Unit type translation.
	private UnitType getUnitType (GameObject ob)
	{
		if (ob.name == "Mothership")
		{
			return UnitType.Mothership;
		}
		else if (ob.name == "Destroyer")
		{
			return UnitType.Destroyer;
		}
		else if (ob.name == "Cruiser")
		{
			return UnitType.Cruiser;
		}
		else if (ob.name == "Fighter")
		{
			return UnitType.Fighter;
		}
		else if (ob.name == "Transporter")
		{
			return UnitType.Transporter;
		}
		else if (ob.name == "Shuttle")
		{
			return UnitType.Shuttle;
		}
		else
		{
			return UnitType.Unknown;
		}
	}

	// Some methods for accessing fields.

	// Should be called when creating new object for a player.
	public void addUnit(GameObject ob)
	{
		UnitType t = getUnitType(ob); 
		units[(int)t].Add(ob);
	}

	// Should be called on destroying an object.
	public void removeUnit(GameObject ob)
	{
		UnitType t = getUnitType(ob); 
		units[(int)t].Add(ob);
	}


	public void setPoints (float p)
	{
		this.points = p;
	}

	public float getPoints ()
	{
		return this.points;
	}

}


public class GameController : MonoBehaviour 
{
	// Space for game variables.
	private const int numberOfPlayers = 2;
	Player[] players = new Player[numberOfPlayers];

	// End of game variables.
	private bool first = false;
	private float ten = 0.0f;
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		// Debug test.
		if (!first)
		{
			//this.navigateTo (GameObject.FindGameObjectWithTag ("Unit"), new Vector3 (50, 0, 100));
			this.attackUnit(GameObject.FindGameObjectsWithTag ("Unit"), GameObject.FindGameObjectWithTag("Cube"));
			first = true;
		}
		ten++;
		//this.moveCamera(new Vector3(this.transform.position.x+(ten/10),this.transform.position.y,this.transform.position.z+(ten/10)));
		// End of debug test.
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

	// Moves main camera into desired position.
	public void moveCamera(Vector3 pos)
	{
		this.transform.position = pos;
	}


}
