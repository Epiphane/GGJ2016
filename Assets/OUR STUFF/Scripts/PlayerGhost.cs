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
		GetComponent<CircleCollider2D> ().isTrigger = true;

		Color c = GetComponent<PlayerMovement>().playerColor;
		c.a = 0.5f;

		GetComponent<PlayerMovement>().player_img.GetComponent<SpriteRenderer> ().color = c;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (captor) {
			Vector3 distToCaptor = transform.position - captor.transform.position;

			Quaternion pointToDad = Quaternion.FromToRotation (Vector3.left, distToCaptor);
			GetComponent<PlayerMovement> ().tether.transform.rotation = pointToDad;

			var shrinkMe = distToCaptor.magnitude / leashLength;
			var meh = GetComponent<PlayerMovement> ().tether.transform.localScale;
			meh.x = 0.4f * shrinkMe;
			GetComponent<PlayerMovement> ().tether.transform.localScale = meh;

			if (distToCaptor.magnitude > leashLength) {
				Vector3 movement = distToCaptor * leashLength / distToCaptor.magnitude;

				transform.position = captor.transform.position + movement;
			}
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
		movement.captive = null;

		transform.position = spawn.position;

		GetComponent<PlayerMovement> ().tether.GetComponent<SpriteRenderer> ().enabled = false;

		GameObject.Find ("AirConsoleLogic").GetComponent<AirconsoleLogic> ().Unlock (movement);
	}

}
