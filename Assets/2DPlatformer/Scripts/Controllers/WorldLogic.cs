using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> To create effects logic </summary>
public class WorldLogic : Singleton<WorldLogic> {

	[SerializeField] int BulletPullCount = 20;
	[SerializeField] int RayBulletPullCount = 20;
	[SerializeField] int ExplosionPullCount = 10;

	List<Bullet> Bullets = new List<Bullet>();
	List<Bullet> RayBullets = new List<Bullet>();
	List<Grenade> Grenades = new List<Grenade>();

	WorldEffects Effects { get { return WorldEffects.Instance; } }

	/// <summary> Creating objects and populating lists </summary>
	protected override void AwakeSingleton () {
		for (int i = 0; i < BulletPullCount; i++) {
			var bullet = Instantiate(B.Resource.Prefabs.BulletPrefab, transform);
			bullet.SetActive(false);
			Bullets.Add(bullet);
		}
		for (int i = 0; i < RayBulletPullCount; i++) {
			var rayBullet = Instantiate(B.Resource.Prefabs.RayBulletPrefab, transform);
			rayBullet.SetActive(false);
			RayBullets.Add(rayBullet);
		}
		for (int i = 0; i < ExplosionPullCount; i++) {
			var grenade = Instantiate(B.Resource.Prefabs.GrenadePrefab, transform);
			grenade.SetActive(false);
			Grenades.Add(grenade);
		}
	}

	/// <summary> Create bullet logic and visual </summary>
	/// <param name = "startPos"> Bullet start position </param>
	/// <param name = "direction"> Bullet start direction </param>
	/// <param name = "speedBullet"> Bullet start speed </param>
	/// <param name = "powerBullet"> Power of repulsion of an object </param>
	/// <param name = "damageBullet"> Damage caused by the bullet </param>
	/// <param name = "lifeTime"> Bullet lifeTime </param>
	/// <param name = "collidedMask"> Bullet collided mask </param>
	public void CreateBullet (Vector2 startPos, Vector2 direction, float speedBullet, float powerBullet, float damageBullet, float lifeTime, LayerMask collidedMask) {
		Bullet bullet = null;
		for (int i = 0; i < Bullets.Count; i++) {
			if (!Bullets[i].gameObject.activeInHierarchy) {
				bullet = Bullets[i];
			}
		}
		if (bullet == null) {
			bullet = Instantiate(B.Resource.Prefabs.BulletPrefab, transform);
			Bullets.Add(bullet);
		}

		bullet.InitBullet(startPos, direction, speedBullet, powerBullet, damageBullet, lifeTime, collidedMask);
	}

	/// <summary> Create Ray bullet logic and visual </summary>
	/// <param name = "startPos"> Bullet start position </param>
	/// <param name = "direction"> Bullet start direction </param>
	/// <param name = "speedBullet"> Bullet start speed </param>
	/// <param name = "powerBullet"> Power of repulsion of an object </param>
	/// <param name = "damageBullet"> Damage caused by the bullet </param>
	/// <param name = "lifeTime"> Bullet lifeTime </param>
	/// <param name = "collidedMask"> Bullet collided mask </param>
	public void CreateRayBullet (Vector2 startPos, Vector2 direction, float speedBullet, float powerBullet, float damageBullet, float lifeTime, LayerMask collidedMask, Color color) {
		Bullet rayBullet = null;
		for (int i = 0; i < RayBullets.Count; i++) {
			if (!RayBullets[i].gameObject.activeInHierarchy) {
				rayBullet = RayBullets[i];
			}
		}
		if (rayBullet == null) {
			rayBullet = Instantiate(B.Resource.Prefabs.RayBulletPrefab, transform);
			RayBullets.Add(rayBullet);
		}

		rayBullet.InitBullet(startPos, direction, speedBullet, powerBullet, damageBullet, lifeTime, collidedMask, color);
	}

	/// <summary> Create grenade logic and visual </summary>
	/// <param name = "startPos"> Grenade start position </param>
	/// <param name = "direction"> Grenade start direction </param>
	/// <param name = "forceImpuls"> Grenade start impuls </param>
	/// <param name = "powerExplosion"> Power of repulsion of an object </param>
	/// <param name = "damageExplosion"> Damage caused by the grenade </param>
	/// <param name = "lifeTime"> Grenade lifeTime </param>
	/// <param name = "collidedMask"> Grenade collided mask </param>
	public void CreateGrenade (Vector2 startPos, Vector2 direction, float forceImpuls, float powerExplosion, float damageExplosion, float lifeTime, LayerMask collidedMask) {
		Grenade grenade = null;
		for (int i = 0; i < Grenades.Count; i++) {
			if (!Grenades[i].gameObject.activeInHierarchy) {
				grenade = Grenades[i];
			}
		}
		if (grenade == null) {
			grenade = Instantiate(B.Resource.Prefabs.GrenadePrefab, transform);
			Grenades.Add(grenade);
		}

		grenade.InitGrenade(startPos, direction, forceImpuls, powerExplosion, damageExplosion, lifeTime, collidedMask);
	}

	/// <summary> Create explosion logic and visual </summary>
	/// <param name = "position"> Explosion position </param>
	/// <param name = "radius"> Explosion damaged radius </param>
	/// <param name = "powerExplosion"> Power of repulsion of an object </param>
	/// <param name = "damageExplosion"> Damage caused by the explosion </param>
	/// <param name = "collidedMask"> Explosion collided mask </param>
	/// <param name = "withEffect"> Neew show explosion effect </param>
	public void CreateExplosion (Vector2 position, float radius, float powerExplosion, float damageExplosion, LayerMask collidedMask, bool withEffect = true) {
		if (withEffect) {
			Effects.CreateExplosionEffect(position);
		}
		var damageColliders = Physics2D.OverlapCircleAll(position, radius, collidedMask);
		for (int i = 0; i < damageColliders.Length; i++) {
			if (position == (Vector2)damageColliders[i].transform.position) {
				continue;
			}
			var collider = damageColliders[i];
			var damageable = collider.GetComponent<IDamageable>();
			var rb = collider.GetComponent<Rigidbody2D>();
			if (rb != null && !rb.isKinematic) {
				var force = position.GetDirectionTo((Vector2)collider.transform.position) * powerExplosion;
				rb.AddForce(force, ForceMode2D.Impulse);
			}
			if (damageable != null) {
				damageable.SetDamage(damageExplosion);
			}
		}
	}
}
