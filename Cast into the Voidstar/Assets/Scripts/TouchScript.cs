using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class TouchScript : MonoBehaviour {
	
	//list of selected units
	private List<GameObject> selected;
	
	private bool swipeOn = false;
	
	public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
	public float orthoZoomSpeed = 0.5f;
	
	private float distanceMoved = 0;
	private float sumXaxis = 0;
	private float sumYaxis = 0;
	
	private bool isOverUI = false;
	
	GameController gameController;
	
	RaycastHit hit;
	
	GameObject asteroid;
	
	Camera cam;
	public float camSpeed = 0.4f;
	
	public void moveCamera(Vector2 pos)
	{
		Vector3 mov = new Vector3(pos.x*camSpeed, cam.transform.position.y, pos.y*camSpeed);
		cam.transform.position = mov;
	}

	private GUIclass gui;
	private AsteroidController asteroidController;

	// variables for selection multiple units
	public Texture2D selectionHighlight;
	public static Rect selectionBox = new Rect(0,0,0,0);

	/*public static Vector2 FixedTouchDelta(this Touch aTouch)
        {
                float dt = Time.deltaTime / aTouch.deltaTime;
                if (dt == 0 || float.IsNaN(dt) || float.IsInfinity(dt))
                        dt = 1.0f;
                return aTouch.deltaPosition * dt;
        }*/
	
	public bool isSelectionEmpty(){
		return selected.Count == 0;
	}
	
	/// <summary>
	/// Deselecting units.
	/// This method will be ONLY available from GUI (as button click);
	/// Unlike in any other RTS, we can't distinct between LMB and RMB
	/// </summary>
	public void DeselectUnits() {
		foreach (GameObject unit in selected) {
			if (unit != null)
			{
				Debug.Log ("Deselecting" +  unit.name );
				// TODO: graphical behaviour
				Projector proj =  unit.transform.GetChild(2).gameObject.GetComponent<Projector>();
				Debug.Log (proj);//.enabled = false;
				proj.enabled = false;
			}

		}
		selected.Clear();
	}
	
	public void DeselectUnit(GameObject toDeselect) {
		if (toDeselect != null)
		{
			selected.Remove (toDeselect);
			Debug.Log ("Deselecting" +  toDeselect.name );
			Projector proj =  toDeselect.transform.GetChild(2).gameObject.GetComponent<Projector>();
			Debug.Log (proj);//.enabled = false;
			proj.enabled = false;

		}

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
	public void Select(GameObject unit){
		if (unit != null)
		{
			Debug.Log ("Selecting" + unit.name);
			Projector proj = unit.transform.GetChild(2).gameObject.GetComponent<Projector>();
			Debug.Log (proj);//.enabled = false;
			proj.enabled = true;
			selected.Add (unit);

		}

		// TODO:
		// change graphical behaviour of unit i.e. highlighted.
		// projector - additive shader (shadow material)
		
	}
	
	//selecting units (adding them on list)
	void Select(List<GameObject> units){
		foreach (GameObject unit in units) {
			Select(unit);
		}
	}
	// selected getter
	List<GameObject> GetSelectedUnits() {
		return selected;
	}
	
	//get asteroid
	public GameObject getAsteroid(){
		return asteroid;
	}


	public bool isSelected(GameObject checkUnit)
	{
		return selected.Contains(checkUnit);
	}
	
	public static float invertWithScreenHeight(float y)
	{
		return Screen.height - y;
	}

	private Touch touchZero;
	private Touch touchOne;
	private Touch touchTwo;


	// Use this for initialization
	// find main camera and gamecontroller
	void Start ()
	{
		selectionBox = new Rect (0,0,0,0);
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		selected = new List<GameObject> ();
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		asteroid = null;
		gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIclass>();
	}

	private void getAsteroidInfo() {
			asteroidController = asteroid.GetComponent<AsteroidController>();
			AsteroidController.Building b = asteroidController.getBuilding();
			
			if (b != null) {
				if (b.isBuilded()) {
					gui.DrawBuildMine("Being built", false);
				} else if (b.isBuilt()) {
					gui.DrawBuildMine("Already built", false);
				} else {
					gui.DrawBuildMine("Build Mine", true);
                }
            } else {
				gui.DrawBuildMine("Build Mine", true);
            }
    }

	private void deselectAsteroid() {
		asteroid = null;
		gui.DrawBuildMine("No asteroid", false);
	}

	public void createBuilding() {
		asteroidController.createBuilding(AsteroidController.Master.Player);
	}
    
	// Update is called once per frame
	void Update ()
	{
		foreach( Touch checkTouch in Input.touches ){
			if( EventSystem.current.IsPointerOverGameObject( checkTouch.fingerId ) ){
				isOverUI = true;
				break;
			}
		}
		if ( !isOverUI ) {
			//single touch - detect taps (select) and drags (move camera)
			switch(Input.touchCount) {
			case 1:
				touchZero = Input.GetTouch(0);
				//checking if any unit is selected
				
				switch( touchZero.phase ) {
				case TouchPhase.Began:
					Ray ray1 = Camera.main.ScreenPointToRay(touchZero.position);
					
					Debug.Log("touchphase began");
					// we have to detect what we are hitting - it must be unit(s)
					if(Physics.Raycast(ray1, out hit, 1000)){
						//Debug.DrawRay (touch.position, hit.transform.position);
						GameObject recipient = hit.transform.gameObject;
						Debug.Log("hitted "+recipient.name);
					}
					break;
				case TouchPhase.Moved:
					distanceMoved += Mathf.Abs(touchZero.deltaPosition.x) + Mathf.Abs(touchZero.deltaPosition.y);
					sumXaxis += touchZero.deltaPosition.x;
					sumYaxis += touchZero.deltaPosition.y;
					
					if( distanceMoved>10 ){
						Debug.Log("swipe");
						swipeOn = true;
					}
					if(swipeOn){
						gameController.moveCamera( cam.transform.position - new Vector3(sumXaxis * camSpeed, 0, sumYaxis * camSpeed) );
						sumXaxis = 0;
						sumYaxis = 0;
					}
					break;
				case TouchPhase.Ended:
					if( !swipeOn ){
						
						Debug.Log("Heelo");
						
						if( !isSelectionEmpty() ){
							//gotta use tags, temp solution - type
							if( hit.collider.gameObject.tag == "Terrain" ){
								Debug.Log("is selected, move to");
								Vector3 pos = hit.point;
								gameController.navigateTo(selected, pos);
								deselectAsteroid();
							}
							else if( hit.collider.gameObject.tag == "Player" ){
								Debug.Log("is selected, move to");
								//SendMessage( "navigateTo", selected );
								Vector3 pos = hit.point;
								gameController.navigateTo(selected, pos);
								deselectAsteroid();
							}
							else if( hit.collider.gameObject.tag == "Enemy" ){
								Debug.Log("is selected, attack");
								gameController.attackUnit( selected, hit.transform.gameObject );
								deselectAsteroid();
							}
							else if( hit.collider.gameObject.tag == "AsteroidBody" ){
								asteroid = hit.collider.gameObject.transform.parent.gameObject;
								getAsteroidInfo();
							}
							else{
								deselectAsteroid();
							}
						}
						else{
							if( hit.collider.gameObject.tag == "Player" ){
								Debug.Log("is not selected, select friendly unit");
								if (hit.collider.gameObject.name != "Mothership")
								{
									Select( hit.transform.gameObject );
								}
								asteroid = null;
							}
							else if( hit.collider.gameObject.tag == "AsteroidBody" ){
								asteroid = hit.collider.gameObject.transform.parent.gameObject;
							}
							else{
								asteroid = null;
							}
						}
					}
					distanceMoved = 0;
					swipeOn = false;
					sumXaxis = 0;
					sumYaxis = 0;
					Debug.Log ("Touchphase ended");
					break;
				}
				break;
				//resize camera (move by Y axis)
			case 2:
				//new code implements traditional RTS selection box

				if (GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIclass>().isCommandModeON)
				{
					touchZero = Input.GetTouch(0);
					touchOne = Input.GetTouch(1);
					
					float largerX = 0;
					float smallerX = 0;
					
					float largerY = 0;
					float smallerY = 0;
					
					if (touchZero.position.x > touchOne.position.x)
					{
						largerX = touchZero.position.x;
						smallerX = touchOne.position.x;
					}
					else
					{
						largerX = touchOne.position.x;
						smallerX = touchZero.position.x;
					}
					
					if ( touchZero.position.y > touchOne.position.y)
					{
						largerY = touchZero.position.y;
						smallerY = touchOne.position.y;
					}
					else
					{
						largerY = touchOne.position.y;
						smallerY = touchZero.position.y;
					}
					
					Debug.Log (smallerX+" smallerX");
					Debug.Log (largerX+" largerX");
					Debug.Log (smallerY+" smallerY");
					Debug.Log (largerY+" largerY");
					
					if( touchZero.phase==TouchPhase.Began || touchOne.phase==TouchPhase.Began ||
					   touchZero.phase==TouchPhase.Moved || touchOne.phase==TouchPhase.Moved )
					{
						selectionBox = new Rect(largerX, invertWithScreenHeight(smallerY), smallerX - largerX,
						                        smallerY - largerY );
					}
					if( touchZero.phase==TouchPhase.Ended || touchOne.phase==TouchPhase.Ended )
					{
						selectionBox = new Rect(0,0,0,0);
						
						largerX = 0;
						smallerX = 0;
						
						largerY = 0;
						smallerY = 0;
					}
					
					/** old code implementing pinch to zoom
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);
				
				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position  - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
				
				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				
				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				
				// If the camera is orthographic...
				if ( cam.orthographic == true)
				{
					// ... change the orthographic size based on the change in distance between the touches.
					cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
					
					// Make sure the orthographic size never drops below zero.
					cam.orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, 0.1f);
				}
				else
				{
					// Otherwise change the field of view based on the change in distance between the touches.
					cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
					
					// Clamp the field of view to make sure it's between 0 and 180.
					cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 10.0f, 90.0f);
				}*/

				}

				break;
			case 3:
				/** New code implementing pinch to zoom with 3 touches.
				 * 	with 3 touches we want to calculate distance between them and move camera by
					Y axis proportionally
				 */
				
				touchZero = Input.GetTouch(0);
				touchOne = Input.GetTouch(1);
				touchTwo = Input.GetTouch(2);
				
				float distance, changedDistance, ratio;
				
				distance = 0.0f;
				
				//calculate distance
				distance = 	Vector2.Distance(touchZero.position, touchOne.position) +
					Vector2.Distance(touchOne.position, touchTwo.position);
				
				changedDistance = 	Vector2.Distance(touchZero.position-touchZero.deltaPosition, touchOne.position-touchOne.deltaPosition) +
					Vector2.Distance(touchOne.position-touchOne.deltaPosition, touchTwo.position-touchTwo.deltaPosition);
				
				if( distance != 0 ){
					ratio = changedDistance/distance;
				}
				else{
					ratio = 1;
				}
				gameController.moveCamera( new Vector3(cam.transform.position.x, cam.transform.position.y * ratio, cam.transform.position.z));
				//change camera position
				
				
				/*else if(touchZero.phase==TouchPhase.Ended ||
				        touchOne.phase==TouchPhase.Ended ||
				        touchTwo.phase==TouchPhase.Ended)
				{
					//end, reinit variables
				}*/
				
				break;
			}
		}
		isOverUI = false;
	}
	
	private void OnGUI()
	{
		if (Input.touchCount == 2) 
		{
			GUI.color = new Color(1, 1, 1, 0.5f);
			GUI.DrawTexture( selectionBox, selectionHighlight );
		}
	}
}