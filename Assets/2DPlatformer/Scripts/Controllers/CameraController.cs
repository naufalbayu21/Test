using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController> {

	[SerializeField] float CameraSpeed = 1;

	//The boundaries beyond which the camera will not go
	[SerializeField] float MostLeftX;
	[SerializeField] float MostRightX;
	[SerializeField] float MostDownY;
	[SerializeField] float MostUpY;

	//TargetPoint followed by a camera
	Transform TargetPoint;

	float LeftX { get; set; }
	float RightX { get; set; }
	float DownY { get; set; }
	float UpY { get; set; }

	protected override void AwakeSingleton () {
		UpdateCameraBorders(MostLeftX, MostRightX, MostDownY, MostUpY);
	}

	public void UpdateCameraBorders (float MostLeftX, float MostRightX, float MostDownY, float MostUpY) {
		LeftX = MostLeftX;
		RightX = MostRightX;
		DownY = MostDownY;
		UpY = MostUpY;
	}

	//FixedUpdate use to synchronize the movement of the character and camera.
	private void LateUpdate () {
		if (TargetPoint != null) {
			var newPos = Vector3.Lerp(transform.position, TargetPoint.position, Time.fixedDeltaTime * CameraSpeed);
			newPos.x = Mathf.Clamp(newPos.x, LeftX, RightX);
			newPos.y = Mathf.Clamp(newPos.y, DownY, UpY);
			transform.position = newPos;
		}
	}

	public void SetTargetPoint (Transform target) {
		TargetPoint = target;
	}
}
