using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;
using System;

/// <summary> Main script to create and controll character entity </summary>
public class Character : MonoBehaviour, IDamageable {

	[SerializeField] Animator HandsAnimator;				//Hands animator for management hands and guns animation
	[SerializeField] SpriteRenderer HeadTransform;			//To control the position and rotation of the head
	[SerializeField] Transform HandsTransform;				//To control the position and rotation of the hands
	[SerializeField] Transform ShotStartTransform;			//To determine the start of the shot
	[SerializeField] float HandsOnDeathRotation;			//Hack, for the position of the hands at death, you can do in the animator.
	[SerializeField] float HeadOnDeathRotation;				//Hack
	[SerializeField] float DeathTime;						//Hack
	[SerializeField] float DestroyCharacterAfterDeathTime;
	[SerializeField] Transform CameraTargetPoint;			//The point behind which the camera will follow
	[SerializeField] Renderer MainRenderer;					//For autoaim and AILogic

	//Parts of the hand, for modification possibilities
	[Header("FirstHand")]
	[SerializeField] SpriteRenderer FirstShoulder;
	[SerializeField] SpriteRenderer FirstForearm;
	[SerializeField] SpriteRenderer FirstForearmOutline;

	//Parts of the hand, for modification possibilities
	[Header("SecondHand")]
	[SerializeField] SpriteRenderer SecondShoulder;
	[SerializeField] SpriteRenderer SecondForearm;
	[SerializeField] SpriteRenderer SecondForearmOutline;

	//Sounds
	[Space(10),Header("Sounds")]
	[SerializeField] AudioClipPreset JumpSound;
	[SerializeField] AudioClipPreset StepSound;

	private int GroundCollided;										//Touch counter to ground
	private int CurrentWeaponIndex;									//Index weapon in hands
	private List<WeaponEntity> Weapons = new List<WeaponEntity>();	//Weapons list
	private Loot LootInInteractionZone;								//Not null if character is near loot

	private WeaponEntity selectedWeapon;							//Weapon in hands

	private HashSet<Collider2D> TriggeredObjects = new HashSet<Collider2D>();		//To control the re-trigger
	private Collider2D LastColision;								//For stop move animation on dynamic props

	#region Public properties

	public CharacterDescription Description { get; private set; }
	public bool IsUserControlled { get { return  Input == InputController.Instance.CurrentController; } }
	public bool IsBot { get { return  Input is AIController; } }
	public GamePanel GamePanel { get { return GamePanel.Instance; } }
	public Rigidbody2D RB { get; private set; }
	public Animator Animator { get; private set; }
	public Vector2 Position { get { return transform.position; } }
	public Transform GetTransform { get { return transform; } }
	public Renderer GetRenderer { get { return MainRenderer; } }
	public int DirectionByX { get; private set; }
	public Animator GetHandsAnimator { get { return HandsAnimator; } }
	public WeaponEntity SelectedWeapon {
		get {
			return selectedWeapon;
		}
		private set {
			if (selectedWeapon != null) {
				selectedWeapon.DeselectWeapon();
			}
			selectedWeapon = value;
			selectedWeapon.SelectWeapon(this);
		}
	}
	public LayerMask EnemyMask {
		get {
			if (B.Layers.AlliesMask.LayerInMask(gameObject.layer)) {
				return B.Layers.EnemyMask;
			} else {
				return B.Layers.AlliesMask;
			}
		}
	}
	public bool InAir { get { return GroundCollided == 0; } }
	public Transform GetShotStartTransform { get { return ShotStartTransform; } }
	public float Health { get; private set; }
	public bool IsDead { get; private set; }

	InputBaseClass Input { get; set; }

	#endregion //Public properties

	#region Private properties

	private LayerMask GroundMask { get { return B.Layers.GroundMask; } }
	private LayerMask LootMask { get { return B.Layers.LootMask; } }
	private Vector2 DirectionToAim { get { return Input.AimPos - (Vector2)transform.position; } }

	#endregion //Private properties

	#region Instantiation

	/// <summary> Create and initialisation character </summary>
	/// <param name="spawnPoint"> Spawn character position </param>
	/// <param name="description"> Character description</param>
	public static Character CreateCharacter (Vector3 spawnPoint, CharacterDescription description) {
		var newCharacter = Instantiate(description.CharacterPrefab);
		newCharacter.transform.position = spawnPoint;
		newCharacter.InitCharacter(description);
		return newCharacter;
	}

	/// <summary> Initialisation character after start </summary>
	/// <param name="description"> Character description</param>
	private void InitCharacter (CharacterDescription description) {
		Description = description;
		Health = Description.MaxHealth;
		RB = GetComponent<Rigidbody2D>();
		Animator = GetComponent<Animator>();

		SelectedWeapon = new WeaponEntity(this, Description.StartedWeapon);
		SelectedWeapon.CartridgesTotal = SelectedWeapon.Weapon.MaxCartridges;
		Weapons.Add(SelectedWeapon);

		if (RB == null) { Debug.LogError("RigidBody2D not found", this); }
		if (Animator == null) { Debug.LogError("Animator not found", this); }

		Animator.runtimeAnimatorController = Description.Animator;

		//Set hands
		if (FirstShoulder) FirstShoulder.sprite = Description.FirstShoulder;
		if (FirstForearm) FirstForearm.sprite = Description.FirstForearm;
		if (FirstForearmOutline) FirstForearmOutline.sprite = Description.FirstForearmOutline;
		if (SecondShoulder) SecondShoulder.sprite = Description.SecondShoulder;
		if (SecondForearm) SecondForearm.sprite = Description.SecondForearm;
		if (SecondForearmOutline) SecondForearmOutline.sprite = Description.SecondForearmOutline;

		//Set head
		if (HeadTransform) HeadTransform.sprite = Description.HeadSprite;
	}

	/// <summary> Input addition during initialization or modification in runtime </summary>
	/// <param name="input"> New input </param>
	public void SetInput (InputBaseClass input) {
		Input = input;
		if (IsUserControlled) {
			CameraController.Instance.SetTargetPoint(CameraTargetPoint);
			UpdateGamePanel();
		}
	}

	public void AddInDamageableObjects () {
		if (BattleEntity.Instance != null) {
			BattleEntity.Instance.DamageableObjects.Add(this);
		}
	}

	#endregion //Instantiation

	#region Stats

	/// <summary> Added health </summary>
	/// <param name="health"> Added health value </param>
	public void AddHealth (float health) {
		Health = Mathf.Clamp(Health + health, 0, Description.MaxHealth);
		if (IsUserControlled) {
			var healthPercent = Health / Description.MaxHealth;
			GamePanel.UpdateHealth(healthPercent);
		}
	}

	public void SetDamage (float damage) {
		if (IsDead) return;
		Health -= damage;
		if (Health <= 0) {
			Death ();
		}

		if (IsUserControlled) {
			var healthPercent = Health / Description.MaxHealth;
			GamePanel.UpdateHealth(healthPercent);
		}
		var aiController = Input as AIController;
		if (aiController != null) {
			aiController.SetAlarmed();
		}
	}

	/// <summary> Death, disabled character controll and destroy gameObject with dellay</summary>
	void Death () {
		IsDead = true;
		if (IsUserControlled) {
			InGameMenu.Instance.ShowGameOver();
		}
		Animator.SetBool(C.IsDead, true);
		StartCoroutine(DeathCoroutine ());
		Destroy(gameObject, DestroyCharacterAfterDeathTime);
		if (BattleEntity.Instance != null) {
			BattleEntity.Instance.DamageableObjects.Remove(this);
		}
	}

	/// <summary> Update GamePanel to display the status of a character </summary>
	public void UpdateGamePanel () {
		if (IsUserControlled) {
			GamePanel.SetWeaponIcon(SelectedWeapon.Weapon.WeaponIcon);
			GamePanel.SetCartridgesInMagazine(SelectedWeapon.CartridgesInMagazine);
			GamePanel.SetTotalCartridges(SelectedWeapon.CartridgesTotal);
		} 
	}

	#endregion //Stats

	#region ControlAndAnimatorLogic

	/// <summary> Update move character, need invoke in FixedUpdate </summary>
	private void UpdateMoveCharacter () {
		//Calculation direction to aim
		if (DirectionToAim.x < 0) {
			DirectionByX = -1;
		} else {
			DirectionByX = 1;
		}

		//Calculation velocity by x axis, for move character
		var newVelocityX = Mathf.MoveTowards(RB.velocity.x, Input.MoveAxis.x * Description.MaxSpeed, Time.fixedDeltaTime * Description.VelocityModifySpeed);

		//Set animator state
		if (Input.MoveAxis.x != 0) {
			RB.velocity = new Vector2(newVelocityX, RB.velocity.y);
			if (!Animator.GetBool(C.IsMove) && !InAir) {
				Animator.SetBool(C.IsMove, true);
			}
			Animator.SetFloat(C.MoveSpeed, newVelocityX * DirectionByX);
		} else if (Animator.GetBool(C.IsMove) && (Mathf.Approximately(newVelocityX, 0) || B.Layers.DynamicPropLayer.LayerInMask(LastColision.Layer()))) {
			Animator.SetBool(C.IsMove, false);
		} else {
			Animator.SetFloat(C.MoveSpeed, RB.velocity.x * DirectionByX);
		}

		//Turn in the direction of the sight
		transform.localScale = new Vector3(DirectionByX,1,1);
	}

	/// <summary> For invoke method from animator /// </summary>
	public void PlayStepSound () {
		if (!BattleEntity.Instance.IsGameOver && GetRenderer.isVisible) {
			SoundController.PlaySound(StepSound);
		}
	}

	/// <summary> Update hands and head rotation, for look to aim </summary>
	private void  UpdateHands () {
		var directionAngle = -Vector2.SignedAngle(DirectionToAim, new Vector2(DirectionByX, 0));
		HandsTransform.rotation = Quaternion.AngleAxis(directionAngle, Vector3.forward);
		HeadTransform.transform.rotation = Quaternion.AngleAxis(directionAngle * Description.HeadDirectionMultiplier, Vector3.forward);
	}

	/// <summary> Update Input </summary>
	private void UpdateInput() {

		//Hands(Weapon) input logic
		if (Input.Shot) {
			SelectedWeapon.Shot(DirectionToAim);
		}

		if (Input.ShotPressed) {
			SelectedWeapon.ShotPressed(DirectionToAim);
		}

		if (Input.Reload && !InAir) {
			SelectedWeapon.Reload();
		}

		if (Input.NextGun) {
			SelectNextWeapon();
		}

		if (Input.Interaction) {
			Interaction();
		}

		//Start jump logic
		if (Input.Jump && !InAir) {
			if (GetRenderer.isVisible) {
				SoundController.PlaySound(JumpSound);
			}
			Animator.SetTrigger(C.InAir);
			RB.velocity = new Vector2(RB.velocity.x, 0);
			RB.AddForce(new Vector2(0, Description.JumpForce), ForceMode2D.Impulse);
		}
	} 

	#endregion //ControlAndAnimatorLogic

	#region Weapons

	/// <summary> Interaction logic </summary>
	private void Interaction () {

		//If character is near loot
		if (LootInInteractionZone) {
			var tempWeapon = SelectedWeapon;
			var newWeapon = new WeaponEntity(this, LootInInteractionZone.Weapon);
			newWeapon.CartridgesInMagazine = LootInInteractionZone.CartridgesInMagazine;
			newWeapon.CartridgesTotal = LootInInteractionZone.CartridgesTotal;
			SelectedWeapon = newWeapon;
			Weapons[CurrentWeaponIndex] = SelectedWeapon;
			LootInInteractionZone.InitLoot(tempWeapon.Weapon, tempWeapon.CartridgesTotal, tempWeapon.CartridgesInMagazine);
			if (LootInInteractionZone != null) {
				LootInInteractionZone.PlayLootSound();
			}
		}
	}

	/// <summary> Select the next weapon if the character has more than one weapon </summary>
	private void SelectNextWeapon () {
		CurrentWeaponIndex++;
		if (CurrentWeaponIndex >= Weapons.Count) {
			CurrentWeaponIndex = 0;
		}
		SelectWeaponByIndex(CurrentWeaponIndex);
	}

	/// <summary> Select weapon by index if the character has more than one weapon </summary>
	private void SelectWeaponByIndex (int index) {
		if (index < 0 || index >= Weapons.Count) {
			Debug.LogError("Invalid weapon index");
			return;
		}
		CurrentWeaponIndex = index;
		if (SelectedWeapon != Weapons[index]) {
			SelectedWeapon = Weapons[index];
			SoundController.PlaySound(B.Sound.NextWeaponSound);
		}
	}

	/// <summary> Select weapon by item if the character has more than one weapon </summary>
	private void SelectWeaponByItem (WeaponItem item) {
		var weapon = Weapons.Find(w => w.Weapon == item);
		if (weapon == null) {
			Debug.LogError(string.Format("Weapon[{0}] not found", item.name));
			return;
		}
		CurrentWeaponIndex = Weapons.IndexOf(weapon);
		SelectedWeapon = weapon;
	}

	/// <summary> Take loot logic, 
	/// If there is an empty slot, then the weapon is added to it
	/// If there is such a weapon, then the number of cartridges is added
	/// </summary>
	/// <param name="loot"> Loot in world entity </param>
	private void TakeLoot (Loot loot) {
		var characterWeapon = Weapons.Find(w => w.Weapon == loot.Weapon);
		if (characterWeapon != null) {
			if (characterWeapon.CartridgesTotal < characterWeapon.Weapon.MaxCartridges) {
				characterWeapon.CartridgesTotal += loot.CartridgesTotal + loot.CartridgesInMagazine;
				UpdateGamePanel ();
				loot.PlayLootSound();
				Destroy(loot.gameObject);
			}
			LootInInteractionZone = null;
		} else if (Weapons.Count < Description.WeaponsCount){
			var newWeapon = new WeaponEntity (this, loot.Weapon);
			newWeapon.CartridgesInMagazine = loot.CartridgesInMagazine;
			newWeapon.CartridgesTotal = loot.CartridgesTotal;
			Weapons.Add(newWeapon);
			loot.PlayLootSound();
			Destroy(loot.gameObject);
			LootInInteractionZone = null;
		}
	}

	#endregion //Weapons

	#region Coroutines

	/// <summary> DeathCoroutine, for head and hands rotation </summary>
	IEnumerator DeathCoroutine () {
		float timer = 0;
		float headAngleStep = Quaternion.Angle(Quaternion.AngleAxis(HeadOnDeathRotation, Vector3.forward), HeadTransform.transform.localRotation) / DeathTime;
		float handsAngleStep = Quaternion.Angle(Quaternion.AngleAxis(HandsOnDeathRotation, Vector3.forward), HandsTransform.localRotation) / DeathTime;
		while (timer < DeathTime) {
			yield return null;
			HeadTransform.transform.localRotation = 
				Quaternion.RotateTowards(HeadTransform.transform.localRotation, Quaternion.AngleAxis(HeadOnDeathRotation, Vector3.forward), headAngleStep * Time.deltaTime);
			HandsTransform.transform.localRotation = 
				Quaternion.RotateTowards(HandsTransform.transform.localRotation, Quaternion.AngleAxis(HandsOnDeathRotation, Vector3.forward), handsAngleStep * Time.deltaTime);
			timer += Time.deltaTime;
		}

	}

	#endregion //Coroutines

	#region Unity callbacks

	private void Start () {
		AddInDamageableObjects();
	}

	void FixedUpdate () {
		if (IsDead  || Input == null || !Input.EnabledControl || BattleEntity.GameInPause) return;
		UpdateMoveCharacter();
		if (SelectedWeapon != null) {
			SelectedWeapon.FixedUpdate();
		}
	}

	void Update () {
		if (IsDead  || Input == null || !Input.EnabledControl || BattleEntity.GameInPause) return;
		UpdateHands();
		UpdateInput();
		TriggeredObjects.Clear();
	}

	private void OnDestroy () {
		StopAllCoroutines();
	}

	private void OnTriggerEnter2D (Collider2D collision) {
		if (TriggeredObjects.Contains(collision)) return;
		TriggeredObjects.Add(collision);
		if (GroundMask.LayerInMask(collision.Layer())) {
			if (InAir) {
				Animator.SetBool(C.InAir, false);
			}
			GroundCollided++;
			LastColision = collision;
		} else if (LootMask.LayerInMask(collision.Layer())) {
			var loot = collision.GetComponent<Loot>();
			LootInInteractionZone = loot;
			TakeLoot(loot);
		}
	}

	private void OnTriggerExit2D (Collider2D collision) {
		if (GroundMask.LayerInMask(collision.Layer())) {
			GroundCollided--;
			if (GroundCollided == 0) {
				Animator.SetBool(C.InAir, true);
			}
		} else if (LootMask.LayerInMask(collision.Layer())) {
			LootInInteractionZone = null;
		}
	}

	#endregion //Unity call backs
}
