using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Linq;

public class AirconsoleLobby : MonoBehaviour {

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
	}

	void Start() {
		if (AirConsole.instance.IsAirConsoleUnityPluginReady ()) {
			List<int> ids = AirConsole.instance.GetControllerDeviceIds ();

			Debug.Log (ids.Count);
			ids.ForEach ((device_id) => {
				AirConsole.instance.Message(device_id, "{\"lobby\":true}");
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
	/// We check which one of the active players has moved the paddle.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="data">Data.</param>
	void OnMessage(int device_id, JToken data) {
		if (data ["color"] != null) {
			AirConsole.instance.Message (device_id, "{\"color\":\"153, 13, 226\"}");
		}
		if (data ["state"] != null) {
			AirConsole.instance.Message (device_id, "{\"lobby\":true}");
		}
	}

	void OnDestroy() {

		// unregister airconsole events on scene change
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;
		}
	}

}
