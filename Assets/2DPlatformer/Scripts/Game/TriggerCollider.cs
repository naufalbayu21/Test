using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> For controlled ISwitch objects </summary>
public class TriggerCollider : MonoBehaviour {

	[SerializeField] GameObject GoWithSwitch;	//Object with switch

	ISwitch Switch;

	private void Awake () {
		Switch = GoWithSwitch.GetComponent<ISwitch>();
	}

	private void OnTriggerEnter2D (Collider2D collision) {
		if (Switch != null) {
			Switch.OnSwitch(collision.gameObject);
		} else {
			Debug.LogError(string.Format("Error: GameObject {0} without ISwitch interface", GoWithSwitch.name));
		}
	}

	private void OnTriggerExit2D (Collider2D collision) {
		if (Switch != null) {
			Switch.OnSwitch(collision.gameObject, value: false);
		} else {
			Debug.LogError(string.Format("Error: GameObject {0} without ISwitch interface", GoWithSwitch.name));
		}
	}
}
