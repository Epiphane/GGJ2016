using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {

	public int score = 0;

	public void GainPoint() {
		score++;
		var device_id = GetComponent<PlayerMovement> ().related_device_id;

		var scoreUI = AirconsoleLogic.activeScoreUI [device_id];
		if (scoreUI) {
			scoreUI.transform.Find ("score_text").GetComponent<Text> ().text = score + " SOULS";

			if (score == 1) {
				scoreUI.transform.Find ("score_text").GetComponent<Text> ().text = score + " SOUL";
			}
		}
	}

	public void LosePoint() {
		score--;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
