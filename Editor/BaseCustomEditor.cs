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
    /// <param name='conditionFieldName'>
    /// The name of the Enum field. (in your case that is "type")
    /// </param>
    /// <param name='conditionValue'>
    /// When the Enum value is this in the editor, the field is visible.
    /// </param>
    /// <param name='showingFieldName'>
    /// The Field name that should only be visible when the chosen enum value is set.
    /// </param>
    protected void ShowOnEnum(string conditionFieldName, string conditionValue, string showingFieldName)
    {
        EnumFieldCondition newFieldCondition = new EnumFieldCondition()
        {
            conditionFieldName = conditionFieldName,
            conditionValue = conditionValue,
            showingFieldName = showingFieldName,
            isValid = true
        };

        // Valildating the "conditionFieldName"
        newFieldCondition.errorMsg = "";
        FieldInfo enumField = target.GetType().GetField(newFieldCondition.conditionFieldName);
        if (enumField == null)
        {
            newFieldCondition.isValid = false;
            newFieldCondition.errorMsg = "Could not find a enum-field named: '" + conditionFieldName + "' in '" + target + "'. Make sure you have spelled the field name for the enum correct in the script '" + ToString() + "'";
        }

        // Valildating the "conditionValue"
        if (newFieldCondition.isValid)
        {
            var currentEnumValue = enumField.GetValue(target);
            var enumNames = currentEnumValue.GetType().GetFields();
            //var enumNames = currentEnumValue.GetType().GetEnumNames();
            bool found = false;
            foreach (FieldInfo enumName in enumNames)
            {
                if (enumName.Name == conditionValue)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                newFieldCondition.isValid = false;
                newFieldCondition.errorMsg = "Could not find the enum value: '" + conditionValue + "' in the enum '" + currentEnumValue.GetType().ToString() + "'. Make sure you have spelled the value name correct in the script '" + ToString() + "'";
            }
        }

        // Valildating the "showingFieldName"
        if (newFieldCondition.isValid)
        {
            FieldInfo fieldWithCondition = target.GetType().GetField(showingFieldName);
            if (fieldWithCondition == null)
            {
                newFieldCondition.isValid = false;
                newFieldCondition.errorMsg = "Could not find the field: '" + showingFieldName + "' in '" + target + "'. Make sure you have spelled the field name correct in the script '" + ToString() + "'";
            }
        }

        if (!newFieldCondition.isValid)
        {
            newFieldCondition.errorMsg += "\nYour error is within the Custom Editor Script to show/hide fields in the inspector depending on the an Enum." +
                    "\n\n" + ToString() + ": " + newFieldCondition.ToString() + "\n";
        }

        enumFieldConditions.Add(newFieldCondition);
    }

    /// <summary>
    /// Use this function to set when witch fields should be visible.
    /// </summary>
    /// <param name='conditionFieldName'>
    /// The name of the Bool field.
    /// </param>
    /// <param name='conditionValue'>
    /// When the Bool value is this in the editor, the field is visible.
    /// </param>
    /// <param name='showingFieldName'>
    /// The Field name that should only be visible when the chosen bool value is set.
    /// </param>
    protected void ShowOnBool(string conditionFieldName, bool conditionValue, string showingFieldName)
    {
        BoolFieldCondition newFieldCondition = new BoolFieldCondition()
        {
            conditionFieldName = conditionFieldName,
            conditionValue = conditionValue,
            showingFieldName = showingFieldName,
            isValid = true
        };

        // Valildating the "conditionFieldName"
        newFieldCondition.errorMsg = "";
        FieldInfo boolField = target.GetType().GetField(newFieldCondition.conditionFieldName);
        if (boolField == null)
        {
            newFieldCondition.isValid = false;
            newFieldCondition.errorMsg = "Could not find a bool-field named: '" + conditionFieldName + "' in '" + target + "'. Make sure you have spelled the field name for the bool correct in the script '" + ToString() + "'";
        }

        // Valildating the "showingFieldName"
        if (newFieldCondition.isValid)
        {
            FieldInfo fieldWithCondition = target.GetType().GetField(showingFieldName);
            if (fieldWithCondition == null)
            {
                newFieldCondition.isValid = false;
                newFieldCondition.errorMsg = "Could not find the field: '" + showingFieldName + "' in '" + target + "'. Make sure you have spelled the field name correct in the script '" + ToString() + "'";
            }
        }

        if (!newFieldCondition.isValid)
        {
            newFieldCondition.errorMsg += "\nYour error is within the Custom Editor Script to show/hide fields in the inspector depending on the an Bool." +
                    "\n\n" + ToString() + ": " + newFieldCondition.ToString() + "\n";
        }

        boolFieldConditions.Add(newFieldCondition);
    }
    
    private List<EnumFieldCondition> enumFieldConditions;
    private List<BoolFieldCondition> boolFieldConditions;
    protected virtual void OnEnable()
    {
        enumFieldConditions = new List<EnumFieldCondition>();
        boolFieldConditions = new List<BoolFieldCondition>();
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
                // Enum field conditions
                foreach (var fieldCondition in enumFieldConditions)
                {
                    // If the fieldCondition isn't valid, display an error msg.
                    if (!fieldCondition.isValid)
                    {
                        Debug.LogError(fieldCondition.errorMsg);
                        continue;
                    }
                    if (fieldCondition.showingFieldName == obj.name)
                    {
                        hasFieldCondition = true;
                        FieldInfo enumField = target.GetType().GetField(fieldCondition.conditionFieldName);
                        var currentEnumValue = enumField.GetValue(target);
                        // If the enum value isn't equal to the wanted value the field will be set not to show
                        if (currentEnumValue.ToString() == fieldCondition.conditionValue)
                        {
                            shouldBeVisible = true;
                            break;
                        }
                    }
                }
                // Bool field conditions
                foreach (var fieldCondition in boolFieldConditions)
                {
                    // If the fieldCondition isn't valid, display an error msg.
                    if (!fieldCondition.isValid)
                    {
                        Debug.LogError(fieldCondition.errorMsg);
                        continue;
                    }
                    if (fieldCondition.showingFieldName == obj.name)
                    {
                        hasFieldCondition = true;
                        FieldInfo boolField = target.GetType().GetField(fieldCondition.conditionFieldName);
                        var currentBoolValue = (bool)boolField.GetValue(target);
                        // If the enum value isn't equal to the wanted value the field will be set not to show
                        if (currentBoolValue == fieldCondition.conditionValue)
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

    private class EnumFieldCondition
    {
        public string conditionFieldName;
        public string conditionValue;
        public string showingFieldName;
        public bool isValid;
        public string errorMsg;
        
        public new string ToString()
        {
            return "'" + conditionFieldName + "', '" + conditionValue + "', '" + showingFieldName + "'.";
        }
    }

    private class BoolFieldCondition
    {
        public string conditionFieldName;
        public bool conditionValue;
        public string showingFieldName;
        public bool isValid;
        public string errorMsg;

        public new string ToString()
        {
            return "'" + conditionFieldName + "', '" + conditionValue + "', '" + showingFieldName + "'.";
        }
    }
}
