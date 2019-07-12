using UnityEngine;

public static class Utils
{
    public static void SetLayerRecursively(GameObject a_gameObject, string a_layerName)
    {
        int layer = LayerMask.GetMask(a_layerName);
        SetLayerRecursively(a_gameObject, layer);
    }

    public static void SetLayerRecursively(GameObject a_gameObject, int a_layer)
    {
        foreach (Transform transform in a_gameObject.transform)
        {
            transform.gameObject.layer = a_layer;
        }
    }
}
