using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Base window, to navigate the game menu  </summary>
public class BaseWindow : MonoBehaviour {

	[SerializeField] protected MainMenuUI MainMenu;
	[SerializeField] Button BackButton;	
	[SerializeField] BaseWindow PrewWindow;

	[SerializeField] float HideWindowPosY = 1500f;
	[SerializeField] float MoveSpeed = 3000f;

	RectTransform currentRect;
	public RectTransform CurrentRect {
		get {
			if (currentRect == null) {
				currentRect = GetComponent<RectTransform>();
			}
			return currentRect;
		}
	}

	virtual public void Init () {
		if (BackButton != null) {
			BackButton.onClick.AddListener(OnBackButton);
		}
	}

	virtual protected void OnBackButton () {
		if (PrewWindow != null) {
			PrewWindow.SelectWindow();
		}
	}

	virtual public void SelectWindow () {

		MainMenu.Blocker.SetActive(true);
		gameObject.SetActive(true);

		var oldHolder = MainMenu.CurrentWindow.CurrentRect;

		StartCoroutine(MoveWindow(oldHolder, HideWindowPosY,
			() => {
				oldHolder.SetActive(false);
			})
		);

		StartCoroutine(MoveWindow(CurrentRect, 0, 
			()=> {
				if (this != null) {
					MainMenu.CurrentWindow = this;
					MainMenu.Blocker.SetActive(false);
				}
			})
		);
	}

	IEnumerator MoveWindow (RectTransform holder, float moveToY, System.Action onCompleteAction = null) {
		var targetPosition = holder.anchoredPosition;
		targetPosition.y = moveToY;
		while (holder.anchoredPosition != targetPosition) {
			yield return null;
			holder.anchoredPosition = Vector2.MoveTowards(holder.anchoredPosition, targetPosition, Time.unscaledDeltaTime * MoveSpeed);
		}
		onCompleteAction.SafeInvoke();
	}

	protected virtual void OnEnable () {
		StartCoroutine(EnabledCoroutine());
	}

	IEnumerator EnabledCoroutine () {
		yield return null;
		if (this.isActiveAndEnabled) {
			OnEnableNextFrame();
		}
	}

	protected virtual void OnEnableNextFrame () { }
}
