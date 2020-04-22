using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Displaying the status of the character </summary>
public class GamePanel : Singleton<GamePanel> {

	[Header("Weapon")]
	[SerializeField] Image WeaponIcon;
	[SerializeField] Text CartridgesInMagazine;
	[SerializeField] Text CartridgesTotal;
	[SerializeField] string InfinityChar;		//If cartridges is infinity

	[Header("HealthBar")]
	[SerializeField] float MaxHealthBarValue;	//Max health bar value in pixels
	[SerializeField] Image HealthBarImage;

	public void SetWeaponIcon (Sprite weaponIcon) {
		WeaponIcon.sprite = weaponIcon;
	}

	public void SetCartridgesInMagazine (int count) {
		if (count == int.MaxValue) {
			CartridgesInMagazine.text = InfinityChar;
		} else {
			CartridgesInMagazine.text = count.ToString();
		}
	}

	public void SetTotalCartridges (int count) {
		if (count == int.MaxValue) {
			CartridgesTotal.text = InfinityChar;
		} else {
			CartridgesTotal.text = count.ToString();
		}
	}

	public void UpdateHealth (float healthPercent) {
		HealthBarImage.SetSizeX(MaxHealthBarValue * healthPercent);
	}

	protected override void AwakeSingleton () {
		HealthBarImage.SetSizeX(MaxHealthBarValue);
	}
}
