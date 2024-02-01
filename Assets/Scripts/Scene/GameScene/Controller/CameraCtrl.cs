using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private static CameraCtrl instance;
    public static CameraCtrl Instance
    {
        get { return instance; }
        set
        {
            if (instance == null)
                instance = value;
        }
    }



    [SerializeField] private CameraMode mode;
    public CameraMode Mode => mode;

    [SerializeField] private GameObject glitchVolume;

    [SerializeField] private PlayerCtrl playerW, playerM;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetActiveGlitch(false);
    }

    // Update is called once per frame
    void Update()
    {
        CameraProcess();
    }

    public void SetActiveGlitch(bool isActive) => glitchVolume.SetActive(isActive);

    public void SetCameraMode(CameraMode cameraMode) => mode = cameraMode;

    public void SetCameraPos(Vector2 pos)
    {
        Vector3 cameraPos = pos;
        cameraPos.z = -10;
        this.transform.position = cameraPos;
    }

    public void SetCamera(CameraMode cameraMode, Vector2 pos)
    {
        SetCameraMode(cameraMode);
        SetCameraPos(pos);
    }

    /* 해상도 설정하는 함수 */
    public void SetDefaultResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1200; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }

    public void SetCameraRect(bool isPanelOn)
    {
        if (isPanelOn)
        {
            // Tag 패널 ON인 상태 => 좌측 상단에 작게 배치
            Camera.main.rect = new Rect(0.05f, 0.05f, 0.9f, 0.9f);
            SetActiveGlitch(true);
        }
        else
        {
            // Tag 선택 OFF인 상태 => 전체 화면
            SetDefaultResolution();
            SetActiveGlitch(false);
        }
    }

    private void CameraProcess()
    {
        switch (mode)
        {
            case CameraMode.PlayerW: SetCameraPos(playerW.transform.position); break;
            case CameraMode.PlayerM: SetCameraPos(playerM.transform.position); break;
        }
    }

    public IEnumerator MoveCamera(Vector2 destination, float moveSpeed, bool isSmooth)
    {
        Vector2 _gap = destination - (Vector2)this.transform.position;
        Vector2 _dir = _gap.normalized;

        while (Vector2.Distance(destination, (Vector2)this.transform.position) > 0.01f)
        {
            // 카메라 이동
            if (isSmooth)
                SetCameraPos(this.transform.position + new Vector3(_gap.x, _gap.y, 0) * moveSpeed * Time.deltaTime);
            else
                SetCameraPos(this.transform.position + new Vector3(_gap.x, _gap.y, 0).normalized * moveSpeed * Time.deltaTime);
            _gap = destination - (Vector2)this.transform.position;

            // 최소 크기 유지
            if (_gap.sqrMagnitude * moveSpeed < 1f)
                _gap = _gap.normalized;
            yield return null;
        }

        SetCameraPos(destination);
    }

    public IEnumerator ZoomCamera(float size, float moveSpeed, bool isSmooth)
    {
        float _gap = size - Camera.main.orthographicSize;
        float _sign = Mathf.Sign(_gap);

        while (Mathf.Abs(_gap) > 0.01f || _sign != Mathf.Sign(_gap))
        {
            if (isSmooth)
                Camera.main.orthographicSize += _gap * moveSpeed * Time.deltaTime;
            else
                Camera.main.orthographicSize += _sign * moveSpeed * Time.deltaTime;
            _gap = size - Camera.main.orthographicSize;
            yield return null;
        }

        Camera.main.orthographicSize = size;
    }
}
