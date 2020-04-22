using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary> Use only for parallax background. Has no external links. </summary>
public class BackGroundController : MonoBehaviour {

	//Array of the backgrounds.
	[SerializeField] BackgroundLayer[] Backgrounds; 

	Vector3 PrevCameraPos;

	CameraController Camera { get { return CameraController.Instance; } }
	Vector3 CameraPos { get { return Camera.transform.position; } }

	private void Start () {
		transform.SetParent(Camera.transform);
		PrevCameraPos = CameraPos;
		for (int i = 0; i < Backgrounds.Length; i++) {
			Backgrounds[i].BorderByHorizontal = Backgrounds[i].FirstSprite.size.x;
			Backgrounds[i].SecondSprite = GameObject.Instantiate(Backgrounds[i].FirstSprite, Backgrounds[i].FirstSprite.transform.parent);
			Backgrounds[i].SecondSprite.transform.localPosition = Backgrounds[i].FirstSprite.transform.localPosition;
			Backgrounds[i].SecondSprite.transform.SetLocalX(Backgrounds[i].BorderByHorizontal);
		}
	}

	private void Update () {
		if (PrevCameraPos == CameraPos) return;

		var deltaCameraPos =  PrevCameraPos - CameraPos;
		PrevCameraPos = CameraPos;

		for (int i = 0; i < Backgrounds.Length; i++) {
			var deltaPos = deltaCameraPos;
			deltaPos *= Backgrounds[i].MultiplierScrollSpeed;
			Backgrounds[i].FirstSprite.transform.localPosition += deltaPos;
			Backgrounds[i].SecondSprite.transform.localPosition += deltaPos;

			CheckBorder(Backgrounds[i].FirstSprite.transform, Backgrounds[i].BorderByHorizontal);
			CheckBorder(Backgrounds[i].SecondSprite.transform, Backgrounds[i].BorderByHorizontal);
		}
	}

	void CheckBorder (Transform transform, float borderByHorizontal) {
		if (transform.localPosition.x > borderByHorizontal) {
			transform.SetLocalX(transform.localPosition.x - (borderByHorizontal * 2));
		} else if (transform.localPosition.x < -borderByHorizontal) {
			transform.SetLocalX(transform.localPosition.x + (borderByHorizontal * 2));
		}
	}

	[Serializable]
	class BackgroundLayer {
		public string CaptionLayer;				//Caption in array.
		public SpriteRenderer FirstSprite;	//SpriteRenderer with sprite of background.
		public float MultiplierScrollSpeed;		//Speed, the further, the less.

		public SpriteRenderer SecondSprite { get; set; }	//Copy main Sprite.
		public float BorderByHorizontal	{ get; set; }	//Border to the seam of the sprite.
	}
}
