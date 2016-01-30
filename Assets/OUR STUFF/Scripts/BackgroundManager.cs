using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour {

	public GameObject bgTemplate;

	// Use this for initialization
	void Start () {
		int X_MAX = 5;
		int Y_MAX = 5;
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				var newBG = GameObject.Instantiate (bgTemplate);
				newBG.GetComponent<SpriteRenderer> ().color = new Color (Random.Range(0.0f, 0.2f), Random.Range(0.0f, 0.2f), Random.Range(0.0f, 0.2f));

				var newX = Random.Range (-3.0f, 3.0f);
				var newY = Random.Range (-3.3f, 3.3f);
				newBG.transform.position = new Vector2 (newX, newY);

				newBG.transform.parent = this.transform;
			}
		}
	}
}
