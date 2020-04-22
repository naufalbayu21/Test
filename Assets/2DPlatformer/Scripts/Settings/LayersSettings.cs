using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Masks and layers settings </summary>
[CreateAssetMenu(fileName = "LayersSettings", menuName = "Game Balance/Settings/LayersSettings")]
public class LayersSettings: ScriptableObject  {

	[SerializeField] LayerMask groundLayer;
	[SerializeField] LayerMask bulletCollidedMask;
 	[SerializeField] LayerMask dynamicPropLayer;
	[SerializeField] LayerMask alliesMask;
	[SerializeField] LayerMask enemyMask;
	[SerializeField] LayerMask lootMask;
 
	public LayerMask GroundMask { get { return groundLayer; } }
	public LayerMask BulletCollidedMask { get { return bulletCollidedMask; } }
	public LayerMask DynamicPropLayer { get { return dynamicPropLayer; } }
	public LayerMask AlliesMask { get { return alliesMask; } }
	public LayerMask EnemyMask { get { return enemyMask; } }
	public LayerMask LootMask { get { return lootMask; } }
}
