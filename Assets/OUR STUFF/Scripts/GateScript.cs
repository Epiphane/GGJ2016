using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other) {
		Debug.Log (other.tag);
		if (other.tag == "Player" && other.GetComponent<PlayerGhost>().enabled) {
			// Give the captor some sweet points dude
			/* ... */

			other.GetComponent<PlayerGhost> ().Respawn ();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
