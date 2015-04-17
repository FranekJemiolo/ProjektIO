using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {	
	// Update is called once per frame
	RaycastHit hit;

	void Update () {

		foreach (Touch touch in Input.touches) {

			Debug.Log("before touchcount, after foreach");
			//single touch - detect taps (select) and drags (move camera)
			if(Input.touchCount==1){
				Ray ray1 = Camera.main.ScreenPointToRay(touch.position);
				Debug.Log("before hit");
				if(Physics.Raycast(ray1, out hit, 1000)){
					GameObject recipient = hit.transform.gameObject;
					Debug.Log("hitted "+recipient.name);
					recipient.SendMessage("singleSelect", recipient);
				}


			}

			//resize camera (move by Y axis)
			else if(Input.touchCount==2){

			}
			//selecting groups
			else if(Input.touchCount==3){

			}
		}
	}
}
