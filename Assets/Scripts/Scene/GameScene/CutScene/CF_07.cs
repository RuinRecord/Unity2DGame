using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CF_07 : CutSceneFunction
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
            case 0: PlayerWalk(); break;
            case 3: PlayerWLookRight(); break;
            case 10: EndFadeOut(); break;
            case 11: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        player_M.MoveSpeed = PlayerCtrl.WALK_SPEED;
        player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
    }

    private void PlayerWalk()
    {
        player_M.SetMove(Vector2.right, 6f, 2f);
        player_W.SetMove(Vector2.right, 5f, 2f);
        Invoke("PlayerWLookUp", 2.5f);
        Invoke("PlayerMLookBack", 3.5f);
    }

    private void PlayerWLookUp()
    {
        player_W.StartJump();
        player_W.SetAnimationDir(Vector2.up);
    }

    private void PlayerMLookBack() => player_M.SetAnimationDir(Vector2.left);

    private void PlayerWLookRight() => player_W.SetAnimationDir(Vector2.right);

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1.5f);
        player_M.SetMove(Vector2.right, 2f, 2f);
    }

    private void EndFadeIn()
    {
        player_M.MovePosition(new Vector2(27f, 5f));
        player_M.SetAnimationDir(Vector2.down);
        CutSceneCtrl.Instance.FadeIn(1.5f);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.WOMEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
    }
}
