using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> For fast easy animations </summary>
public class HideObjectOnExit : StateMachineBehaviour {

	override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetActive(false);
	}
}
