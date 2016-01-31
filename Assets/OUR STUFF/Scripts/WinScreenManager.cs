using UnityEngine;
using System.Collections;
using System.Linq;

public class WinScreenManager : MonoBehaviour {

	int first_score = 0;
	int second_score = 0;
	int third_score = 0;

	// Use this for initialization
	void Start () {
		var players = FindObjectOfType<AirconsoleLogic> ().activePlayers.Values;
		players = players.OrderByDescending(score => -score.GetComponent<PlayerScore> ().score).ToList ();
		foreach (var player in players) {
			
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
