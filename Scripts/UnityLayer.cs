using UnityEngine;

[System.Serializable]
public class UnityLayer
{
    [SerializeField]
    private int layerIndex = 0;
    public int LayerIndex
    {
        get { return layerIndex; }
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
