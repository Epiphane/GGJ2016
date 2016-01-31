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
	public Sprite soulSprite; // Unimplemented so far

	public bool dashing = false;

	public Vector2 input = new Vector2(0, 0);

	public Color playerColor = new Color(1.0f, 1.0f, 1.0f);

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

	// Scales the wiz down along the axis they're facing towards
	void ScaleAlongFacingAxis(float howMuch) {
		var angle = -Mathf.Atan2(input.x, -input.y) * howMuch;

		Debug.Log (howMuch);

		player_img.transform.rotation = Quaternion.AngleAxis (angle * 180 / Mathf.PI, Vector3.forward);
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

		// Move according to input
		var vel = GetComponent<Rigidbody2D> ().velocity;
		vel.x = input.x * speed;// * outVec.z;
		vel.y = input.y * -speed;

		GetComponent<Rigidbody2D> ().velocity = vel;
		// End moving

		// dashing = (vel.sqrMagnitude >= 20);

		if (GetComponent<Rigidbody2D> ().velocity.sqrMagnitude < 5) {
			if (dashing) {
				// Going from dashingTRUE => dashingFALSE
				dashAnim = DASH_ANIM_LENGTH;
				buttParticles.Stop ();

				player_img.GetComponent<SpriteRenderer> ().sprite = wizardSprite;
			}
			dashing = false;
		} else {
			if (!dashing) {
				dashAnim = 0;

				player_img.GetComponent<SpriteRenderer> ().sprite = fireballSprite;
			}

			dashing = true;
		}
	
		playerColor.a = 1.0f;
		player_img.GetComponent<SpriteRenderer> ().color = playerColor;

//		TODO Bring quaternions back
//		if (dashAnim >= 0.0f && dashing) { // Transitioning TO a fireball
//			if (dashAnim <= DASH_ANIM_LENGTH / 2.0f) {
//				player_img.GetComponent<SpriteRenderer> ().sprite = fireballSprite;
//			}
//			ScaleAlongFacingAxis ((DASH_ANIM_LENGTH - dashAnim) / DASH_ANIM_LENGTH);
//
//			dashAnim -= Time.deltaTime;
//		} else if (dashAnim >= 0.0f && !dashing) {    // Transitioning FROM a fireball
//			if (dashAnim <= DASH_ANIM_LENGTH / 2.0f) {
//				player_img.GetComponent<SpriteRenderer> ().sprite = wizardSprite;
//			}
//			ScaleAlongFacingAxis (1 - (DASH_ANIM_LENGTH - dashAnim) / DASH_ANIM_LENGTH);
//
//			dashAnim -= Time.deltaTime;
		if (dashing) { // I am a fireball. Hear me roar!
			var angle = -Mathf.Atan2(input.x, -input.y);

			player_img.transform.rotation = Quaternion.AngleAxis (angle * 180 / Mathf.PI, Vector3.forward);
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
		if (dashing && coll.tag == "Player" && !coll.GetComponent<PlayerMovement> ().dashing) {
			// They weren't dashing! DESTROY THEM
			ClaimSoul (coll.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.collider.tag == "Player" && dashing) {
			// Collided with player...
			PlayerMovement movement = coll.collider.GetComponent<PlayerMovement> ();
			if (movement.dashing) {
				movement.Stun (1);
				Stun (1);

				GameObject.Find ("AirConsoleLogic").GetComponent<AirconsoleLogic> ().Message (movement, "{\"vibrate\":1000}");

				collision.Play ();
			} else {
				// They weren't dashing! DESTROY THEM
				ClaimSoul (coll.collider.gameObject);
			}
		}
	}
}
