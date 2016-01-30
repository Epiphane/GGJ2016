using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public KeyCode movementKey;
	public float speed = 12.0f;
	public GameObject particles;

	public int related_device_id = 0;

	public float cooldown = 0.0f;

	// Keep track of the janky system I use to animate the transition to being a fireball
	public float dashAnim = 0.0f;
	public float DASH_ANIM_LENGTH = 0.2f;

	public Sprite fireballSprite;
	public Sprite wizardSprite;

	public bool dashing = false;

	private bool wantsToDash = false;

	public Color playerColor = new Color(1.0f, 1.0f, 1.0f);

	private GameObject arrow;
	public GameObject player_img;

	public PlayerGhost captive;

	public bool debug_wizz = false;

	void Start() {
		arrow = transform.Find ("arrow_parent").gameObject;
		player_img = transform.Find ("player_img").gameObject;
	}

	public void Unlock() {
		enabled = true;

		PlayerGhost ghost = GetComponent<PlayerGhost>();
		ghost.enabled = false;

		if (captive) {
			ghost.captor.GetComponent<PlayerMovement>().captive = captive;
		}

		captive = null;
		ghost.captor = null;
	}

	public void StartDashing() {
		wantsToDash = true;
	}

	public void StopDashing() {
		wantsToDash = false;
	}

	public float GetArrowAngle() {
		float angle;
		Vector3 outVec;

		arrow.transform.rotation.ToAngleAxis (out angle, out outVec);

		angle = Mathf.Deg2Rad * angle;

		return angle;
	}

	public void Dash() {
		if (!dashing) {
			dashAnim = DASH_ANIM_LENGTH;
			player_img.transform.rotation = new Quaternion (0, 0, 0, 0);
		}

		float angle;
		Vector3 outVec;

		arrow.transform.rotation.ToAngleAxis (out angle, out outVec);

		angle = Mathf.Deg2Rad * angle;

		var vel = GetComponent<Rigidbody2D> ().velocity;
		vel.x = Mathf.Sin (angle) *  speed * outVec.z;
		vel.y = Mathf.Cos (angle) * -speed;

		GetComponent<Rigidbody2D> ().velocity = vel;

		cooldown = 0.3f;
		dashing = true;
	}

	// Scales the wiz down along the axis they're facing towards
	void ScaleAlongFacingAxis(float howMuch) {
		var angle = GetArrowAngle ();

//		var squishQuaternion = Quaternion.AngleAxis (howMuch, new Vector3 (Mathf.Sin (angle), Mathf.Cos (angle), 0.0f).normalized);
		var squishQuaternion = Quaternion.AngleAxis (howMuch, Vector3.right);
		var facingQuaternion = Quaternion.AngleAxis (GetArrowAngle (), Vector3.forward);

		player_img.transform.rotation = squishQuaternion * facingQuaternion;
	}

	// Update is called once per frame
	void Update () {

		if (debug_wizz) {
			if (Input.GetKey (movementKey)) {
				Dash ();
			}
		}

		if (wantsToDash) {
			Dash();
		}

		if (cooldown <= 0.0f) {
			arrow.transform.RotateAround (arrow.transform.position, Vector3.forward, 3.0f);
		}

		if (GetComponent<Rigidbody2D> ().velocity.sqrMagnitude < 20) {
			if (dashing) {
				// Going from dashingTRUE => dashingFALSE
				dashAnim = DASH_ANIM_LENGTH;
			}
			dashing = false;
		}
	
		playerColor.a = 1.0f;
		player_img.GetComponent<SpriteRenderer> ().color = playerColor;

		cooldown -= Time.deltaTime;

		if (cooldown >= 0.0f && !dashing) {
			player_img.GetComponent<SpriteRenderer> ().color = new Color (0.0f, 0.0f, 1.0f);
		}
			
		if (dashing) {
			GetComponent<CircleCollider2D> ().isTrigger = false;
		} else {
			GetComponent<CircleCollider2D> ().isTrigger = true;
		}


		if (dashAnim >= 0.0f && dashing) { // Transitioning TO a fireball
			if (dashAnim <= DASH_ANIM_LENGTH / 2.0f) {
				player_img.GetComponent<SpriteRenderer> ().sprite = fireballSprite;
			}
			ScaleAlongFacingAxis ((dashAnim - DASH_ANIM_LENGTH) / DASH_ANIM_LENGTH * 180.0f);

			dashAnim -= Time.deltaTime;
		} else if (dashAnim >= 0.0f && !dashing) {    // Transitioning FROM a fireball
			if (dashAnim <= DASH_ANIM_LENGTH / 2.0f) {
				player_img.GetComponent<SpriteRenderer> ().sprite = wizardSprite;
			}
			ScaleAlongFacingAxis ((dashAnim - DASH_ANIM_LENGTH) / DASH_ANIM_LENGTH * 180.0f);

			dashAnim -= Time.deltaTime;
		} else if (dashing) { // I am a fireball. Hear me roar!

		} else { // Normal, non-dashing wizard. reset rotation.
			player_img.transform.rotation = new Quaternion(0, 0, 0, 0);
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

					GameObject.Find("AirConsoleLogic").GetComponent<AirconsoleLogic>().Lock(captiveMovement);
				}
			}
		}
	}
}
