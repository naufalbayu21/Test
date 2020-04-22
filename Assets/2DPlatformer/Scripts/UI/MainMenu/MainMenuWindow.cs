using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : BaseWindow {

	[SerializeField] CustomButton ContinueButton;
	[SerializeField] CustomButton SelectLevelButton;
	[SerializeField] CustomButton SettingsButton;
	[SerializeField] CustomButton QuitButton;

	public override void Init () {
		ContinueButton.onClick.AddListener(OnContinue);
		SelectLevelButton.onClick.AddListener(OnSelectLevelsWindow);
		SettingsButton.onClick.AddListener(OnSettings);
		QuitButton.onClick.AddListener(OnQuit);
		base.Init();
	}

	protected override void OnEnableNextFrame () {
		ContinueButton.SetSelectedGameObject();
	}

	void OnContinue () {
		MainMenu.SelectLevelWindow.LastUnlockedLevel.LoadLevel();
	}

	void OnSelectLevelsWindow () {
		MainMenu.SelectLevelWindow.SelectWindow();
		
	}

	void OnSettings () {
		MainMenu.SettingsWindow.SelectWindow();
	}

	void OnQuit () {
	#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
	#else
		Application.Quit();
	#endif
	}
}
