using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringShowConditionalAttribute : PropertyAttribute
{
    public string conditionFieldName { get; private set; }
    public string conditionValue { get; private set; }
    public StringShowConditionalAttribute(string conditionFieldName, string conditionValue)
    {
        this.conditionFieldName = conditionFieldName;
        this.conditionValue = conditionValue;
    }
}
