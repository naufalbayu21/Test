using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary> To spawn complex objects that require initialization </summary>
public class SpawnController : MonoBehaviour {
	[Header("Camera settings")]

	[SerializeField] Transform CameraBorderPointA;
	[SerializeField] Transform CameraBorderPointB;

	[Header("Spawn characters settings")]

	[SerializeField] Transform PlayerSpawn;
	[SerializeField] GameBalance.CharacterDescription PlayerCharacter;


	[SerializeField] SpawnNPCPreset[] SpawnNPC;

	BattleEntity Battle;

	private void Awake () {
		Battle = new BattleEntity();	
	}

	private void Start () {
		var playerCharacter = Character.CreateCharacter(PlayerSpawn.position, PlayerCharacter);
		InputController.Instance.SelectCharacter(playerCharacter);
		Battle.SetControlledCharacter(playerCharacter);
		foreach (var npc in SpawnNPC) {
			var positions = npc.FolderWithSpawnPoints.GetComponentsInChildren<Transform>();
			foreach (var pos in positions.Where(p => p != npc.FolderWithSpawnPoints)) {
				var character = Character.CreateCharacter(pos.position, npc.Character);
				var controller = character.gameObject.AddComponent<AIController>();
				controller.Init(character, playerCharacter, npc.AIConfig);
				Battle.EnemiesCharacters.Add(character);
			}
		}
		UpdateCameraBorders();
	}

	void UpdateCameraBorders () {
		var camera = CameraController.Instance;
		if (camera != null && CameraBorderPointA != null && CameraBorderPointB != null) {
			var maxX = Mathf.Max(CameraBorderPointA.position.x, CameraBorderPointB.position.x);
			var minX = Mathf.Min(CameraBorderPointA.position.x, CameraBorderPointB.position.x);
			var maxY = Mathf.Max(CameraBorderPointA.position.y, CameraBorderPointB.position.y);
			var minY = Mathf.Min(CameraBorderPointA.position.y, CameraBorderPointB.position.y);
			camera.UpdateCameraBorders(minX, maxX, minY, maxY);
		}
	}

	[Serializable]
	class SpawnNPCPreset {
		public string Caption;								//Caption in array
		public GameBalance.CharacterDescription Character;	//Character description
		public AIConfig AIConfig;							//Settings for AI behavior
		public Transform FolderWithSpawnPoints;				//Folder with spawn point, for spawn NPC
	}
}
