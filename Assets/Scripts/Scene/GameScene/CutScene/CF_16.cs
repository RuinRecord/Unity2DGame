using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CF_16 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;
    [SerializeField] private CaptureObject tv;
    [SerializeField] private Teleport R4ToV;
    [SerializeField] private Teleport VToR4;

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
            case 5: EndFadeOut(); break;
            case 6: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTutorial("환풍구를 통해 밖으로 나가세요.");
        EventCtrl.Instance.SetCurrentEvent(Event.EscapeR4);

        Destroy(tv);
        R4ToV.blockDialog = null;
        R4ToV.IsOn = true;
        VToR4.lightIntensity = 0.5f;
    }

    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);

        playerW.MovePosition(new Vector3(26f, -14f, 0f));
        playerW.SetAnimationDir(Vector2.up);
    }

    private void EndFadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.WOMEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
        CameraCtrl.Instance.SetCameraSize(5f);
    }
}
