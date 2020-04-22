using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> To bullet logic and visualisation </summary>
public class Bullet : MonoBehaviour {

	[SerializeField] Rigidbody2D RB;
	[SerializeField] Renderer Renderer;
	[SerializeField] TrailRenderer Trail;
	[SerializeField] SparkType SparkType;

	float PowerBullet;
	float DamageBullet;
	Vector2 Direction;

	LayerMask CollidedMask;

	Coroutine SetActiveCoroutine;

	/// <summary> To bullet logic and visualisation </summary>
	/// <param name = "startPos"> Bullet start position </param>
	/// <param name = "direction"> Bullet start direction </param>
	/// <param name = "speedBullet"> Bullet start speed </param>
	/// <param name = "powerBullet"> Power of repulsion of an object </param>
	/// <param name = "damageBullet"> Damage caused by the bullet </param>
	/// <param name = "lifeTime"> Bullet lifeTime </param>
	/// <param name = "collidedMask"> Bullet collided mask </param>
	public void InitBullet (Vector2 startPos, 
							Vector2 direction, 
							float speedBullet, 
							float powerBullet, 
							float damageBullet, 
							float lifeTime, 
							LayerMask collidedMask,
							Color RayColor = new Color()) 
	{
		gameObject.SetActive(true);
		Trail.Clear(); 
		transform.position = startPos;
		Direction = direction;
		RB.velocity = direction * speedBullet;
		PowerBullet = powerBullet;
		DamageBullet = damageBullet;
		CollidedMask = collidedMask;
		if (RayColor != new Color()) {
			Trail.startColor = RayColor;
			Trail.endColor = RayColor;
		}
		StartCoroutine(SetActiveDellay(lifeTime));
	}

	//When collided an object from the mask, inflict damage on it and push it away
	private void OnTriggerEnter2D (Collider2D collision) {
		if (!CollidedMask.LayerInMask(collision.gameObject.layer)) return;

		var collidedRB = collision.GetComponent<Rigidbody2D>();
		var damageable = collision.GetComponent<IDamageable>();
		if (damageable != null) {
			damageable.SetDamage(DamageBullet);
		}
		if (collidedRB != null) {
			collidedRB.AddForce(Direction * PowerBullet, ForceMode2D.Impulse);
		}

		if (Renderer != null && Renderer.isVisible) {
			WorldEffects.Instance.CreateSparkEffect(transform.position, SparkType);
		}
		gameObject.SetActive(false);
	}

	//Hide object at the end of life time
	IEnumerator SetActiveDellay (float lifeTime) {
		yield return new WaitForSeconds(lifeTime);
		gameObject.SetActive(false);
	}
}
