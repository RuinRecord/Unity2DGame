using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CF_14 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;
    [SerializeField] private PlayerCtrl playerM;
    [SerializeField] private SpecialMonitor specialMonitor;
    [SerializeField] private InteractionObject adminPC;
    [SerializeField] private Animator cardPort;

    private InteractionObject cardPortIO;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        cardPortIO = cardPort.GetComponent<InteractionObject>();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 0: StartFadeOut(); break;
            case 1: StartFadeIn(); break;
            case 2: SystemOn(); break;
            case 4: OpenCardPort(); break;
            case 5: EndFadeOut(); break;
            case 6: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTutorial("디스펜서를 조사하세요.");
    }


    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector3(29.5f, 36f, 0f));
        EventCtrl.Instance.SetCurrentEvent(Event.PuzzleDoneR4);
    }

    private void SystemOn()
    {
        GameManager.Sound.PlaySE("시스템온");
        MapCtrl.Instance.SetGlobalLight(0.5f);
    }

    private void OpenCardPort()
    {
        GameManager.Sound.PlaySE("디스펜서열림");
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
        MapCtrl.Instance.ChangeAllMonitor(MonitorType.On);

        playerW.CurrentLightIntensity = 0.5f;
        specialMonitor.ChangeType(MonitorType.On);
        adminPC.Code = 46;
        cardPortIO.Code = 47;
        cardPortIO.IsEvent = true;
    }
}
