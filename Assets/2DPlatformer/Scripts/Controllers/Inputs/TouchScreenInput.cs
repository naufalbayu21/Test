using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchScreenInput : InputBaseClass {

	public override InputController Controller { get; protected set; }
	public override Vector2 AimPos { get; protected set; }
	public override Vector2 MoveAxis {
		get {
			if (MoveToLeftButton.Pressed) {
				return Vector2.left;
			} else if (MoveToRightButton.Pressed) {
				return Vector2.right;
			}
			return Vector2.zero;
		}
		protected set { }
	}
	public override bool Shot { get { return FireButton.ButtonDown; } protected set { } }
	public override bool ShotPressed { get { return FireButton.Pressed; } protected set { } }
	public override bool Jump { get { return JumpButton.ButtonDown; } protected set { } }
	public override bool Reload { get { return ReloadButton.ButtonDown; } protected set { } }
	public override bool Interaction { get { return InteractionButton.ButtonDown; } protected set { } }
	public override bool NextGun { get { return NextGunButton.ButtonDown; } protected set { } }
	public override bool EnabledControl { get; protected set; }
	List<IDamageable> DamageableObjects { get { return BattleEntity.Instance.DamageableObjects; } }
	Character Character { get { return Controller.SelectedCharacter; } }
	Transform TargetObject;

	[SerializeField] GameObject ButtonsHolder;
	[SerializeField] CustomButton MoveToLeftButton;
	[SerializeField] CustomButton MoveToRightButton;
	[SerializeField] CustomButton FireButton;
	[SerializeField] CustomButton JumpButton;
	[SerializeField] CustomButton ReloadButton;
	[SerializeField] CustomButton InteractionButton;
	[SerializeField] CustomButton NextGunButton;

	[SerializeField] float CustomUpdateTickTime = 0.5f;
	[SerializeField] float MultiplierToCharacterDistance = 0.7f;
	[SerializeField] float AutoAimHeightOfset = 5f;

	public override void SelectInput (InputController controller) {
		base.SelectInput(controller);
		ButtonsHolder.SetActive(true);
		if (CustomUpdateCoroutine != null) {
			StopCoroutine(CustomUpdateCoroutine);
		}
		CustomUpdateCoroutine = StartCoroutine(CustomUpdate());
	}

	public override void DeselectInput () {
		base.DeselectInput();
		ButtonsHolder.SetActive(false);
		if (CustomUpdateCoroutine != null) {
			StopCoroutine(CustomUpdateCoroutine);
			CustomUpdateCoroutine = null;
		}
	}

	public override void UpdateInput () {
		if (BattleEntity.GameInPause) {
			return;
		}
		if (TargetObject != null) {
			var aimPos = TargetObject.transform.position;
			aimPos.y += Mathf.Abs(Character.Position.x - TargetObject.position.x) * Character.SelectedWeapon.Weapon.OffsetAimByY;
			AimPos = aimPos;
		} else {
			Vector2 aimPos = new Vector2();
			aimPos.x = MoveAxis.x;
			if (MoveAxis.x == 0) {
				aimPos.x = Controller.SelectedCharacter.DirectionByX;
			}
			aimPos *= Controller.DistanceToAim;
			aimPos.y += Mathf.Abs(aimPos.x) * Character.SelectedWeapon.Weapon.OffsetAimByY;
			AimPos = Controller.SelectedCharacter.Position + aimPos;
		}
	}

	Coroutine CustomUpdateCoroutine;
	IEnumerator CustomUpdate () {
		yield return null;

		while (true) {
			yield return new WaitForSeconds(CustomUpdateTickTime);
			float minSqrDistance = 400;
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
				if (sqrMagnitude < minSqrDistance && obj.GetRenderer != null && obj.GetRenderer.isVisible) {
					newTarget = obj.GetTransform;
					minSqrDistance = sqrMagnitude;
				}
			}
			TargetObject = newTarget;
		}
	}
}
