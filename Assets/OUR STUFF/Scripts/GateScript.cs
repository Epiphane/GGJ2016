﻿using UnityEngine;
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

	public void GainSoul() {
		Debug.Log ("Got a soul");
		bool didCoolThing = runeTop.Gain (1);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
