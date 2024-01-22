using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CutSceneCtrl : MonoBehaviour
{
    /// <summary> CutSceneCtrl 싱글톤 </summary>
    private static CutSceneCtrl Instance;
    public static CutSceneCtrl instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get { return Instance; }
    }

    public static bool isCutSceneOn;

    [SerializeField]
    private Camera cameraW, cameraM;

    [SerializeField]
    private List<CutSceneFunction> events;

    private Coroutine cameraMoveCo, cameraZoomCo;

    private int cutSceneCode;

    private int currentActionIdx;

    public bool isDialogDone;

    private bool isActionDone;


    private void Awake()
    {
        instance = this;
        isCutSceneOn = false;
    }

    private void Start()
    {
        cutSceneCode = -1;
        currentActionIdx = -1;
        isActionDone = false;
        isDialogDone = true;
        events.AddRange(GetComponentsInChildren<CutSceneFunction>());

        // 프롤로그 시작
        StartPrologue();
    }

    private void StartPrologue()
    {
        SetCutScene(GameManager._data.cutSceneDatas[0]);
    }


    private Camera GetCamera()
    {
        if (PlayerTag.playerType.Equals(PlayerType.WOMEN))
            return cameraW;
        else
            return cameraM;
    }


    public void SetCutScene(CutSceneSO cutSceneSO)
    {
        isCutSceneOn = true;
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
            isDialogDone = false;
            UIManager._interactUI.StartDialog(new DialogSet[] { action.dialogs });
        }

        if (action.isCameraMoveOn)
            cameraMoveCo = StartCoroutine(MoveCamera(GetCamera(), action.camera_destination, action.camera_moveSpeed, action.camera_isMoveSmooth));
        if (action.isCameraZoomOn)
            cameraZoomCo = StartCoroutine(ZoomCamera(GetCamera(), action.camera_zoomSize, action.camera_zoomSpeed, action.camera_isZoomSmooth));

        events[cutSceneCode].Play(currentActionIdx);

        // 대화가 끝날 때까지 대기
        while (!isDialogDone)
            yield return null;

        yield return new WaitForSeconds(action.playTime);

        isActionDone = true;
    }

    IEnumerator MoveCamera(Camera camera, Vector2 destination, float moveSpeed, bool isSmooth)
    {
        Vector2 gap = destination - (Vector2)camera.transform.localPosition;
        Vector2 dir = gap.normalized;

        while (gap.normalized == dir)
        {
            // 카메라 이동
            if (isSmooth)
                camera.transform.localPosition += new Vector3(gap.x, gap.y, 0) * moveSpeed * Time.deltaTime;
            else
                camera.transform.localPosition += new Vector3(gap.x, gap.y, 0).normalized * moveSpeed * Time.deltaTime;
            gap = destination - (Vector2)camera.transform.localPosition;

            // 최소 크기 유지
            if (gap.sqrMagnitude * moveSpeed < 1f)
                gap = Vector2.ClampMagnitude(gap, 1f);
            yield return null;
        }

        camera.transform.localPosition = new Vector3(destination.x, destination.y, -10);
    }

    IEnumerator ZoomCamera(Camera camera, float size, float moveSpeed, bool isSmooth)
    {
        float gap = size - camera.orthographicSize;
        float sign = Mathf.Sign(gap);

        while (Mathf.Abs(gap) > 0.01f || sign != Mathf.Sign(gap))
        {
            if (isSmooth)
                camera.orthographicSize += gap * moveSpeed * Time.deltaTime;
            else
                camera.orthographicSize += sign * moveSpeed * Time.deltaTime;
            gap = size - camera.orthographicSize;
            yield return null;
        }

        camera.orthographicSize = size;
    }

    private void EndCutScene()
    {
        isCutSceneOn = false;
    }
}
