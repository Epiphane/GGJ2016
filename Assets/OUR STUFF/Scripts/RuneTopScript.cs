using UnityEngine;
using System.Collections;

public class RuneTopScript : MonoBehaviour {

	public Sprite[] frames;

	void Set(int sprite) {
		GetComponent<SpriteRenderer> ().sprite = frames [sprite];
	}
}
