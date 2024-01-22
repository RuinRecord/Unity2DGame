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
            case 3: MoveUp(); break;
            case 4: StartCoroutine(LookArround()); break;
            case 5: MoveDown(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
    }

    private void MoveUp()
        => player_W.SetMove(Vector2Int.up, 1, 1f);

    IEnumerator LookArround()
    {
        player_W.SetAnimationDir(Vector2.down);

        yield return new WaitForSeconds(1f);

        player_W.SetAnimationDir(Vector2.up);

        yield return new WaitForSeconds(1f);

        player_W.SetAnimationDir(Vector2.down);

        yield return new WaitForSeconds(1f);

        player_W.SetAnimationDir(Vector2.up);

        yield return new WaitForSeconds(1f);
    }

    private void MoveDown()
        => player_W.SetMove(Vector2Int.down, 1, 1f);
}
