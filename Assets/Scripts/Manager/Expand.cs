using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Expand 
{
    public static T GetOrAddComponenet<T>(this GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
}
