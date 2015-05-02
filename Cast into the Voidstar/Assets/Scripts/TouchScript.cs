using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TouchScript : MonoBehaviour {
	
	
	private List<GameObject> selected = new List<GameObject> ();
	
	private bool swipeOn = false;
	
	public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
	public float orthoZoomSpeed = 0.5f;
	
	private float distanceMoved = 0;
	private float sumXaxis = 0;
	private float sumYaxis = 0;

	GameController gameController;
	
	RaycastHit hit;



	Camera cam;
	public float camSpeed = 0.4f;
	
	public void moveCamera(Vector2 pos)
	{
		Vector3 mov = new Vector3(pos.x*camSpeed, cam.transform.position.y, pos.y*camSpeed);
		cam.transform.position = mov;
	}
	
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
	void Deselect() {
		foreach (GameObject unit in selected) {
			Debug.Log ("Deselecting" +  unit.name );
			// TODO: graphical behaviour
		}
		selected.Clear();
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
	void Select(GameObject unit){
		
		Debug.Log ("Selecting" + unit.name);
		
		selected.Add (unit);
		
		// TODO:
		// change graphical behaviour of unit i.e. highlighted.
		// projector - additive shader (shadow material)
		
	}
	
	void Select(List<GameObject> units){
		foreach (GameObject unit in units) {
			Select(unit);
		}
	}
	
	List<GameObject> GetSelectedUnits() {
		return selected;
	}
	
	// Use this for initialization
	void Start () 
	{
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//single touch - detect taps (select) and drags (move camera)
		switch(Input.touchCount) {
		case 1:
			Touch touch = Input.GetTouch(0);
			//checking if any unit is selected
			
			switch( touch.phase ) {
			case TouchPhase.Began:
				Ray ray1 = Camera.main.ScreenPointToRay(touch.position);
				Debug.Log("touchphase began");
				// we have to detect what we are hitting - it must be unit(s)
				if(Physics.Raycast(ray1, out hit, 1000)){
					GameObject recipient = hit.transform.gameObject;
					Debug.Log("hitted "+recipient.name);
				}
				break;
			case TouchPhase.Moved:
				distanceMoved += Mathf.Abs(touch.deltaPosition.x) + Mathf.Abs(touch.deltaPosition.y);
				sumXaxis += touch.deltaPosition.x;
				sumYaxis += touch.deltaPosition.y;
				
				if( distanceMoved>10 ){
					Debug.Log("swipe");
					swipeOn = true;
				}
				if(swipeOn){
					//SendMessage("moveCamera", Vector3(sumXaxis,0,sumYaxis), SendMessageOptions.DontRequireReceiver);
					Vector2 movement = new Vector2(sumXaxis, sumYaxis);
					Vector2 touchDeltaPosition = touch.deltaPosition;
					transform.Translate(-sumXaxis * camSpeed, -sumYaxis * camSpeed, 0);
					sumXaxis = 0;
					sumYaxis = 0;
				}
				break;
			case TouchPhase.Ended:
				if( !swipeOn ){
					
					if( !isSelectionEmpty() ){
						//gotta use tags, temp solution - type
						if( hit.collider.tag == "Terrain" ){
							Debug.Log("is selected, move to");
							Vector3 pos = hit.point;
							gameController.navigateTo(selected, pos);
						}
						else if( hit.collider.tag == "Player" ){
							Debug.Log("is selected, move to");
							//SendMessage( "navigateTo", selected );
							Vector3 pos = hit.point;
							gameController.navigateTo(selected, pos);
						}
						else if( hit.collider.tag == "Enemy" ){
							Debug.Log("is selected, attack");
						}
					}
					else{
						if( hit.collider.tag== "Player" ){
							Debug.Log("is not selected, select friendly unit");
							Select( hit.collider.gameObject );
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
				cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
			}
			break;
			//selecting groups
		case 3:
			break;
		}
	}
}
