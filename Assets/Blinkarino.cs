using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Blinkarino : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	float cooldown = 2.8f;
	bool showing = false;

	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;

		if (cooldown <= 0.0f) {
			cooldown = 0.3f;

			showing = !showing;

			if (showing) {
				GetComponent<Text> ().color = Color.red;
			}
			else {
				GetComponent<Text> ().color = Color.clear;
			}
		}
	}
}
