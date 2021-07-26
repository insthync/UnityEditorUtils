using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public abstract class BaseCustomEditor : Editor
{
    protected readonly List<string> hiddenFields = new List<string>();

    /////////////////////////////////////////////////////////
    /// ShowOnEnum() - Made by JWolf
    /// Modified by Insthync
    /////////////////////////////////////////////////////////

    /// <summary>
    /// Dictionary<ShowingFieldName(string), List<FieldCondition>>
    /// </summary>
    private Dictionary<string, List<FieldCondition>> fieldConditions;

    protected virtual void OnEnable()
    {
        fieldConditions = new Dictionary<string, List<FieldCondition>>();
        SetFieldCondition();
    }

    /// <summary>
    /// Implement fields condition here
    /// </summary>
    protected virtual void SetFieldCondition()
    {

    }

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
        EnumFieldCondition newFieldCondition = new EnumFieldCondition(conditionMemberName, conditionValue, showingFieldName);
        if (!newFieldCondition.Validate(target, ToString()))
        {
            Debug.LogError(newFieldCondition.error);
            return;
        }
        AddFieldCondition(showingFieldName, newFieldCondition);
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
        BoolFieldCondition newFieldCondition = new BoolFieldCondition(conditionMemberName, conditionValue, showingFieldName);
        if (!newFieldCondition.Validate(target, ToString()))
        {
            Debug.LogError(newFieldCondition.error);
            return;
        }
        AddFieldCondition(showingFieldName, newFieldCondition);
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
        IntFieldCondition newFieldCondition = new IntFieldCondition(conditionMemberName, conditionValue, showingFieldName);
        if (!newFieldCondition.Validate(target, ToString()))
        {
            Debug.LogError(newFieldCondition.error);
            return;
        }
        AddFieldCondition(showingFieldName, newFieldCondition);
    }

    private void AddFieldCondition(string showingFieldName, FieldCondition condition)
    {
        if (!fieldConditions.ContainsKey(showingFieldName))
            fieldConditions.Add(showingFieldName, new List<FieldCondition>());
        fieldConditions[showingFieldName].Add(condition);
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();
        // 
        FieldsLookup(serializedObject.GetIterator());
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void FieldsLookup(SerializedProperty obj)
    {
        if (obj.NextVisible(true))
        {
            // Loops through all visiuble fields
            do
            {
                RenderField(obj);
            } while (obj.NextVisible(false));
        }
    }

    protected virtual void RenderField(SerializedProperty obj)
    {
        if (hiddenFields.Contains(obj.name))
        {
            // The field is being hidden
            return;
        }
        if (fieldConditions.ContainsKey(obj.name))
        {
            foreach (var condition in fieldConditions[obj.name])
            {
                if (!condition.ShouldVisible(target, obj))
                {
                    // The field should not visible
                    return;
                }
            }
        }
        EditorGUILayout.PropertyField(obj, true);
    }

    protected class FieldCondition
    {
        public string conditionMemberName { get; protected set; }
        public string showingFieldName { get; protected set; }
        public string error { get; protected set; }

        public FieldCondition(string conditionMemberName, string showingFieldName)
        {
            this.conditionMemberName = conditionMemberName;
            this.showingFieldName = showingFieldName;
            error = $"Field condition is not validated yet: '{conditionMemberName}', '{showingFieldName}'";
        }

        public new string ToString()
        {
            return $"'{conditionMemberName}', '{showingFieldName}'.";
        }

        public bool Validate(Object target, string scriptName = "")
        {
            return Validate(target, out _, out _, scriptName);
        }

        public virtual bool Validate(Object target, out MemberInfo conditionMember, out FieldInfo showingField, string scriptName = "")
        {
            showingField = null;

            // Valildating the "conditionMemberName"
            conditionMember = target.GetType().GetField(conditionMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (conditionMember == null)
                conditionMember = target.GetType().GetProperty(conditionMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (conditionMember == null)
            {
                error = $"Could not find a field named: '{conditionMemberName}' in '{target}'. Make sure you have spelled the field name for `conditionMemberName` correct in the script '{scriptName}', '{ToString()}'";
                return false;
            }

            // Valildating the "showingFieldName"
            showingField = target.GetType().GetField(showingFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (showingField == null)
            {
                error = $"Could not find a field named: '{showingFieldName}' in '{target}'. Make sure you have spelled the field name for `showingFieldName` correct in the script '{scriptName}', '{ToString()}'";
                return false;
            }

            error = string.Empty;
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

        public virtual bool ShouldVisible(Object target, SerializedProperty obj)
        {
            return IsShowingField(target, obj);
        }
    }

    protected class FieldCondition<T> : FieldCondition
    {
        public T conditionValue { get; protected set; }

        public FieldCondition(string conditionMemberName, T conditionValue, string showingFieldName) : base(conditionMemberName, showingFieldName)
        {
            this.conditionValue = conditionValue;
            error = $"Field condition is not validated yet: '{conditionMemberName}', '{conditionValue}', '{showingFieldName}'";
        }

        public new string ToString()
        {
            return $"'{conditionMemberName}', '{conditionValue}', '{showingFieldName}'.";
        }

        public override bool ShouldVisible(Object target, SerializedProperty obj)
        {
            if (base.ShouldVisible(target, obj))
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

    protected class EnumFieldCondition : FieldCondition<string>
    {
        public EnumFieldCondition(string conditionMemberName, string conditionValue, string showingFieldName) : base(conditionMemberName, conditionValue, showingFieldName) { }

        public override bool Validate(Object target, out MemberInfo conditionMember, out FieldInfo showingField, string scriptName = "")
        {
            if (!base.Validate(target, out conditionMember, out showingField, scriptName))
                return false;

            // Valildating the "conditionValue"
            bool found = false;
            object currentConditionValue = null;

            if (conditionMember is FieldInfo)
                currentConditionValue = (conditionMember as FieldInfo).GetValue(target);

            if (conditionMember is PropertyInfo)
                currentConditionValue = (conditionMember as PropertyInfo).GetValue(target, null);

            if (currentConditionValue != null)
            {
                // Finding enum value
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
                error = $"Could not find the enum value: '{conditionValue}' in the enum '{currentConditionValue.GetType().ToString()}'. Make sure you have spelled the field name for `conditionValue` correct in the script '{scriptName}', '{ToString()}'";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }

    protected class BoolFieldCondition : FieldCondition<bool>
    {
        public BoolFieldCondition(string conditionMemberName, bool conditionValue, string showingFieldName) : base(conditionMemberName, conditionValue, showingFieldName) { }
    }

    protected class IntFieldCondition : FieldCondition<int>
    {
        public IntFieldCondition(string conditionMemberName, int conditionValue, string showingFieldName) : base(conditionMemberName, conditionValue, showingFieldName) { }
    }
}
