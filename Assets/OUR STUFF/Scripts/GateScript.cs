using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player" && other.GetComponent<PlayerMovement>().enabled) {
			// Give the captor some sweet points dude
			/* ... */

			other.GetComponent<PlayerMovement> ().CollectPoints ();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
