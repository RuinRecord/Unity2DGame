using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_05 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_M;

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 2: PlayerMLookLeft(); break;
            case 12: PlayerMGoBack(); break;
            case 14: PlayerMGoFront(); break;
            case 16: ChangeScene(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        CutSceneCtrl.Instance.StartCutScene(6);
    }

    private void PlayerMLookLeft() => player_M.SetAnimationDir(Vector2.left);

    private void PlayerMGoBack() => player_M.SetMove(Vector2.left, 1f, 3f);

    private void PlayerMGoFront() => player_M.SetMove(Vector2.right, 1f, 3f);

    private void ChangeScene()
    {
        GameManager.Sound.PlaySE("노이즈");
        CameraCtrl.Instance.GlitchEffect(0.1f);
        Invoke("SwitchCutScene", 0.1f);
    }

    private void SwitchCutScene()
    {
        CameraCtrl.Instance.SetCameraPos(Camera.main.transform.position - new Vector3(0f, 101f, 0f));
        player_M.MovePosition(player_M.transform.position + new Vector3(-1f, -101f, 0f));
    }
}