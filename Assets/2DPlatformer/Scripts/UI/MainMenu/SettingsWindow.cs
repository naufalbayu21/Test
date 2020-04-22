using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : BaseWindow {

	[SerializeField] Slider SoundSlider;
	[SerializeField] Slider MusicSlider;
	[SerializeField] Button KeyBindingButton;

	public override void Init () {
		
		KeyBindingButton.onClick.AddListener(OnKeyBinding);
		base.Init();

		PlayerProfile.OnSetInputType += UpdateKeyBindingButtonState;

		UpdateKeyBindingButtonState(PlayerProfile.InputType);

		SoundSlider.value = PlayerProfile.SoundVolume;
		MusicSlider.value = PlayerProfile.MusicVolume;

		SoundSlider.onValueChanged.AddListener(OnSoundValueChanged);
		MusicSlider.onValueChanged.AddListener(OnMusicValueChanged);
	}

	

	protected override void OnEnableNextFrame () {
		SoundSlider.SetSelectedGameObject();
	}

	void UpdateKeyBindingButtonState (InputType inputType) {
		KeyBindingButton.SetActive(inputType != InputType.TouchScreen);
	}

	void OnKeyBinding () {
		MainMenu.KeyBindingWindow.SelectWindow();
	}

	void OnSoundValueChanged (float newValue) {
		PlayerProfile.SoundVolume = newValue;
		SoundController.Instance.UpdateVolume();
	}

	void OnMusicValueChanged (float newValue) {
		PlayerProfile.MusicVolume = newValue;
		SoundController.Instance.UpdateVolume();
	}

	private void OnDestroy () {
		PlayerProfile.OnSetInputType -= UpdateKeyBindingButtonState;
	}
}
