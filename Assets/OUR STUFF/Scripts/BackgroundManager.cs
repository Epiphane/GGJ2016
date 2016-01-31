using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour {

	public GameObject bgTemplate;

	// Use this for initialization
	void Start () {
		int X_MAX = 3;
		int Y_MAX = 3;
		for (int x = 0; x < X_MAX; x++) {
			for (int y = 0; y < Y_MAX; y++) {
				var newBG = GameObject.Instantiate (bgTemplate);
				newBG.GetComponent<SpriteRenderer> ().color = new Color (Random.Range(0.0f, 0.1f), Random.Range(0.16f, 0.37f), Random.Range(0.1f, 0.37f));

				newBG.GetComponent<BGDrift>().basePosn = new Vector3 (-3.0f + x * 2.0f, -3.0f + y * 2.0f, 8.0f);
				newBG.transform.RotateAround (transform.position, Vector3.forward, Random.Range (-90.0f, 90.0f));

				newBG.transform.parent = this.transform;
			}
		}
	}
}
