// These codes are come from: https://raw.githubusercontent.com/zaikman/UnityPublic/master/InspectorButton.cs
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

namespace Insthync.UnityEditorUtils.Editor
{
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo = null;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
            Rect buttonRect = new Rect(position.x, position.y, position.width, position.height);
            string labelText = label.text;
            if (!string.IsNullOrWhiteSpace(inspectorButtonAttribute.labelText))
                labelText = inspectorButtonAttribute.labelText;
            if (GUI.Button(buttonRect, labelText))
            {
                Type eventOwnerType = prop.serializedObject.targetObject.GetType();
                string eventName = inspectorButtonAttribute.methodName;

                do
                {
                    _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    eventOwnerType = eventOwnerType.BaseType;
                } while (_eventMethodInfo == null);

                if (_eventMethodInfo != null)
                {
                    _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
                }
                else
                {
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
                }
            }
        }
    }
}
