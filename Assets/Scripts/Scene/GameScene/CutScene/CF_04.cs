using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CF_04 : CutSceneFunction
{
    [SerializeField] private PlayerCtrl player_M;
    [SerializeField] private PlayerCtrl player_W;
    [SerializeField] private GameObject sister;

    private const float WALK_DISTANCE = 16f;
    private const float WALK_SPEED = 2f;

    private Coroutine walkCo;
    private Vector3 startPos;
    private float currentDistance;
    private bool isTrackingCamera;

    private void Start()
    {
        sister.SetActive(false);
    }

    public void Update()
    {
        if (isTrackingCamera)
            CameraCtrl.Instance.SetCameraPos(player_M.transform.position);
    }

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        CutSceneCtrl.Instance.FadeIn(1.5f);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        UIManager.Instance.SetActiveUI(false);

        startPos = new Vector3(-74f, 4.5f, 0f);
        currentDistance = 0f;
        isTrackingCamera = true;

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
            case 14: PlayerMLookRight(); break;
            case 20: PlayerWStop(); break;
            case 22: ChangeScene(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        CutSceneCtrl.Instance.StartCutScene(5);
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
        isTrackingCamera = false;
        currentDistance = player_M.transform.localPosition.x - startPos.x;
        StopCoroutine(walkCo);
        player_M.SetMove(Vector2.right, 0f, 0f);
        player_W.SetMove(Vector2.right, 0f, 0f);
    }

    private void PlayerMLookRight()
    {
        player_M.SetAnimationDir(Vector2.right);
        Invoke("PlayerReWalk", 1f);
    }

    private void PlayerReWalk()
    {
        isTrackingCamera = true;
        walkCo = StartCoroutine(WalkForward(currentDistance));
    }

    private void PlayerWStop()
    {
        isTrackingCamera = false;
        currentDistance = player_M.transform.localPosition.x - startPos.x;
        StopCoroutine(walkCo);
        player_W.SetMove(Vector2.right, 0f, 0f);
        player_M.SetMove(Vector2.right, 1f, WALK_SPEED);
    }

    private void ChangeScene()
    {
        GameManager.Sound.PlaySE("노이즈");
        CameraCtrl.Instance.GlitchEffect(0.1f);
        Invoke("SwitchCutScene", 0.1f);
    }

    private void SwitchCutScene()
    {
        CameraCtrl.Instance.SetCameraPos(Camera.main.transform.position + new Vector3(1.5f, 101f, 0f));
        player_M.MovePosition(player_M.transform.position + Vector3.up * 100);
        sister.SetActive(true);
        sister.transform.position = player_M.transform.position + Vector3.left * 2;
    }
}
