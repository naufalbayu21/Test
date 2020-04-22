using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For play sound onCollision event.
/// The volume of the sound depends on the strength of the collision.
/// </summary>

public class OnCollisionSound : MonoBehaviour {

	[SerializeField] AudioClipPreset OnColisionSound;
	[SerializeField] bool PlayIfIsVisible;
	[SerializeField] Renderer Renderer;
	[SerializeField] LayerMask PlaySoundIfCollidedLayers;

	private void OnCollisionEnter2D (Collision2D collision) {
		if ((!PlayIfIsVisible || Renderer.isVisible) && PlaySoundIfCollidedLayers.LayerInMask(collision.collider.gameObject.layer)) {
			SoundController.PlaySound(OnColisionSound, collision.relativeVelocity.magnitude / 2);
		}
	}
}
