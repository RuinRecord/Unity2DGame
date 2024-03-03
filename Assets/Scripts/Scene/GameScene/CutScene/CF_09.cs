using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CF_09 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_W;
    [SerializeField] private PlayerCtrl player_M;
    [SerializeField] private InteractionObject vent;
    [SerializeField] private InteractionObject ladder;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 1: StartFadeOut(); break;
            case 2: StartFadeIn(); break;
            case 9: FadeOut(); break;
            case 10: GoToRoom(); break;
            case 11: FadeIn(); break;
            case 17: EndFadeOut(); break;
            case 18: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTutorial("비밀스러운 공간을 발견했습니다. 추가로 더 조사하세요.");

        player_M.CurrentTeleport = null;
        player_W.CurrentTeleport = null;
        player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
        player_M.MoveSpeed = PlayerCtrl.WALK_SPEED;
    }

    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(1f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);

        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector3(18.5f, 21f, 0f));
        UIManager.Instance.SetActiveUI(false);

        player_M.Mode = PlayerMode.DEFAULT;
        player_M.State = PlayerState.IDLE;
        player_W.MovePosition(new Vector3(17f, 20f, 0f));
        player_M.MovePosition(new Vector3(18f, 20f, 0f));
        player_W.SetAnimationDir(Vector2.up);
        player_M.SetAnimationDir(Vector2.up);
    }

    private void FadeOut() => CutSceneCtrl.Instance.FadeOut(1.5f);

    private void GoToRoom()
    {
        GameManager.Sound.PlaySE("비밀방입장");
        CameraCtrl.Instance.SetCameraPos(new Vector3(30.5f, 34f, 0f));
        MapCtrl.Instance.SetGlobalLight(0.15f);

        player_W.MovePosition(new Vector3(30f, 32f, 0f));
        player_M.MovePosition(new Vector3(29f, 32f, 0f));
        player_W.CurrentLightIntensity = 0.15f;
    }

    private void FadeIn() => CutSceneCtrl.Instance.FadeIn(1.5f);

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1f);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);

        EventCtrl.Instance.SetCurrentEvent(Event.FindSecretRoom);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.WOMEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
        UIManager.Instance.SetActiveUI(true);

        player_M.MovePosition(new Vector2(27f, 5f));
        player_M.SetAnimationDir(Vector2.down);
        vent.Code = 31;
        ladder.Code = 32;
    }
}
