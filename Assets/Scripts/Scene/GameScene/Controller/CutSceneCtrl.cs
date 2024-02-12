using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CutSceneCtrl : MonoBehaviour
{
    private const string FADE_IN_ANIM = "Cutscene_FadeIn";
    private const string FADE_OUT_ANIM = "Cutscene_FadeOut";

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

    [SerializeField] private Animation anim;

    [SerializeField] private List<CutSceneFunction> events;

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
        foreach (var function in events)
            function.gameObject.SetActive(false);

        // 프롤로그 시작
        StartCutScene(0);
    }

    public void StartCutScene(int cutSceneCode) => SetCutScene(GameManager.Data.cutSceneDatas[cutSceneCode]);


    public void FadeIn(float fadeTime)
    {
        anim[FADE_IN_ANIM].speed = 1f / fadeTime;
        anim.Play(FADE_IN_ANIM);
    }

    public void FadeOut(float fadeTime)
    {
        anim[FADE_OUT_ANIM].speed = 1f / fadeTime;
        anim.Play(FADE_OUT_ANIM);
    }

    private void SetCutScene(CutSceneSO cutSceneSO)
    {
        IsCutSceneOn = true;
        events[cutSceneSO.cutSceneCode].gameObject.SetActive(true);
        cutSceneCode = cutSceneSO.cutSceneCode;
        currentActionIdx = 0;

        StartCoroutine(StartCutScene(cutSceneSO));
    }

    IEnumerator StartCutScene(CutSceneSO cutSceneSO)
    {
        events[cutSceneCode].OnFuntionEnter();

        while (currentActionIdx < cutSceneSO.actions.Count)
        {
            StartCoroutine(StartAction(cutSceneSO.actions[currentActionIdx]));
            isActionDone = false;

            // 액션이 종료할 때까지 대기
            while (!isActionDone)
                yield return null;
            Debug.Log(currentActionIdx);
        }

        EndCutScene();

        events[cutSceneCode].OnFunctionExit();
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
            cameraMoveCo = StartCoroutine(CameraCtrl.Instance.MoveCamera(action.camera_destination, action.camera_moveSpeed, action.camera_isMoveSmooth));
        if (action.isCameraZoomOn)
            cameraZoomCo = StartCoroutine(CameraCtrl.Instance.ZoomCamera(action.camera_zoomSize, action.camera_zoomSpeed, action.camera_isZoomSmooth));

        events[cutSceneCode].Play(currentActionIdx);

        // 대화가 끝날 때까지 대기
        while (!IsDialogDone)
            yield return null;

        yield return new WaitForSeconds(action.playTime);

        currentActionIdx++;
        isActionDone = true;
    }

    private void EndCutScene() => IsCutSceneOn = false;
}
