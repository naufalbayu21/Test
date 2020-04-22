using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameBalance {
	/// <summary> Properties defining the character settings </summary>
	[CreateAssetMenu(fileName = "CharacterDescription", menuName = "Game Balance/CharacterDescription")]
	public class CharacterDescription : ScriptableObject {

		[SerializeField] RuntimeAnimatorController animator;		//Animator controller сontaining animations of character
		[SerializeField] Character characterPrefab;					//Character prefab
		[SerializeField] Sprite headSprite;							//Head sprite for visual customization
		[SerializeField] WeaponItem startedWeapon;					//Weapon given to the character at the start

		[SerializeField] float maxHealth = 100;						//Max health and health on start
		[SerializeField] float velocityModifySpeed = 10f;			//Velocity modify speed per second
		[SerializeField] float maxSpeed = 5f;						//Max speed by x axis
		[SerializeField] float jumpForce = 5f;						//The strength of the impulse jump
		[SerializeField] int weaponsCount;							//Count of weapon slots

		[SerializeField] float headDirectionMultiplier = 0.5f;

		[Header("Hands")]								//Hands sprites for visual customization
		[SerializeField] Sprite firstShoulder;
		[SerializeField] Sprite firstForearm;
		[SerializeField] Sprite firstForearmOutline;

		[SerializeField] Sprite secondShoulder;
		[SerializeField] Sprite secondForearm;
		[SerializeField] Sprite secondForearmOutline;

		//Public properties
		public RuntimeAnimatorController Animator { get { return animator; } }
		public Character CharacterPrefab { get { return characterPrefab; } }
		public Sprite HeadSprite { get { return headSprite; } }
		public WeaponItem StartedWeapon { get { return startedWeapon; } }
		public float MaxHealth { get { return maxHealth; } }
		public float VelocityModifySpeed { get { return velocityModifySpeed; } }
		public float MaxSpeed { get { return maxSpeed; } }
		public float JumpForce { get { return jumpForce; } }
		public float HeadDirectionMultiplier { get { return headDirectionMultiplier; } }
		public int WeaponsCount { get { return weaponsCount; } }

		public Sprite FirstShoulder { get { return firstShoulder; } }
		public Sprite FirstForearm { get { return firstForearm; } }
		public Sprite FirstForearmOutline { get { return firstForearmOutline; } }
		public Sprite SecondShoulder { get { return secondShoulder; } }
		public Sprite SecondForearm { get { return secondForearm; } }
		public Sprite SecondForearmOutline { get { return secondForearmOutline; } }
	}
}
