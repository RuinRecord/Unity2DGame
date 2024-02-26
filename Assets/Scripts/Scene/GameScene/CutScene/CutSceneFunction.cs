using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutSceneFunction : MonoBehaviour
{
    protected bool isOn;

    public virtual void OnFuntionEnter() { isOn = true; }

    public virtual void Play(int actionIdx) { }

    public virtual void OnFuntionUpdate() { }

    public virtual void OnFunctionExit() 
    { 
        isOn = false;
        EventCtrl.Instance.CheckEvent(EventTiming.CutScene);
    }
}
