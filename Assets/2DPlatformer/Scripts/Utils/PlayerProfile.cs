using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> For save and load all settings from PlayerPrefs </summary>
public static class PlayerProfile {

	/// <summary> Get/Set saved input type </summary>
	static public InputType InputType{
		get {
			return (InputType)PlayerPrefs.GetInt(C.InputTypeKey, (int)B.GameSettings.DefaultInputType);
		}
		set {
			PlayerPrefs.SetInt(C.InputTypeKey, (int)value);
			OnSetInputType.SafeInvoke(value);
		}
	}

	static public event System.Action<InputType> OnSetInputType;

	/// <summary> Get/Set saved sound volume </summary>
	static public float SoundVolume {
		get {
			return PlayerPrefs.GetFloat(C.SoundVolumeKey, 1);
		}
		set {
			PlayerPrefs.SetFloat(C.SoundVolumeKey, value);
		}
	}

	/// <summary> Get/Set saved music volume </summary>
	static public float MusicVolume {
		get {
			return PlayerPrefs.GetFloat(C.MusicVolumeKey, 1);
		}
		set {
			PlayerPrefs.SetFloat(C.MusicVolumeKey, value);
		}
	}

	/// <summary> Get/Set saved level state </summary>
	static public bool GetLevelIsCompleted (string sceneName) {
		return PlayerPrefs.GetInt(sceneName + C.LevelCompletedKey, 0) == 0? false: true;
	}

	static public void SetLevelIsCompleted (string sceneName, bool completed = true) {
		PlayerPrefs.SetInt(sceneName + C.LevelCompletedKey, completed? 1: 0);
	}

	/// <summary> Get/Set Key code for keyboard key binding </summary>
	static public KeyCode GetKeyBoard (ActionKey actionKey, KeyCode defaultKey) {
		int defaultInt = (int)defaultKey;
		var keyInt = PlayerPrefs.GetInt(C.KeyBoardKey + actionKey, defaultInt);
		return (KeyCode)keyInt;
	}

	static public void SetKeyBoard (ActionKey actionKey, KeyCode newKey) {
		int newInt = (int)newKey;
		PlayerPrefs.SetInt(C.KeyBoardKey + actionKey, newInt);
	}

	/// <summary> Get/Set Key code for gamepad key binding </summary>
	static public KeyCode GetGamePad (ActionKey actionKey, KeyCode defaultKey) {
		int defaultInt = (int)defaultKey;
		var keyInt = PlayerPrefs.GetInt(C.GamePadKey + actionKey, defaultInt);
		return (KeyCode)keyInt;
	}

	static public void SetGamePad (ActionKey actionKey, KeyCode newKey) {
		int newInt = (int)newKey;
		PlayerPrefs.SetInt(C.GamePadKey + actionKey, newInt);
	}
}
