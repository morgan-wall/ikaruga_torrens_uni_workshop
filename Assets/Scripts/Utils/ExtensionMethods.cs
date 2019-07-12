using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static T[] GetComponentsInDirectChildren<T>(this Transform a_parent, bool a_includeInactive = false)
        where T : class
    {
        List<T> tempComponentCache = new List<T>();
 
        foreach (Transform child in a_parent)
        {
            if ((!child.gameObject.activeInHierarchy && !a_includeInactive)
                || child.parent != a_parent)
            {
                continue;
            }

            var component = child.GetComponent<T>();
            if (component == null)
            {
                continue;
            }

            tempComponentCache.Add(component);
        }
 
        return tempComponentCache.ToArray();
    }

    public static T GetComponentInDirectChildren<T>(this Transform a_parent, bool a_includeInactive = false)
        where T : class
    {
        foreach (Transform child in a_parent)
        {
            if ((!child.gameObject.activeInHierarchy && !a_includeInactive)
                || child.parent != a_parent)
            {
                continue;
            }

            var component = child.GetComponent<T>();
            if (component == null)
            {
                continue;
            }

            return component;
        }
 
        return null;
    }
}
