using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// For paly sound from animator event
/// </summary>
public class PlaySound : MonoBehaviour {

	[SerializeField] AudioClipPreset ClipPresset;

	[SerializeField] public bool PlayIfRendererIsVisible;

	[SerializeField, ShowInInspectorCondition("PlayIsVisible")] Renderer Renderer;

	public bool PlayIsVisible { get { return PlayIfRendererIsVisible; } }

	private void Awake () {
		if (PlayIfRendererIsVisible && Renderer == null) {
			Debug.LogError("Renderer is null");
		}
	}

	public void PlaySoundNow () {
		if (!PlayIfRendererIsVisible || (Renderer != null && Renderer.isVisible)) {
			SoundController.PlaySound(ClipPresset);
		}
	}
}
