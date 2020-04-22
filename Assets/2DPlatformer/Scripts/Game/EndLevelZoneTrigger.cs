using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> To display a window "GameOver" </summary>
public class EndLevelZoneTrigger : MonoBehaviour {

	[SerializeField] LayerMask TriggeredFilter;

	private void OnTriggerEnter2D (Collider2D collision) {
		if (TriggeredFilter.LayerInMask(collision.gameObject.layer)) {
			InGameMenu.Instance.ShowGameOver(endLevelTriggered: true);
		}
	}
}
