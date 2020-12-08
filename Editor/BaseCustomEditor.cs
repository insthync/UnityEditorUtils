using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public abstract class BaseCustomEditor : Editor
{
    protected readonly List<string> hiddenFields = new List<string>();
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
    /// <param name='conditionMemberName'>
    /// The name of the Enum field. (in your case that is "type")
    /// </param>
    /// <param name='conditionValue'>
    /// When the Enum value is this in the editor, the field is visible.
    /// </param>
    /// <param name='showingFieldName'>
    /// The Field name that should only be visible when the chosen enum value is set.
    /// </param>
    protected void ShowOnEnum(string conditionMemberName, string conditionValue, string showingFieldName)
    {
        EnumFieldCondition newFieldCondition = new EnumFieldCondition()
        {
            conditionMemberName = conditionMemberName,
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
    /// <param name='conditionMemberName'>
    /// The name of the Bool field.
    /// </param>
    /// <param name='conditionValue'>
    /// When the Bool value is this in the editor, the field is visible.
    /// </param>
    /// <param name='showingFieldName'>
    /// The Field name that should only be visible when the chosen bool value is set.
    /// </param>
    protected void ShowOnBool(string conditionMemberName, bool conditionValue, string showingFieldName)
    {
        BoolFieldCondition newFieldCondition = new BoolFieldCondition()
        {
            conditionMemberName = conditionMemberName,
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
    /// <param name='conditionMemberName'>
    /// The name of the Int field.
    /// </param>
    /// <param name='conditionValue'>
    /// When the Int value is this in the editor, the field is visible.
    /// </param>
    /// <param name='showingFieldName'>
    /// The Field name that should only be visible when the chosen int value is set.
    /// </param>
    protected void ShowOnInt(string conditionMemberName, int conditionValue, string showingFieldName)
    {
        IntFieldCondition newFieldCondition = new IntFieldCondition()
        {
            conditionMemberName = conditionMemberName,
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
                if (!hasFieldCondition && !hiddenFields.Contains(obj.name))
                    shouldBeVisible = true;
                if (shouldBeVisible)
                    EditorGUILayout.PropertyField(obj, true);
            } while (obj.NextVisible(false));
        }
    }

    private class FieldCondition
    {
        public string conditionMemberName;
        public string showingFieldName;
        public bool isValid;
        public string errorMsg;

        public new string ToString()
        {
            return "'" + conditionMemberName + "', '" + showingFieldName + "'.";
        }

        public bool Validate(Object target, string scriptName = "")
        {
            MemberInfo conditionMember;
            FieldInfo showingField;
            return Validate(target, out conditionMember, out showingField, scriptName);
        }

        public virtual bool Validate(Object target, out MemberInfo conditionMember, out FieldInfo showingField, string scriptName = "")
        {
            conditionMember = null;
            showingField = null;

            // Valildating the "conditionMemberName"
            conditionMember = target.GetType().GetField(conditionMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (conditionMember == null)
                conditionMember = target.GetType().GetProperty(conditionMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (conditionMember == null)
            {
                isValid = false;
                errorMsg = "Could not find a field named: '" + conditionMemberName + "' in '" + target + "'. Make sure you have spelled the field name for `conditionMemberName` correct in the script '" + scriptName + "'";
                return false;
            }

            // Valildating the "showingFieldName"
            if (isValid)
            {
                showingField = target.GetType().GetField(showingFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (showingField == null)
                {
                    isValid = false;
                    errorMsg = "Could not find a field named: '" + showingFieldName + "' in '" + target + "'. Make sure you have spelled the field name for `showingFieldName` correct in the script '" + scriptName + "'";
                    return false;
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
            return conditionMemberName.Equals(obj.name);
        }

        public virtual bool IsShowingField(Object target, SerializedProperty obj)
        {
            return showingFieldName.Equals(obj.name);
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
            return "'" + conditionMemberName + "', '" + conditionValue + "', '" + showingFieldName + "'.";
        }

        public override bool CheckShouldVisible(Object target, SerializedProperty obj)
        {
            if (base.CheckShouldVisible(target, obj))
            {
                MemberInfo conditionMember = target.GetType().GetField(conditionMemberName);
                if (conditionMember == null)
                    conditionMember = target.GetType().GetProperty(conditionMemberName);

                object currentConditionValue = null;

                if (conditionMember is FieldInfo)
                    currentConditionValue = (conditionMember as FieldInfo).GetValue(target);

                if (conditionMember is PropertyInfo)
                    currentConditionValue = (conditionMember as PropertyInfo).GetValue(target, null);

                // If the `conditionValue` value isn't equal to the wanted value the field will be set not to show
                return currentConditionValue.ToString().Equals(conditionValue.ToString());
            }
            return false;
        }
    }

    private class EnumFieldCondition : FieldCondition<string>
    {
        public override bool Validate(Object target, out MemberInfo conditionMember, out FieldInfo showingField, string scriptName = "")
        {
            if (base.Validate(target, out conditionMember, out showingField, scriptName))
            {
                // Valildating the "conditionValue"
                if (isValid)
                {
                    bool found = false;
                    object currentConditionValue = null;

                    if (conditionMember is FieldInfo)
                        currentConditionValue = (conditionMember as FieldInfo).GetValue(target);

                    if (conditionMember is PropertyInfo)
                        currentConditionValue = (conditionMember as PropertyInfo).GetValue(target, null);

                    if (currentConditionValue != null)
                    {
                        // finding enum value
                        FieldInfo[] enumNames = currentConditionValue.GetType().GetFields();
                        foreach (FieldInfo enumName in enumNames)
                        {
                            if (enumName.Name == conditionValue)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    // If cannot find enum value
                    if (!found)
                    {
                        isValid = false;
                        errorMsg = "Could not find the enum value: '" + conditionValue + "' in the enum '" + currentConditionValue.GetType().ToString() + "'. Make sure you have spelled the field name for `conditionValue` correct in the script '" + scriptName + "'";
                    }
                }
            }
            return false;
        }
    }

    private class BoolFieldCondition : FieldCondition<bool>
    {
    }

    private class IntFieldCondition : FieldCondition<int>
    {
    }
}
