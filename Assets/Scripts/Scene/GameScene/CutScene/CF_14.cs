using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CF_14 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;
    [SerializeField] private PlayerCtrl playerM;
    [SerializeField] private Animator cardPort;

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
            case 2: SystemOn(); break;
            case 3: EndFadeOut(); break;
            case 4: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTutorial("디스펜서를 조사하세요.");
        EventCtrl.Instance.SetCurrentEvent(Event.PuzzleDoneR4);
    }


    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector3(29.5f, 36f, 0f));
    }

    private void SystemOn()
    {
        GameManager.Sound.PlaySE("시스템온");
        MapCtrl.Instance.SetGlobalLight(0.5f);

        Invoke("OpenCardPort", 1f);
    }

    private void OpenCardPort()
    {
        GameManager.Sound.PlaySE("시스템온");
        cardPort.SetBool("isOpen", true);
        playerW.SetAnimationDir(Vector2.up);
    }

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1.5f);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.MEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerM);
        CameraCtrl.Instance.SetCameraSize(5f);
    }
}
