using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IntShowConditionalAttribute : BaseShowConditionalAttribute
{
    public int conditionValue { get; private set; }
    public IntShowConditionalAttribute(string conditionFieldName, int conditionValue) : base(conditionFieldName)
    {
        this.conditionValue = conditionValue;
    }

#if UNITY_EDITOR
    public override bool GetShowResult(SerializedProperty sourcePropertyValue)
    {
        bool isShow = false;
        switch (sourcePropertyValue.propertyType)
        {
            case SerializedPropertyType.Integer:
                isShow = sourcePropertyValue.intValue == conditionValue;
                break;
        }
        return isShow;
    }
#endif
}
