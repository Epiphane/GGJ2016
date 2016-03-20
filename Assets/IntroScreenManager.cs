using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Linq;

public class IntroScreenManager : MonoBehaviour {

	public AudioSource music;

	// Use this for initialization
	void Start () {
	
	}
		
	void Awake() {
		AirConsole.instance.onMessage += OnMessage;
	}

	/// <summary>
	/// We check which one of the active players has moved the paddle.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="data">Data.</param>
	void OnMessage(int device_id, JToken data) {
		if (data ["color"] != null) {
			AirConsole.instance.Message (device_id, "{\"color\":\"153, 13, 226\"}");
		}
		if (data ["state"] != null) {
			AirConsole.instance.Message (device_id, "{\"lobby\":true,\"skip\":true}");
		}
		if (data ["skip"] != null) {
			SceneManager.LoadScene ("ElliotMinigame");
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.anyKey && !Input.GetMouseButton(0)) {
			Skip ();
		}

		if (music != null && !music.isPlaying) {
			Skip ();
		}
	}

	public void Skip() {
		SceneManager.LoadScene ("ElliotMinigame");
	}
}
