using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_18 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl playerW;
    [SerializeField] private PlayerCtrl playerM;
    [SerializeField] private GameObject endUI;

    private void Start()
    {
        endUI.SetActive(false);
    }

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector3(20f, 4.5f, 0f));

        playerW.MovePosition(new Vector3(20f, 4.5f, 0f));
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx) 
        {
            //case 0: StartFadeOut(); break;
            //case 1: StartFadeIn(); break;
            case 3: DoorSound(); break;
            case 4: PlayerMCome(); break;
            case 13: SwitchFadeOut(); break;
            case 14: SwitchFadeIn(); break;
            case 23: PlayerWRightJump(); break;
            case 28: PlayerWLookUp(); break;
            case 32: EndFadeOut(); break;
            case 33: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();
    }


    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(0f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);
    }

    private void DoorSound()
    {
        GameManager.Sound.PlaySE("남주문열기");
        playerM.MovePosition(new Vector3(12f, 4.5f, 0f));
        playerM.SetAnimationDir(Vector2.down);
    }

    private void PlayerMCome()
    {
        playerM.SetMove(Vector2.right, 6f, 2f);
        Invoke("PlaywerWLookLeft", 1.5f);
    }

    private void PlaywerWLookLeft() => playerW.SetAnimationDir(Vector2.left);

    private void SwitchFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(2f);

        playerM.SetMove(Vector2.right, 3f, 1f);
        Invoke("PlayerWMoveRight", 1f);
    }

    private void PlayerWMoveRight() => playerW.SetMove(Vector2.right, 2f, 1f);

    private void SwitchFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(2f);
        CameraCtrl.Instance.SetCameraPos(new Vector3(27f, 4.5f, 0f));

        playerW.MovePosition(new Vector3(27f, 4.5f, 0f));
        playerW.SetAnimationDir(Vector2.up);
        playerM.MovePosition(new Vector3(28f, 4.5f, 0f));
        playerM.SetAnimationDir(Vector2.up);
    }

    private void PlayerWRightJump()
    {
        playerW.SetAnimationDir(Vector2.right);
        playerW.StartJump();
    }

    private void PlayerWLookUp() => playerW.SetAnimationDir(Vector2.up);

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(3f);
        GameManager.Sound.PlayBGM("End_BGM", 3f);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(5f);
        endUI.SetActive(true);
    }
}
