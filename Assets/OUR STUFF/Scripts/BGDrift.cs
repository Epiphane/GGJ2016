using UnityEngine;
using System.Collections;

public class BGDrift : MonoBehaviour {

	public Vector3 basePosn;
	float offset = 0.0f;

	void Start() {
		offset = Random.Range (-4.0f, 4.0f);
	}
	
	// Update is called once per frame
	void Update () {
		var newX = Mathf.Sin (Time.time + offset);
		var newY = Mathf.Cos (Time.time + offset);
		transform.localPosition = new Vector3 (newX, newY, 8.0f) + basePosn;
		transform.RotateAround (transform.position, Vector3.forward, offset * 0.1f);
	}
}
