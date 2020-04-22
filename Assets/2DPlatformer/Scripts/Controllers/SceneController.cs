using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameBalance;


public class SceneController {

	public static LevelSettings.LevelPreset CurrentLevel { get; private set; }

	private static int CurrentLevelIndex;
	private static int MainWorldSceneIndex { get { return SceneUtility.GetBuildIndexByScenePath(B.Levels.MainWorldScene); } }
	private static int MainMenuSceneIndex { get { return SceneUtility.GetBuildIndexByScenePath(B.Levels.MainMenuScene); } }

	public static void LoadLevel (LevelSettings.LevelPreset newLevel) {
		CurrentLevelIndex = SceneUtility.GetBuildIndexByScenePath(newLevel.LevelScenePath);
		if (CurrentLevelIndex == -1 || MainWorldSceneIndex == -1) {
			Debug.LogErrorFormat("Level {0} or Main Scene not found", newLevel.LevelName);
			return;
		}

		LoadingScreen.Show();
		SceneManager.LoadScene(MainWorldSceneIndex);
		SceneManager.LoadScene(CurrentLevelIndex, LoadSceneMode.Additive);
		
		CurrentLevel = newLevel;

		if (CurrentLevel.MusicInLevel != null && CurrentLevel.MusicInLevel.Count > 0) {
			SoundController.Instance.SetMusicPlayList(CurrentLevel.MusicInLevel);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void SubscribeOnLoaded () {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public static void OnSceneLoaded (Scene loadedScene, LoadSceneMode sceneMode) {
		if (loadedScene.buildIndex == CurrentLevelIndex || loadedScene.buildIndex == MainMenuSceneIndex) {
			LoadingScreen.Hide();
		}
	}

	public static void RestartCurrentLevel () {
		LoadLevel(CurrentLevel);
	}

	public static void LoadNextLevel () {
		LevelSettings.LevelPreset nextLevel = null;
		if (HasNextLevel(out nextLevel)) {
			LoadLevel(nextLevel);
		} else {
			Debug.LogError("Next level is not available, current level name: " + CurrentLevel.LevelName);
		}
	}

	public static bool HasNextLevel () {
		if (CurrentLevel == null) return false;
		var levels = B.Levels.Levels;
		var levelIndex = levels.IndexOf(CurrentLevel);

		return (levels.Count - 1) > levelIndex;
	}

	public static bool HasNextLevel (out LevelSettings.LevelPreset nextLevel) {
		nextLevel = null;
		if (CurrentLevel == null) return false;
		var levels = B.Levels.Levels;
		var levelIndex = levels.IndexOf(CurrentLevel);
		bool value = (levels.Count - 1) > levelIndex;
		if (value) {
			nextLevel = B.Levels.Levels[levelIndex + 1];
		}
		return value;
	}

	public static void ToMainMenuScene () {
		LoadingScreen.Show();
		SoundController.Instance.SetMusicPlayList(B.Sound.Music);
		SceneManager.LoadScene(MainMenuSceneIndex);
		CurrentLevel = null;
		CurrentLevelIndex = -1;
	}
	
}
