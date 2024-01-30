using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_02 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_W;
    [SerializeField] private PlayerCtrl player_M;

    [SerializeField] private GameObject playerBox;

    private Coroutine soundCo;

    private void Start()
    {
        playerBox.SetActive(false);
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
            case 9: PlayerMMove(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();
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
        PlayerTag.Instance.SetPlayerTypeImmediately(PlayerType.MEN, false);
        player_M.MovePosition(new Vector3(-6.5f, 16.5f, 0f));
        player_W.MovePosition(new Vector3(-6.5f, 100f, 0f));
        playerBox.SetActive(true);
    }

    private void PlayerMMove() => player_M.SetMove(Vector2.up, 2f, PlayerCtrl.WALK_SPEED * 0.5f);
}
