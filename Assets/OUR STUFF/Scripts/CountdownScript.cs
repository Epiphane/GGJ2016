using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountdownScript : MonoBehaviour {

	public float time = 60;
	public string nextScene = "";

	private Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;

		if (text != null) {
			text.text = "" + Mathf.Ceil (time);
		}

		if (time <= 0 && nextScene != "") {
			SceneManager.LoadScene (nextScene);
		}
	}
}
