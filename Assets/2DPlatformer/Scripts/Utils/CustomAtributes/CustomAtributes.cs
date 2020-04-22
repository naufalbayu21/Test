using System;
using UnityEngine;

/// <summary> To hide unnecessary fields from the inspector </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ShowInInspectorCondition: PropertyAttribute {
	public string PropertyToCheck;

	public bool ShowIfTrue;
	 
	public ShowInInspectorCondition(string propertyToCheck, bool showIfTrue = true) {
		PropertyToCheck = propertyToCheck;
		ShowIfTrue = showIfTrue;
	}
}