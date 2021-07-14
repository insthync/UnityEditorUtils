// These codes are come from: https://raw.githubusercontent.com/zaikman/UnityPublic/master/InspectorButton.cs

using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonPropertyDrawer : PropertyDrawer
{
    private MethodInfo eventMethodInfo = null;

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
        Rect buttonRect = new Rect(position.x, position.y, position.width, position.height);
        if (GUI.Button(buttonRect, label.text))
        {
            Type eventOwnerType = prop.serializedObject.targetObject.GetType();
            string eventName = inspectorButtonAttribute.MethodName;

            do
            {
                eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                eventOwnerType = eventOwnerType.BaseType;
            } while (eventMethodInfo == null);

            if (eventMethodInfo != null)
                eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
            else
                Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
        }
    }
}
