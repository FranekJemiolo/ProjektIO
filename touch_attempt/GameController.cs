using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	private const bool debug = true;

	void test() {
		Unit a = new Unit ("a");
		Unit b = new Unit ("b");
		Unit c = new Unit ("c");

		Unit[] input = {new Unit ("a"), new Unit ("b"), new Unit ("c")};

		if (isSelectionEmpty ())
			Debug.Log ("Selecja pusta na poczatku");
		Select (new List<Unit> (input));

		if (!isSelectionEmpty ())
			Debug.Log ("Udalo sie wrzucic");

		Deselect ();
		if (isSelectionEmpty ()) 
			Debug.Log ("Udalo sie wyrzucic");


	}
	/// <summary>
	/// Collection of selected units
	/// </summary>
	private List<Unit> selectedUnits = new List<Unit>();
	
	public bool isSelectionEmpty(){
		return selectedUnits.Count == 0;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Deselecting units.
	/// This method will be ONLY available from GUI (as button click);
	/// Unlike in any other RTS, we can't distinct between LMB and RMB
	/// </summary>
	void Deselect() {
		foreach (Unit unit in selectedUnits) {
			if (debug)
				Debug.Log ("Deselecting" +  unit.Name());
			// TODO: graphical behaviour
		}
		selectedUnits.Clear();
	}

	/// <summary>
	/// Select the specified unit.
	/// When selecting unit, we are not disposing previously selected ones,
	/// since we don't have any distinction between order: Move To / Select.
	/// 
	/// This will be solved by TouchController
	/// 
	/// </summary>
	/// <param name="unit">Unit.</param>
	void Select(Unit unit){

		if (debug)
			Debug.Log ("Selecting" + unit.Name());

		selectedUnits.Add (unit);

		// TODO:
		// change graphical behaviour of unit i.e. highlighted.
		// projector - additive shader (shadow material)

	}

	void Select(List<Unit> units){
		foreach (Unit unit in units) {
			Select(unit);
		}
	}

	List<Unit> GetSelectedUnits() {
		return selectedUnits;
	}
}
