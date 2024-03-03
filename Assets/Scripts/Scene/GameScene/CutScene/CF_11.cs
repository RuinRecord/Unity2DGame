using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CF_11 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;
    [SerializeField] private InteractionObject flashlight;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        MapCtrl.Instance.SetGlobalLight(0f);
        playerW.SetLight(false);
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 3: EndFadeOut(); break;
            case 4: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        flashlight.Code = 34;
    }


    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1f);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);
        MapCtrl.Instance.SetGlobalLight(0.5f);

        playerW.MovePosition(new Vector3(20f, 4.5f, 0f));
        playerW.SetLight(false);
        playerW.SetShadow(true);
        playerW.EndCrawl();
        playerW.MoveSpeed = PlayerCtrl.Instance.MoveSpeed;
    }
}
