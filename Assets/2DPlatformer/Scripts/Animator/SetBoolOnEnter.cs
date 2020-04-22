using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> For set bool value from animator </summary>
public class SetBoolOnEnter : StateMachineBehaviour {

	[SerializeField] string BoolName;
	[SerializeField] bool NewValue;

	override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool(BoolName, NewValue);
	}
}
