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

		float redness = (GetComponent<Rigidbody2D> ().velocity.sqrMagnitude) / 1000.0f;
		GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f - redness, 1.0f - redness);
			
		if (redness > 0.02f) {
			GetComponent<SpriteRenderer> ().color = new Color (1.0f, 0.0f, 0.0f);
			dashing = true;
		} else {
			playerColor.a = 1.0f;
			GetComponent<SpriteRenderer> ().color = playerColor;
			dashing = false;
		}

		cooldown -= Time.deltaTime;

		if (cooldown >= 0.0f && !dashing) {
			GetComponent<SpriteRenderer> ().color = new Color (0.0f, 0.0f, 1.0f);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.collider.tag == "Player") {
			if (dashing) {

				// Collided with player...
				if (coll.collider.GetComponent<PlayerMovement> ().dashing) {
					// They were dashing! Bounce away.
					var diffX = coll.collider.transform.position.x - transform.position.x;
					var diffY = coll.collider.transform.position.y - transform.position.y;

					GetComponent<Rigidbody2D>().velocity = new Vector2 (diffX, diffY) * speed;
					coll.collider.GetComponent<Rigidbody2D>().velocity = new Vector2 (diffX, diffY) * -speed;
				} else {
					// They weren't dashing! DESTROY THEM
					var new_particles = GameObject.Instantiate(particles);
					new_particles.transform.position = coll.collider.transform.position;

					PlayerMovement captiveMovement = coll.collider.GetComponent<PlayerMovement> ();
						
					// Disable movement
					captiveMovement.enabled = false;

					PlayerGhost ghost = coll.collider.GetComponent<PlayerGhost> ();
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
