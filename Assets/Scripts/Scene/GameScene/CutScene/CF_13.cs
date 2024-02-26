using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CF_13 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;
    [SerializeField] private SpecialMonitor specialMonitor;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        MapCtrl.Instance.SetGlobalLight(0.05f);
        playerW.SetLight(true);
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 0: StartFadeOut(); break;
            case 1: StartFadeIn(); break;
            case 5: SystemOn(); break;
            case 6: PlayerWGoDown(); break;
            case 8: SystemError(); break;
            case 9: PlayerWJump(); break;
            case 12: EndFadeOut(); break;
            case 13: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTutorial("시스템을 복구할 방법을 찾으세요.");
        EventCtrl.Instance.SetCurrentEvent(Event.PuzzleForR4);
    }


    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(1f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector3(26f, -14f, 0f));

        playerW.SetAnimationDir(Vector2.up);
        playerW.MovePosition(new Vector3(29.25f, -14f, 0f));
    }

    private void SystemOn()
    {
        GameManager.Sound.PlaySE("시스템온");
        MapCtrl.Instance.ChangeAllMonitor(MonitorType.On);
        MapCtrl.Instance.SetGlobalLight(0.5f);
        specialMonitor.ChangeType(MonitorType.On);
        Invoke("PlayerWLookDown", 1f);
    }

    private void PlayerWGoDown() => playerW.SetMove(Vector2.down, 2f, 1f);

    private void SystemError()
    {
        GameManager.Sound.PlaySE("시스템다운");
        MapCtrl.Instance.ChangeAllMonitor(MonitorType.Off);
        MapCtrl.Instance.ChangeSomeMonitor(MonitorType.Error);
        MapCtrl.Instance.SetGlobalLight(0.05f);
        specialMonitor.ChangeType(MonitorType.Error);
    }

    private void PlayerWLookDown() => playerW.SetAnimationDir(Vector2.down);

    private void PlayerWJump()
    {
        playerW.MoveSpeed = PlayerCtrl.WALK_SPEED;
        playerW.StartJump();
    }

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1.5f);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.WOMEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
        CameraCtrl.Instance.SetCameraSize(5f);
    }
}