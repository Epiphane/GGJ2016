﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirconsoleLogic : MonoBehaviour {

	public GameObject playerTemplate;

	Dictionary<int, PlayerMovement> activePlayers = new Dictionary<int, PlayerMovement>();

	void Awake() {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;
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
		newFriend.GetComponent<PlayerMovement> ().related_device_id = device_id;

//		if (AirConsole.instance.GetControllerDeviceIds ().Count == 1) {
//			newFriend.transform.position = GameObject.Find ("start_1").transform.position;
//		}
//		else if (AirConsole.instance.GetControllerDeviceIds ().Count == 2) {
//			newFriend.transform.position = GameObject.Find ("start_2").transform.position;
//		}
//		else if (AirConsole.instance.GetControllerDeviceIds ().Count == 3) {
//			newFriend.transform.position = GameObject.Find ("start_3").transform.position;
//		}
//		else if (AirConsole.instance.GetControllerDeviceIds ().Count == 4) {
//			newFriend.transform.position = GameObject.Find ("start_4").transform.position;
//		}
		ResetButton();
	}

	public void ResetButton() {
		int x = 1;
		foreach(KeyValuePair<int, PlayerMovement> entry in activePlayers) {
			var newFriend = entry.Value;
			if (x == 1) {
				newFriend.transform.position = GameObject.Find ("start_1").transform.position;
			}
			else if (x == 2) {
				newFriend.transform.position = GameObject.Find ("start_2").transform.position;
			}
			else if (x == 3) {
				newFriend.transform.position = GameObject.Find ("start_3").transform.position;
			}
			else if (x >= 4) {
				newFriend.transform.position = GameObject.Find ("start_4").transform.position;
			}

			x++;
		}
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
		print ("HERE WE GO!");
		var move = (float)data ["move"];

		if (Mathf.Abs (move) > 1.0f) {
			activePlayers [device_id].Dash ();
		}
	}

	void StartGame() {
		AirConsole.instance.SetActivePlayers (2);
		ResetBall (true);
		UpdateScoreUI();
	}

	void ResetBall(bool move) {
		
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
