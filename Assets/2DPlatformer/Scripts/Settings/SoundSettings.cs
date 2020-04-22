using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance {

	/// <summary> Links to sounds and music used in game </summary>
	[CreateAssetMenu(fileName = "SoundSettings", menuName = "Game Balance/Settings/SoundSettings")]
	public class SoundSettings : ScriptableObject {

		[Header("UI Sounds")]
		[SerializeField] AudioClipPreset buttonOnHighlightSound;
		[SerializeField] AudioClipPreset buttonOnClickSound;


		[Header("Game sounds")]
		[SerializeField] AudioClipPreset nextWeaponSound;

		[Header("Effects")]
		[SerializeField] List<AudioClipPreset> sparkSounds = new List<AudioClipPreset>();
		[SerializeField] List<AudioClipPreset> raySparkSounds = new List<AudioClipPreset>();
		[SerializeField] AudioClipPreset explosionSounds;

		[SerializeField] List<AudioClip> music = new List<AudioClip>();

		public List<AudioClip> Music { get { return music; } }
		public List<AudioClipPreset> SparkSounds { get { return sparkSounds; } }
		public List<AudioClipPreset> RaySparkSounds { get { return raySparkSounds; } }
		public AudioClipPreset ExplosionSound { get { return explosionSounds; } }
		public AudioClipPreset NextWeaponSound { get { return nextWeaponSound; } }
		public AudioClipPreset ButtonOnHighlightSound { get { return buttonOnHighlightSound; } }
		public AudioClipPreset ButtonOnClickSound { get { return buttonOnClickSound; } }
	}
}

	public enum SoundType {
		Standart,
		Custom,
		None,
	}
