using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CutSceneCtrl : MonoBehaviour
{
    /// <summary> CutSceneCtrl 싱글톤 </summary>
    private static CutSceneCtrl instance;
    public static CutSceneCtrl Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }

    public static bool IsCutSceneOn;

    public bool IsDialogDone;

    [SerializeField]
    private Camera cameraW, cameraM;

    [SerializeField]
    private List<CutSceneFunction> events;

    private Coroutine cameraMoveCo, cameraZoomCo;

    private int cutSceneCode;

    private int currentActionIdx;

    private bool isActionDone;


    private void Awake()
    {
        Instance = this;
        IsCutSceneOn = false;
    }

    private void Start()
    {
        cutSceneCode = -1;
        currentActionIdx = -1;
        isActionDone = false;
        IsDialogDone = true;
        events.AddRange(GetComponentsInChildren<CutSceneFunction>());

        // 프롤로그 시작
        StartCutScene(0);
    }

    public void StartCutScene(int cutSceneCode)
    {
        SetCutScene(GameManager.Data.cutSceneDatas[cutSceneCode]);
    }


    private Camera GetCamera()
    {
        if (PlayerTag.PlayerType.Equals(PlayerType.WOMEN))
            return cameraW;
        else
            return cameraM;
    }


    private void SetCutScene(CutSceneSO cutSceneSO)
    {
        IsCutSceneOn = true;
        cutSceneCode = cutSceneSO.cutSceneCode;
        currentActionIdx = 0;

        StartCoroutine(StartCutScene(cutSceneSO));
    }

    IEnumerator StartCutScene(CutSceneSO cutSceneSO)
    {
        while (currentActionIdx < cutSceneSO.actions.Count)
        {
            StartCoroutine(StartAction(cutSceneSO.actions[currentActionIdx++]));
            isActionDone = false;

            // 액션이 종료할 때까지 대기
            while (!isActionDone)
                yield return null;
        }

        events[cutSceneCode].OnFunctionExit();

        EndCutScene();
    }

    IEnumerator StartAction(CutSceneAction action)
    {
        if (cameraMoveCo != null)
            StopCoroutine(cameraMoveCo);
        if (cameraZoomCo != null)
            StopCoroutine(cameraZoomCo);

        if (action.isDialogOn)
        {
            IsDialogDone = false;
            UIManager.InteractUI.StartDialog(new DialogSet[] { action.dialogs });
        }

        if (action.isCameraMoveOn)
            cameraMoveCo = StartCoroutine(MoveCamera(GetCamera(), action.camera_destination, action.camera_moveSpeed, action.camera_isMoveSmooth));
        if (action.isCameraZoomOn)
            cameraZoomCo = StartCoroutine(ZoomCamera(GetCamera(), action.camera_zoomSize, action.camera_zoomSpeed, action.camera_isZoomSmooth));

        events[cutSceneCode].Play(currentActionIdx);

        // 대화가 끝날 때까지 대기
        while (!IsDialogDone)
            yield return null;

        yield return new WaitForSeconds(action.playTime);

        isActionDone = true;
    }

    IEnumerator MoveCamera(Camera camera, Vector2 destination, float moveSpeed, bool isSmooth)
    {
        Vector2 _gap = destination - (Vector2)camera.transform.localPosition;
        Vector2 _dir = _gap.normalized;

        while (_gap.normalized == _dir)
        {
            // 카메라 이동
            if (isSmooth)
                camera.transform.localPosition += new Vector3(_gap.x, _gap.y, 0) * moveSpeed * Time.deltaTime;
            else
                camera.transform.localPosition += new Vector3(_gap.x, _gap.y, 0).normalized * moveSpeed * Time.deltaTime;
            _gap = destination - (Vector2)camera.transform.localPosition;

            // 최소 크기 유지
            if (_gap.sqrMagnitude * moveSpeed < 1f)
                _gap = Vector2.ClampMagnitude(_gap, 1f);
            yield return null;
        }

        camera.transform.localPosition = new Vector3(destination.x, destination.y, -10);
    }

    IEnumerator ZoomCamera(Camera camera, float size, float moveSpeed, bool isSmooth)
    {
        float _gap = size - camera.orthographicSize;
        float _sign = Mathf.Sign(_gap);

        while (Mathf.Abs(_gap) > 0.01f || _sign != Mathf.Sign(_gap))
        {
            if (isSmooth)
                camera.orthographicSize += _gap * moveSpeed * Time.deltaTime;
            else
                camera.orthographicSize += _sign * moveSpeed * Time.deltaTime;
            _gap = size - camera.orthographicSize;
            yield return null;
        }

        camera.orthographicSize = size;
    }

    private void EndCutScene()
    {
        IsCutSceneOn = false;
    }
}
