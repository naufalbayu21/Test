using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;

/// <summary> Main weapon logic script </summary>

public class WeaponEntity {

	Character Character;								//Character owning weapon
	Animator HandsAnimator;								//Hands Animator

	float currentScatter;								//Scatter, Increases after a shot and during a jump
	int cartridgesInMagazine;							//Count of cartridges in the magazine
	int totalCartridges;                                //Count of cartridges total

	#region Public properties

	public WeaponItem Weapon { get; private set; }
	public bool IsShooting { get; private set; }
	public bool IsReloading { get; private set; }
	public int CartridgesInMagazine {
		get {
			if (Weapon.MaxCartridgesInMagazine == 0) {
				return int.MaxValue;
			}
			return cartridgesInMagazine;
		}
		set {
			cartridgesInMagazine = value;
		}
	}
	public int CartridgesTotal {
		get {
			if (Weapon.MaxCartridges == 0 || Character.IsBot) {
				return int.MaxValue;
			}
			return totalCartridges;
		}
		set {
			totalCartridges = Mathf.Min(Weapon.MaxCartridges, value);
		}
	}
	public bool CartridgesTotalIsEmpty { get { return CartridgesTotal <= 0; } }
	public bool MagazineIsFull { get { return cartridgesInMagazine == Weapon.MaxCartridgesInMagazine; } }
	public bool MagazineIsEmpty { get { return cartridgesInMagazine <= 0; } }
	public bool WeaponIsSelected { get { return Character.SelectedWeapon == this; } }
	public float CurrentScatter {
		get { return Character.InAir? currentScatter + Weapon.ScatterInAirOffset: currentScatter; }
		private set { currentScatter = value; }
	}

	#endregion //Public properties

	#region Private properties

	private LayerMask BulletCollidedMask { get { return B.Layers.BulletCollidedMask; } }
	private Aim Aim { get { return InputController.Instance.Aim; } }
	private GamePanel GamePanel { get { return GamePanel.Instance; } }
	private LayerMask EnemyMask { get { return Character.EnemyMask; } }

	#endregion //Private properties

	#region Instantiation and select logic

	/// <summary> A new instance WeaponEntity </summary>
	/// <param name="character"> Character owning weapon </param>
	/// <param name="weapon"> Ref at WeaponItem </param>
	public WeaponEntity (Character character, WeaponItem weapon) {
		Character = character;
		Weapon = weapon;
		CurrentScatter = Weapon.MinScatter;
		CartridgesInMagazine = Weapon.MaxCartridgesInMagazine;
		CartridgesTotal = 0;
	}

	/// <summary> Select this weapon </summary>
	/// <param name="character"> Character owning weapon </param>
	public void SelectWeapon (Character character) {
		Character = character;
		HandsAnimator = Character.GetHandsAnimator;
		HandsAnimator.runtimeAnimatorController = Weapon.AnimatorController;
		Character.UpdateGamePanel();
	}

	/// <summary> Deselect this weapon </summary>
	public void DeselectWeapon () {
		if (ReloadCoroutineRef != null) {
			SoundController.StopSound(Weapon.ReloadSound);
			CoroutineHelper.BreakCoroutine(ReloadCoroutineRef);
		}
		if (ShotCoroutineRef != null) {
			CoroutineHelper.BreakCoroutine(ShotCoroutineRef);

		}
		IsReloading = false;
		IsShooting = false;
		ReloadCoroutineRef = null;
		ShotCoroutineRef = null;
	}

	#endregion //Instantiation and select logic

	#region ShotLogic

	/// <summary> Method for a single shot </summary>
	/// <param name="shotDirection"> The direction in which the shot will be fired </param>
	public void Shot (Vector2 shotDirection) {
		DoShot (shotDirection);
	}

	/// <summary> Method for a multi shot </summary>
	/// <param name="shotDirection"> The direction in which the shot will be fired </param>
	public void ShotPressed (Vector2 shotDirection) {
		if (Weapon.WeaponType == WeaponTypes.AutomaticWeapon) {
			DoShot(shotDirection);
		}
	}

	/// <summary> Make a shot </summary>
	/// <param name="shotDirection"> The direction in which the shot will be fired </param>
	void DoShot (Vector2 shotDirection) {
		//Checking for the possibility of a shot
		if (IsReloading || IsShooting || MagazineIsEmpty || Character.IsDead) return;

		//Finding the angle of scatter
		if (CurrentScatter != 0) {
			var angle = Random.Range(-CurrentScatter, CurrentScatter);

			float cosAngle = B.Math.GetCos(angle);
			float sinAngle = B.Math.GetSin(angle);

			var newX = shotDirection.x * cosAngle - shotDirection.y * sinAngle;
			var newY = shotDirection.x * sinAngle + shotDirection.y * cosAngle;
			shotDirection.x = newX;
			shotDirection.y = newY;
		}

		//Selecting the type of shot
		if (Weapon.IsBulletType || Weapon.IsBulletRayShotType) {
			BulletShot(shotDirection);
		} else if (Weapon.IsGrenadeType) {
			GrenadeShot(shotDirection);
		} else {
			FastRayShot(shotDirection);
		}
	}

	/// <summary> Ray shot type </summary>
	/// <param name="shotDirection"> The direction in which the shot will be fired </param>
	void BulletRayShot (Vector2 shotDirection) {

	}

	/// <summary> Ray shot type </summary>
	/// <param name="shotDirection"> The direction in which the shot will be fired </param>
	void FastRayShot (Vector2 shotDirection) {
		RaycastHit2D hit;
		Vector2 shotStartPos = Character.GetShotStartTransform.position;
		hit = Physics2D.Raycast(Character.transform.position, shotDirection, Weapon.ShotDistance, BulletCollidedMask | EnemyMask);
		if (hit.collider == null || hit.distance > Weapon.MinDistance) {
			Vector2 point = hit.point;
			bool needCreateSpark = false;
			if (hit.collider != null) {
				needCreateSpark = true;
				if (B.Layers.DynamicPropLayer.LayerInMask(hit.collider.Layer()) || EnemyMask.LayerInMask(hit.collider.Layer())) {
					var collidedRB = hit.collider.GetComponent<Rigidbody2D>();
					var damageable = hit.collider.GetComponent<IDamageable>();
					if (damageable != null) {
						damageable.SetDamage(Weapon.DamageBullet);
					}
					if (collidedRB != null) {
						collidedRB.AddForce(Vector2.MoveTowards(Vector2.zero, shotDirection, Weapon.PowerBullet), ForceMode2D.Impulse);
					} else {
						Debug.LogError("DynamicProp without rigidBody2D", hit.collider.gameObject);
					}
				}
			} else {
				shotDirection = shotDirection / shotDirection.magnitude;
				point = shotStartPos + (shotDirection * Weapon.ShotDistance);
			}
			WorldEffects.Instance.CreateTrace(shotStartPos, point, needCreateSpark);
			OnShotAction();
		}
	}

	/// <summary> Bullet or RayBullet shot type </summary>
	/// <param name="shotDirection"> The direction in which the shot will be fired </param>
	void BulletShot (Vector2 shotDirection) {
		shotDirection = shotDirection / shotDirection.magnitude;
		var bulletMask = (LayerMask)(EnemyMask | BulletCollidedMask);
		Vector2 shotStartPos = Character.GetShotStartTransform.position;
		if (Weapon.IsBulletRayShotType) {
			WorldLogic.Instance.CreateRayBullet(shotStartPos, shotDirection, Weapon.SpeedBullet, Weapon.PowerBullet, Weapon.DamageBullet, Weapon.LifeTimeBullet, bulletMask, Weapon.RayColor);
		} else {
			WorldLogic.Instance.CreateBullet(shotStartPos, shotDirection, Weapon.SpeedBullet, Weapon.PowerBullet, Weapon.DamageBullet, Weapon.LifeTimeBullet, bulletMask);
		}
		OnShotAction();
	}

	/// <summary> Grenade shot type </summary>
	/// <param name="shotDirection"> The direction in which the shot will be fired </param>
	void GrenadeShot (Vector2 shotDirection) {
		shotDirection = shotDirection / shotDirection.magnitude;
		var grenadeMask = (LayerMask)(EnemyMask | BulletCollidedMask);
		Vector2 shotStartPos = Character.GetShotStartTransform.position;
		WorldLogic.Instance.CreateGrenade(shotStartPos, shotDirection, Weapon.SpeedBullet, Weapon.PowerBullet, Weapon.DamageBullet, Weapon.LifeTimeBullet, grenadeMask);
		OnShotAction();
	}

	/// <summary> On shot action, general rules are implemented for all types </summary>
	void OnShotAction () {
		HandsAnimator.SetTrigger(C.Shot);
		SoundController.PlaySound(Weapon.ShotSound);
		IsShooting = true;
		ShotCoroutineRef = CoroutineHelper.LaunchCoroutineWithEndAction(ShotCoroutine(), () => { IsShooting = false; });
		CurrentScatter += Weapon.IncScatterInOneShot;
		CurrentScatter = Mathf.Min(CurrentScatter, Weapon.MaxScatter);

		cartridgesInMagazine--;
		if (Character.IsUserControlled) {
			GamePanel.SetCartridgesInMagazine(cartridgesInMagazine);
		}
		if (MagazineIsEmpty) {
			Reload(ignoreShooting: true);
		}
	}

	#endregion //ShotLogic

	#region Reload logic

	/// <summary> Reload logic </summary>
	/// <param name="ignoreShooting"> To reload during the last shot</param>
	public void Reload (bool ignoreShooting = false) {
		if (IsReloading || MagazineIsFull || CartridgesTotalIsEmpty || (IsShooting && !ignoreShooting)) return;
		SoundController.PlaySound(Weapon.ReloadSound);
		HandsAnimator.SetTrigger(C.Reload);
		IsReloading = true;
		ReloadCoroutineRef = CoroutineHelper.LaunchCoroutineWithEndAction(ReloadCoroutine(), () => { IsReloading = false; });
	}

	#endregion //Reload logic

	#region Update logic

	public void FixedUpdate () {
		if (currentScatter > Weapon.MinScatter) {
			CurrentScatter = Mathf.MoveTowards(currentScatter, Weapon.MinScatter, Time.fixedDeltaTime * Weapon.MoveToMinScatterSpeed);
		}
		if (Character.IsUserControlled) {
			Aim.SetScatter(CurrentScatter);
		}
	}

	#endregion

	#region Coroutines

	Coroutine ShotCoroutineRef;
	Coroutine ReloadCoroutineRef;

	IEnumerator ShotCoroutine () {
		yield return new WaitForSeconds(Weapon.ShotTime);
		ShotCoroutineRef = null;
	}

	IEnumerator ReloadCoroutine () {
		yield return new WaitForSeconds(Weapon.ReloadTime);
		if (!WeaponIsSelected) yield break;
		var cartridges = Mathf.Min(
			Weapon.MaxCartridgesInMagazine, 
			CartridgesTotal == int.MaxValue? CartridgesTotal:
											 CartridgesTotal +  CartridgesInMagazine
		);
		CartridgesTotal -= cartridges - CartridgesInMagazine;
		CartridgesInMagazine = cartridges;
		if (Character.IsUserControlled && WeaponIsSelected) {
			GamePanel.SetCartridgesInMagazine(CartridgesInMagazine);
			GamePanel.SetTotalCartridges(CartridgesTotal);
		}
		ReloadCoroutineRef = null;
	}

	#endregion //Coroutines
}
