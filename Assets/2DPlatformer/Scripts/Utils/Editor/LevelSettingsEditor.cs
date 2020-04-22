using UnityEngine;
using UnityEditor;
using GameBalance;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LevelSettings), true)]
public class LevelSettingsEditor : Editor
{

	bool ListOfLevelsFoldout;
	List<bool> LevelsFoldOut = new List<bool>();

    public override void OnInspectorGUI()
    {
		EditorGUI.BeginChangeCheck();

		var mainWorldScene = serializedObject.FindProperty("mainWorldScene");
        var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(mainWorldScene.stringValue);
        
		serializedObject.Update();

        var newScene = EditorGUILayout.ObjectField("mainWorldScene", oldScene, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck()) {
            var newPath = AssetDatabase.GetAssetPath(newScene);
            mainWorldScene.stringValue = newPath;
        }

		serializedObject.ApplyModifiedProperties();

		EditorGUI.BeginChangeCheck();

		var mainMenuScene = serializedObject.FindProperty("mainMenuScene");
        oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(mainMenuScene.stringValue);
        
		serializedObject.Update();

        newScene = EditorGUILayout.ObjectField("mainMenuScene", oldScene, typeof(SceneAsset), false) as SceneAsset;

		if (EditorGUI.EndChangeCheck()) {
            var newPath = AssetDatabase.GetAssetPath(newScene);
            mainMenuScene.stringValue = newPath;
        }

		serializedObject.ApplyModifiedProperties();

		ListOfLevelsFoldout = EditorGUILayout.Foldout(ListOfLevelsFoldout, "Levels");

		if (ListOfLevelsFoldout) {

			var levels = serializedObject.FindProperty("levels");
			EditorGUI.indentLevel += 1;

			EditorGUI.BeginChangeCheck();
			var sizeProperty = levels.FindPropertyRelative("Array.size");
			EditorGUILayout.PropertyField(sizeProperty);

			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < levels.arraySize; i++) {

				if (i + 1 > LevelsFoldOut.Count) {
					LevelsFoldOut.Add(false);
				}

				var element = levels.GetArrayElementAtIndex(i);
				var levelPreset = B.Levels.Levels[i];

				EditorGUI.BeginChangeCheck();

				var scenePathProperty = element.FindPropertyRelative("levelScenePath");
				var sceneNameProperty = element.FindPropertyRelative("levelSceneName");

				LevelsFoldOut[i] = EditorGUILayout.Foldout(LevelsFoldOut[i], sceneNameProperty.stringValue);

				if (LevelsFoldOut[i]) {

					EditorGUI.indentLevel += 1;

					var rect = EditorGUILayout.BeginVertical();

					oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePathProperty.stringValue);

					serializedObject.Update();

					newScene = EditorGUILayout.ObjectField("levelScene", oldScene, typeof(SceneAsset), false) as SceneAsset;

					if (EditorGUI.EndChangeCheck()) {
						var newPath = AssetDatabase.GetAssetPath(newScene);
						var assetName = newScene.name;
						scenePathProperty.stringValue = newPath;
						sceneNameProperty.stringValue = assetName;
					}

					serializedObject.ApplyModifiedProperties();

					EditorGUI.BeginChangeCheck();

					EditorGUILayout.PropertyField(element.FindPropertyRelative("levelName"), true);

					var unlockCondition = element.FindPropertyRelative("unlockLevelCondition");
					EditorGUILayout.PropertyField(unlockCondition, true);
					if ((LevelSettings.LevelPreset.UnlockCondition)unlockCondition.enumValueIndex == LevelSettings.LevelPreset.UnlockCondition.ConcreteLevelCompleted) {
						EditorGUILayout.PropertyField(element.FindPropertyRelative("concreteLevelIndex"), true);
					}

					EditorGUILayout.PropertyField(element.FindPropertyRelative("backGroundForMainMenu"), true);
					EditorGUILayout.PropertyField(element.FindPropertyRelative("buttonLevelBackSprite"), true);
					EditorGUILayout.PropertyField(element.FindPropertyRelative("musicInLevel"), true);


					EditorGUILayout.EndVertical();

					EditorGUI.IndentedRect(rect);

					EditorGUILayout.Space();

					EditorGUI.indentLevel -= 1;
				}

				if (EditorGUI.EndChangeCheck()) {
					serializedObject.ApplyModifiedProperties();
				}
			}

			EditorGUI.indentLevel -= 1;
		}
    }
}
