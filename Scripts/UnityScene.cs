using UnityEngine;

[System.Serializable]
public class UnityScene
{
    [SerializeField]
    private Object sceneAsset;
    [SerializeField]
    private string sceneName = "";

    public string SceneName
    {
        get { return sceneName; }
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
