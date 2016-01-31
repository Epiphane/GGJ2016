using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WinScreenManager : MonoBehaviour {

	public int first_score = 0;
	public int second_score = 0;
	public int third_score = 0;

	public int displayed_first = 0;
	public int displayed_second = 0;
	public int displayed_third = 0;

	public string currState = "grow3";
	float cooldown = 2.0f;
	float max_cool = 0.5f;
	float cur_cool = 0.5f;

	public ParticleSystem confetti;

	// Use this for initialization
	void Start () {
		confetti.Stop ();
		List<PlayerMovement> players = FindObjectOfType<AirconsoleLogic> ().activePlayers.Values.ToList();
		players = players.OrderByDescending(score => -score.GetComponent<PlayerScore> ().score).ToList ();

		first_score = players [0].GetComponent<PlayerScore> ().score;

		if (players.Count >= 2) {
			second_score = players [1].GetComponent<PlayerScore> ().score;
		}

		if (players.Count >= 3) {
			third_score = players [2].GetComponent<PlayerScore> ().score;
		}
	}

	GameObject GetGUY(string name) {
		return FindObjectOfType<Canvas> ().transform.Find (name).gameObject;
	}

	void SetDudeColor(GameObject dude, Color color) {
		if (dude.GetComponent<Image> ()) {

		}

		if (dude.GetComponent<SpriteRenderer> ()) {

		}
	}

	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;

		if (cooldown >= 0) {
			return;
		}

		if (currState == "grow3") {
			if (third_score > 0) {
				cooldown = cur_cool;
				cur_cool *= 0.9f;
				third_score--;
				displayed_third++;

				GetGUY ("third").SetActive (true);

				var size = GetGUY("third").GetComponent<RectTransform>().sizeDelta;
				size.y += 20.0f;
				GetGUY("third").GetComponent<RectTransform> ().sizeDelta = size;

				GetComponent<AudioSource> ().Play ();
				GetComponent<AudioSource> ().pitch++;
			}

			if (third_score == 0) {
				cur_cool = max_cool;
				currState = "grow2";

				GetComponent<AudioSource> ().pitch = 0;
			}
		}



		if (currState == "grow2") {
			if (second_score > 0) {
				cooldown = cur_cool;
				cur_cool *= 0.9f;
				second_score--;
				displayed_second++;

				GetGUY ("second").SetActive (true);

				var size = GetGUY("second").GetComponent<RectTransform>().sizeDelta;
				size.y += 20.0f;
				GetGUY("second").GetComponent<RectTransform> ().sizeDelta = size;

				GetComponent<AudioSource> ().Play ();
				GetComponent<AudioSource> ().pitch++;
			}

			if (second_score == 0) {
				cur_cool = max_cool;
				currState = "grow1";
				GetComponent<AudioSource> ().pitch = 0;
			}
		}



		if (currState == "grow1") {
			if (first_score > 0) {
				cooldown = cur_cool;
				cur_cool *= 0.9f;
				first_score--;
				displayed_first++;

				GetGUY ("first").SetActive (true);

				var size = GetGUY("first").GetComponent<RectTransform>().sizeDelta;
				size.y += 20.0f;
				GetGUY("first").GetComponent<RectTransform> ().sizeDelta = size;

				GetComponent<AudioSource> ().Play ();
				GetComponent<AudioSource> ().pitch++;
			}

			if (first_score == 0) {
				cur_cool = max_cool;
				currState = "confetti";
				confetti.Play ();
				GetComponent<AudioSource> ().pitch = 0;
			}
		}

		if (currState == "confetti") {

		}


		GetGUY ("third").transform.Find ("scoredisplay").GetComponent<Text> ().text = displayed_third.ToString();
		GetGUY ("second").transform.Find ("scoredisplay").GetComponent<Text> ().text = displayed_second.ToString();
		GetGUY ("first").transform.Find ("scoredisplay").GetComponent<Text> ().text = displayed_first.ToString();
	}


}
