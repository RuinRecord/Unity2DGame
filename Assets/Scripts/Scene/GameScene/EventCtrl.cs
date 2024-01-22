using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCtrl : MonoBehaviour
{
    /// <summary> EventCtrl 싱글톤 </summary>
    private static EventCtrl instance;
    public static EventCtrl Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }

    public int CurrentEvent;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CurrentEvent = 0;
    }
}