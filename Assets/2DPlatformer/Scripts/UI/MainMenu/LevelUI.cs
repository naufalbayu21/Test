using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBalance;

[RequireComponent(typeof(CustomButton))]
public class LevelUI : MonoBehaviour {

	[SerializeField] Image LevelBackGround;
	[SerializeField] Text LevelName;
	[SerializeField] GameObject LockedLevelGO;

	LevelSettings.LevelPreset LevelPreset;

	CustomButton levelButton;
	CustomButton LevelButton {
		get {
			if (levelButton == null) {
				levelButton = GetComponent<CustomButton>();
			}
			return levelButton;
		}
	}

	public void Init (LevelSettings.LevelPreset levelPreset) {
		LevelPreset = levelPreset;
		var levelIsUnlocked = levelPreset.LevelIsUnlocked();
		LevelButton.interactable = levelIsUnlocked;
		LockedLevelGO.SetActive(!levelIsUnlocked);

		LevelName.text = LevelPreset.LevelName;
		LevelBackGround.sprite = LevelPreset.ButtonLevelBackSprite;

		LevelButton.onClick.AddListener(LevelPreset.LoadLevel);
	}
}
