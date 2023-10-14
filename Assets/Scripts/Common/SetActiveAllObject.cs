using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a class to set active 'true' all objects that exist at scene.
/// If you add this script at scene, you can work without worrying about object active.
/// </summary>
public class SetActiveAllObject : MonoBehaviour
{
    /// <summary>
    /// All objects that exist at scene.
    /// </summary>
    private GameObject[] objects;

    void Awake()
    {
        objects = Resources.FindObjectsOfTypeAll<GameObject>();
        SetActiveAllObjects();
    }

    /// <summary>
    /// Function to set active 'true' all objects that exist at scene.
    /// </summary>
    private void SetActiveAllObjects()
    {
        for (int i = 0; i < objects.Length; i++)
            objects[i].SetActive(true);
    }
}
