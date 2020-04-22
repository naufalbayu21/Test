using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameBalance;

/// <summary> Controller for input management </summary>
public class InputController : Singleton<InputController> {

	[SerializeField] KeyBoardAndMouseInput KeyBoardAndMouseInput;
	[SerializeField] GamePadInput GamePadInput;
	[SerializeField] TouchScreenInput TouchScreenInput;

	[SerializeField] KeyCode KeySelectKeyBoard = KeyCode.Space;
	[SerializeField] KeyCode KeySelectGamePad = KeyCode.JoystickButton0;

	[SerializeField] Aim aim;
	[SerializeField] float distanceToAim = 3;

	InputType InputType;

	public Aim Aim { get { return aim; } }
	public float DistanceToAim { get { return distanceToAim; } }

	public InputBaseClass CurrentController { get; private set; }

	public Character SelectedCharacter { get; private set; }

	protected override void AwakeSingleton () {
		SetController(PlayerProfile.InputType);
	}

	/// <summary> Selected input conroller on start game or in runtime </summary>
	/// /// <param name="type"> New input type </param>
	public void SetController (InputType type) {
		if (CurrentController != null) {
			CurrentController.DeselectInput();
		}
		switch (type) {
			case InputType.KeyboardAndMouse: CurrentController = KeyBoardAndMouseInput; break;
			case InputType.GamePad: CurrentController = GamePadInput; break;
			case InputType.TouchScreen: CurrentController = TouchScreenInput; break;
		}
		
		InputType = type;
		PlayerProfile.InputType = InputType;

		CurrentController.SelectInput(this);

		if (SelectedCharacter != null) {
			SelectedCharacter.SetInput(CurrentController);
		}
	}

	/// <summary> Select controlled character </summary>
	/// <param name="character"> New selected character </param>
	public void SelectCharacter (Character character) {
		SelectedCharacter = character;
		SelectedCharacter.SetInput(CurrentController);
	}

	private void Update () {
		CurrentController.UpdateInput();
		Aim.transform.position = new Vector3(CurrentController.AimPos.x, CurrentController.AimPos.y, Aim.transform.position.z);

		if (InputType != InputType.TouchScreen && Input.touchCount > 0) {
			SetController(InputType.TouchScreen);
		}

		if (InputType != InputType.GamePad && Input.GetKeyDown(KeySelectGamePad)) {
			SetController(InputType.GamePad);
		}

		if (InputType != InputType.KeyboardAndMouse && Input.GetKeyDown(KeySelectKeyBoard)) {
			SetController(InputType.KeyboardAndMouse);
		}
	}

}

public enum InputType {
	KeyboardAndMouse = 0,
	GamePad = 1,
	TouchScreen = 2
}
