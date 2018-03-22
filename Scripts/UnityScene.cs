using UnityEngine;

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

    // makes it work with the existing Unity methods (LoadLevel/LoadScene)
    public static implicit operator string(UnityScene unityScene)
    {
        return unityScene.SceneName;
    }

    public bool IsSet()
    {
        return !string.IsNullOrEmpty(sceneName);
    }
}
