using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	[SerializeField] Image StickImage;
	[SerializeField] Image CircleZone;
	[SerializeField] float Step = 200f;

	public Vector2 StickAxis { get; private set; }

	Vector2 PressedPos;

	public void OnPointerDown (PointerEventData eventData) {
		ActivateStick(true, eventData.position);
	}

	public void OnPointerUp (PointerEventData eventData) {
		ActivateStick(false);
		StickAxis = Vector2.zero;
	}

	private void ActivateStick (bool enable, Vector2 pos = new Vector2()) {
		PressedPos = pos;
		StickImage.SetActive(enable);
		CircleZone.SetActive(enable);

		StickImage.transform.position = pos;
		CircleZone.transform.position = pos;
	}

	public void OnDrag (PointerEventData eventData) {
		Vector2 newStickPos;
		newStickPos.x = Mathf.Clamp(eventData.position.x, PressedPos.x - Step, PressedPos.x + Step);
		newStickPos.y = Mathf.Clamp(eventData.position.y, PressedPos.y - Step, PressedPos.y + Step);
		StickImage.transform.position = newStickPos;
		StickAxis = (newStickPos - PressedPos) / Step;
	}
}
