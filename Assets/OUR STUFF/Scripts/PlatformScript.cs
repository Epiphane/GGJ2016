using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D jumper) {
		//make the parent platform ignore the jumper
//		Debug.Log(jumper);
//		var platform = transform.parent;
//		Physics.IgnoreCollision(jumper.GetComponent<BoxCollider2D>(), platform.GetComponent<BoxCollider2D>());
	}

	void OnTriggerExit2D (Collider2D jumper) {
		//reset jumper's layer to something that the platform collides with
		//just in case we wanted to jump throgh this one
		jumper.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
//		Debug.Log(jumper);
//
//		//re-enable collision between jumper and parent platform, so we can stand on top again
//		var platform = transform.parent;
//		Physics.IgnoreCollision(jumper.GetComponent<CharacterController>(), platform.GetComponent<BoxCollider>());
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
