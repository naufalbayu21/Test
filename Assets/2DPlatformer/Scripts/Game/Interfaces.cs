using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary> Interface container script </summary>

/// <summary> For all objects that have health and can take damage </summary>
public interface IDamageable {
	Renderer GetRenderer { get; }
	Transform GetTransform { get; }
	Vector2 Position { get; }
	float Health { get; }
	void AddInDamageableObjects();
	void SetDamage (float damage);
}

/// <summary> For switch objects </summary>
public interface ISwitch {
	/// <summary> OnSwitch </summary>
	/// <param name="go"> The object that triggered the switch </param>
	/// /// <param name="value"> On/Off switch </param>
	void OnSwitch(GameObject go, bool value = true);
}

