using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_00 : CutSceneFunction
{
    [SerializeField]
    private PlayerCtrl player_W;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 1: MoveRight(); break;
            case 2: MoveUp(); break;
            case 3: StartCoroutine(LookArround()); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
        TutorialManager.instance.ShowTooltip("모니터 앞으로 다가가 [Q] 버튼을 눌러 조사하세요.");
    }

    private void MoveRight()
        => player_W.SetMove(Vector2Int.right, 1, 2f);

    private void MoveUp()
        => player_W.SetMove(Vector2Int.up, 1, 2f);

    IEnumerator LookArround()
    {
        player_W.SetAnimationDir(Vector2.down);

        yield return new WaitForSeconds(0.75f);

        player_W.SetAnimationDir(Vector2.right);

        yield return new WaitForSeconds(0.75f);

        player_W.SetAnimationDir(Vector2.up);

        yield return new WaitForSeconds(0.75f);
    }
}
