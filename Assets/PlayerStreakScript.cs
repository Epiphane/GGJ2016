using UnityEngine;
using System.Collections;

public class PlayerStreakScript : MonoBehaviour {

	public SpriteRenderer streakSprite;
	public TextMesh text;
	public float shakeSpeed = 4;
	public float scaleUp = 4;

	private int streak_official = 0;
	private float original_scale = 1;
	private float scale = 1;
	private float streak = 0;

	// Use this for initialization
	void Start () {
		text = GetComponent<TextMesh> ();
		original_scale = transform.localScale.x;

		Reset ();
	}
	
	// Update is called once per frame
	void Update () {
		if (scale > 1) {
			scale = Mathf.Max (1, scale - Time.deltaTime * shakeSpeed);

			transform.localScale = Vector3.one * scale * original_scale;
		}

		if (streak < streak_official) {
			if (Mathf.Round (streak + Time.deltaTime * shakeSpeed) != Mathf.Round (streak)) {
				scale = scaleUp;
			}

			streak = Mathf.Min (streak + Time.deltaTime * shakeSpeed, streak_official);

			text.text = "" + Mathf.Round(streak);
		}
	}

	public void Add(int amount) {
		streak_official += amount;
		
		streakSprite.enabled = true;
	}

	public void Reset() {
		streakSprite.enabled = false;
		text.text = "";

		streak_official = 0;
	}
}
