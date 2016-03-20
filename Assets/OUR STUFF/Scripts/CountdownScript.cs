using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountdownScript : MonoBehaviour {

	public float time = 60;
	public string nextScene = "";
	public AudioClip beep;

	private AudioSource source;
	private Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();

		source = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (beep != null && time < 11 && Mathf.Floor (time) != Mathf.Floor (time - Time.fixedDeltaTime)) {
			source.PlayOneShot (beep);
		}

		time -= Time.fixedDeltaTime;

		if (text != null) {
			text.text = "" + Mathf.Ceil (time);
		}

		if (time <= 0 && nextScene != "") {
			AirconsoleLogic.CreateFinalScoreTally ();
			SceneManager.LoadScene (nextScene);
		}
	}
}
