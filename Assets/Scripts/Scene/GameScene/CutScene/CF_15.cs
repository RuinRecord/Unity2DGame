using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CF_15 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;
    [SerializeField] private PlayerCtrl playerM;
    [SerializeField] private Animator cardPort;
    [SerializeField] private CaptureObject tv;

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
            case 4: GetCard(); break;
            case 5: PlayerMWalkDown(); break;
            case 7: SwitchFadeOut(); break;
            case 8: SwitchFadeIn(); break;
            case 11: PlayerWJump(); break;
            case 12: PlayerWWalkUp(); break;
            case 14: EndFadeOut(); break;
            case 15: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTutorial("연구 자료를 카메라로 기록하세요.");
        EventCtrl.Instance.SetCurrentEvent(Event.SearchR4);

        playerM.MoveSpeed = PlayerCtrl.WALK_SPEED;
        playerW.MoveSpeed = PlayerCtrl.WALK_SPEED;
        tv.enabled = true;
        tv.GetComponent<Collider2D>().isTrigger = true;
        tv.gameObject.layer = LayerMask.NameToLayer("CaptureObject");
    }

    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        UIManager.Instance.SetActiveUI(false);
    }

    private void GetCard()
    {
        cardPort.SetBool("isGetCard", true);
        GameManager.Data.player.AddItem(5); // 카드 획득
    }

    private void PlayerMWalkDown() => playerM.SetMove(Vector2.down, 1f, 0.5f);

    private void SwitchFadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void SwitchFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);
        CameraCtrl.Instance.SetCameraPos(new Vector3(26f, -16f, 0f));

        playerW.MovePosition(new Vector3(26f, -15f, 0f));
        playerW.SetAnimationDir(Vector2.down);
    }

    private void PlayerWJump() => playerW.StartJump();

    private void PlayerWWalkUp() => playerW.SetMove(Vector2.up, 1f, 0.5f);

    private void EndFadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.WOMEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
        CameraCtrl.Instance.SetCameraSize(5f);
        UIManager.Instance.SetActiveUI(true);
    }
}
