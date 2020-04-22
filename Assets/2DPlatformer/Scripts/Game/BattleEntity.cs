using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> To store the state of the world and objects </summary>
public class BattleEntity {

	public static BattleEntity Instance;

	public Character ControlledCharacter { get; private set; }
	public List<Character> EnemiesCharacters { get; private set; }
	public List<IDamageable> DamageableObjects { get; private set; }
	public bool IsGameOver { get; set; }
	public static bool GameInPause { get { return Mathf.Approximately(Time.timeScale, 0) || (Instance != null && Instance.IsGameOver); } }

	public BattleEntity () {
		EnemiesCharacters = new List<Character>();
		DamageableObjects = new List<IDamageable>();
		Instance = this;
	}

	public void SetControlledCharacter (Character character) {
		ControlledCharacter = character;
	}
}
