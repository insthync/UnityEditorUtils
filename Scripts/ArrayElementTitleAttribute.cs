using UnityEngine;

public class ArrayElementTitleAttribute : PropertyAttribute
{
    public string variableName { get; private set; }
    public float[] nullColorValue { get; private set; }
    public float[] notNullColorValue { get; private set; }

    public Color nullColor
    {
        get
        {
            if (nullColorValue.Length > 3)
                return new Color(nullColorValue[0], nullColorValue[1], nullColorValue[2], nullColorValue[3]);
            else if (nullColorValue.Length == 3)
                return new Color(nullColorValue[0], nullColorValue[1], nullColorValue[2]);
            return Color.black;
        }
    }

    public Color notNullColor
    {
        get
        {
            if (notNullColorValue.Length > 3)
                return new Color(notNullColorValue[0], notNullColorValue[1], notNullColorValue[2], notNullColorValue[3]);
            else if (notNullColorValue.Length == 3)
                return new Color(notNullColorValue[0], notNullColorValue[1], notNullColorValue[2]);
            return Color.black;
        }
    }

    public ArrayElementTitleAttribute(string variableName) : this(variableName, new float[] { 0, 0, 0, 1 }, new float[] { 0, 0, 0, 1 })
    {
    }

    public ArrayElementTitleAttribute(string variableName, float[] nullColorValue, float[] notNullColorValue)
    {
        this.variableName = variableName;
        this.nullColorValue = nullColorValue;
        this.notNullColorValue = notNullColorValue;
    }
}
