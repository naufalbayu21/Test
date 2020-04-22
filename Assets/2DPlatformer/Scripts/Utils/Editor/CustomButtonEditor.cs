using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(CustomButton))]
[CanEditMultipleObjects]
public class CustomButtonEditor : UnityEditor.UI.ButtonEditor {

	SerializedProperty soundTypeProperty;
	SerializedProperty onClickSoundProperty;
	SerializedProperty onHighlightSoundProperty;

	protected override void OnEnable () {
		base.OnEnable();
		soundTypeProperty = serializedObject.FindProperty("SoundType");
		onClickSoundProperty = serializedObject.FindProperty("OnClickSound");
		onHighlightSoundProperty = serializedObject.FindProperty("OnHighlightSound");
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.PropertyField(soundTypeProperty);

		if ((SoundType)(soundTypeProperty.enumValueIndex) == SoundType.Custom) {
			EditorGUILayout.PropertyField(onClickSoundProperty, includeChildren: true);
			EditorGUILayout.PropertyField(onHighlightSoundProperty, includeChildren: true);
		}

		if (EditorGUI.EndChangeCheck()) {
			serializedObject.ApplyModifiedProperties();
		}

	}
}