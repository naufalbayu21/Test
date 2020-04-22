using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary> Extensions is used from the whole project </summary>
public static class Extensions {

	#region LayerExtensions

	/// <summary> Checking whether the layer belongs to the current mask </summary>
	/// <returns> Return true if lauer in mask</returns>
	public static bool LayerInMask (this LayerMask mask, int layer) {
		return ((mask.value & (1 << layer)) != 0);
	}

	/// <summary> Fast layer link </summary>
	/// <returns> Return lauer int </returns>
	public static int Layer (this Collision2D collision) {
		return collision.gameObject.layer;
	}

	/// <summary> Fast layer link </summary>
	/// <returns> Return lauer int </returns>
	public static int Layer (this Collider2D collision) {
		return collision.gameObject.layer;
	}

	#endregion

	#region ColorExtensions

	/// <summary> Easy set alpha </summary>
	public static void SetStartColorAlpha (this LineRenderer line, float alpha) {
		var c = line.startColor;
		c.a = alpha;
		line.startColor = c;
	}

	/// <summary> Easy set alpha </summary>
	public static void SetEndColorAlpha (this LineRenderer line, float alpha) {
		var c = line.endColor;
		c.a = alpha;
		line.endColor = c;
	}

	#endregion

	#region SpriteRendererExtensions

	/// <summary> Easy set size 'x' on SpriteRenderer </summary>
	public static void SetSizeX (this SpriteRenderer sprite, float newX) {
		var s = sprite.size;
		s.x = newX;
		sprite.size = s;
	}

	/// <summary> Easy set size 'y' on SpriteRenderer </summary>
	public static void SetSizeY (this SpriteRenderer sprite, float newY) {
		var s = sprite.size;
		s.y = newY;
		sprite.size = s;
	}

	/// <summary> Easy set size 'z' on SpriteRenderer </summary>
	public static void SetSize (this SpriteRenderer sprite, Vector2 newSize) {
		var s = sprite.size;
		s = newSize;
		sprite.size = s;
	}

	#endregion

	#region Transform Extensions

	/// <summary> Find direction to transform </summary>
	/// <returns> Return direction with magnitude = 1 </returns>
	public static Vector2 GetDirectionTo(this Transform transform, Transform directionTransform) {
		return GetDirectionTo(transform.position, directionTransform.position);
	}

	/// <summary> Find direction to Vector2 </summary>
	/// <returns> Return direction with magnitude = 1 </returns>
	public static Vector2 GetDirectionTo(this Vector2 pos, Vector2 directionPos) {
		var direction = directionPos - pos;
		if (direction.sqrMagnitude == 0) {
			return Vector2.zero;
		}
		direction = direction/direction.magnitude;
		return direction;
	}

	/// <summary> Find direction to Vector3 </summary>
	/// <returns> Return direction with magnitude = 1 </returns>
	public static Vector3 GetDirectionTo(this Vector3 pos, Vector3 directionPos) {
		var direction = directionPos - pos;
		if (direction.sqrMagnitude == 0) {
			return Vector3.zero;
		}
		direction = direction/direction.magnitude;
		return direction;
	}

	/// <summary> Fast set global X </summary>
	public static void SetX (this Transform transform, float newX) {
		var p = transform.position;
		p.x = newX;
		transform.position = p;
	}

	/// <summary> Fast set global Y </summary>
	public static void SetY (this Transform transform, float newY) {
		var p = transform.position;
		p.y = newY;
		transform.position = p;
	}

	/// <summary> Fast set global Z </summary>
	public static void SetZ (this Transform transform, float newZ) {
		var p = transform.position;
		p.z = newZ;
		transform.position = p;
	}

	/// <summary> Fast set local X </summary>
	public static void SetLocalX (this Transform transform, float newX) {
		var p = transform.localPosition;
		p.x = newX;
		transform.localPosition = p;
	}

	/// <summary> Fast set local Y </summary>
	public static void SetLocalY (this Transform transform, float newY) {
		var p = transform.localPosition;
		p.y = newY;
		transform.localPosition = p;
	}

	/// <summary> Fast set local Z </summary>
	public static void SetLocalZ (this Transform transform, float newZ) {
		var p = transform.localPosition;
		p.z = newZ;
		transform.localPosition = p;
	}

	/// <summary> Fast set Ancored X </summary>
	public static void SetAncoredX (this RectTransform transform, float newX) {
		var p = transform.anchoredPosition;
		p.x = newX;
		transform.anchoredPosition = p;
	}

	/// <summary> Fast set Ancored Y </summary>
	public static void SetAncoredY (this RectTransform transform, float newY) {
		var p = transform.anchoredPosition;
		p.y = newY;
		transform.anchoredPosition = p;
	}

	#endregion //Transform Extensions

	#region UI Extensions

	/// <summary> Set image size </summary>
	public static void SetSize (this Image image, Vector2 newSize) {
		image.rectTransform.sizeDelta = newSize;
	}

	/// <summary> Set image size 'x' </summary>
	public static void SetSizeX (this Image image, float x) {
		var s = image.rectTransform.sizeDelta;
		s.x = x;
		image.rectTransform.sizeDelta = s;
	}

	/// <summary> Set image size 'y' </summary>
	public static void SetSizeY (this Image image, float y) {
		var s = image.rectTransform.sizeDelta;
		s.y = y;
		image.rectTransform.sizeDelta = s;
	}

	#endregion //UI Extensions

	#region System Extensions

	/// <summary>  Safe call action </summary>
	public static void SafeInvoke (this Action action) {
		if (action != null) {
			action.Invoke();
		}
	}

	/// <summary>  Safe call action with one parameter </summary>
	public static void SafeInvoke<P> (this Action<P> action, P p1) {
		if (action != null) {
			action.Invoke(p1);
		}
	}

	/// <summary>  Safe call action with two parameters </summary>
	public static void SafeInvoke<P, P2> (this Action<P, P2> action, P p1, P2 p2) {
		if (action != null) {
			action.Invoke(p1, p2);
		}
	}

	#endregion //System Extensions

	#region List Extensions

	/// <summary>  Get safe element of List  </summary>
	public static T GetSafe<T> (this List<T> list, int index) {
		if (list == null || list.Count == 0) {
			Debug.LogErrorFormat("List is empty or null, listType:", list.GetType().ToString());
			return default(T);
		}
		if (index < 0 || index > list.Count - 1) {
			Debug.LogErrorFormat("ArgumentOutOfRangeException listType:{0}, index:", list.GetType().ToString(), index.ToString());
			return default(T);
		}
		return list[index];
	} 

	/// <summary>  Get safe element of Array  </summary>
	public static T GetSafe<T> (this T[] array, int index) {
		if (array == null || array.Length == 0) {
			Debug.LogErrorFormat("Array is empty or null, arrayType:", array.GetType().ToString());
			return default(T);
		}
		if (index < 0 || index > array.Length - 1) {
			Debug.LogErrorFormat("ArgumentOutOfRangeException ArrayType:{0}, index:", array.GetType().ToString(), index.ToString());
			return default(T);
		}
		return array[index];
	}

	#endregion //List Extensions

	#region Dictionary Extensions

	/// <summary>  Try get or default value </summary>
	public static TValue TryGetOrDefault<TKey, TValue> (this Dictionary<TKey, TValue> dictionary, TKey key) {
		TValue value;
		dictionary.TryGetValue(key, out value);
		return value;
	}

	/// <summary>  Try get or create key value </summary>
	public static TValue TryGetOrCreate<TKey, TValue> (this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) {
		TValue value;
		if (!dictionary.TryGetValue(key, out value)) {
			dictionary.Add(key, defaultValue);
			return defaultValue;
		} 
		return value;
	}

	/// <summary>  Try set or create key value </summary>
	public static void TrySetOrCreate<TKey, TValue> (this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
		TValue tempValue;
		if (dictionary.TryGetValue(key, out tempValue)) {
			dictionary[key] = value;
		} else {
			dictionary.Add(key, value);
		}
	}

	/// <summary>  Try get key value pair by value </summary>
	public static bool TryGetKeyValuePair<TKey, TValue> (this Dictionary<TKey, TValue> dictionary, TValue value, out KeyValuePair<TKey, TValue> kv) {
		foreach (var kvInDict in dictionary) {
			if (kvInDict.Value.Equals(value)) {
				kv = kvInDict;
				return true;
			}
		}
		kv = new KeyValuePair<TKey, TValue>();
		return false;
	}

	#endregion //Dictionary Extensions

	#region GameObject Extensions

	/// <summary>  SetActive gameObject  </summary>
	public static void SetActive (this Component c, bool value) {
		if (c != null && c.gameObject != null) {
			c.gameObject.SetActive(value);
		}
	}

	/// <summary>  SetSelectedGameObject in UI, to control using the keyboard or gamepad </summary>
	public static void SetSelectedGameObject (this Component c, bool selectInTouchInput = false) {
		if (selectInTouchInput || PlayerProfile.InputType != InputType.TouchScreen) {
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(c.gameObject);
		}
	} 

	#endregion //GameObject Extensions
}
