using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> This script for controlled AI entities </summary>
public class AIController: InputBaseClass {

	#region Base properties

	public override Vector2 AimPos { get; protected set; }

	public override InputController Controller { get; protected set; }

	public override bool Interaction { get; protected set; }

	public override bool Jump { get; protected set; }

	public override Vector2 MoveAxis { get; protected set; }

	public override bool NextGun { get; protected set; }

	public override bool Reload { get; protected set; }

	public override bool Shot { get; protected set; }

	public override bool ShotPressed { get; protected set; }
	public override bool EnabledControl { get; protected set; }

	#endregion //Base properties

	#region Fast links

	private LayerMask GroundMask { get { return B.Layers.GroundMask; } }
	private LayerMask BulletCollidedMask { get { return B.Layers.BulletCollidedMask; } }

	#endregion ////Fast link

	#region Private fields

	AIConfig Config;
	Character ControlledCharacter;
	Character EnemyCharacter;
	List<Vector2> PathPoints = new List<Vector2>();
	Renderer Renderer;

	int MoveDirection;
	bool Alarmed;
	private float SpeedPatrol;
	private float SpeedPursuit;
	float InvisibleTimer;

	#endregion //Private fields

	#region Private fields with properties
	
	//Target point is selected next after reaching the current point.
	Vector2 targetPoint;
	Vector2 TargetPoint {
		get { return targetPoint; }
		set {
			targetPoint = value;
			MoveDirection = value.x < ControlledCharacter.Position.x ? -1 : 1;
		}
	}

	//IsStoped for waiting or shoting at enemy.
	bool isStoped;
	bool IsStoped {
		get { return isStoped; }
		set {
			if (value) {
				MoveAxis = new Vector2(0, 0);
				var v = ControlledCharacter.RB.velocity;
				v.x = 0;
				ControlledCharacter.RB.velocity = v;
			}
			isStoped = value;
		}
	}

	#endregion //Private fields with properties

	#region Initialization
	public void Init (Character controlledCharacter, Character enemyCharacter, AIConfig config) {
		MoveDirection = 1;
		ControlledCharacter = controlledCharacter;
		EnemyCharacter = enemyCharacter;
		Config = config;
		InitPath();
		ControlledCharacter.SetInput(this);
		var random = UnityEngine.Random.Range(-Config.SpeedRandom, Config.SpeedRandom);
		SpeedPatrol = Config.SpeedPatrol + random;
		SpeedPursuit = Config.SpeedPursuit + random;

		Renderer = ControlledCharacter.GetRenderer;
	}


	/// <summary> 
	/// Find most left and right points, In turn, in both directions, rays are launched, to search for walls and abysses. 
	/// One step is one unit.
	/// the distance is specified in the config "PathHalfLength".
	/// </summary>
	private void InitPath () {
		RaycastHit2D hitHorizontal;
		RaycastHit2D hitVertical;
		Vector2 point = ControlledCharacter.Position;
		Vector2 rayPoint = point;
		for (int step = 0; step < Config.PathHalfLength; step++) {
			rayPoint.x += 1;
			hitHorizontal = Physics2D.Raycast(rayPoint, Vector2.right, 1, BulletCollidedMask | GroundMask);
			hitVertical = Physics2D.Raycast(rayPoint, Vector2.down, 1, BulletCollidedMask | GroundMask);
			if (hitHorizontal.collider || !hitVertical.collider) {
				break;
			}
			point = rayPoint;
		}

		PathPoints.Add(point);
		point = ControlledCharacter.Position;
		rayPoint = point;
		for (int step = 0; step < Config.PathHalfLength; step++) {
			rayPoint.x -= 1;
			hitHorizontal = Physics2D.Raycast(rayPoint, Vector2.left, 1, BulletCollidedMask | GroundMask);
			hitVertical = Physics2D.Raycast(rayPoint, Vector2.down, 1, BulletCollidedMask | GroundMask);
			if (hitHorizontal.collider || !hitVertical.collider) {
				break;
			}
			point = rayPoint;
		}
		if (PathPoints[0] == point) {
			PathPoints.Clear();
			return;
		} else {
			PathPoints.Add(point);
		}


		TargetPoint = PathPoints[UnityEngine.Random.Range(0, 2)];
	}

	#endregion //Initialization

	#region Move logic

	/// <summary> Alarmed translates to the true if an enemy is seen or damage is received. </summary>
	/// <param name = "aimPos"> New aim position </param>
	/// /// <param name = "alarmed"> New alarmed value </param>
	public void SetAlarmed (Vector2 aimPos = new Vector2(), bool alarmed = true) {
		if (Alarmed == alarmed) return;

		Alarmed = alarmed;
		if (Alarmed) {
			IsStoped = true;
			if (aimPos != Vector2.zero || EnemyCharacter.IsDead) {
				AimPos = aimPos;
			} else {
				AimPos = EnemyCharacter.Position;
			}
		}
	}

	private void UpdateMove () {
		if (IsStoped || !EnabledControl) return;

		//Check for reaching a point
		var еargetPointOvertake = ( 
			(MoveDirection < 0 && ControlledCharacter.Position.x < TargetPoint.x) ||
			(MoveDirection >= 0 && ControlledCharacter.Position.x >= TargetPoint.x)
		);
		if (еargetPointOvertake) {
			WaitCoroutine = StartCoroutine(StartWaitCoroutine ());
			return;
		}
		MoveAxis = new Vector2((Alarmed? SpeedPursuit: SpeedPatrol) * MoveDirection, 0);
		if (!Alarmed) {
			AimPos = (Vector2)ControlledCharacter.Position + MoveAxis;
		}
	}

	#endregion //Move logic

	#region Coroutines

	Coroutine WaitCoroutine;
	Coroutine CustomUpdateCoroutine;

	bool HasWaitCoroutine { get { return WaitCoroutine != null; } }

	/// <summary> Wait coroutine, started after reaching a TargetPoint </summary>
	IEnumerator StartWaitCoroutine () {
		IsStoped = true;
		yield return new WaitForSeconds(Config.WaitTime);
		if (PathPoints.Count == 0) {
			MoveDirection *= -1;
			AimPos = ControlledCharacter.Position + new Vector2(MoveDirection, 0);
		} else {
			var nextIndex = (PathPoints.Contains(TargetPoint)? PathPoints.IndexOf(TargetPoint): 0) + 1; //Find next target point index.
			if (nextIndex >= PathPoints.Count) {
				nextIndex = 0;
			}
			TargetPoint = PathPoints[nextIndex];
		}
		Alarmed = false;		//Reset Alarmed status (If Target point is last visible enemy position).
		IsStoped = false;		//Reset for continue move.
		WaitCoroutine = null;	//Reset WaitCoroutine field.
	}

	/// <summary> For optimization, logic works with a certain frequency </summary>
	IEnumerator CustomUpdate () {

		var collidedMask = (BulletCollidedMask | ControlledCharacter.EnemyMask);

		while (true) {

			//Checking for the presence of the enemy
			if (EnemyCharacter == null || EnemyCharacter.IsDead) {
				Alarmed = false;
				Shot = false;
				ShotPressed = false;
				IsStoped = false;
			}

			yield return null; //Wait one frame for shoting.
			Shot = false;	

			yield return new WaitForSeconds(Config.UpdateTick); //

			ShotPressed = false;

			//Find direction to enemy
			var directionToEnemy = EnemyCharacter.IsDead? Vector2.zero: EnemyCharacter.Position - ControlledCharacter.Position;		
			var directionsEqual = Alarmed || (directionToEnemy.x < 0 && MoveDirection < 0) || (directionToEnemy.x > 0 && MoveDirection > 0);

			//Checking the visibility and obstacles to the enemy
			if (directionsEqual) {
				RaycastHit2D hit = Physics2D.Raycast(
					(Vector2)ControlledCharacter.Position + Config.OffsetHeadPosition,
					directionToEnemy, 
					Config.MaxDistanceToCharacter, 
					collidedMask
				); 
				if (hit.collider != null && ControlledCharacter.EnemyMask.LayerInMask(hit.collider.gameObject.layer)) {
					if (HasWaitCoroutine) {
						StopCoroutine(WaitCoroutine);
						WaitCoroutine = null;
					}
					Alarmed = true;
					IsStoped = true;
					AimPos = hit.collider.transform.position;
					Shot = true;
					ShotPressed = true;
				} else if (!HasWaitCoroutine && Alarmed) {
					TargetPoint = AimPos;
					IsStoped = false;
				}
			}

			//Jump if leaned against the wall
			Jump = MoveAxis.x != 0 && Mathf.Abs(ControlledCharacter.RB.velocity.x) < 0.05f;
		}
	}

	#endregion //Coroutines

	#region Update Enabled/Disabled logic

	/// <summary> UpdateIsVisibleState </summary>
	private void UpdateIsVisibleState () {
		if (Renderer.isVisible) {
			if (!EnabledControl) {
				OnEnabledControl(true);
			}
			InvisibleTimer = 0;
		} else if (EnabledControl) {
			if (InvisibleTimer >= Config.EnabledTimeOnInvisible) {
				OnEnabledControl(false);
			} else {
				InvisibleTimer += Time.fixedDeltaTime;
			}
		}
	} 

	/// <summary> Disabled for optimization </summary>
	private void OnEnabledControl (bool visible) {
		EnabledControl = visible;
		if (CustomUpdateCoroutine != null) {
			StopCoroutine(CustomUpdateCoroutine);
			CustomUpdateCoroutine = null;
		}
		if (visible) {
			CustomUpdateCoroutine = StartCoroutine(CustomUpdate());
		}
	}

	#endregion //Update Enabled/Disabled logic

	#region Unity callbacks

	private void FixedUpdate () {
		if (Config == null || ControlledCharacter.IsDead) { return; }
		UpdateMove();
		UpdateIsVisibleState();
	}

	private void OnDisable () {
		StopAllCoroutines();
	}

	#endregion //Unity callbacks
}

//AIConfig contains all custom fields for initialization AIController

[Serializable]
public class AIConfig {
	public int PathHalfLength;					//Half length of path, for create path
	public float SpeedRandom;					//Random speed border
	public float SpeedPatrol;					//Speed at rest
	public float SpeedPursuit;					//Speed in the excited state
	public float WaitTime;						//Wait time in target point of path
	public float MaxDistanceToCharacter;		//Length of ray to enemy character
	public float UpdateTick = 1;				//Update tick for optimization
	public float EnabledTimeOnInvisible = 5f;	//The time through which the update will turn off if the character is not visible
	public Vector2 OffsetHeadPosition;			//Offset for start point of ray
}
