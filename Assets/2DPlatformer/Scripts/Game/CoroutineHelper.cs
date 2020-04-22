using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary> Helper for invoke coroutines from is not MonoBehaviour </summary>
public class CoroutineHelper : MonoBehaviour {

	static CoroutineHelper instance;

	static CoroutineHelper Instance {
		get {
			if (instance == null) {
				var go = Instantiate<GameObject>(new GameObject());
				DontDestroyOnLoad(go);
				instance = go.AddComponent<CoroutineHelper>();
			}
			return instance;
		}
	}

	/// <summary> LaunchCoroutine method </summary>
	/// <param name="iEnumerator"> Coroutine </param>
	/// <param name="onCompleteAction"> Action invoked on end coroutine </param>
	/// <returns> Link on coroutine </returns>
	public static Coroutine LaunchCoroutineWithEndAction (IEnumerator iEnumerator, Action onCompleteAction = null) {
		return Instance.StartCoroutine(Instance.LaunchCoroutine(iEnumerator, onCompleteAction));
	}

	/// <summary> Stop coroutine </summary> 
	public static void BreakCoroutine (Coroutine coroutine) {
		Instance.StopCoroutine(coroutine);
	}

	/// <summary> LaunchCoroutine logic </summary> 
	private IEnumerator LaunchCoroutine (IEnumerator iEnumerator, Action onCompleteAction) {
        while (true) {
			object current = null;
			try {
				if (!iEnumerator.MoveNext()) {
					break;
				}
				current = iEnumerator.Current;
			}
			catch (Exception ex) {
				Debug.LogException(ex);
			}
			yield return current;
		}
		onCompleteAction.Invoke();
	}
}
