using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public KeyCode movementKey;
	public float speed = 20.0f;
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

		if (cooldown <= 0.0f) {
			angle = Mathf.Deg2Rad * angle;

			var vel = GetComponent<Rigidbody2D> ().velocity;
			vel.x = Mathf.Sin (angle) * speed * outVec.z;
			vel.y = Mathf.Cos (angle) * -speed;

			GetComponent<Rigidbody2D> ().velocity = vel;

			cooldown = 0.3f;
			dashing = true;
		}
	}

	// Update is called once per frame
	void Update () {

		if (wantsToDash) {
			Dash (); 
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
					var new_particles = GameObject.Instantiate(particles);
					new_particles.transform.position = coll.transform.position;

					PlayerMovement captiveMovement = coll.GetComponent<PlayerMovement> ();
						
					// Disable movement
					captiveMovement.enabled = false;

					PlayerGhost ghost = coll.GetComponent<PlayerGhost> ();
					ghost.enabled = true;
					if (ghost.captor != null) {
						ghost.captor.GetComponent<PlayerMovement> ().captive = captiveMovement.captive;
					}
					ghost.captor = gameObject;

					// Make the "linked list" work
					captiveMovement.captive = captive;
					if (captive != null) {
						captive.captor = ghost.gameObject;
					}
					captive = ghost;
				}
			}
		}
	}
}
