using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public int speed = 3;

	private float xScale = 0;

	// Use this for initialization
	void Start () {
		xScale = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		Rigidbody2D body = GetComponent<Rigidbody2D> ();
		Vector2 v = new Vector2(Input.GetAxis("Horizontal"), body.velocity.y);
		body.velocity = speed * v;

		if (v.x < 0) {
			transform.localScale = new Vector3 (xScale * -1, transform.localScale.y, transform.localScale.z);
		}
		else {
			transform.localScale = new Vector3 (xScale, transform.localScale.y, transform.localScale.z);
		}
	}
}
