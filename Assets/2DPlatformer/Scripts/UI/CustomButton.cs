using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary> Custom button To sign the events: OnClick, OnDown, OnPressed, OnEnter, OnExit </summary>
[Serializable]
public class CustomButton : Button {

	[SerializeField] SoundType SoundType;
	[SerializeField] AudioClipPreset OnHighlightSound;
	[SerializeField] AudioClipPreset OnClickSound;

	public bool Pressed { get; private set; }
	public bool ButtonDown { get; private set; }
	public bool ButtonUp { get; private set; }

	public event Action OnDown;

	public event Action OnUp;

	public event Action<GameObject> OnSelectAction;

	public event Action OnPointerEnterAction;

	public event Action OnHighlightAction;

	public bool SoundIsCustom { get { return SoundType == SoundType.Custom; } }

	protected override void Awake () {
		onClick.AddListener(OnClick);
	}

	public void OnClick () {
		var clickSound = GetOnClickSound;
		if (clickSound != null) {
			SoundController.PlaySound(clickSound);
		}

		Pressed = false;
	}

	private AudioClipPreset GetOnHighlightSound {
		get {
			switch (SoundType) {
				case SoundType.Custom: return OnHighlightSound;
				case SoundType.Standart: return B.Sound.ButtonOnHighlightSound;
				default: return null;
			}
		}
	}

	private AudioClipPreset GetOnClickSound {
		get {
			switch (SoundType) {
				case SoundType.Custom: return OnClickSound;
				case SoundType.Standart: return B.Sound.ButtonOnClickSound;
				default: return null;
			}
		}
	}

	public override void OnSelect (BaseEventData eventData) {
		OnSelectAction.SafeInvoke(gameObject);
		OnHighlight();

		base.OnSelect(eventData);
	}

	public override void OnPointerEnter (PointerEventData eventData) {
		OnPointerEnterAction.SafeInvoke();
		OnHighlight();

		base.OnPointerEnter(eventData);
	}

	public override void OnPointerDown (PointerEventData eventData) {
		OnDown.SafeInvoke();

		ButtonDown = true;
		Pressed = true;

		base.OnPointerDown(eventData);
	}

	public override void OnPointerUp (PointerEventData eventData) {
		OnUp.SafeInvoke();

		ButtonUp = true;
		Pressed = false;

		base.OnPointerUp(eventData);
	}

	private void OnHighlight () {
		if (GetOnHighlightSound != null) {
			SoundController.PlaySound(GetOnHighlightSound);
		}

		OnHighlightAction.SafeInvoke();
	}

	private void LateUpdate () {
		ButtonDown = false;
		ButtonUp = false;
	}
}
