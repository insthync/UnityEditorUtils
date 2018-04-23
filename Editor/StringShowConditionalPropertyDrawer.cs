using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StringShowConditionalAttribute))]
public class StringShowConditionalPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        StringShowConditionalAttribute condAttribute = (StringShowConditionalAttribute)attribute;
        bool isShow = GetHideResult(condAttribute, property);
        if (isShow)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            //The property is not being drawn
            //We want to undo the spacing added before and after the property
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        StringShowConditionalAttribute condAttribute = (StringShowConditionalAttribute)attribute;
        bool isShow = GetHideResult(condAttribute, property);
        if (isShow)
            EditorGUI.PropertyField(position, property, label, true);
    }

    private bool GetHideResult(StringShowConditionalAttribute attribute, SerializedProperty property)
    {
        bool isShow = false;
        var propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        var conditionPath = propertyPath.Replace(property.name, attribute.conditionFieldName);
        var sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        switch (sourcePropertyValue.propertyType)
        {
            case SerializedPropertyType.Enum:
                isShow = sourcePropertyValue.enumNames[sourcePropertyValue.enumValueIndex].Equals(attribute.conditionValue);
                break;
            case SerializedPropertyType.String:
                isShow = sourcePropertyValue.stringValue.Equals(attribute.conditionValue);
                break;
        }
        return isShow;
    }
}
