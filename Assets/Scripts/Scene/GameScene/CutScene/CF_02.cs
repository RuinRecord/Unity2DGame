using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_02 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_W;
    [SerializeField] private PlayerCtrl player_M;

    [SerializeField] private Animator playerBox;
    [SerializeField] private GameObject eventArea;

    private Coroutine soundCo;

    private void Start()
    {
        playerBox.gameObject.SetActive(false);
        eventArea.SetActive(false);
    }

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 0: soundCo = StartCoroutine(StartFootStepAudio("발소리_1")); break;
            case 1: PlayerWJump(); break;
            case 3: StartCoroutine(PlayerWArround()); break;
            case 5: RunToBox(); break;
            case 6: HideOnBox(); break;
            case 7: PlayerMOpen(); break;
            case 8: PlayerMAppear(); break;
            case 12: PlayerMUpMove(); break;
            case 13: PlayerMLeftMove(); break;
            case 15: PlayRatAudio(); break;
            case 19: EndFadeOut(); break;
            case 20: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        player_M.MoveSpeed = PlayerCtrl.WALK_SPEED;
        TutorialManager.Instance.ShowTooltip("시점이 변경되었습니다. 상자에 붙어 [Space] 키로 가운데 상자를 표시된 위치로 옮기세요.\n[R] 키를 누르면 재시작이 가능합니다.");
    }

    IEnumerator StartFootStepAudio(string SE)
    {
        yield return new WaitForSeconds(1f);

        switch (SE)
        {
            case "발소리_1":
                GameManager.Sound.PlaySE("발소리_1");
                soundCo = StartCoroutine(StartFootStepAudio("발소리_2"));
                break;

            case "발소리_2":
                GameManager.Sound.PlaySE("발소리_2");
                soundCo = StartCoroutine(StartFootStepAudio("발소리_3"));
                break;

            case "발소리_3":
                GameManager.Sound.PlaySE("발소리_3");
                soundCo = StartCoroutine(StartFootStepAudio("발소리_1"));
                break;
        }
    }

    private void PlayerWJump()
    {
        player_W.StartJump();
        player_W.SetAnimationDir(Vector2.down);
    }

    IEnumerator PlayerWArround()
    {
        player_W.SetAnimationDir(Vector2.left);
        yield return new WaitForSeconds(0.75f);
        player_W.SetAnimationDir(Vector2.down);
        yield return new WaitForSeconds(0.75f);
        player_W.SetAnimationDir(Vector2.right);
    }

    private void RunToBox()
    {
        player_W.SetMove(Vector2.right, 4, PlayerCtrl.WALK_SPEED);
        CutSceneCtrl.Instance.FadeOut(0.5f);
    }

    private void HideOnBox()
    {
        GameManager.Sound.PlaySE("숨기");
        StopCoroutine(soundCo);
    }

    private void PlayerMOpen() => GameManager.Sound.PlaySE("남주문열기");

    private void PlayerMAppear()
    {
        CutSceneCtrl.Instance.FadeIn(0.5f);

        PlayerTag.PlayerType = PlayerType.NONE;
        CameraCtrl.Instance.SetCamera(CameraMode.Free, new Vector2(-6.5f, 16.5f));

        player_M.MovePosition(new Vector3(-6.5f, 16.5f, 0f));
        player_W.MovePosition(new Vector3(-6.5f, 100f, 0f));
        playerBox.gameObject.SetActive(true);
    }

    private void PlayerMUpMove() => player_M.SetMove(Vector2.up, 3f, PlayerCtrl.WALK_SPEED * 0.5f);

    private void PlayerMLeftMove()
    {
        GameManager.Sound.PlaySE("여주박스흔들림");
        player_M.SetMove(Vector2.left, 1f, PlayerCtrl.WALK_SPEED * 0.5f);
        playerBox.SetBool("isShake", true);
        Invoke("EndBoxShake", 2f);
    }

    private void EndBoxShake()
    {
        playerBox.SetBool("isShake", false);
        player_M.SetAnimationDir(Vector2.right);
    }

    private void PlayRatAudio() => GameManager.Sound.PlaySE("쥐소리");

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1f);
        player_M.SetAnimationDir(Vector2.down);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.MEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerM);
        eventArea.SetActive(true);
    }
}
