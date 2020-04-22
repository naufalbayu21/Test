using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance {

	/// <summary> Properties defining the weapon settings </summary>
	[CreateAssetMenu(fileName = "WeaponItem", menuName = "Game Balance/WeaponItem")]
	public class WeaponItem : ScriptableObject {

		[SerializeField] RuntimeAnimatorController animator;											//Animator controller сontaining animations of hands and weapon
		[SerializeField] Sprite weaponIcon;																//Weapon icon for game panel ui

		[Header("Sounds")]
		[SerializeField] AudioClipPreset shotSound;														//Play sound with each shot
		[SerializeField] AudioClipPreset reloadSound;													//Reload sound

		[Header("Main parameters")]
		[SerializeField] float shotTime;																//Minimum interval between shots
		[SerializeField] ShotTypes shotType;															//Charging type
		[SerializeField] WeaponTypes weaponType;														//Weapon type: MeleeWeapon,	SemiAutomaticWeapon, AutomaticWeapon

		[Header("Firearms")]
		[SerializeField, ShowInInspectorCondition("IsFastRayShotType")] float minDistance;					//Min distance, use if shotType == Ray 	
		[SerializeField, ShowInInspectorCondition("IsFastRayShotType")] float shotDistance;					//Min distance, use if shotType == Ray 

		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float reloadTime;			//Reload time, use if weaponType != MeleeWeapon

		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] int cartridgesInMagazine;	//Cartridges in magazine, use if weaponType != MeleeWeapon
		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] int maxCartridges;			//Cartridges in magazine, use if weaponType != MeleeWeapon

		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float powerBullet;			//Power of repulsion of an object, use if weaponType != MeleeWeapon
		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float damageBullet;			//Damage caused by the bullet, use if weaponType != MeleeWeapon
		[SerializeField, ShowInInspectorCondition("IsDynamyc")] float speedBullet;						//Cartridge start speed, use if shotType == (Bullet || Grenade)
		[SerializeField, ShowInInspectorCondition("IsDynamyc")] float lifeTimeBullet;					//Cartridge lifeTime, use if shotType == (Bullet || Grenade)
		[SerializeField, ShowInInspectorCondition("IsDynamyc")] float offsetAimByY;						//Offset aim by Y for autoaim

		[SerializeField, ShowInInspectorCondition("IsBulletRayShotType")] Color rayColor;				//Color for ray

		[Header("Scatter")]																				//The scatter is considered in both directions, measured in degrees
		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float minScatter;			//Min angle scatter
		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float maxScatter;			//Max angle scatter
		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float incScatterInOneShot;	//The increase in scatter after the shot
		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float moveToMinScatterSpeed;	//Speed of scatter of a spread to a minimum in a second
		[SerializeField, ShowInInspectorCondition("IsMeleeWeapon", false)] float scatterInAirOffset;	//Increase the scatter if the character is in the air

		//Public properties
		public RuntimeAnimatorController AnimatorController { get { return animator; } }
		public Sprite WeaponIcon { get { return weaponIcon; } }
		public AudioClipPreset ShotSound { get { return shotSound; } }
		public AudioClipPreset ReloadSound { get { return reloadSound; } }
		public float ShotTime { get { return shotTime; } }
		public float MinDistance { get { return minDistance; } }
		public float ShotDistance { get { return shotDistance; } }
		public float ReloadTime { get { return reloadTime; } }
		public int MaxCartridgesInMagazine { get { return cartridgesInMagazine; } }
		public int MaxCartridges { get { return maxCartridges; } }
		public float MinScatter { get { return minScatter; } }
		public float MaxScatter { get { return maxScatter; } }
		public float IncScatterInOneShot { get { return incScatterInOneShot; } }
		public float MoveToMinScatterSpeed { get { return moveToMinScatterSpeed; } }
		public float ScatterInAirOffset { get { return scatterInAirOffset; } }
		public float PowerBullet { get { return powerBullet; } }
		public float DamageBullet { get { return damageBullet; } }
		public float SpeedBullet { get { return speedBullet; } }
		public float LifeTimeBullet { get { return lifeTimeBullet; } }
		public Color RayColor { get { return rayColor; } }
		public float OffsetAimByY { get { return offsetAimByY; } }	
		public WeaponTypes WeaponType { get { return weaponType; } }
		public ShotTypes ShotType { get { return shotType; } }

		//Public properties, Used for custom attribute "ShowInInspectorCondition"
		public bool IsFastRayShotType { get { return ShotType == ShotTypes.FastRay; } }
		public bool IsMeleeWeapon { get { return WeaponType == WeaponTypes.MeleeWeapon; } }
		public bool IsBulletType { get { return !IsMeleeWeapon && ShotType == ShotTypes.Bullet; } }
		public bool IsBulletRayShotType { get { return ShotType == ShotTypes.BulletRay; } }
		public bool IsGrenadeType { get { return !IsMeleeWeapon && ShotType == ShotTypes.Grenade; } }
		public bool IsDynamyc { get { return IsBulletType || IsBulletRayShotType || IsGrenadeType; } }
	}

	public enum ShotTypes {
		Bullet,
		BulletRay,
		Grenade,
		FastRay,
	}

	public enum WeaponTypes {
		MeleeWeapon,			//TODO Add Melee Weapons
		SemiAutomaticWeapon,
		AutomaticWeapon
	}
}
