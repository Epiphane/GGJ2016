using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public KeyCode movementKey;
	public float speed = 12.0f;
	public GameObject particles;

	public int related_device_id = 0;

	public float cooldown = 0.0f;

	public bool dashing = false;

	private bool wantsToDash = false;

	public Color playerColor = new Color(1.0f, 1.0f, 1.0f);

	private GameObject arrow;

	public PlayerGhost captive;

	void Start() {
		arrow = transform.Find ("arrow_parent").gameObject;
	}

	public void Unlock() {
		enabled = true;

		PlayerGhost ghost = GetComponent<PlayerGhost>();
		ghost.enabled = false;

		if (ghost.captor) {
			ghost.captor.GetComponent<PlayerMovement>().captive = null;
		}

		ghost.captor = null;
	}

	public void StartDashing() {
		wantsToDash = true;
	}

	public void StopDashing() {
		wantsToDash = false;
	}

	public void Dash() {
		float angle;
		Vector3 outVec;

		arrow.transform.rotation.ToAngleAxis (out angle, out outVec);

//		if (cooldown <= 0.0f) {
			angle = Mathf.Deg2Rad * angle;

			var vel = GetComponent<Rigidbody2D> ().velocity;
			vel.x = Mathf.Sin (angle) * speed * outVec.z;
			vel.y = Mathf.Cos (angle) * -speed;

			GetComponent<Rigidbody2D> ().velocity = vel;

			cooldown = 0.3f;
			dashing = true;
//		}
	}

	// Update is called once per frame
	void Update () {

		if (wantsToDash) {
			Dash();
		}

		if (cooldown <= 0.0f) {
			arrow.transform.RotateAround (arrow.transform.position, Vector3.forward, 3.0f);
		}

		if (GetComponent<Rigidbody2D> ().velocity.sqrMagnitude < 20) {
			dashing = false;
		}
	
		playerColor.a = 1.0f;
		GetComponent<SpriteRenderer> ().color = playerColor;

		cooldown -= Time.deltaTime;

		if (cooldown >= 0.0f && !dashing) {
			GetComponent<SpriteRenderer> ().color = new Color (0.0f, 0.0f, 1.0f);
		}
			
		if (dashing) {
			GetComponent<CircleCollider2D> ().isTrigger = false;
		} else {
			GetComponent<CircleCollider2D> ().isTrigger = true;
		}
	}

	public void CollectPoints() {
		while (captive != null) {
			/* Get them sweet points duuuuude */
			Debug.Log ("Point!");

			captive.Respawn ();
			// This will set captive to something else
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.tag == "Player") {
			if (dashing) {

				// Collided with player...
				if (coll.GetComponent<PlayerMovement> ().dashing) {
					// They were dashing! Bounce away.
					var diffX = coll.transform.position.x - transform.position.x;
					var diffY = coll.transform.position.y - transform.position.y;

					GetComponent<Rigidbody2D>().velocity = new Vector2 (diffX, diffY) * speed;
					coll.GetComponent<Rigidbody2D>().velocity = new Vector2 (diffX, diffY) * -speed;
				} else {
					// They weren't dashing! DESTROY THEM

					// Go to the last element in the list
					PlayerMovement cursor = this;
					while (cursor.captive != null) {
						cursor = cursor.captive.GetComponent<PlayerMovement> ();

						// Abort if we already have this guy captive
						if (cursor == coll.GetComponent<PlayerMovement> ()) {
							return;
						}
					}
						
					PlayerGhost ghost = coll.GetComponent<PlayerGhost> ();
					cursor.captive = ghost;
					if (ghost.captor != null) {
						ghost.captor.GetComponent<PlayerMovement> ().captive = null;
					}
					ghost.captor = cursor.gameObject;

					ghost.enabled = true;

					// Disable movement
					GameObject.Find("AirConsoleLogic").GetComponent<AirconsoleLogic>().Lock(coll.GetComponent<PlayerMovement> ());

					var new_particles = GameObject.Instantiate(particles);
					new_particles.transform.position = coll.transform.position;;
				}
			}
		}
	}
}
