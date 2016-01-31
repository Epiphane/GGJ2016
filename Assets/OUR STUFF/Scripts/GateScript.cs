using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

	public RuneTopScript runeTop;

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player" && other.GetComponent<PlayerMovement>().enabled) {
			// Give the captor some sweet points dude
			/* ... */

			other.GetComponent<PlayerMovement> ().CollectPoints ();
		}
	}

	private int unlockedGates = 0;

	public void GainSoul() {
		Debug.Log ("Got a soul");
		bool didCoolThing = runeTop.Gain (1);

		if (didCoolThing && unlockedGates < 8) {
			GameObject.Find ("Rune " + unlockedGates).GetComponent<RuneScript>().enabled = true; 
			unlockedGates++;
		}
	}

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 8; i++) {
			GameObject.Find ("Rune " + i).GetComponent<RuneScript> ().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
