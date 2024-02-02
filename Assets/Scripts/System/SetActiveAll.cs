using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveAll : MonoBehaviour
{
    private void Awake()
    {
        SetActiveAllObjects();
    }

    public void SetActiveAllObjects()
    {
        Transform[] all = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (var tr in all)
            tr.gameObject.SetActive(true);
    }
}
