using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : Singleton<LoadingScreen> {

	[SerializeField] GameObject LoadingScreenObject;
	protected override void AwakeSingleton () {	}

	public static void Show () {
		if (Instance != null) {
			Instance.LoadingScreenObject.SetActive(true);
		}
	}

	public static void Hide () {
		if (Instance != null) {
			Instance.LoadingScreenObject.SetActive(false);
		}
	}
}
