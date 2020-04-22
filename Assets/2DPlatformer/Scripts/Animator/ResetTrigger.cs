using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> For reset trigger after use </summary>
public class ResetTrigger : StateMachineBehaviour {

	[SerializeField] string TriggerName;

	void OnStateEnter (Animator animator) {
		animator.ResetTrigger(TriggerName);
	}
}
