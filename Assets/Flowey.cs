using UnityEngine;
using System.Collections;

public class Flowey : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var movement = transform.position.y * 0.02f;
		transform.Translate (movement, 0, 0);
			
		if (kill_me) {
			gameObject.SetActive (false);
		}
	}

	public static bool kill_me = false;
}
