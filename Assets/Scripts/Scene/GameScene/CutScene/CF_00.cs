using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_00 : CutSceneFunction
{
    [SerializeField]
    private PlayerCtrl player_W;

    [SerializeField]
    private PlayerCtrl player_M;


    public override void Play()
    {
        base.Play();

        #region 수행 이벤트

        MovePlayerM();
        Invoke("TurnPlayerW", 1.5f);

        #endregion

        base.OnEventEnd();
    }

    private void MovePlayerM() => player_M.SetMove(Vector2Int.left, 4, 2);

    private void TurnPlayerW() => player_W.SetAnimationDir(Vector2.right);
}
