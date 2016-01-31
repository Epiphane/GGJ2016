using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirconsoleLogic : MonoBehaviour {

	public GameObject playerTemplate;

	public List<Color> possibleColors = new List<Color>();
	private List<Color> usedColors = new List<Color>();

	public List<Transform> possibleSpawns = new List<Transform>();
	private List<Transform> usedSpawns = new List<Transform>();

	Dictionary<int, PlayerMovement> activePlayers = new Dictionary<int, PlayerMovement>();

	void Awake() {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;
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
		var newFriend = GameObject.Instantiate (playerTemplate);
		var newPlayer = newFriend.GetComponent<PlayerMovement> ();

		activePlayers [device_id] = newPlayer;
		newPlayer.related_device_id = device_id;
		newPlayer.playerColor = possibleColors[0];
		usedColors.Add (possibleColors [0]);

		AirConsole.instance.Message (device_id, ColorToJSONMessage(possibleColors[0]));
		possibleColors.RemoveAt (0);

		Transform spawn = possibleSpawns [0];
		possibleSpawns.RemoveAt (0);
		newFriend.transform.position = spawn.position;
		newFriend.GetComponent<PlayerGhost> ().spawn = spawn;
	}

	public void Lock(PlayerMovement victim) {
		AirConsole.instance.Message (victim.related_device_id, "{\"lock\":200}");

		victim.enabled = false;
	}

	public void Unlock(PlayerMovement player) {
		AirConsole.instance.Message (player.related_device_id, "{\"unlock\":true}");

		player.enabled = true;
	}

	/// <summary>
	/// If the game is running and one of the active players leaves, we reset the game.
	/// </summary>
	/// <param name="device_id">The device_id that has left.</param>
	void OnDisconnect(int device_id) {
		var oldFriend = activePlayers [device_id];
		if (oldFriend) {
			activePlayers.Remove (device_id);
		}
		GameObject.Destroy (oldFriend.gameObject);
	}

	/// <summary>
	/// We check which one of the active players has moved the paddle.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="data">Data.</param>
	void OnMessage(int device_id, JToken data) {
		int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

		if (data ["color"] != null) {
			AirConsole.instance.Message (device_id, ColorToJSONMessage(activePlayers [device_id].playerColor));
		}
		if (data["dash"] != null) {
			activePlayers [device_id].Dash ();
		}
		if (data["start_dash"] != null) {
			activePlayers [device_id].StartDashing ();
		}
		if (data["stop_dash"] != null) {
			activePlayers [device_id].StopDashing ();
		}
		if (data["unlock"] != null) {
			activePlayers [device_id].Unlock ();
		}
	}

	void StartGame() {
		AirConsole.instance.SetActivePlayers (2);
		UpdateScoreUI();
	}

	void UpdateScoreUI() {
	}

	void FixedUpdate() {
	}

	void OnDestroy() {

		// unregister airconsole events on scene change
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;
		}
	}
}
