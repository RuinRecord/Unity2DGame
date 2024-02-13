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

        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector2(12f, 5f));
        player_W.MovePosition(new Vector3(7f, 4.5f, 0f));
        player_M.MovePosition(new Vector3(9f, 4.5f, 0f));
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 0: PlayerWalk(); break;
            case 3: PlayerWLookRight(); break;
            case 9: DropItem(); break;
            case 10: EndFadeOut(); break;
            case 11: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        TutorialManager.Instance.ShowTooltip("'유진'을 설득하였습니다. 이제 시설을 조사하세요.");
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

    private void DropItem() => GameManager.Data.player.AddItem(2); // 무전기 획득

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1.5f);
        player_M.SetMove(Vector2.right, 2.9f, 2f);
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
