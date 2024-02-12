using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_05 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_M;
    [SerializeField] private PlayerCtrl sister;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 2: PlayerMLookLeft(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();
    }

    private void PlayerMLookLeft() => player_M.SetAnimationDir(Vector2.left);
}
