using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬에 존재하는 모든 오브젝트를 키는 작업을 하는 클래스이다.
/// </summary>
public class SetActiveAllObject : MonoBehaviour
{
    private GameObject[] objects;

    void Awake()
    {
        objects = Resources.FindObjectsOfTypeAll<GameObject>();
        SetActiveAllObjects();
    }

    private void SetActiveAllObjects()
    {
        for (int i = 0; i < objects.Length; i++)
            objects[i].SetActive(true);
    }
}
