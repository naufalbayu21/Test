using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsWindow : BaseWindow {

	[SerializeField] LevelUI LevelRef;
	[SerializeField] Transform BackGroundParent;
	[SerializeField] Scrollbar LevelsScroll;
	[SerializeField] float ScrollSpeed = 1f;

	List<GameObject> Levels = new List<GameObject>();
	LevelUI LastUnlockedLevelGO;
	public GameBalance.LevelSettings.LevelPreset LastUnlockedLevel { get; private set; }

	protected override void OnEnableNextFrame () {
		if (LastUnlockedLevelGO == null) return;
		LastUnlockedLevelGO.SetSelectedGameObject(selectInTouchInput: true);
		OnSelectLevelGameObject(LastUnlockedLevelGO.gameObject, withAnimation: false);
	}


	/// <summary> Init all levels method </summary>
	public override void Init () {
		LastUnlockedLevel = B.Levels.Levels[0];
		foreach (var level in B.Levels.Levels) {
			var newLevel = Instantiate(LevelRef, LevelRef.transform.parent);
			newLevel.Init(level);
			Levels.Add(newLevel.gameObject);
			if (level.LevelIsUnlocked()) {
				LastUnlockedLevel = level;
				LastUnlockedLevelGO = newLevel;
				var levelButton = newLevel.GetComponent<CustomButton>();
				if (levelButton != null) {
					levelButton.OnSelectAction += OnSelectLevelGameObject;
				}
			}
		}
		SetBackGround(LastUnlockedLevel.BackGroundForMainMenu);
		LevelRef.SetActive(false);
		base.Init();
	}

	///<summary> Set back grond in main menu </summary>
	public void SetBackGround (GameObject backGround) {
		if (BackGroundParent.childCount > 0) {
			var childrens = BackGroundParent.GetComponentsInChildren<GameObject>(true);
			foreach (var child in childrens) {
				Destroy(child);
			}
		}
		var newBack = GameObject.Instantiate(backGround, BackGroundParent);
	}

	/// <summary> To set scroll without animation </summary>
	private void OnSelectLevelGameObject (GameObject selectedObject) {
		if (Input.GetMouseButtonDown(0)) return;
		OnSelectLevelGameObject(selectedObject, withAnimation: true);
	}

	/// <summary> To set scroll with animation </summary>
	private void OnSelectLevelGameObject (GameObject selectedObject, bool withAnimation) {
		float levelsCount = (float)Levels.Count-1;
		float sliderValue = levelsCount == 0? 0: (float)Levels.IndexOf(selectedObject) / levelsCount;
		if (ScrollLevelsCoroutine != null) {
			StopCoroutine(ScrollLevelsCoroutine);
			ScrollLevelsCoroutine = null;
		}
		if (withAnimation) {
			ScrollLevelsCoroutine = StartCoroutine(SetScrollLevels(sliderValue));
		} else {
			LevelsScroll.value = sliderValue;
		}
	}

	Coroutine ScrollLevelsCoroutine;

	IEnumerator SetScrollLevels (float newValue) {
		while (this != null && LevelsScroll != null && !Mathf.Approximately(LevelsScroll.value, newValue)) {
			LevelsScroll.value = Mathf.MoveTowards(LevelsScroll.value, newValue, Time.unscaledDeltaTime * ScrollSpeed);
			yield return null;
		}
		if (this != null) {
			ScrollLevelsCoroutine = null;
		}
	}
}
