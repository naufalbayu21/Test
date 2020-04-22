using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary> Key binding UI element </summary>
[RequireComponent(typeof(CustomButton))]
public class KeyBindingUI : MonoBehaviour {

	[SerializeField] Text KeyText;			//Text key in bind window
	[SerializeField] Text ValueText;		//Text value in bind window
	[SerializeField] Animator Animator;		//Animator for enabled or disabled wait bind
	[SerializeField] GameObject Blocker;	//Block UI interfase gameObject

	InputType InputType;
	ActionKey ActionKey;

	Dictionary<ActionKey, KeyCode> DictionaryKey {
		get {
			if (InputType == InputType.KeyboardAndMouse) {
				return KeyBinding.KeyboardKeys;
			} else if (InputType == InputType.GamePad) {
				return KeyBinding.GamePadKeys;
			} else {
				return new Dictionary<ActionKey, KeyCode>();
			}
		}
	}

	List<KeyCode> AvailableKeys {
		get {
			if (InputType == InputType.GamePad) {
				return KeyBinding.AvailableGamePadButtons;
			} else if (InputType == InputType.KeyboardAndMouse) {
				return KeyBinding.AvailableKeyBoardsButtons;
			}
			return new List<KeyCode>();
		}
	}

	/// <summary> Init key bind element </summary>
	public void Init (InputType inputType, ActionKey actionKey) {
		Blocker.SetActive(false);
		InputType = inputType;
		ActionKey = actionKey;
		KeyText.text = actionKey.ToString();
		ValueText.text = DictionaryKey.TryGetOrDefault(actionKey).ToString();

		var btn = GetComponent<CustomButton>();
		btn.onClick.AddListener(OnClickButton);
	}

	/// <summary> Update value after change any key </summary>
	public void UpdateValue () {
		ValueText.text = DictionaryKey.TryGetOrDefault(ActionKey).ToString();
	}

	private void OnDisable () {
		StopAllCoroutines();
	}

	/// <summary> OnClick action, enable blocker and start coroutine for wait press key </summary>
	void OnClickButton () {
		Animator.SetBool(C.Animation, true);
		Blocker.SetActive(true);
		ValueText.text = string.Empty;
		StartCoroutine(WaitToPressKey());
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
	}

	/// <summary> Coroutine for wait press key </summary>
	IEnumerator WaitToPressKey () {
		yield return null;
		while (true) {
			yield return null;
			if (Input.anyKeyDown) {
				foreach (var key in AvailableKeys) {
					if (Input.GetKeyDown(key)) {
						KeyBinding.SetInDictionary(InputType, ActionKey, key);
						break;
					}
				}
				UpdateValue();
				Blocker.SetActive(false);
				Animator.SetBool(C.Animation, false);
				this.SetSelectedGameObject();
				yield break;
			}
		}
	}
}
