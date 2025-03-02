﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace Insthync.UnityEditorUtils.Editor
{
    [CustomPropertyDrawer(typeof(UnityScene))]
    public class UnityScenePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
            SerializedProperty sceneName = property.FindPropertyRelative("sceneName");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null)
            {
                EditorGUI.BeginChangeCheck();

                Object value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    sceneAsset.objectReferenceValue = value;
                    if (sceneAsset.objectReferenceValue != null)
                    {
                        var _sceneName = (sceneAsset.objectReferenceValue as SceneAsset).name;
                        sceneName.stringValue = _sceneName;
                        var sceneObj = GetSceneObject(_sceneName);
                        if (sceneObj == null)
                        {
                            // Just warning, do not change value to null
                            Debug.LogWarning("The scene [" + _sceneName + "] cannot be used. To use this scene add it to the build settings for the project");
                        }
                    }
                    else
                        sceneName.stringValue = null;
                }
            }
            EditorGUI.EndProperty();
        }

        protected SceneAsset GetSceneObject(string sceneObjectName)
        {
            if (string.IsNullOrEmpty(sceneObjectName))
            {
                return null;
            }

            foreach (var editorScene in EditorBuildSettings.scenes)
            {
                var sceneNameWithoutExtension = Path.GetFileNameWithoutExtension(editorScene.path);
                if (sceneNameWithoutExtension == sceneObjectName)
                {
                    return AssetDatabase.LoadAssetAtPath(editorScene.path, typeof(SceneAsset)) as SceneAsset;
                }
            }
            return null;
        }
    }
}
