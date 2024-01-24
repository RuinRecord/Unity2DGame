using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_01 : CutSceneFunction
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
            case 2: MoveDown(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        player_W.MoveSpeed = PlayerCtrl.WALK_SPEED;
    }

    private void MoveDown()
        => player_W.SetMove(Vector2Int.down, 1, 2f);
}
