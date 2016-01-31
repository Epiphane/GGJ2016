using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {

	public float shakeAmp = 0.5f;
	private float shakeTime = 0;
	private float shakeFreq = 12;
	private Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		originalPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (shakeTime > 0) {
			transform.localPosition = originalPosition + new Vector3(shakeAmp * Mathf.Sin(Mathf.PI * 2 * shakeTime * shakeFreq), 0, 0);

			shakeTime -= Time.deltaTime;
		}
	}

	void Shake() {
		shakeTime = 1.5f;
	}
}
