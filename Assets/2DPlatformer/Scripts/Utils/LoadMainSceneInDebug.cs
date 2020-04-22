using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> To load the main scene from any scene </summary>
public class LoadMainSceneInDebug : MonoBehaviour {

	private void Awake () {
		var inputController = FindObjectOfType<LoadingScreen>();
		if (inputController == null) {
			Debug.LogWarningFormat("Level scene started, load main menu scene.");
			var mainSceneIndex = SceneUtility.GetBuildIndexByScenePath(B.Levels.MainMenuScene);
			SceneManager.LoadScene(mainSceneIndex);
		}
	}
}
