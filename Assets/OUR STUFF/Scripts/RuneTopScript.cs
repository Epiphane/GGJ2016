using UnityEngine;
using System.Collections;

public class RuneTopScript : MonoBehaviour {

	public Sprite[] frames;

	private int currentFrame = 0;

	public bool Gain(int amount) {
		currentFrame += amount;

		Set (currentFrame % frames.Length);

		if (currentFrame >= frames.Length) {
			currentFrame = currentFrame % frames.Length;
		
			return true;
		}

		return false;
	}

	public void Set(int sprite) {
		Debug.Log ("Sprite" +  sprite);
		GetComponent<SpriteRenderer> ().sprite = frames [sprite];
	}
}
