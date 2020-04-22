using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace GameBalance {

	/// <summary> Level settings, have custom editor to be able to specify links to scenes </summary>

	[CreateAssetMenu(fileName = "LevelSettings", menuName = "Game Balance/Settings/LevelSettings")]
	public class LevelSettings : ScriptableObject {

		[SerializeField] string mainMenuScene;

		[SerializeField] string mainWorldScene;

		[SerializeField] List<LevelPreset> levels = new List<LevelPreset>();

		public string MainWorldScene { get { return mainWorldScene; } }

		public string MainMenuScene { get { return mainMenuScene; } }

		public List<LevelPreset> Levels { get { return levels; } }


		/// <summary> Level preset. Have links to level scene, unlock settings and visual settings </summary>
		[Serializable]
		public class LevelPreset {
			[SerializeField] string levelName;

			[SerializeField] public string levelScenePath;
			[SerializeField] string levelSceneName;

			[SerializeField] UnlockCondition unlockLevelCondition = UnlockCondition.PrevLevelCompleted;
			[SerializeField] int concreteLevelIndex;

			[SerializeField] GameObject backGroundForMainMenu;
			[SerializeField] Sprite buttonLevelBackSprite;

			[SerializeField] List<AudioClip> musicInLevel = new List<AudioClip>();

			public string LevelName { get { return levelName; } }
			public string LevelScenePath { get { return levelScenePath; } }
			public string LevelSceneName { get { return levelSceneName; } }
			public UnlockCondition UnlockLevelCondition { get { return unlockLevelCondition; } }
			public int ConcreteLevelIndex { get { return concreteLevelIndex; } }
			public GameObject BackGroundForMainMenu { get { return backGroundForMainMenu; } }
			public Sprite ButtonLevelBackSprite { get { return buttonLevelBackSprite; } }
			public List<AudioClip> MusicInLevel { get { return musicInLevel; } }

			public bool ConditionIsConcreteLevel { get { return UnlockLevelCondition == UnlockCondition.ConcreteLevelCompleted; } }

			public bool LevelIsUnlocked () {
				switch (UnlockLevelCondition) {
					case UnlockCondition.PrevLevelCompleted: {
						var level = B.Levels.Levels.GetSafe(B.Levels.Levels.IndexOf(this) - 1);
						if (level != null) {
							return level.LevelIsCompleted();
						}
						return true;
					}
					case UnlockCondition.ConcreteLevelCompleted: {
						var level = B.Levels.Levels.GetSafe(ConcreteLevelIndex);
						if (level != null) {
							return level.LevelIsCompleted();
						}
						return true;
					}

					default: return true;
				}
			}

			public void MarkAsCompleted () {
				PlayerProfile.SetLevelIsCompleted(LevelSceneName);
			}

			public bool LevelIsCompleted () {
				return PlayerProfile.GetLevelIsCompleted(LevelSceneName);
			}

			public void LoadLevel () {
				SceneController.LoadLevel(this);
			}

			public enum UnlockCondition {
				PrevLevelCompleted,
				ConcreteLevelCompleted,
				None,
			}
		}
	}
}
