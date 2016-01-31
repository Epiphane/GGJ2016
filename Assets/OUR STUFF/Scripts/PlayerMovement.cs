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

	public ParticleSystem buttParticles;
	private float stunned = 0;

	public AudioSource collision;
	public AudioSource death;

	public AudioClip collisionClip;
	public AudioClip deathClip;

	public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip; 
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol; 
		return newAudio; 
	}

	void Awake () {
		collision = AddAudio (collisionClip, false, false, 1.0f);
		death = AddAudio (deathClip, false, false, 1.0f);
	}

	void Start() {
		arrow = transform.Find ("arrow_parent").gameObject;
		player_img = transform.Find ("player_img").gameObject;
		buttParticles = transform.Find ("player_img/particle_poop").gameObject.GetComponent<ParticleSystem>();
		playerColor.a = 1.0f;
		buttParticles.startColor = playerColor;
		buttParticles.Stop ();
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

	public void Stun(float t) {
		stunned = t;

		player_img.GetComponent<SpriteRenderer> ().sprite = wizardSprite;
		dashing = false;
	}

	public void StartDashing() {
		wantsToDash = true;
	}

	public void StopDashing() {
		wantsToDash = false;
	}

	public float GetArrowAngle(out Vector3 outVec) {
		float angle;

		arrow.transform.rotation.ToAngleAxis (out angle, out outVec);

		angle = Mathf.Deg2Rad * angle;

		return angle;
	}

	public void Dash() {
		if (!dashing) {
			dashAnim = DASH_ANIM_LENGTH;
			player_img.transform.rotation = new Quaternion (0, 0, 0, 0);
			buttParticles.Play ();
		}

		float angle;
		Vector3 outVec;

		angle = GetArrowAngle (out outVec);

		var vel = GetComponent<Rigidbody2D> ().velocity;
		vel.x = Mathf.Sin (angle) *  speed * outVec.z;
		vel.y = Mathf.Cos (angle) * -speed;

		GetComponent<Rigidbody2D> ().velocity = vel;

		cooldown = 0.05f;
		dashing = true;
	}

	// Scales the wiz down along the axis they're facing towards
	void ScaleAlongFacingAxis(float howMuch) {
		Vector3 outVec;
		var angle = GetArrowAngle (out outVec) * Mathf.Rad2Deg;

//		var squishQuaternion = Quaternion.AngleAxis (howMuch, new Vector3 (Mathf.Sin (angle), Mathf.Cos (angle), 0.0f).normalized);
//		var squishQuaternion = Quaternion.AngleAxis (howMuch, Vector3.right);
		var flipQuat = Quaternion.AngleAxis (180.0f, Vector3.right);

		var newQuat = Quaternion.Slerp (new Quaternion (0, 0, 0, 0), arrow.transform.rotation * flipQuat, howMuch);

		player_img.transform.rotation = newQuat;
	}

	// Update is called once per frame
	void Update () {
		GetComponent<CircleCollider2D> ().isTrigger = false;

		if (stunned > 0) {
			stunned -= Time.deltaTime;

			player_img.transform.rotation = new Quaternion (0, 0, 0, 0);

			if (Mathf.Ceil(stunned * 8) % 2 == 1) {
				playerColor.a = 0.25f;
			}
			else {
				playerColor.a = 1.0f;
			}
			player_img.GetComponent<SpriteRenderer> ().color = playerColor;

			return;
		}

		if (debug_wizz) {
			if (Input.GetKey (movementKey)) {
				Dash ();
			}
		}

		if (wantsToDash) {
			Dash ();
		}

		if (cooldown <= 0.0f) {
			arrow.transform.RotateAround (arrow.transform.position, Vector3.forward, 3.5f);
		}

		if (GetComponent<Rigidbody2D> ().velocity.sqrMagnitude < 20) {
			if (dashing) {
				// Going from dashingTRUE => dashingFALSE
				dashAnim = DASH_ANIM_LENGTH;
				buttParticles.Stop ();
			}
			dashing = false;
		}
	
		playerColor.a = 1.0f;
		player_img.GetComponent<SpriteRenderer> ().color = playerColor;

		cooldown -= Time.deltaTime;

		if (cooldown >= 0.0f && !dashing) {
			player_img.GetComponent<SpriteRenderer> ().color = new Color (0.0f, 0.0f, 1.0f);
		}

		if (dashAnim >= 0.0f && dashing) { // Transitioning TO a fireball
			if (dashAnim <= DASH_ANIM_LENGTH / 2.0f) {
				player_img.GetComponent<SpriteRenderer> ().sprite = fireballSprite;
			}
			ScaleAlongFacingAxis ((DASH_ANIM_LENGTH - dashAnim) / DASH_ANIM_LENGTH);

			dashAnim -= Time.deltaTime;
		} else if (dashAnim >= 0.0f && !dashing) {    // Transitioning FROM a fireball
			if (dashAnim <= DASH_ANIM_LENGTH / 2.0f) {
				player_img.GetComponent<SpriteRenderer> ().sprite = wizardSprite;
			}
			ScaleAlongFacingAxis ((DASH_ANIM_LENGTH - dashAnim) / DASH_ANIM_LENGTH);

			dashAnim -= Time.deltaTime;
		} else if (dashing) { // I am a fireball. Hear me roar!

		} else { // Normal, non-dashing wizard. reset rotation.
			player_img.transform.rotation = new Quaternion(0, 0, 0, 0);
		}

	}

	public void CollectPoints() {
		while (captive != null) {
			GetComponent<PlayerScore> ().GainPoint ();
			captive.GetComponent<PlayerScore> ().LosePoint ();

			captive.Respawn ();
			// This will set captive to something else
		}
	}

	void ClaimSoul(GameObject victim) {
		// Go to the last element in the list
		PlayerMovement cursor = this;
		while (cursor.captive != null) {
			cursor = cursor.captive.GetComponent<PlayerMovement> ();

			// Abort if we already have this guy captive
			if (cursor == victim.GetComponent<PlayerMovement> ()) {
				return;
			}
		}

		PlayerGhost ghost = victim.GetComponent<PlayerGhost> ();
		cursor.captive = ghost;
		if (ghost.captor != null) {
			ghost.captor.GetComponent<PlayerMovement> ().captive = null;
		}
		ghost.captor = cursor.gameObject;

		ghost.enabled = true;

		// Disable movement
		GameObject.Find ("AirConsoleLogic").GetComponent<AirconsoleLogic> ().Lock (victim.GetComponent<PlayerMovement> ());

		var new_particles = GameObject.Instantiate (particles);
		new_particles.transform.position = victim.transform.position;

		death.Play ();
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.tag == "Player") {
			if (dashing) {

				// Collided with player...
				if (!coll.GetComponent<PlayerMovement> ().dashing) {
					// They weren't dashing! DESTROY THEM

					ClaimSoul (coll.gameObject);
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.collider.tag == "Wall") {
			arrow.transform.RotateAround (arrow.transform.position, Vector3.forward, 180.0f);
		}

		if (coll.collider.tag == "Player" && dashing) {
			// Collided wi;th player...
			PlayerMovement movement = coll.collider.GetComponent<PlayerMovement> ();
			if (movement.dashing) {
				movement.Stun (1);
				Stun (1);

				collision.Play ();
			} else {
				// They weren't dashing! DESTROY THEM

				ClaimSoul (coll.collider.gameObject);
				return;
				// Go to the last element in the list
				PlayerMovement cursor = this;
				while (cursor.captive != null) {
					cursor = cursor.captive.GetComponent<PlayerMovement> ();

					// Abort if we already have this guy captive
					if (cursor == coll.collider.GetComponent<PlayerMovement> ()) {
						return;
					}
				}

				PlayerGhost ghost = coll.collider.GetComponent<PlayerGhost> ();
				cursor.captive = ghost;
				if (ghost.captor != null) {
					ghost.captor.GetComponent<PlayerMovement> ().captive = null;
				}
				ghost.captor = cursor.gameObject;

				ghost.enabled = true;

				// Disable movement
				GameObject.Find ("AirConsoleLogic").GetComponent<AirconsoleLogic> ().Lock (coll.collider.GetComponent<PlayerMovement> ());

				var new_particles = GameObject.Instantiate (particles);
				new_particles.transform.position = coll.transform.position;

				death.Play ();
			}
		}
	}
}
