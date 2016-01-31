using UnityEngine;
using System.Collections;

public class RuneScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
	}

	private float fade = 0;

	// Update is called once per frame
	void Update () {
		if (fade < 1) {
			fade = Mathf.Min (fade + Time.deltaTime, 1);

			GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, fade);
		}
	}
}
