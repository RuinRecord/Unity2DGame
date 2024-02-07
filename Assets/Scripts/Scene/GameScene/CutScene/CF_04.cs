using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_04 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_M;
    [SerializeField] private PlayerCtrl player_W;

    private const float WALK_DISTANCE = 16f;
    private const float WALK_SPEED = 2f;

    private Coroutine walkCo;
    private Vector3 startPos;
    private float currentDistance;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        CutSceneCtrl.Instance.FadeIn(1.5f);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerM);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);

        startPos = new Vector3(-74f, 4.5f, 0f);

        player_M.SetAnimationDir(Vector2.right);
        player_M.MovePosition(startPos);
        player_W.MovePosition(startPos + Vector3.left);

        walkCo = StartCoroutine(WalkForward(currentDistance));
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 10: PlayerStop(); break;
            case 16: PlayerReWalk(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        //player_M.MoveSpeed = PlayerCtrl.WALK_SPEED;
        //player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
    }

   IEnumerator WalkForward(float currentDistance)
    {
        player_M.SetMove(Vector2.right, WALK_DISTANCE - currentDistance, WALK_SPEED);
        player_W.SetMove(Vector2.right, WALK_DISTANCE - currentDistance, WALK_SPEED);

        yield return new WaitForSeconds((WALK_DISTANCE - currentDistance) / WALK_SPEED);

        this.currentDistance = 0f;
        player_M.MovePosition(startPos);
        player_W.MovePosition(startPos + Vector3.left);

        walkCo = StartCoroutine(WalkForward(this.currentDistance));
    }

    private void PlayerStop()
    {
        currentDistance = player_M.transform.position.x - startPos.x;
        StopCoroutine(walkCo);
        player_M.SetMove(Vector2.left, 0f, 0f);
        player_W.SetMove(Vector2.right, 0f, 0f);
    }

    private void PlayerReWalk()
    {
        walkCo = StartCoroutine(WalkForward(currentDistance));
    }
}
