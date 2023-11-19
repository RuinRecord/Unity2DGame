using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneFunction : MonoBehaviour
{
    public bool isFuncOn;

    public virtual void Play()
    {
        OnEventStart();
    }


    protected virtual void OnEventStart() => isFuncOn = true;

    protected virtual void OnEventUpdate() { }

    protected virtual void OnEventEnd() => isFuncOn = false;
}
