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

	public int SCORE_LOL = 0;

	public ParticleSystem buttParticles;
	private float stunned = 0;

	public AudioSource collision;
	public AudioSource death;
	public AudioSource kill2;
	public AudioSource kill3;
	public AudioSource kill4;
	public AudioSource kill5;
	public AudioSource kill6;
	public AudioSource kill7;
	public AudioSource kill8;
	public AudioSource kill9;
	public AudioSource kill10;

	public AudioClip collisionClip;
	public AudioClip deathClip;
	public AudioClip kill2Clip;
	public AudioClip kill3Clip;
	public AudioClip kill4Clip;
	public AudioClip kill5Clip;
	public AudioClip kill6Clip;
	public AudioClip kill7Clip;
	public AudioClip kill8Clip;
	public AudioClip kill9Clip;
	public AudioClip kill10Clip;

	public PlayerStreakScript streak;

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
		kill2 = AddAudio (kill2Clip, false, false, 1.0f);
		kill3 = AddAudio (kill3Clip, false, false, 1.0f);
		kill4 = AddAudio (kill4Clip, false, false, 1.0f);
		kill5 = AddAudio (kill5Clip, false, false, 1.0f);
		kill6 = AddAudio (kill6Clip, false, false, 1.0f);
		kill7 = AddAudio (kill7Clip, false, false, 1.0f);
		kill8 = AddAudio (kill8Clip, false, false, 1.0f);
		kill9 = AddAudio (kill9Clip, false, false, 1.0f);
		kill10 = AddAudio (kill10Clip, false, false, 1.0f);
	}

	void Start() {
		player_img = transform.Find ("player_img").gameObject;
		buttParticles = transform.Find ("player_img/particle_poop").gameObject.GetComponent<ParticleSystem>();
		playerColor.a = 1.0f;
		buttParticles.startColor = playerColor;
		buttParticles.Stop ();
	}

	public GameObject tether;
	public GameObject bubble;

	public void Unlock() {
		enabled = true;

		PlayerGhost ghost = GetComponent<PlayerGhost>();
		ghost.enabled = false;
		ghost.GetComponent<PlayerMovement> ().tether.GetComponent<SpriteRenderer> ().enabled = false;
		ghost.GetComponent<PlayerMovement> ().bubble.GetComponent<SpriteRenderer> ().enabled = false;
		ghost.GetComponent<PlayerMovement> ().transform.Find ("player_img").transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

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

			var targetRotation = Quaternion.AngleAxis (angle * 180 / Mathf.PI, Vector3.forward);
			var zenoRotation = Quaternion.Slerp (player_img.transform.rotation, targetRotation, 0.5f);

			player_img.transform.rotation = zenoRotation;
		} else { // Normal, non-dashing wizard. reset rotation.
			player_img.transform.rotation = new Quaternion(0, 0, 0, 0);
		}

	}

	public int CollectPoints() {
		var count = 0;

		while (captive != null) {
			GetComponent<PlayerScore> ().GainPoint ();
			count++;
			captive.GetComponent<PlayerScore> ().LosePoint ();

			captive.Sacrifice ();
			// This will set captive to something else
		}

		if (count == 2)
			kill2.Play ();
		else if (count == 3)
			kill3.Play ();
		else if (count == 4)
			kill4.Play ();
		else if (count == 5)
			kill5.Play ();
		else if (count == 6)
			kill6.Play ();
		else if (count == 7)
			kill7.Play ();
		else if (count == 8)
			kill8.Play ();
		else if (count == 9)
			kill9.Play ();
		else if (count >= 10)
			kill10.Play ();

		if (streak != null) {
			streak.Add (count);
		}

		return count;
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
		victim.GetComponent<PlayerMovement> ().tether.GetComponent<SpriteRenderer> ().enabled = true;
		ghost.GetComponent<PlayerMovement> ().bubble.GetComponent<SpriteRenderer> ().enabled = true;
		ghost.GetComponent<PlayerMovement> ().transform.Find ("player_img").transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);

		// Disable movement
		GameObject.Find ("AirConsoleLogic").GetComponent<AirconsoleLogic> ().Lock (victim.GetComponent<PlayerMovement> ());

		var new_particles = GameObject.Instantiate (particles);
		new_particles.transform.position = victim.transform.position;
		new_particles.GetComponent<ParticleSystem> ().startColor = victim.GetComponent<PlayerMovement> ().playerColor;

		death.Play ();
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (dashing && coll.tag == "Player" && !coll.GetComponent<PlayerMovement> ().dashing) {
			// They weren't dashing! DESTROY THEM
			if (!coll.GetComponent<PlayerGhost> ().sacrificing)
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
				if (!coll.collider.GetComponent<PlayerGhost> ().sacrificing)
					ClaimSoul (coll.collider.gameObject);
			}
		}
	}
}
