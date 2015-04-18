using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {	
	// Update is called once per frame
	RaycastHit hit;
	private float distanceMoved = 0;

	void Update () {
		//single touch - detect taps (select) and drags (move camera)
		switch(Input.touchCount) {
		case 1:
			Touch touch = Input.GetTouch(0);
			switch(touch.phase) {
			case TouchPhase.Began:
				Ray ray1 = Camera.main.ScreenPointToRay(touch.position);
				Debug.Log("touchphase began");
				// we have to detect what we are hitting - it must be unit(s)
				if(Physics.Raycast(ray1, out hit, 1000)){
					GameObject recipient = hit.transform.gameObject;
					Debug.Log("hitted "+recipient.name);
					//if( recipient.GetType() == typeof(Unit) )
					recipient.SendMessage("select", recipient, SendMessageOptions.DontRequireReceiver);
				}
				break;
			case TouchPhase.Moved:
				distanceMoved += Mathf.Abs(touch.deltaPosition.x) + Mathf.Abs(touch.deltaPosition.y);
				if( distanceMoved>10 ){
					Debug.Log("swipe");
				}
				break;
			case TouchPhase.Ended:
				distanceMoved = 0;
				Debug.Log ("Touchphase ended");
				break;
			}
			break;
		//resize camera (move by Y axis)
		case 2:
			Touch touch1 = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);
			if( touch1.phase==TouchPhase.Began && touch2.phase==TouchPhase.Began ){

			}
			else if( touch1.phase==TouchPhase.Moved || touch2.phase==TouchPhase.Moved ){

			}
			else if( touch1.phase==TouchPhase.Ended || touch2.phase==TouchPhase.Ended ){

			}
			break;
		//selecting groups
		case 3:
			break;
		}
	}
}