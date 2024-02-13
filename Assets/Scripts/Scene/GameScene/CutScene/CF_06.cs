using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CF_06 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_M;
    [SerializeField] private PlayerCtrl player_W;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 2: PlayerMLookBack(); break;
            case 19: ChangeScene(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        CutSceneCtrl.Instance.StartCutScene(7);
    }

    private void PlayerMLookBack() => player_M.SetAnimationDir(Vector2.left);


    private void ChangeScene()
    {
        CutSceneCtrl.Instance.FadeOut(1f);
        Invoke("SwitchCutScene", 1f);
    }

    private void SwitchCutScene()
    {
        CutSceneCtrl.Instance.FadeIn(1f);
        CameraCtrl.Instance.SetCameraPos(new Vector2(12f, 5f));
        player_W.MovePosition(new Vector3(7f, 4.5f, 0f));
        player_M.MovePosition(new Vector3(9f, 4.5f, 0f));
    }
}
