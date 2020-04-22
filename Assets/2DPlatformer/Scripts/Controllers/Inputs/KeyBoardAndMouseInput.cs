using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Management with the mouse and keyboard, heir "InputBaseClass" </summary>
public class KeyBoardAndMouseInput : InputBaseClass {
	public override InputController Controller { get; protected set; }
	public override Vector2 MoveAxis { get ; protected set; }
	public override bool Interaction { get { return KeyBinding.GetKeyDown(ActionKey.Interaction); }  protected set { } }
	public override bool Jump { get { return KeyBinding.GetKeyDown(ActionKey.Jump); } protected set { } }
	public override bool NextGun { get { return KeyBinding.GetKeyDown(ActionKey.NextGun); }  protected set { } }
	public override bool Reload { get { return KeyBinding.GetKeyDown(ActionKey.Reload); } protected set { } }
	public override bool Shot { get { return KeyBinding.GetKeyDown(ActionKey.Shot); } protected set { } }
	public override bool ShotPressed { get { return KeyBinding.GetKey(ActionKey.Shot); } protected set { } }
	public override Vector2 AimPos { get; protected set; }
	public override bool EnabledControl { get; protected set; }

	//At used debug
	private bool MouseInputEnabled {
		get {
	#if UNITY_EDITOR || DEBUG
			return !Input.GetKey(KeyCode.LeftAlt);
	#else
			return true;
	#endif
		}
	}

	public override void UpdateInput () {
		if (BattleEntity.GameInPause) {
			Cursor.visible = true;
			return;
		}
		if (MouseInputEnabled && BattleEntity.Instance != null &&!BattleEntity.Instance.IsGameOver) {
			Cursor.visible = false;
			AimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		} else {
			Cursor.visible = true;
		}
		Vector2 moveAxis = Vector2.zero;
		if (KeyBinding.GetKey(ActionKey.MoveToLeft)) {
			moveAxis.x = -1;
		} else if (KeyBinding.GetKey(ActionKey.MoveToRight)) {
			moveAxis.x = 1;
		} else {
			moveAxis.x = 0;
		}
		MoveAxis = moveAxis;
	}

	public override void SelectInput (InputController controller) {
		base.SelectInput(controller);
	}
}
