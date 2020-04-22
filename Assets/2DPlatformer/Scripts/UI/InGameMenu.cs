using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary> GameOverUI controller  </summary>
public class InGameMenu : Singleton<InGameMenu> {

	[SerializeField] Text CaptionText;
	[SerializeField] string PauseString = "Pause";
	[SerializeField] string GameOverString = "GameOver";
	[SerializeField] Button ContinueBtn;
	[SerializeField] Button RestartLevelBtn;
	[SerializeField] Button NextLevelBtn;
	[SerializeField] Button MainMenuBtn;
	[SerializeField] GameObject HolderMenu;		//SetActive gameObject on call ShowMenu or ShowGameOver

	protected override void AwakeSingleton () {
		ContinueBtn.onClick.AddListener(OnContinue);
		RestartLevelBtn.onClick.AddListener(OnRestartLevel);
		NextLevelBtn.onClick.AddListener(OnNextLevel);
		MainMenuBtn.onClick.AddListener(OnMainMenu);
	}

	/// <summary> Show game over window </summary>
	/// <param name="endLevelTriggered"> if need show "next level button"</param>
	public void ShowGameOver (bool endLevelTriggered = false) {
		CaptionText.text = GameOverString;
		BattleEntity.Instance.IsGameOver = true;
		ContinueBtn.SetActive(false);
		if (endLevelTriggered) {
			SceneController.CurrentLevel.MarkAsCompleted();
		}
		ShowMenu();
		if (NextLevelBtn.interactable && NextLevelBtn.gameObject.activeInHierarchy) {
			 NextLevelBtn.SetSelectedGameObject();
		} else {
			RestartLevelBtn.SetSelectedGameObject();
		}
		NextLevelBtn.SetActive(SceneController.HasNextLevel() && SceneController.CurrentLevel.LevelIsCompleted());
	}

	/// <summary> Show game pause </summary>
	public void ShowPause () {
		Time.timeScale = 0;
		CaptionText.text = PauseString;
		ContinueBtn.SetActive(true);
		ShowMenu();
		ContinueBtn.SetSelectedGameObject();
	}

	void ShowMenu () {
		HolderMenu.SetActive(true);
		NextLevelBtn.SetActive(SceneController.HasNextLevel() && SceneController.CurrentLevel.LevelIsCompleted());
	}

	void OnContinue () {
		if (!BattleEntity.Instance.IsGameOver) {
			Time.timeScale = 1;
			HolderMenu.SetActive(false);
		}
	}

	void OnRestartLevel () {
		SceneController.RestartCurrentLevel();
	}

	void OnNextLevel () {
		SceneController.LoadNextLevel();
	}

	void OnMainMenu () {
		SceneController.ToMainMenuScene();
	}

	private void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (HolderMenu.activeInHierarchy) {
				OnContinue();
			} else {
				ShowPause();
			}
		}
	}

	private void OnDestroy () {
		Time.timeScale = 1;
	}
}
