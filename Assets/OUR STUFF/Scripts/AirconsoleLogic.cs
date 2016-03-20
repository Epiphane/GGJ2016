using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Linq;

public class AirconsoleLogic : MonoBehaviour {

	public GameObject playerTemplate;
	public GameObject scoreUITemplate;

	public List<Color> possibleColors = new List<Color>();
	private List<Color> usedColors = new List<Color>();

	public List<Transform> possibleSpawns = new List<Transform>();
	private List<Transform> usedSpawns = new List<Transform>();

	public Dictionary<int, PlayerMovement> activePlayers = new Dictionary<int, PlayerMovement>();
	public static Dictionary<int, GameObject> activeScoreUI = new Dictionary<int, GameObject>();

	public Transform playerParent;

	public static List<int> winningScores;
	public static List<Color> winningColors;

	void Awake() {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;
	}

	void Start() {
		if (AirConsole.instance.IsAirConsoleUnityPluginReady ()) {
			List<int> ids = AirConsole.instance.GetControllerDeviceIds ();

			ids.ForEach ((device_id) => {
				OnConnect (device_id);
				if (activePlayers.ContainsKey(device_id)) {
					AirConsole.instance.Message(device_id, "{\"controller\":true}");
				}
				else {
					AirConsole.instance.Message(device_id, "{\"lobby\":true}");
				}
			});
		}
	}

	string ColorToRGB(Color c) {
		return Mathf.Floor(255 * c.r) + ", " + Mathf.Floor(255 * c.g) + ",  " + Mathf.Floor(255 * c.b);
	}

	string ColorToJSONMessage(Color c) {
		return "{\"color\":\"" + ColorToRGB (c) + "\"}";
	}

	/// <summary>
	/// We start the game if 2 players are connected and the game is not already running (activePlayers == null).
	/// 
	/// NOTE: We store the controller device_ids of the active players. We do not hardcode player device_ids 1 and 2,
	///       because the two controllers that are connected can have other device_ids e.g. 3 and 7.
	///       For more information read: http://developers.airconsole.com/#/guides/device_ids_and_states
	/// 
	/// </summary>
	/// <param name="device_id">The device_id that connected</param>
	void OnConnect(int device_id) {
		GameObject log = GameObject.Find ("Log");
		if (log != null) {
			log.GetComponent<Text> ().text += "Connected " + device_id + "\n";
		}

		if (possibleColors.Count == 0) {
			// No more space! Gotta wait brooooo
//			AirConsole.instance.Message (device_id, "{\"lobby\":true}");
			return;
		}

		Color coolColor = possibleColors [0];

		var newFriend = GameObject.Instantiate (playerTemplate);
		var newPlayer = newFriend.GetComponent<PlayerMovement> ();

		newFriend.transform.parent = playerParent;

		activePlayers [device_id] = newPlayer;
		newPlayer.related_device_id = device_id;
		newPlayer.playerColor = possibleColors[0];
//		newPlayer.buttParticles.startColor = possibleColors [0];
		usedColors.Add (possibleColors [0]);

		AirConsole.instance.Message (device_id, ColorToJSONMessage(possibleColors[0]));
		AirConsole.instance.Message (device_id, "{\"controller\":true}");
		possibleColors.RemoveAt (0);

		Transform spawn = possibleSpawns [0];
		possibleSpawns.RemoveAt (0);
		newFriend.transform.position = spawn.position;
		newFriend.GetComponent<PlayerGhost> ().spawn = spawn;

		var newUI = GameObject.Instantiate (scoreUITemplate);

		coolColor.a = 1.0f;
		newUI.GetComponent<Image> ().color = coolColor;
		AirconsoleLogic.activeScoreUI [device_id] = newUI;

		newUI.transform.parent = GameObject.Find ("Scoreboard").transform;
		newUI.transform.Find ("player_name").GetComponent<Text> ().text = AirConsole.instance.GetNickname (device_id);

		AirconsoleLogic.ReorderScoreList ();
	}

	// Put the Score Tabs on the side in order of score
	public static void ReorderScoreList() {
		List<PlayerMovement> scoreList = FindObjectOfType<AirconsoleLogic>().activePlayers.Values.ToList ();

		scoreList = scoreList.OrderByDescending (score => score.GetComponent<PlayerScore> ().score).ToList ();

		float increment = 1.0f / 8.0f;
		float top = 1.0f;
		float bottom = top - increment;

		foreach (var score in scoreList) {
			var device_id = score.GetComponent<PlayerMovement> ().related_device_id;
			var rect_transform = AirconsoleLogic.activeScoreUI [device_id].GetComponent<RectTransform> ();
			rect_transform.offsetMin = Vector2.zero;
			rect_transform.offsetMax = Vector2.zero;

			var amax = rect_transform.anchorMax;
			amax.y = top;
			rect_transform.anchorMax = amax;

			var amin = rect_transform.anchorMin;
			amin.y = bottom;
			rect_transform.anchorMin = amin;

			top -= increment;
			bottom = top - increment;

			rect_transform.sizeDelta = Vector2.zero;
		}
	}

	public void Lock(PlayerMovement victim) {
		Message (victim, "{\"lock\":200}");

		victim.enabled = false;
	}

	public void Unlock(PlayerMovement player) {
		Message (player, "{\"unlock\":true}");

		player.enabled = true;
	}

	public void Message(PlayerMovement player, string message) {
		if (player.related_device_id == 0)
			return;
		
		AirConsole.instance.Message (player.related_device_id, message);
	}

	/// <summary>
	/// If the game is running and one of the active players leaves, we reset the game.
	/// </summary>
	/// <param name="device_id">The device_id that has left.</param>
	void OnDisconnect(int device_id) {
		// If they weren't even playing we don't care
		if (!activePlayers.ContainsKey (device_id)) {
			return;
		}

		var oldFriend = activePlayers [device_id];

		// Remove score UI
		activePlayers.Remove (device_id);
		GameObject oldScore = activeScoreUI [device_id];
		activeScoreUI.Remove (device_id);
		Destroy (oldScore);
		ReorderScoreList ();

		// Add color back to options
		usedColors.Remove (oldFriend.playerColor);
		usedSpawns.Remove (oldFriend.GetComponent<PlayerGhost>().spawn);
		possibleColors.Add (oldFriend.playerColor);
		possibleSpawns.Add (oldFriend.GetComponent<PlayerGhost>().spawn);

		ReorderScoreList ();

		GameObject.Destroy (oldFriend.gameObject);

		// Add a new player maybe
		List<int> ids = AirConsole.instance.GetControllerDeviceIds ();

		if (ids.Count >= 8) {
			for (int i = 0; i < ids.Count; i++) {
				if (!activePlayers.ContainsKey(ids[i])) {
					OnConnect (ids [i]);
					return;
				}
			}
		}
	}

	/// <summary>
	/// We check which one of the active players has moved the paddle.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="data">Data.</param>
	void OnMessage(int device_id, JToken data) {
		int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

		if (data ["joystick-right"] != null) {
			Vector2 direction;
			if ((bool) data ["joystick-right"] ["pressed"] == true) {
				direction = new Vector2 ((float)data ["joystick-right"] ["message"] ["x"], (float)data ["joystick-right"] ["message"] ["y"]);
			} else {
				direction = new Vector2 (0, 0);
			}

			activePlayers [device_id].input = direction;
		}
		if (data ["color"] != null) {
			
			if (activePlayers.ContainsKey(device_id)) {
				AirConsole.instance.Message (device_id, ColorToJSONMessage(activePlayers[device_id].playerColor));
			}
			else {
				AirConsole.instance.Message (device_id, "{\"color\":\"153, 13, 226\"}");
			}
		}
		if (data ["state"] != null) {
			if (activePlayers.ContainsKey(device_id)) {
				AirConsole.instance.Message (device_id, "{\"controller\":true}");
			}
			else {
				AirConsole.instance.Message (device_id, "{\"lobby\":true}");
			}
		}
	}

	void StartGame() {
		AirConsole.instance.SetActivePlayers (2);
		UpdateScoreUI();
	}

	void UpdateScoreUI() {
	}

	void FixedUpdate() {
		if (Input.GetKeyDown (KeyCode.Z)) { 
			SceneManager.LoadScene ("WINNER_WINNER_CHICKEN_DINER");
		}
	}

	void OnDestroy() {

		// unregister airconsole events on scene change
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;
			AirConsole.instance.onDisconnect -= OnDisconnect;
			AirConsole.instance.onConnect -= OnConnect;
		}
	}

	public static void CreateFinalScoreTally() {
		var me_irl = FindObjectOfType<AirconsoleLogic> ();

		winningScores = new List<int> ();
		winningColors = new List<Color> ();

		if (me_irl) {
			List<PlayerMovement> scoreList = me_irl.activePlayers.Values.ToList ();
			scoreList = scoreList.OrderByDescending (score => score.GetComponent<PlayerScore> ().score).ToList ();

			foreach (var p in scoreList) {
				winningScores.Add (p.GetComponent<PlayerScore>().score);
				winningColors.Add (p.playerColor);
			}
		}
	}

}
