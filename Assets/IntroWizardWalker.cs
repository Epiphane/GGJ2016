using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroWizardWalker : MonoBehaviour {

	public Sprite second_bg;

	// Use this for initialization
	void Start () {
	
	}

	public static bool done_it = false;

	public void WizardWalkEnded() {
		this.gameObject.SetActive (false);

		if (done_it)
			return;

		done_it = true;

		GameObject.Find("bg").GetComponent<SpriteRenderer>().sprite = second_bg;
		GameObject.Find ("mountains").SetActive (false);

		WizWalkToArena.IsWalking = true;

		GameObject.Find ("wizz_red").GetComponent<Animator> ().SetTrigger ("do_drama");
		GameObject.Find ("wizz_blue").GetComponent<Animator> ().SetTrigger ("do_drama");

		GameObject.Find ("TITLE").GetComponent<Animator> ().SetTrigger ("title_start");
		GameObject.Find ("TITLE").GetComponent<Text> ().color = Color.white;

		Flowey.kill_me = true;
	}
}
