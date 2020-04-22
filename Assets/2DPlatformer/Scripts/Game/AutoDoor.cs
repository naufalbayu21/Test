using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour, ISwitch {

	[SerializeField] Renderer DoorRenderer;
	[SerializeField] AudioClipPreset OpenDoorSound;
	[SerializeField] AudioClipPreset CloseDoorSound;
	[SerializeField] BoxCollider2D OnOffCollider;
	[SerializeField] LayerMask LayersTriggered;


	Animator DoorAnimator;
	
	ZeroCounter IsOpen;

	void Awake () {
		DoorAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnSwitch (GameObject go, bool value = true) {
		if (LayersTriggered.LayerInMask(go.layer)) {
			bool needPlayAnimation = false;

			if (value) {
				needPlayAnimation = !IsOpen;
				IsOpen++;
			} else {
				IsOpen--;
				needPlayAnimation = !IsOpen;
			}

			if (needPlayAnimation) {
				DoorAnimator.SetBool(C.OpenDoorAnimation, value);
				OnOffCollider.enabled = !IsOpen;
				if (DoorRenderer.isVisible) {
					if (IsOpen) {
						SoundController.StopSound(OpenDoorSound);
						SoundController.PlaySound(CloseDoorSound);
					} else {
						SoundController.StopSound(OpenDoorSound);
						SoundController.PlaySound(CloseDoorSound);
					}
				}
			}

		}
	}
}
