using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : Singleton<MainMenuUI> {

	[SerializeField] MainMenuWindow mainButtonsWindow;
	[SerializeField] LevelsWindow selectLevelWindow;
	[SerializeField] SettingsWindow settingsWindow;
	[SerializeField] KeyBinding keyBindingWindow;
	[SerializeField] RectTransform blocker;

	public MainMenuWindow MainButtonsWindow { get { return mainButtonsWindow; } }
	public LevelsWindow SelectLevelWindow { get { return selectLevelWindow; } }
	public SettingsWindow SettingsWindow { get { return settingsWindow; } }
	public KeyBinding KeyBindingWindow { get { return keyBindingWindow; } }
	public RectTransform Blocker { get { return blocker; } }
	public BaseWindow CurrentWindow { get; set; }

	protected override void AwakeSingleton () {
		MainButtonsWindow.Init();
		SelectLevelWindow.Init();
		SettingsWindow.Init();
		KeyBindingWindow.Init();

		SelectLevelWindow.SetActive(false);
		SettingsWindow.SetActive(false);
		KeyBindingWindow.SetActive(false);
		Blocker.SetActive(false);

		CurrentWindow = MainButtonsWindow;
	}
}
