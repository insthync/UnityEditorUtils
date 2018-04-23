using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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
        newFieldCondition.Validate(target, ToString());
        fieldConditions.Add(newFieldCondition);
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
        newFieldCondition.Validate(target, ToString());
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
        
        SetPropertyFieldVisibilities(serializedObject.GetIterator());
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

    private void SetPropertyFieldVisibilities(SerializedProperty obj)
    {
        if (obj.NextVisible(true))
        {
            // Loops through all visiuble fields
            do
            {
                bool hasFieldCondition = false;
                bool shouldBeVisible = false;
                // Enum field conditions
                foreach (var fieldCondition in fieldConditions)
                {
                    // If the fieldCondition isn't valid, display an error msg.
                    if (!fieldCondition.isValid)
                    {
                        Debug.LogError(fieldCondition.errorMsg);
                        continue;
                    }
                    if (fieldCondition.IsShowingField(target, obj))
                    {
                        hasFieldCondition = true;
                        if (fieldCondition.CheckShouldVisible(target, obj))
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
    }

    private class FieldCondition
    {
        public string conditionFieldName;
        public string showingFieldName;
        public bool isValid;
        public string errorMsg;

        public new string ToString()
        {
            return "'" + conditionFieldName + "', '" + showingFieldName + "'.";
        }

        public bool Validate(Object target, string scriptName = "")
        {
            FieldInfo conditionField;
            FieldInfo showingField;
            return Validate(target, out conditionField, out showingField, scriptName);
        }

        public virtual bool Validate(Object target, out FieldInfo conditionField, out FieldInfo showingField, string scriptName = "")
        {
            conditionField = null;
            showingField = null;

            System.Type currentNestLevel;
            string[] nestedFieldNames;
            // Valildating the "conditionFieldName"
            currentNestLevel = target.GetType();
            nestedFieldNames = conditionFieldName.Split('/');
            for (var i = 0; i < nestedFieldNames.Length; ++i)
            {
                var currentField = currentNestLevel.GetField(nestedFieldNames[i]);
                if (currentField == null)
                {
                    isValid = false;
                    errorMsg = "Could not find a field named: '" + conditionFieldName + "' in '" + target + "'. Make sure you have spelled the field name for `conditionFieldName` correct in the script '" + scriptName + "'";
                    return false;
                }
                if (i >= nestedFieldNames.Length - 1)
                {
                    conditionField = currentField;
                    break;
                }
                currentNestLevel = currentField.FieldType;
            }

            // Valildating the "showingFieldName"
            if (isValid)
            {
                currentNestLevel = target.GetType();
                nestedFieldNames = showingFieldName.Split('/');
                for (var i = 0; i < nestedFieldNames.Length; ++i)
                {
                    var currentField = currentNestLevel.GetField(nestedFieldNames[i]);
                    if (currentField == null)
                    {
                        isValid = false;
                        errorMsg = "Could not find a field named: '" + showingFieldName + "' in '" + target + "'. Make sure you have spelled the field name for `showingFieldName` correct in the script '" + scriptName + "'";
                        return false;
                    }
                    if (i >= nestedFieldNames.Length - 1)
                    {
                        showingField = currentField;
                        break;
                    }
                    currentNestLevel = currentField.FieldType;
                }
            }

            if (!isValid)
            {
                errorMsg += "\nYour error is within the Custom Editor Script to show/hide fields in the inspector depending on the an values." +
                        "\n\n" + scriptName + ": " + ToString() + "\n";
            }
            return true;
        }

        public virtual bool IsConditionField(Object target, SerializedProperty obj)
        {
            string[] nestedFieldNames;
            nestedFieldNames = conditionFieldName.Split('/');
            return nestedFieldNames[nestedFieldNames.Length - 1].Equals(obj.name);
        }

        public virtual bool IsShowingField(Object target, SerializedProperty obj)
        {
            string[] nestedFieldNames;
            nestedFieldNames = showingFieldName.Split('/');
            return nestedFieldNames[nestedFieldNames.Length - 1].Equals(obj.name);
        }

        public virtual bool CheckShouldVisible(Object target, SerializedProperty obj)
        {
            return isValid && IsShowingField(target, obj);
        }
    }

    private class FieldCondition<T> : FieldCondition
    {
        public T conditionValue;
        public new string ToString()
        {
            return "'" + conditionFieldName + "', '" + conditionValue + "', '" + showingFieldName + "'.";
        }

        public override bool CheckShouldVisible(Object target, SerializedProperty obj)
        {
            if (base.CheckShouldVisible(target, obj))
            {
                var currentNestLevel = target.GetType();
                object currentNestValue = target;
                var nestedFieldNames = conditionFieldName.Split('/');
                for (var i = 0; i < nestedFieldNames.Length; ++i)
                {
                    var currentField = currentNestLevel.GetField(nestedFieldNames[i]);
                    if (currentField == null)
                        return false;
                    if (i >= nestedFieldNames.Length - 1)
                    {
                        var currentConditionValue = currentField.GetValue(currentNestValue);
                        // If the `conditionValue` value isn't equal to the wanted value the field will be set not to show
                        return currentConditionValue.ToString().Equals(conditionValue.ToString());
                    }
                    currentNestLevel = currentField.FieldType;
                    currentNestValue = currentField.GetValue(currentNestValue);
                }
            }
            return false;
        }
    }

    private class EnumFieldCondition : FieldCondition<string>
    {
        public override bool Validate(Object target, out FieldInfo conditionField, out FieldInfo showingField, string scriptName = "")
        {
            if (base.Validate(target, out conditionField, out showingField, scriptName))
            {
                // Valildating the "conditionValue"
                if (isValid)
                {
                    var currentNestLevel = target.GetType();
                    object currentNestValue = target;
                    var nestedFieldNames = conditionFieldName.Split('/');
                    for (var i = 0; i < nestedFieldNames.Length; ++i)
                    {
                        var currentField = currentNestLevel.GetField(nestedFieldNames[i]);
                        if (currentField == null)
                            return false;
                        if (i >= nestedFieldNames.Length - 1)
                        {
                            bool found = false;
                            var currentConditionValue = currentField.GetValue(currentNestValue);
                            // finding enum value
                            var enumNames = currentConditionValue.GetType().GetFields();
                            foreach (FieldInfo enumName in enumNames)
                            {
                                if (enumName.Name == conditionValue)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            // If cannot find enum value
                            if (!found)
                            {
                                isValid = false;
                                errorMsg = "Could not find the enum value: '" + conditionValue + "' in the enum '" + currentConditionValue.GetType().ToString() + "'. Make sure you have spelled the field name for `conditionValue` correct in the script '" + scriptName + "'";
                            }
                            break;
                        }
                        currentNestLevel = currentField.FieldType;
                        currentNestValue = currentField.GetValue(currentNestValue);
                    }
                }
            }
            return false;
        }
    }

    private class BoolFieldCondition : FieldCondition<bool>
    {
    }
}
