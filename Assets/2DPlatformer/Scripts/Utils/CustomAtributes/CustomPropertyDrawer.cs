using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary> To hide unnecessary fields from the inspector </summary>
[CustomPropertyDrawer(typeof(ShowInInspectorCondition))]
public class ShowInInspectorIfAttributeDrawer : PropertyDrawer
{

	private ShowInInspectorCondition Attribute { get { return _attribute ?? (_attribute = attribute as ShowInInspectorCondition); } }
	private string PropertyToCheck { get { return Attribute != null ? Attribute.PropertyToCheck : null; } }
	private bool ShowIfTrue { get { return Attribute != null ? Attribute.ShowIfTrue : true; } }

	private ShowInInspectorCondition _attribute;
	private bool toShow = true;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return toShow ? EditorGUI.GetPropertyHeight(property) : 0;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (ShowIfTrue) {
			toShow = true;
			if (!string.IsNullOrEmpty(PropertyToCheck)) {
				if (!(CheckProperty(property, PropertyToCheck))) {
					toShow = false;
				}
			}
		} else {
			toShow = false;
			if (!string.IsNullOrEmpty(PropertyToCheck)) {
				if (!(CheckProperty(property, PropertyToCheck))) {
					toShow = true;
				}
			}
		}
		if (toShow) {
			EditorGUI.PropertyField(position, property, label, true);
		}
	}

	private bool CheckProperty(SerializedProperty property, string toGet)
	{
		SerializedProperty parent = property;
		if (property.depth != 0) {
			var path = property.propertyPath.Replace(".Array.data[", "[");
			var elements = path.Split('.');

			for (int i = 0; i < elements.Length - 1; i++) {
				var element = elements[i];
				int index = -1;
				if (element.Contains("[")) {
					index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
					element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
				}

				parent = i == 0 ?
					property.serializedObject.FindProperty(element) :
					parent.FindPropertyRelative(element);

				if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
			}
		}

		var obj = property.serializedObject.targetObject;
		var propertyInfo = obj.GetType().GetProperty(toGet);
		if (propertyInfo != null) {
			var propertyValue = propertyInfo.GetValue(obj, null);
			if (propertyValue is bool) {
				return (bool)propertyValue;
			}
		}
		Debug.LogError("Property not found " + toGet);
		return false;
	}
}
#endif