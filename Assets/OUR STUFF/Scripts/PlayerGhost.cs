using UnityEngine;
using System.Collections;

public class PlayerGhost : MonoBehaviour {

	public GameObject captor;

	public Transform spawn;

	public float leashLength = 0.8f;

	public Transform runeDude;
	public bool sacrificing = false;
	private float shakeTime = 0;
	private float shakeSeed = 0;

	// Use this for initialization
	void Start () {
		runeDude = GameObject.Find ("Gate").transform;

		shakeSeed = Random.value * 100;
	}

	void Update() {
		GetComponent<CircleCollider2D> ().isTrigger = true;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (sacrificing) {
			PlayerMovement movement = GetComponent<PlayerMovement> ();
			movement.tether.GetComponent<SpriteRenderer> ().enabled = false;
			movement.bubble.GetComponent<SpriteRenderer> ().enabled = false;

			Vector3 distToRuneDude = runeDude.position - transform.position;

			if (distToRuneDude.magnitude > 2) {
				transform.position = transform.position + distToRuneDude / 2;
			} else if (shakeTime < 2) {
				shakeTime += Time.fixedDeltaTime;

				transform.position = runeDude.position + (2 - shakeTime) / 3 * (new Vector3 (Mathf.Cos (shakeSeed + shakeTime * 60), Mathf.Sin (shakeSeed + shakeTime * 73), 0));
			} else {
				Respawn ();

				runeDude.GetComponent<GateScript> ().GainSoul ();
				sacrificing = false;
			}
		}
		else if (captor) {
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

	public void Sacrifice() {
		sacrificing = true;
		transform.Find ("player_img").transform.localScale = Vector3.one;

		PlayerMovement movement = GetComponent<PlayerMovement> ();
		movement.tether.GetComponent<SpriteRenderer> ().enabled = false;
		movement.bubble.GetComponent<SpriteRenderer> ().enabled = false;

		if (movement.streak) {
			movement.streak.Reset ();
		}

		if (movement.captive) {
			movement.captive.captor = captor;
		}

		captor.GetComponent<PlayerMovement> ().captive = movement.captive;
		captor = null;
		movement.captive = null;

		shakeTime = 0;
	}

	public void Respawn() {
		PlayerMovement movement = GetComponent<PlayerMovement> ();

		movement.enabled = true;
		enabled = false;

		transform.position = spawn.position;

		GameObject.Find ("AirConsoleLogic").GetComponent<AirconsoleLogic> ().Unlock (movement);
	}

}
