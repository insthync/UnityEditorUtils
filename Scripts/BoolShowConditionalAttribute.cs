using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolShowConditionalAttribute : PropertyAttribute
{
    public string conditionFieldName { get; private set; }
    public bool conditionValue { get; private set; }
    public BoolShowConditionalAttribute(string conditionFieldName, bool conditionValue)
    {
        this.conditionFieldName = conditionFieldName;
        this.conditionValue = conditionValue;
    }
}
