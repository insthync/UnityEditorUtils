using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StringShowConditionalAttribute : BaseShowConditionalAttribute
{
    public string conditionValue { get; private set; }
    public StringShowConditionalAttribute(string conditionFieldName, string conditionValue) : base(conditionFieldName)
    {
        this.conditionValue = conditionValue;
    }

#if UNITY_EDITOR
    public override bool GetShowResult(SerializedProperty sourcePropertyValue)
    {
        bool isShow = false;
        switch (sourcePropertyValue.propertyType)
        {
            case SerializedPropertyType.Enum:
                isShow = sourcePropertyValue.enumNames[sourcePropertyValue.enumValueIndex].Equals(conditionValue);
                break;
            case SerializedPropertyType.String:
                isShow = sourcePropertyValue.stringValue.Equals(conditionValue);
                break;
        }
        return isShow;
    }
#endif
}
