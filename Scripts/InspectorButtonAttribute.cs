﻿// These codes are come from: https://raw.githubusercontent.com/zaikman/UnityPublic/master/InspectorButton.cs
using UnityEngine;

namespace Insthync.UnityEditorUtils
{
    /// <summary>
    /// This attribute can only be applied to fields because its
    /// associated PropertyDrawer only operates on fields (either
    /// public or tagged with the [SerializeField] attribute) in
    /// the target MonoBehaviour.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly string methodName;
        public readonly string labelText;

        public InspectorButtonAttribute(string methodName, string labelText = "")
        {
            this.methodName = methodName;
            this.labelText = labelText;
        }
    }
}
