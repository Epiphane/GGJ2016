using UnityEngine;
using System.Collections;

public class PlayerGhost : MonoBehaviour {

	public GameObject captor;

	public Transform spawn;

	public float leashLength = 1.5f;

	// Use this for initialization
	void Start () {
	}

	void Update() {
		Color c = GetComponent<PlayerMovement>().playerColor;
		c.a = 0.5f;

		GetComponent<SpriteRenderer> ().color = c;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 distToCaptor = transform.position - captor.transform.position;

		if (distToCaptor.magnitude > leashLength) {
			Vector3 movement = distToCaptor * leashLength / distToCaptor.magnitude;

			transform.position = captor.transform.position + movement;
		}
	}

	public void Respawn() {
		PlayerMovement movement = GetComponent<PlayerMovement> ();

		movement.enabled = true;
		enabled = false;

		if (movement.captive) {
			movement.captive.captor = captor;
		}


		captor.GetComponent<PlayerMovement> ().captive = movement.captive;
		captor = null;

		transform.position = spawn.position;

		GameObject.Find ("AirConsoleLogic").GetComponent<AirconsoleLogic> ().Unlock (movement);
	}

}
