using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For damaged props
/// </summary>
public class DamagedProps : MonoBehaviour, IDamageable {

	[SerializeField] bool destroyIfHealthEmpty;
	[SerializeField] float HealthOnSatrt;
	[SerializeField, ShowInInspectorCondition("DestroyIfHealthEmpty")] float DestroyDellay;
	[SerializeField, ShowInInspectorCondition("DestroyIfHealthEmpty", showIfTrue:false)] bool destroyOnlyBoxCollider;
	[SerializeField, ShowInInspectorCondition("DestroyOnlyBoxCollider")] BoxCollider2D[] DestroyedBoxColliders;

	[SerializeField] DamageViewType DamageView;

	[SerializeField, ShowInInspectorCondition("DamageViewIsAnimation")] Animator AnimatorController;

	[SerializeField, ShowInInspectorCondition("DamageViewIsSprite")] SpriteRenderer SpriteRenderer;

	[SerializeField] AudioClipPreset DestroySound;
	[SerializeField] bool PlayIfIsVisible = true;

	//!!! The count of array (HealthSteps and Sprites) elements must be equal to !!!
	[SerializeField, ShowInInspectorCondition("DamageViewIsSprite")] float[] HealthSteps;
	[SerializeField, ShowInInspectorCondition("DamageViewIsSprite")] Sprite[] Sprites;

	public bool DestroyIfHealthEmpty { get { return destroyIfHealthEmpty; } }
	public bool DestroyOnlyBoxCollider { get { return destroyOnlyBoxCollider; } }
	public bool DamageViewIsAnimation { get { return DamageView == DamageViewType.Animation; } }
	public bool DamageViewIsSprite { get { return DamageView == DamageViewType.Sprite; } }

	public Renderer GetRenderer { get; private set; }
	public Transform GetTransform {	get; private set; }
	public float Health { get; set; }
	public Vector2 Position { get { return GetTransform.position; } }

	void Awake() {
		GetTransform = GetComponent<Transform>();
		GetRenderer = GetComponent<Renderer>();
		Health = HealthOnSatrt;

		if (DamageViewIsSprite && HealthSteps.Length != Sprites.Length) {
			Debug.LogError("DamageView select DamageViewType.Animation and DamageViewType.Sprite HealthSteps.Length is not equal Sprites.Length");
			DestroyObject(gameObject);
		}

		if (DamageViewIsAnimation && AnimatorController == null) {
			Debug.LogError("DamageView select DamageViewType.Animation and Animator is null");
			DestroyObject(gameObject);
		}
	}

	private void Start () {
		AddInDamageableObjects();
	}

	public void AddInDamageableObjects () {
		if (BattleEntity.Instance != null) {
			BattleEntity.Instance.DamageableObjects.Add(this);
		}
	}

	public void SetDamage (float damage) {
		if (Health <= 0) return;

		Health -= damage;

		switch (DamageView) {
			case DamageViewType.Animation: break;				//TODO Add animation logic
			case DamageViewType.Sprite: {
				for (int i = HealthSteps.Length - 1; i >= 0 ; i--) {
					if (HealthSteps[i] >= Health) {
						SpriteRenderer.sprite = Sprites[i];
						break;
					}
				}
				break;
			}
			default: break;
		}

		if (Health <= 0) {
			if (!PlayIfIsVisible || GetRenderer.isVisible) {
				SoundController.PlaySound(DestroySound);
			}
			if (BattleEntity.Instance != null && BattleEntity.Instance.DamageableObjects.Contains(this)) {
				BattleEntity.Instance.DamageableObjects.Remove(this);
			}
			if (DestroyIfHealthEmpty) {
				DestroyObject(gameObject);
			} else if (DestroyOnlyBoxCollider) {
				foreach (var collider in DestroyedBoxColliders) {
					DestroyObject(collider);
				}
			}
		}
	}

	new void DestroyObject (UnityEngine.Object obj) {
		Destroy(obj, DestroyDellay);
	}

	enum DamageViewType {
		None,
		Animation,
		Sprite
	}
}
