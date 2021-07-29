﻿using UnityEngine;

[System.Serializable]
public struct UnityTag
{
    [SerializeField]
    private string tag;
    public string Tag
    {
        get { return tag; }
    }

    public UnityTag(string tag)
    {
        this.tag = tag;
    }
    
    public static implicit operator string(UnityTag unityTag)
    {
        return unityTag.Tag;
    }

    public void Set(string _tag)
    {
        tag = _tag;
    }
}
