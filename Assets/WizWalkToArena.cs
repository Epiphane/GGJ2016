using UnityEngine;
using System.Collections;

public class WizWalkToArena : MonoBehaviour {

	public float targetX = 0;
	public float targetY = -1.35f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (IsWalking) {
			Vector3 diff_vec = (new Vector3(targetX, targetY, 0)) - this.transform.position;

			if (diff_vec.magnitude <= 0.2f) {
				this.gameObject.SetActive (false);
			}

			diff_vec.Normalize ();

			this.transform.position += diff_vec * 0.02f;

		}
	}

	public static bool IsWalking = false;
}
