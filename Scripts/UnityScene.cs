using UnityEngine;

[System.Serializable]
public class UnityScene
{
    [SerializeField]
    public Object sceneAsset;
    [SerializeField]
    public string sceneName = string.Empty;

    public string SceneName
    {
        get { return sceneName; }
        set { sceneName = value; }
    }
    
    public static implicit operator string(UnityScene unityScene)
    {
        return unityScene.SceneName;
    }

    public bool IsSet()
    {
        return !string.IsNullOrEmpty(sceneName);
    }
}
