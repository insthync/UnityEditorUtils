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
    public float height;
    [NonSerialized]
    public Type type;

    public UnityHelpBox(string text, float height, Type type = Type.Info)
    {
        this.text = text;
        this.height = height;
        this.type = type;
    }

    public UnityHelpBox(string text, Type type = Type.Info)
    {
        this.text = text;
        height = 40;
        this.type = type;
    }

    public UnityHelpBox()
    {
        height = 40;
        text = "";
        type = Type.Info;
    }
}
