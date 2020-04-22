using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;

/// <summary> 
/// Fast access to settings 
/// B - Balance
/// </summary>
public static class B {

	private static GameSettings gameSettings;

	public static GameSettings GameSettings {
		get {
			if (gameSettings == null) {
				gameSettings = Resources.Load <GameSettings> ("GameSettings");
			}
			return gameSettings;
		}
	}

	public static LevelSettings Levels { get { return GameSettings.Levels; } }

	public static SoundSettings Sound { get { return GameSettings.Sound; } }

	public static ResourceSettings Resource { get { return GameSettings.Resource; } }

	public static LayersSettings Layers { get { return GameSettings.Layers; } }

	public static MathSettings Math { get { return GameSettings.Math; } }
}

/// <summary> 
/// Constants used in game
/// C - Constants
/// </summary>
/// 
public static class C {

	//PlayerProfile constants
	public const string InputTypeKey = "InputType";
	public const string SoundVolumeKey = "SoundVolume";
	public const string MusicVolumeKey = "MusicVolume";
	public const string LevelCompletedKey = "_Completed";
	public const string KeyBoardKey = "KeyBoadrd_";
	public const string GamePadKey = "GamePad_";

	//HeroAnimator constants
	public const string InAir = "InAir";
	public const string Jump = "Jump";
	public const string IsMove ="IsMove";
	public const string MoveSpeed = "MoveSpeed";
	public const string IsDead = "IsDead";

	//HandsAnimator constants
	public const string Shot = "Shot";
	public const string Reload = "Reload";

	//HandsAnimator constants
	public const string Animation = "Animation";

	//OldAnimator constants
	public const string StartAnimation = "StartAnimation";
	public const string OpenDoorAnimation = "OpenDoor";
}
