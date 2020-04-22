using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> HealthLoot, heals only allies </summary>
public class HealthLoot : MonoBehaviour {

	[SerializeField] float Health;					//Health value
	[SerializeField] AudioClipPreset TakeSound;		//Sound star play on take health
	[SerializeField] LayerMask MaskTriggered;		//Layers can take health

	private void OnTriggerEnter2D (Collider2D collision) {
		if (MaskTriggered.LayerInMask(collision.gameObject.layer)) {
			var character = collision.gameObject.GetComponent<Character>();
			if (character != null) {
				character.AddHealth(Health);
				SoundController.PlaySound(TakeSound);
				Destroy(gameObject);
			}
		}
	}
}
