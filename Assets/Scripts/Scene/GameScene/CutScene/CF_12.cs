using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CF_12 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        MapCtrl.Instance.SetGlobalLight(0.05f);
        playerW.SetLight(true);
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTutorial("전원을 킬 수 있는 방법을 찾으세요.");
    }
}
