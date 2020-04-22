using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;

/// <summary> Loot in the world </summary>
public class Loot : MonoBehaviour {

	[SerializeField] WeaponItem weapon;			//Weapon in loot
	[SerializeField] int cartridgesTotal;		//Cartridges total on create loot
	[SerializeField] int cartridgesInMagazine;	//Cartridges In Magazine on create loot

	[SerializeField] AudioClipPreset TakeSound;	//Sound start play on take loot

	public WeaponItem Weapon { get; private set; }			//A Link to a weapon that can change in runtime
	public int CartridgesTotal { get; private set; }		//CartridgesTotal value that can change in runtime
	public int CartridgesInMagazine { get; private set; }	//CartridgesInMagazine value that can change in runtime

	SpriteRenderer LootIcon;								//LootIcon, for visual

	private void Awake () {
		LootIcon = GetComponent<SpriteRenderer>();
		InitLoot(weapon, cartridgesTotal, cartridgesInMagazine);
	}

	/// <summary> InitLoot method </summary>
	/// <param name="weapon"> New weapon </param>
	/// <param name="cartridgesTotal"> New CartridgesTotal count </param>
	/// <param name="cartridgesInMagazine"> New CartridgesInMagazine count </param>
	public void InitLoot (WeaponItem weapon, int cartridgesTotal, int cartridgesInMagazine) {
		if (cartridgesTotal == 0 && cartridgesInMagazine == 0) {
			Destroy(gameObject);
			return;
		}
		Weapon = weapon;
		CartridgesTotal = cartridgesTotal;
		CartridgesInMagazine = cartridgesInMagazine;
		LootIcon.sprite = Weapon.WeaponIcon;
	}

	public void PlayLootSound () {
		if (SoundController.Instance != null) {
			SoundController.PlaySound(TakeSound);
		}
	}
}
