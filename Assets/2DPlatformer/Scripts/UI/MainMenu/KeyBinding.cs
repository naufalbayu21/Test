using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary> KeyBinding window, and keybinding static parameters </summary>
public class KeyBinding : BaseWindow {

	[SerializeField] Button ToDefaultButton;				//Button reset binding keys to default
	[SerializeField] Button ApplyButton;					//Button reset binding keys to default
	[SerializeField] KeyBindingUI KeyBindingKeyboardRef;	//Ref to binding button for keyboard
	[SerializeField] KeyBindingUI KeyBindingGamePadRef;		//Ref to binding button for gamepad
	[SerializeField] GameObject ObjectForKeyBoardKeys;		//GameObject SetActive if input is keyboard
	[SerializeField] GameObject ObjectForGamePadKeys;		//GameObject SetActive if input is gamepad

	List<KeyBindingUI> KeyBindingButtons = new List<KeyBindingUI>();

	static KeyBinding Instance;

	/// <summary> Initialize KeyBinding window </summary>
	public override void Init () {
		base.Init();
		LoadKeys();
		Instance = this;
		InitButtons ();
		ApplyButton.onClick.AddListener(SaveKeys);
		ToDefaultButton.onClick.AddListener(ResetKeys);
	}

	/// <summary> Create and initialize all KeyBinding buttons </summary>
	private void InitButtons () {
		foreach (var kv in GamePadKeys) {
			var newBtn = Instantiate(KeyBindingGamePadRef, KeyBindingGamePadRef.transform.parent);
			newBtn.Init(InputType.GamePad, kv.Key);
			KeyBindingButtons.Add(newBtn);
		}
		KeyBindingGamePadRef.SetActive(false);

		foreach (var kv in KeyboardKeys) {
			var newBtn = Instantiate(KeyBindingKeyboardRef, KeyBindingKeyboardRef.transform.parent);
			newBtn.Init(InputType.KeyboardAndMouse, kv.Key);
			KeyBindingButtons.Add(newBtn);
		}
		KeyBindingKeyboardRef.SetActive(false);
	}

	/// <summary> Update all KeyBinding buttons after any change </summary>
	void UpdateValueButtons () {
		foreach (var btn in KeyBindingButtons) {
			btn.UpdateValue();
		}
	}

	protected override void OnEnable () {
		if (PlayerProfile.InputType == InputType.GamePad) {
			ObjectForGamePadKeys.SetActive(true);
			ObjectForKeyBoardKeys.SetActive(false);
		} else if (PlayerProfile.InputType == InputType.KeyboardAndMouse) {
			ObjectForGamePadKeys.SetActive(false);
			ObjectForKeyBoardKeys.SetActive(true);
		} else {
			ObjectForGamePadKeys.SetActive(false);
			ObjectForKeyBoardKeys.SetActive(false);
		}
		base.OnEnable();
	}

	protected override void OnEnableNextFrame () {
		var btn = KeyBindingButtons.First(b => b.gameObject.activeInHierarchy);
		if (btn != null) {
			btn.SetSelectedGameObject();
		}
	}

	#region Binding logic

	/// <summary> 
	/// Binding logic, 
	/// all fields and methods is static, for fast acces from any script
	/// To add any action you need to change "ActionKey" and add new lines in methods: LoadKeys, SaveKeys, ResetKeys.
	/// </summary>

	public static Dictionary<ActionKey, KeyCode> GamePadKeys = new Dictionary<ActionKey, KeyCode>();
	public static Dictionary<ActionKey, KeyCode> KeyboardKeys = new Dictionary<ActionKey, KeyCode>();

	private static bool IsInitialized;
	private static Dictionary<ActionKey, KeyCode> SelectedKeys;

	/// <summary> LoadKeys need call on start game </summary>
	private static void LoadKeys () {
		GamePadKeys.Clear();
		KeyboardKeys.Clear();

		AddInGamePadDictionary(ActionKey.Shot, KeyCode.JoystickButton5);
		AddInGamePadDictionary(ActionKey.Jump, KeyCode.JoystickButton0);
		AddInGamePadDictionary(ActionKey.Reload, KeyCode.JoystickButton2);
		AddInGamePadDictionary(ActionKey.NextGun, KeyCode.JoystickButton3);
		AddInGamePadDictionary(ActionKey.Interaction, KeyCode.JoystickButton1);

		AddInKeyBoardDictionary(ActionKey.MoveToLeft, KeyCode.A);
		AddInKeyBoardDictionary(ActionKey.MoveToRight, KeyCode.D);
		AddInKeyBoardDictionary(ActionKey.Shot, KeyCode.Mouse0);
		AddInKeyBoardDictionary(ActionKey.Jump, KeyCode.Space);
		AddInKeyBoardDictionary(ActionKey.Reload, KeyCode.R);
		AddInKeyBoardDictionary(ActionKey.NextGun, KeyCode.Tab);
		AddInKeyBoardDictionary(ActionKey.Interaction, KeyCode.E);

		if (!IsInitialized) {
			PlayerProfile.OnSetInputType += OnSetInputType;
			OnSetInputType(PlayerProfile.InputType);
			IsInitialized = true;
		}
	}

	/// <summary> Save keys after any changed </summary>
	private static void SaveKeys () {
		if (PlayerProfile.InputType == InputType.GamePad) {
			SaveGamePadKey(ActionKey.Shot, GamePadKeys.TryGetOrDefault(ActionKey.Shot));
			SaveGamePadKey(ActionKey.Jump, GamePadKeys.TryGetOrDefault(ActionKey.Jump));
			SaveGamePadKey(ActionKey.Reload, GamePadKeys.TryGetOrDefault(ActionKey.Reload));
			SaveGamePadKey(ActionKey.NextGun, GamePadKeys.TryGetOrDefault(ActionKey.NextGun));
			SaveGamePadKey(ActionKey.Interaction, GamePadKeys.TryGetOrDefault(ActionKey.Interaction));
		} else if (PlayerProfile.InputType == InputType.KeyboardAndMouse) {
			SaveKeyBoardKey(ActionKey.MoveToLeft, KeyboardKeys.TryGetOrDefault(ActionKey.MoveToLeft));
			SaveKeyBoardKey(ActionKey.MoveToRight, KeyboardKeys.TryGetOrDefault(ActionKey.MoveToRight));
			SaveKeyBoardKey(ActionKey.Shot, KeyboardKeys.TryGetOrDefault(ActionKey.Shot));
			SaveKeyBoardKey(ActionKey.Jump, KeyboardKeys.TryGetOrDefault(ActionKey.Jump));
			SaveKeyBoardKey(ActionKey.Reload, KeyboardKeys.TryGetOrDefault(ActionKey.Reload));
			SaveKeyBoardKey(ActionKey.NextGun, KeyboardKeys.TryGetOrDefault(ActionKey.NextGun));
			SaveKeyBoardKey(ActionKey.Interaction, KeyboardKeys.TryGetOrDefault(ActionKey.Interaction));
		}
		LoadKeys();
		if (Instance != null) {
			Instance.UpdateValueButtons();
		}
	}

	/// <summary> Reset all keys to default </summary>
	private static void ResetKeys () {
		SaveGamePadKey(ActionKey.Shot, KeyCode.JoystickButton5);
		SaveGamePadKey(ActionKey.Jump, KeyCode.JoystickButton0);
		SaveGamePadKey(ActionKey.Reload, KeyCode.JoystickButton2);
		SaveGamePadKey(ActionKey.NextGun, KeyCode.JoystickButton3);
		SaveGamePadKey(ActionKey.Interaction, KeyCode.JoystickButton1);

		SaveKeyBoardKey(ActionKey.MoveToLeft, KeyCode.A);
		SaveKeyBoardKey(ActionKey.MoveToRight, KeyCode.D);
		SaveKeyBoardKey(ActionKey.Shot, KeyCode.Mouse0);
		SaveKeyBoardKey(ActionKey.Jump, KeyCode.Space);
		SaveKeyBoardKey(ActionKey.Reload, KeyCode.R);
		SaveKeyBoardKey(ActionKey.NextGun, KeyCode.Tab);
		SaveKeyBoardKey(ActionKey.Interaction, KeyCode.E);
		
		LoadKeys();
		if (Instance != null) {
			Instance.UpdateValueButtons();
		}
	}

	/// <summary> Set key and checking for binding with this key </summary>
	public static void SetInDictionary (InputType inputType, ActionKey key, KeyCode newKey) {
		Dictionary<ActionKey, KeyCode> dictionary = new Dictionary<ActionKey, KeyCode>();

		if (inputType == InputType.KeyboardAndMouse) {
			dictionary = KeyboardKeys;
		} else if (inputType == InputType.GamePad) {
			dictionary = GamePadKeys;
		}

		KeyValuePair<ActionKey, KeyCode> kv;
		if (dictionary.TryGetKeyValuePair(newKey, out kv)) {
			dictionary[kv.Key] = KeyCode.None;
		}
		dictionary.TrySetOrCreate(key, newKey);

		if (Instance != null) {
			Instance.UpdateValueButtons();
		}
	}

	/// <summary> Add in gamepad dictionary, for initialization </summary>
	private static void AddInGamePadDictionary (ActionKey key, KeyCode defaultKey) {
		var keykode = PlayerProfile.GetGamePad(key, defaultKey);
		GamePadKeys.Add(key, keykode);
	}

	/// <summary> Save gamepad key in PlayerProfile </summary>
	private static void SaveGamePadKey (ActionKey key, KeyCode newKey) {
		PlayerProfile.SetGamePad(key, newKey);
	}

	/// <summary> Add in keyboard dictionary, for initialization </summary>
	private static void AddInKeyBoardDictionary (ActionKey key, KeyCode defaultKey) {
		var keykode = PlayerProfile.GetKeyBoard(key, defaultKey);
		KeyboardKeys.Add(key, keykode);
	}

	/// <summary> Save keyboard key in PlayerProfile </summary>
	private static void SaveKeyBoardKey (ActionKey key, KeyCode newKey) {
		PlayerProfile.SetKeyBoard(key, newKey);
	}

	/// <summary> Called if changed InputType </summary>
	private static void OnSetInputType (InputType inputType) {
		if (inputType == InputType.GamePad) {
			SelectedKeys = GamePadKeys;
		} else if (inputType == InputType.KeyboardAndMouse) {
			SelectedKeys = KeyboardKeys;
		}
	}

	/// <summary> Return true if pressed binded on actionKey key </summary>
	public static bool GetKey (ActionKey actionKey) {
		var key = SelectedKeys.TryGetOrDefault(actionKey);
		return Input.GetKey(key);
	}

	/// <summary> Return true if down binded on actionKey key </summary>
	public static bool GetKeyDown (ActionKey actionKey) {
		var key = SelectedKeys.TryGetOrDefault(actionKey);
		return Input.GetKeyDown(key);
	}

	/// <summary> Return true if up binded on actionKey key </summary>
	public static bool GetKeyUp (ActionKey actionKey) {
		var key = SelectedKeys.TryGetOrDefault(actionKey);
		return Input.GetKeyUp(key);
	}

	/// <summary> Gamepad keys to which you can bind an action </summary>
	#region Available GamePad Buttons

	static public readonly List<KeyCode> AvailableGamePadButtons = new List<KeyCode>() {
		KeyCode.JoystickButton0,
		KeyCode.JoystickButton1,
		KeyCode.JoystickButton2,
		KeyCode.JoystickButton3,
		KeyCode.JoystickButton4,
		KeyCode.JoystickButton5,
		KeyCode.JoystickButton6,
		KeyCode.JoystickButton7,
		KeyCode.JoystickButton8,
		KeyCode.JoystickButton9,
		KeyCode.JoystickButton10,
		KeyCode.JoystickButton11,
		KeyCode.JoystickButton12,
		KeyCode.JoystickButton13,
		KeyCode.JoystickButton14,
		KeyCode.JoystickButton15,
		KeyCode.JoystickButton16,
		KeyCode.JoystickButton17,
		KeyCode.JoystickButton18,
		KeyCode.JoystickButton19,
	};

	#endregion //Available GamePad Buttons

	/// <summary> Keyboard keys to which you can bind an action </summary>
	#region Available KeyBoards Buttons

	static public readonly List<KeyCode> AvailableKeyBoardsButtons = new List<KeyCode>() {
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6,
		KeyCode.A,
		KeyCode.B,
		KeyCode.C,
		KeyCode.D,
		KeyCode.E,
		KeyCode.F,
		KeyCode.G,
		KeyCode.H,
		KeyCode.I,
		KeyCode.J,
		KeyCode.K,
		KeyCode.L,
		KeyCode.M,
		KeyCode.N,
		KeyCode.O,
		KeyCode.P,
		KeyCode.Q,
		KeyCode.R,
		KeyCode.S,
		KeyCode.T,
		KeyCode.U,
		KeyCode.V,
		KeyCode.W,
		KeyCode.X,
		KeyCode.Y,
		KeyCode.Z,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9,
		KeyCode.Comma,
		KeyCode.Period,
		KeyCode.Question,
		KeyCode.Quote,
		KeyCode.LeftBracket,
		KeyCode.RightBracket,
		KeyCode.Backslash,
		KeyCode.Semicolon,
		KeyCode.Minus,
		KeyCode.Equals,
		KeyCode.Keypad0,
		KeyCode.Keypad1,
		KeyCode.Keypad2,
		KeyCode.Keypad3,
		KeyCode.Keypad4,
		KeyCode.Keypad5,
		KeyCode.Keypad6,
		KeyCode.Keypad7,
		KeyCode.Keypad8,
		KeyCode.Keypad9,
		KeyCode.KeypadDivide,
		KeyCode.KeypadEquals,
		KeyCode.KeypadMinus,
		KeyCode.KeypadMultiply,
		KeyCode.KeypadPeriod,
		KeyCode.KeypadPlus,
		KeyCode.LeftControl,
		KeyCode.RightControl,
		KeyCode.LeftCommand,
		KeyCode.RightCommand,
		KeyCode.LeftShift,
		KeyCode.RightShift,
		KeyCode.Tab,
		KeyCode.Space,
		KeyCode.LeftArrow,
		KeyCode.UpArrow,
		KeyCode.RightArrow,
		KeyCode.DownArrow,
	};
	
	#endregion //Available KeyBoards Buttons

	#endregion //Binding logic
}

/// <summary> All actions in game </summary>
public enum ActionKey {
	MoveToLeft,
	MoveToRight,
	Shot,
	Jump,
	Reload,
	NextGun,
	Interaction,
}
