using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnityHelpBox))]
public class UnityHelpBoxPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        UnityHelpBox target = fieldInfo.GetValue(property.serializedObject.targetObject) as UnityHelpBox;
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.HelpBox(position, target.text, (MessageType)target.type);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        UnityHelpBox target = fieldInfo.GetValue(property.serializedObject.targetObject) as UnityHelpBox;
        return target.height;
    }
}