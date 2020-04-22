using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Aim visualisation</summary>
public class Aim : MonoBehaviour {

	[SerializeField] SpriteRenderer HorizontalAim;
	[SerializeField] SpriteRenderer VerticalAim;
	[SerializeField] float MinScatter = 0.5f;
	[SerializeField] float ScatterMultiplier = 0.1f;

	/// <summary> Set scatter size </summary>
	/// <param name="Scatter"> New scatter size </param>
	public void SetScatter (float Scatter) {
		var newScatter = Scatter * ScatterMultiplier;
		newScatter += MinScatter;
		HorizontalAim.SetSizeX(newScatter);
		VerticalAim.SetSizeX(newScatter);
	}
 }
