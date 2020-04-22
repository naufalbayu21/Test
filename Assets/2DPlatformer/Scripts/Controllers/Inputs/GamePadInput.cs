using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Management with gamepad, heir "InputBaseClass" </summary>
public class GamePadInput : InputBaseClass {

	public override InputController Controller { get; protected set; }
	public override Vector2 MoveAxis { get; protected set; }
	public override Vector2 AimPos { get; protected set; }
	public override bool Interaction { get { return KeyBinding.GetKeyDown(ActionKey.Interaction); } protected set { } }
	public override bool Jump { get { return KeyBinding.GetKeyDown(ActionKey.Jump); } protected set { } }
	public override bool NextGun { get { return KeyBinding.GetKeyDown(ActionKey.NextGun); } protected set { } }
	public override bool Reload { get { return KeyBinding.GetKeyDown(ActionKey.Reload); } protected set { } }
	public override bool Shot { get { return KeyBinding.GetKeyDown(ActionKey.Shot); } protected set { } }
	public override bool ShotPressed { get { return KeyBinding.GetKey(ActionKey.Shot); } protected set { } }
	public override bool EnabledControl { get; protected set; }
	List<IDamageable> DamageableObjects { get { return BattleEntity.Instance.DamageableObjects; } }
	Character Character { get { return Controller.SelectedCharacter; } }
	Transform TargetObject;

	[SerializeField] float MinAimAxisSqrMagnitude = 0.25f;
	[SerializeField] float CustomUpdateTickTime = 0.5f;
	[SerializeField] float MultiplierToCharacterDistance = 0.7f;
	[SerializeField] float AutoAimHeightOfset = 5f;

	public override void UpdateInput () {
		if (BattleEntity.GameInPause) {
			return;
		}
		Vector2 moveAxis = new Vector2();
		moveAxis.x = Input.GetAxis("Horizontal");
		moveAxis.y = Input.GetAxis("Vertical");
		MoveAxis = moveAxis;

		Vector2 aimPos = new Vector2();
		aimPos.x = Input.GetAxis("SecondHorizontal");
		aimPos.y = Input.GetAxis("SecondVertical");
		if (aimPos == Vector2.zero) {
			aimPos = Vector2.zero;
			if (TargetObject != null) {
				aimPos = TargetObject.transform.position;
				aimPos.y += Mathf.Abs(Character.Position.x - TargetObject.position.x) * Character.SelectedWeapon.Weapon.OffsetAimByY;
				AimPos = aimPos;
				return;
			}
			if (moveAxis == Vector2.zero) {
				aimPos.x = Controller.SelectedCharacter.DirectionByX;
			} else {
				aimPos.x = moveAxis.x > 0? 1: -1;
			}
		}
		aimPos *= Controller.DistanceToAim;
		aimPos.y += Mathf.Abs(aimPos.x) * Character.SelectedWeapon.Weapon.OffsetAimByY;
		AimPos = Controller.SelectedCharacter.Position + aimPos;
	}

	public override void SelectInput (InputController controller) {
		base.SelectInput(controller);

		if (CustomUpdateCoroutine != null) {
			StopCoroutine(CustomUpdateCoroutine);
		}
		CustomUpdateCoroutine = StartCoroutine(CustomUpdate());
	}

	public override void DeselectInput () {
		base.DeselectInput();

		if (CustomUpdateCoroutine != null) {
			StopCoroutine(CustomUpdateCoroutine);
			CustomUpdateCoroutine = null;
		}
	}

	Coroutine CustomUpdateCoroutine;
	IEnumerator CustomUpdate () {
		yield return null;

		while (true) {
			yield return new WaitForSeconds(CustomUpdateTickTime);
			float minDistance = 100;
			Transform newTarget = null;
			if (DamageableObjects.Contains(Character)) {
				DamageableObjects.Remove(Character);
			}
			for (int i = 0; i < DamageableObjects.Count; i++) {
				var obj = DamageableObjects[i];
				if (Mathf.Abs(Character.Position.y - obj.Position.y) > AutoAimHeightOfset) {
					continue;
				}
				var sqrMagnitude = (Character.Position - obj.Position).sqrMagnitude;
				if (obj is Character) {
					sqrMagnitude *= MultiplierToCharacterDistance;
				}
				if (sqrMagnitude < minDistance && obj.GetRenderer != null && obj.GetRenderer.isVisible) {
					newTarget = obj.GetTransform;
					minDistance = sqrMagnitude;
				}
			}
			TargetObject = newTarget;
		}
	}

}
