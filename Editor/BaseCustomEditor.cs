using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CanEditMultipleObjects]
public abstract class BaseCustomEditor : Editor
{
    /// <summary>
    /// Add the ShowOnEnum methods in here
    /// </summary>
    protected abstract void SetFieldCondition();

    /////////////////////////////////////////////////////////
    /// DO NOT TOUCH THE REST
    /// If you make changes, it is at your own risk.
    /// ShowOnEnum() - Made by JWolf 13 / 6 - 2012
    /// Edited by Insthync 23 / 4 - 2018
    /////////////////////////////////////////////////////////
    
    /// <summary>
    /// Use this function to set when witch fields should be visible.
    /// </summary>
    /// <param name='enumFieldName'>
    /// The name of the Enum field. (in your case that is "type")
    /// </param>
    /// <param name='enumValue'>
    /// When the Enum value is this in the editor, the field is visible.
    /// </param>
    /// <param name='fieldName'>
    /// The Field name that should only be visible when the chosen enum value is set.
    /// </param>
    protected void ShowOnEnum(string enumFieldName, string enumValue, string fieldName)
    {
        FieldCondition newFieldCondition = new FieldCondition()
        {
            enumFieldName = enumFieldName,
            enumValue = enumValue,
            fieldName = fieldName,
            isValid = true
        };
        
        // Valildating the "enumFieldName"
        newFieldCondition.errorMsg = "";
        FieldInfo enumField = target.GetType().GetField(newFieldCondition.enumFieldName);
        if (enumField == null)
        {
            newFieldCondition.isValid = false;
            newFieldCondition.errorMsg = "Could not find a enum-field named: '" + enumFieldName + "' in '" + target + "'. Make sure you have spelled the field name for the enum correct in the script '" + this.ToString() + "'";
        }

        // Valildating the "enumValue"
        if (newFieldCondition.isValid)
        {
            var currentEnumValue = enumField.GetValue(target);
            var enumNames = currentEnumValue.GetType().GetFields();
            //var enumNames = currentEnumValue.GetType().GetEnumNames();
            bool found = false;
            foreach (FieldInfo enumName in enumNames)
            {
                if (enumName.Name == enumValue)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                newFieldCondition.isValid = false;
                newFieldCondition.errorMsg = "Could not find the enum value: '" + enumValue + "' in the enum '" + currentEnumValue.GetType().ToString() + "'. Make sure you have spelled the value name correct in the script '" + this.ToString() + "'";
            }
        }

        // Valildating the "fieldName"
        if (newFieldCondition.isValid)
        {
            FieldInfo fieldWithCondition = target.GetType().GetField(fieldName);
            if (fieldWithCondition == null)
            {
                newFieldCondition.isValid = false;
                newFieldCondition.errorMsg = "Could not find the field: '" + fieldName + "' in '" + target + "'. Make sure you have spelled the field name correct in the script '" + this.ToString() + "'";
            }
        }

        if (!newFieldCondition.isValid)
        {
            newFieldCondition.errorMsg += "\nYour error is within the Custom Editor Script to show/hide fields in the inspector depending on the an Enum." +
                    "\n\n" + ToString() + ": " + newFieldCondition.ToString() + "\n";
        }

        fieldConditions.Add(newFieldCondition);
    }
    
    private List<FieldCondition> fieldConditions;
    protected virtual void OnEnable()
    {
        fieldConditions = new List<FieldCondition>();
        SetFieldCondition();
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        var obj = serializedObject.GetIterator();
        if (obj.NextVisible(true))
        {
            // Loops through all visiuble fields
            do
            {
                bool hasFieldCondition = false;
                bool shouldBeVisible = false;
                // Tests if the field is a field that should be hidden/shown due to the enum value
                foreach (var fieldCondition in fieldConditions)
                {
                    // If the fieldCondition isn't valid, display an error msg.
                    if (!fieldCondition.isValid)
                    {
                        Debug.LogError(fieldCondition.errorMsg);
                    }
                    else if (fieldCondition.fieldName == obj.name)
                    {
                        hasFieldCondition = true;
                        FieldInfo enumField = target.GetType().GetField(fieldCondition.enumFieldName);
                        var currentEnumValue = enumField.GetValue(target);
                        // If the enum value isn't equal to the wanted value the field will be set not to show
                        if (currentEnumValue.ToString() == fieldCondition.enumValue)
                        {
                            shouldBeVisible = true;
                            break;
                        }
                    }
                }
                // If there are no an conditions for this field, show it
                if (!hasFieldCondition)
                    shouldBeVisible = true;
                if (shouldBeVisible)
                    EditorGUILayout.PropertyField(obj, true);
            } while (obj.NextVisible(false));
        }
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

    private class FieldCondition
    {
        public string enumFieldName { get; set; }
        public string enumValue { get; set; }
        public string fieldName { get; set; }
        public bool isValid { get; set; }
        public string errorMsg { get; set; }
        
        public new string ToString()
        {
            return "'" + enumFieldName + "', '" + enumValue + "', '" + fieldName + "'.";
        }
    }
}
