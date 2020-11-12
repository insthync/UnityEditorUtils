using System;

[Serializable]
public class UnityHelpBox
{
    public enum Type
    {
        None,
        Info,
        Warning,
        Error
    }

    [NonSerialized]
    public string text;

    [NonSerialized]
    public Type type;

    public UnityHelpBox(string text, Type type = Type.Info)
    {
        this.text = text;
        this.type = type;
    }
}
