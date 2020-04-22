using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Horizontal and vertical lift </summary>
public class Lift : MonoBehaviour {

	[SerializeField] Transform TargetPoint;

	[SerializeField] float Speed = 0.05f;
	[SerializeField] float WaitTimeOnBorders = 1;

	Vector2 Direction;
	Transform Target;
	Transform OldTarget;
	float MaxSqrDistance;
	Coroutine WaitTimeCoroutine;
	Rigidbody2D RB;

	private void Awake () {
		var startPoint = GameObject.Instantiate(TargetPoint);
		startPoint.position = transform.position;

		MaxSqrDistance = (TargetPoint.position - startPoint.position).sqrMagnitude;
		Target = TargetPoint;
		OldTarget = startPoint;
		Direction = transform.GetDirectionTo(Target);

		startPoint.SetParent(null);
		TargetPoint.SetParent(null);

		RB = GetComponent<Rigidbody2D>();
		RB.velocity = Direction * Speed;
	}

	private void FixedUpdate () {
		if (WaitTimeCoroutine != null) return;

		if ((OldTarget.position - transform.position).sqrMagnitude >= MaxSqrDistance) {
			WaitTimeCoroutine = StartCoroutine(StartWaitTime());
			RB.velocity = Vector2.zero;
			Direction *= -1;

			var temp = Target;
			Target = OldTarget;
			OldTarget = temp;
		}
	}

	IEnumerator StartWaitTime () {
		yield return new WaitForSeconds (WaitTimeOnBorders);
		RB.velocity = Direction * Speed;
		WaitTimeCoroutine = null;
	}
}
