using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To kill an object caught in a trigger
public class DeathTrigger : MonoBehaviour {

	[SerializeField] LayerMask CollidedMask;
	[SerializeField] ParticleSystem CollidedEffect;
	[SerializeField] AudioClipPreset CollidedSound;
	[SerializeField] bool SetEffectPositionByX = true;
	[SerializeField] bool SetEffectPositionByY = true;

	private void OnTriggerEnter2D (Collider2D collision) {
		if (!CollidedMask.LayerInMask(collision.Layer())) return;

		var damageable = collision.gameObject.GetComponent<IDamageable>();

		if (damageable != null) {
			damageable.SetDamage(damageable.Health);
			if (CollidedEffect != null) {
				Vector3 newPos = CollidedEffect.transform.position;
				if (SetEffectPositionByX) {
					newPos.x = collision.transform.position.x;
				}
				if (SetEffectPositionByY) {
					newPos.x = collision.transform.position.y;
				}
				CollidedEffect.transform.position = newPos;
				CollidedEffect.Play();
			}
			if (CollidedSound != null) {
				SoundController.PlaySound(CollidedSound);
			}
		}
	}
}
