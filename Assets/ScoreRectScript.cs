using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreRectScript : MonoBehaviour {

	public CanvasRenderer profilePicture;

	public IEnumerator DisplayUrlPicture(string url) {
		// Start a download of the given URL
		WWW www = new WWW(url);

		// Wait for download to complete
		yield return www;

		// assign texture
		profilePicture.SetTexture(www.texture);
		Color color = Color.white;
		color.a = 1;
		profilePicture.SetColor(color);

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
