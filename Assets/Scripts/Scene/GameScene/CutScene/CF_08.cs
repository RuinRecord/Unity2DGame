using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CF_08 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_W;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 0: StartFadeOut(); break;
            case 1: StartFadeIn(); break;
            case 3: PlayerWMove(); break;
            case 5: PlayerWJump(); break;
            case 6: EndFadeOut(); break;
            case 7: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
    }

    private void StartFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1f);
    }

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector3(18f, 20f, 0f));
        player_W.MovePosition(new Vector3(16f, 21f, 0f));
        player_W.SetAnimationDir(Vector2.right);
    }

    private void PlayerWMove()
    {
        player_W.SetMove(Vector2.down, 1f, 1f);
        Invoke("PlayerTurnRight", 1f);

    }

    private void PlayerTurnRight()
    {
        player_W.SetMove(Vector2.right, 1f, 1f);
        Invoke("PlayerLookUp", 1f);
    }

    private void PlayerLookUp() => player_W.SetAnimationDir(Vector2.up);

    private void PlayerWJump()
    {
        player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
        player_W.StartJump();
    }

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1f);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);

        TutorialManager.Instance.ShowTutorial("[TAG] 버튼을 눌러 유진 시점은 전환하여 선반을 미세요.");
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.WOMEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
    }
}
