using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ToAnimationState : MonoBehaviour {

	[SerializeField] float StartDellay;

	private IEnumerator Start () {
		yield return new WaitForSeconds(StartDellay);
		var amimator = GetComponent<Animator>();
		amimator.SetBool(C.Animation, true);
	}

}
