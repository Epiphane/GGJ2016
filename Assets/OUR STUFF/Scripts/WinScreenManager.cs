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

	public SpriteRenderer[] losers;

	public string currState = "grow3";
	float cooldown = 2.0f;
	float max_cool = 0.5f;
	float cur_cool = 0.5f;

	public ParticleSystem confetti;

	List<Color> colors;
	List<int> scores;

	// Use this for initialization
	void Start () {
		confetti.Stop ();
		colors = AirconsoleLogic.winningColors; //players = FindObjectOfType<AirconsoleLogic> ().activePlayers.Values.ToList();
		scores = AirconsoleLogic.winningScores; //players = players.OrderByDescending(score => -score.SCORE_LOL).ToList ();

		first_score = scores [0];
		SetDudeColor (GetGUY("first"), colors[0]);
		SetDudeColor (GetGUY("first/viking"), colors[0]);

		if (scores.Count >= 2) {
			second_score = scores[1];
			SetDudeColor (GetGUY("second"), colors[1]);
			SetDudeColor (GetGUY("second/viking"), colors[1]);
		}

		if (scores.Count >= 3) {
			third_score = scores[2];
			SetDudeColor (GetGUY("third"), colors[2]);
			SetDudeColor (GetGUY("third/viking"), colors[2]);
		}
	}

	GameObject GetGUY(string name) {
		return FindObjectOfType<Canvas> ().transform.Find (name).gameObject;
	}

	void SetDudeColor(GameObject dude, Color color) {
		color.a = 1.0f;

		if (dude.GetComponent<Image> ()) {
			dude.GetComponent<Image> ().color = color;
		}

		if (dude.GetComponent<SpriteRenderer> ()) {
			dude.GetComponent<SpriteRenderer> ().color = color;
		}
	}

	int loserNum = 3;
	float cd = 0.2f;
	public Text LOSER;

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

			if (third_score <= 0) {
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

			if (second_score <= 0) {
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

			if (first_score <= 0) {
				cur_cool = max_cool;
				currState = "confetti";
				confetti.Play ();
				GetComponent<AudioSource> ().pitch = 0;
				LOSER.gameObject.SetActive (true);
			}
		}

		if (currState == "confetti") {			
			cd -= Time.deltaTime;

			if (cd < 0.0f) {
				if (losers.Length > loserNum && scores.Count > loserNum) {
					losers [loserNum].gameObject.SetActive (true);
					losers [loserNum].color = colors[loserNum];
					cd = 0.2f;
					loserNum++;
				}
			}
		}

		GetGUY ("third").transform.Find ("scoredisplay").GetComponent<Text> ().text = displayed_third.ToString();
		GetGUY ("second").transform.Find ("scoredisplay").GetComponent<Text> ().text = displayed_second.ToString();
		GetGUY ("first").transform.Find ("scoredisplay").GetComponent<Text> ().text = displayed_first.ToString();
	}


}
