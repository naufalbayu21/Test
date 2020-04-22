using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> To grenade logic and visualisation </summary>
public class Grenade : MonoBehaviour {

	[SerializeField] Rigidbody2D RB;
	[SerializeField] Renderer Renderer;
	[SerializeField] TrailRenderer Trail;
	[SerializeField] float Radius = 1.5f;

	float PowerGrenade;
	float DamageGrenade;

	LayerMask CollidedMask;

	Coroutine SetActiveCoroutine;

	/// <summary> To bullet logic and visualisation </summary>
	/// <param name = "startPos"> Grenade start position </param>
	/// <param name = "direction"> Grenade start direction </param>
	/// <param name = "speedGrenade"> Grenade start speed </param>
	/// <param name = "powerGrenade"> Power of repulsion of an object </param>
	/// <param name = "damageGrenade"> Damage caused by the grenade </param>
	/// <param name = "lifeTime"> Grenade lifeTime </param>
	/// <param name = "collidedMask"> Grenade collided mask </param>
	public void InitGrenade (Vector2 startPos, Vector2 direction, float speedGrenade, float powerGrenade, float damageGrenade, float lifeTime, LayerMask collidedMask) {
		gameObject.SetActive(true);
		Trail.Clear(); 
		transform.position = startPos;
		RB.AddForce(direction * speedGrenade, ForceMode2D.Impulse);
		PowerGrenade = powerGrenade;
		DamageGrenade = damageGrenade;
		CollidedMask = collidedMask;

		StartCoroutine(SetActiveDellay(lifeTime));
	}

	//When collided an object from the mask, inflict damage on it and push it away
	private void OnTriggerEnter2D (Collider2D collision) {
		if (!CollidedMask.LayerInMask(collision.gameObject.layer)) return;

		var damageable = collision.GetComponent<IDamageable>();
		if (damageable != null) {
			damageable.SetDamage(DamageGrenade);
		}

		if (Renderer != null && Renderer.isVisible) {
			WorldEffects.Instance.CreateExplosionEffect(transform.position);
		}
		WorldLogic.Instance.CreateExplosion(transform.position, Radius, PowerGrenade, DamageGrenade, CollidedMask);
		gameObject.SetActive(false);
	}

	//Hide object at the end of life time
	IEnumerator SetActiveDellay (float lifeTime) {
		yield return new WaitForSeconds(lifeTime);
		gameObject.SetActive(false);
	}
}
