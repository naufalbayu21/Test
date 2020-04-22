using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance {

	/// <summary> Keeps links to all the settings used in the game </summary>
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Balance/Settings/GameSettings")]
	public class GameSettings : ScriptableObject {

		[SerializeField] LevelSettings levels;
		[SerializeField] SoundSettings sound;
		[SerializeField] ResourceSettings resource;
		[SerializeField] LayersSettings layers;
		[SerializeField] MathSettings math;
		[SerializeField] InputType defaultInputType = InputType.KeyboardAndMouse;
		public LevelSettings Levels { get { return levels; } }
		public SoundSettings Sound { get { return sound; } }
		public ResourceSettings Resource { get { return resource; } }
		public LayersSettings Layers { get { return layers; } }
		public MathSettings Math { get { return math; } }
		public InputType DefaultInputType { get { return defaultInputType; } }

	}
}
