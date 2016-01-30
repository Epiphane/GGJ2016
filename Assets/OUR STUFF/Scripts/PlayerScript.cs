using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public int speed = 3;
	public int jumpHeight = 12;

	public Transform top_left;
	public Transform bottom_right;
	public LayerMask ground_layers;

	private float xScale = 0;
	private bool isGrounded = false;

	// Use this for initialization
	void Start () {
		xScale = transform.localScale.x;
	}

	// Update is called once per frame
	void FixedUpdate () {
		Rigidbody2D body = GetComponent<Rigidbody2D> ();

		// Update Velocity
		Vector2 v = new Vector2(speed * Input.GetAxis("Horizontal"), body.velocity.y);

		if (v.x < 0) {
			GetComponent<Animator> ().SetBool ("isWalking", true);
			transform.localScale = new Vector3 (xScale * -1, transform.localScale.y, transform.localScale.z);
		} else if (v.x > 0) {
			GetComponent<Animator> ().SetBool ("isWalking", true);
			transform.localScale = new Vector3 (xScale, transform.localScale.y, transform.localScale.z);
		} else {
			GetComponent<Animator> ().SetBool ("isWalking", false);
		}

		isGrounded = Physics2D.OverlapArea(top_left.position, bottom_right.position, ground_layers);  

		if (Input.GetButton ("Jump") && isGrounded) {
			v.y = jumpHeight;

			transform.position = transform.position + new Vector3 (0.0f, 0.5f, 0.0f);

			GetComponent<BoxCollider2D> ().isTrigger = true;
		}
		body.velocity = v;

		if (body.velocity.y < 0) {
			GetComponent<BoxCollider2D> ().isTrigger = false;
		}
	}
}
