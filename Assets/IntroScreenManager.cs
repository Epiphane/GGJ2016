﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroScreenManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	


	// Update is called once per frame
	void Update () {
		if (Input.anyKey && !Input.GetMouseButton(0)) {
			SceneManager.LoadScene ("ElliotMinigame");
		}

	}
}
