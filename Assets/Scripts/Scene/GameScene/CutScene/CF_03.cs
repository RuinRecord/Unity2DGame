using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_03 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_M;
    [SerializeField] private PlayerCtrl player_W;

    [SerializeField] private Animator playerBox;
    [SerializeField] private GameObject eventArea;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        eventArea.SetActive(false);
        CutSceneCtrl.Instance.FadeOut(0.5f);
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 1: FadeOutSetting(); break;
            case 3: PlayerMLeftMove(); break;
            case 4: PlayerMDownMove(); break;
            case 5: PlayerWShake(); break;
            case 7: PlayerMUpMove(); break;
            case 8: EndFadeOut(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        CutSceneCtrl.Instance.StartCutScene(4);
        playerBox.gameObject.SetActive(false);
    }

    private void FadeOutSetting()
    {
        CutSceneCtrl.Instance.FadeIn(0.5f);
        PlayerTag.PlayerType = PlayerType.NONE;
        CameraCtrl.Instance.SetCamera(CameraMode.Free, new Vector2(-5.5f, 19.5f));
        UIManager.Instance.SetActiveUI(false);

        player_M.Mode = PlayerMode.DEFAULT;
        player_M.MovePosition(new Vector3(-6f, 19f, 0f));
    }

    private void PlayerMLeftMove() => player_M.SetMove(Vector2.left, 1f, PlayerCtrl.WALK_SPEED * 0.5f);

    private void PlayerMDownMove() => player_M.SetMove(Vector2.down, 3f, PlayerCtrl.WALK_SPEED * 0.5f);

    private void PlayerWShake()
    {
        GameManager.Sound.PlaySE("여주박스흔들림");
        playerBox.SetBool("isShake", true);
        Invoke("EndBoxShake", 0.5f);
    }

    private void EndBoxShake()
    {
        playerBox.SetBool("isShake", false);
        PlayerLookUp();
    }

    private void PlayerMUpMove()
    {
        playerBox.SetBool("isOpen", true);
        player_M.SetMove(Vector2.up, 2f, PlayerCtrl.WALK_SPEED * 0.25f);
        Invoke("PlayerMRightMove", 2f);
        // 조준 모드 추가
    }

    private void PlayerMRightMove()
    {
        player_M.SetMove(Vector2.right, 0.75f, PlayerCtrl.WALK_SPEED * 0.25f);
        Invoke("PlayerLookUp", 0.75f);
    }

    private void PlayerLookUp() => player_M.SetAnimationDir(Vector2.up);

    private void EndFadeOut()
    {
        playerBox.SetBool("isDown", true);
        CutSceneCtrl.Instance.FadeOut(1.5f);
    }
}
