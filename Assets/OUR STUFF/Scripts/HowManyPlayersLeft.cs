using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NDream.AirConsole;

public class HowManyPlayersLeft : MonoBehaviour {

	public Text textField;

	public int numNeeded = 3;
	private int numConnected = 0;

	void Awake() {
		AirConsole.instance.onConnect += OnConnect;
	}

	void Calculate() {
		if (AirConsole.instance.IsAirConsoleUnityPluginReady ()) {
			numConnected = AirConsole.instance.GetControllerDeviceIds ().Count;
		}

		if (numConnected >= numNeeded) {
			Time.timeScale = 1;
			GameObject.Destroy (gameObject);
		} else {
			Time.timeScale = 0;
			textField.text = (numNeeded - numConnected) + " left";
		}

		Time.fixedDeltaTime = 0.02F * Time.timeScale;
	}

	void Start() {
		Calculate ();
	}

	void OnConnect(int device_id) {
		Calculate ();
	}
}
