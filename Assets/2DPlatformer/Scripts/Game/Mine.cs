using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Mine in world object</summary>
public class Mine : MonoBehaviour, IDamageable, ISwitch {

	[SerializeField] LayerMask DitonateMask;		//Objects that can cause dithonation
	[SerializeField] LayerMask DamageMask;			//Objects that will receive damage
	[SerializeField] float DellayDitonate = 0.5f;	//Explosion delay after detonation
	[SerializeField] float StartHealth = 1;			//Health current object
	[SerializeField] float Radius = 1.5f;			//Explosion radius
	[SerializeField] float PowerExplosion = 5f;		//Force pushing all objects within a radius
	[SerializeField] float DamageExplosion = 50f;	//Damage to all objects within a radius
	[SerializeField] Renderer MainRenderer;			//For autoaim

	public Vector2 Position { get { return transform.position; } }
	public Transform GetTransform { get { return transform; } }
	public Renderer GetRenderer { get { return MainRenderer; } }
	public float Health { get; set; }
	bool IsDitonate { get { return Health <= 0; } }
	private void Awake () {
		Health = StartHealth;
	}

	private void Start () {
		AddInDamageableObjects();
	}

	public void AddInDamageableObjects () {
		if (BattleEntity.Instance != null) {
			BattleEntity.Instance.DamageableObjects.Add(this);
		}
	}

	/// <summary> Getting damage method </summary>
	/// <param name="damage"> Damage value </param>
	public void SetDamage (float damage) {
		if (IsDitonate) return;

		Health -= damage;
		if (Health <= 0) {
			StartCoroutine(StartExplosion());
		}
	}

	/// <summary> StartExplosion coroutine </summary>
	IEnumerator StartExplosion () {
		if (BattleEntity.Instance != null) {
			BattleEntity.Instance.DamageableObjects.Remove(this);
		}
		yield return new WaitForSeconds(DellayDitonate);
		WorldLogic.Instance.CreateExplosion(transform.position, Radius, PowerExplosion, DamageExplosion, DamageMask);
		Destroy(gameObject);
	}

	public void OnSwitch (GameObject go, bool value = true) {
		if (!IsDitonate && DitonateMask.LayerInMask(go.layer)) {
			StartCoroutine(StartExplosion());
		}
	}
}
