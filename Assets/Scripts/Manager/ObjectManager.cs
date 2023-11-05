using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public void Init()
    {
        SetActiveAllObjects();
    }

    /// <summary>
    /// 씬에 존재하는 모든 오브젝트의 Active를 키는 함수이다.
    /// </summary>
    public void SetActiveAllObjects()
    {
        var objects = Resources.FindObjectsOfTypeAll<GameObject>();

        for (int i = 0; i < objects.Length; i++)
            objects[i].SetActive(true);
    }
}
