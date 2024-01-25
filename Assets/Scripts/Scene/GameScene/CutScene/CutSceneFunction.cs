using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutSceneFunction : MonoBehaviour
{
    public virtual void OnFuntionEnter() { }

    public virtual void Play(int actionIdx) { }

    public virtual void OnFunctionExit() { }
}
