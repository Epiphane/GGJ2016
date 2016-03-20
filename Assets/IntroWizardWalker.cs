using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroWizardWalker : MonoBehaviour {

	public Sprite second_bg;

	// Use this for initialization
	void Start () {
	
	}

	public void WizardWalkEnded() {
		GameObject.Find("bg").GetComponent<SpriteRenderer>().sprite = second_bg;
		GameObject.Find ("mountains").SetActive (false);
		this.gameObject.SetActive (false);
		WizWalkToArena.IsWalking = true;

		GameObject.Find ("wizz_red").GetComponent<Animator> ().SetTrigger ("do_drama");
		GameObject.Find ("wizz_blue").GetComponent<Animator> ().SetTrigger ("do_drama");

		GameObject.Find ("TITLE").GetComponent<Animator> ().SetTrigger ("title_start");
		GameObject.Find ("TITLE").GetComponent<Text> ().color = Color.white;
	}
}
