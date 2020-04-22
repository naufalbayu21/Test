using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameBalance {

	/// <summary> Links to resources and prefabs </summary>
	[CreateAssetMenu(fileName = "ResourceSettings", menuName = "Game Balance/Settings/ResourceSettings")]
	public class ResourceSettings : ScriptableObject {
		[Serializable]
		public class PrefabsClass {

			[SerializeField] SoundController soundController;
			[SerializeField] InputController inputController;
			[SerializeField] Bullet bulletPrefab;
			[SerializeField] Bullet rayBulletPrefab;
			[SerializeField] Grenade grenadePrefab;
			[SerializeField] ParticleSystem explosionPrefab;
			[SerializeField] ParticleSystem sparkPrefab;
			[SerializeField] ParticleSystem raySparkPrefab;
			[SerializeField] LineRenderer traceLine;

			public SoundController SoundController { get { return soundController; } }
			public InputController InputController { get { return inputController; } }
			public Bullet BulletPrefab { get { return bulletPrefab; } }
			public Bullet RayBulletPrefab { get { return rayBulletPrefab; } }
			public Grenade GrenadePrefab { get { return grenadePrefab; } }
			public ParticleSystem ExplosionPrefab { get { return explosionPrefab; } }
			public ParticleSystem SparkPrefab { get { return sparkPrefab; } }
			public ParticleSystem RaySparkPrefab { get { return raySparkPrefab; } }
			public LineRenderer TraceLine { get { return traceLine; } }
		}

		[SerializeField]
		PrefabsClass prefabs = new PrefabsClass();

		[SerializeField] Texture textureForEditor;

		public PrefabsClass Prefabs { get { return prefabs; } }
		public Texture TextureForEditor { get { return textureForEditor; } }
	}

}
