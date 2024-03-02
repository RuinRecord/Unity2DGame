using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CF_17 : CutSceneFunction
{
    [SerializeField] private GameObject trigger;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        Destroy(trigger);
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx) 
        {
            case 0: GameManager.Sound.PlaySE("울음소리"); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();
    }
}
