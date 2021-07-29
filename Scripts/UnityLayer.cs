using UnityEngine;

[System.Serializable]
public struct UnityLayer
{
    [SerializeField]
    private int layerIndex;
    public int LayerIndex
    {
        get { return layerIndex; }
    }

    public UnityLayer(int layerIndex)
    {
        this.layerIndex = layerIndex;
    }

    public static implicit operator int(UnityLayer unityLayer)
    {
        return unityLayer.LayerIndex;
    }

    public void Set(int _layerIndex)
    {
        if (_layerIndex > 0 && _layerIndex < 32)
            layerIndex = _layerIndex;
    }

    public int Mask
    {
        get { return 1 << layerIndex; }
    }
}
