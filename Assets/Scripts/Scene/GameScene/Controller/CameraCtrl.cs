using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using URPGlitch.Runtime.AnalogGlitch;

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

    [SerializeField] private Volume glitchVolume;

    [SerializeField] private PlayerCtrl playerW, playerM;

    private VolumeProfile volumeProfile;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        volumeProfile = glitchVolume.profile;
        SetActiveGlitch(false);
        SetDefaultGlitch();
    }

    // Update is called once per frame
    void Update()
    {
        CameraProcess();
    }

    public void SetActiveGlitch(bool isActive) => glitchVolume.gameObject.SetActive(isActive);

    public void SetDefaultGlitch()
    {
        volumeProfile.TryGet(out AnalogGlitchVolume glitch);
        glitch.scanLineJitter.value = 0;
        glitch.verticalJump.value = 0.01f;
        glitch.horizontalShake.value = 0.01f;
        glitch.colorDrift.value = 0.05f;
    }

    public void GlitchEffect(float fadeTime) => StartCoroutine(GlitchEffectCoroutine(fadeTime));

    IEnumerator GlitchEffectCoroutine(float fadeTime)
    {
        volumeProfile.TryGet(out AnalogGlitchVolume glitch);
        glitchVolume.gameObject.SetActive(true);

        while (glitch.scanLineJitter.value < 1f)
        {
            glitch.scanLineJitter.value += Time.deltaTime / fadeTime;
            glitch.horizontalShake.value += Time.deltaTime / fadeTime;
            yield return null;
        }

        while (glitch.scanLineJitter.value > 0f)
        {
            glitch.scanLineJitter.value -= Time.deltaTime / fadeTime;
            glitch.horizontalShake.value -= Time.deltaTime / fadeTime;
            yield return null;
        }

        glitchVolume.gameObject.SetActive(false);
        SetDefaultGlitch();
    }

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
            SetDefaultGlitch();
        }
        else
        {
            // Tag 선택 OFF인 상태 => 전체 화면
            Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
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
        Vector2 _gap = destination;
        Vector2 _dir = _gap.normalized;
        Vector3 _cur;

        while (_gap.magnitude > 0.01f && _gap.normalized == _dir)
        {
            // 카메라 이동
            if (isSmooth)
                _cur = new Vector3(_gap.x, _gap.y, 0) * moveSpeed;
            else
                _cur = new Vector3(_gap.x, _gap.y, 0).normalized * moveSpeed;

            // 최소 크기 유지
            if (_cur.magnitude * moveSpeed < 1f)
                _cur = _cur.normalized;

            SetCameraPos(this.transform.position + _cur * Time.deltaTime);
            _gap -= (Vector2)_cur * Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ZoomCamera(float size, float moveSpeed, bool isSmooth)
    {
        float _gap = size;
        float _sign = Mathf.Sign(_gap);
        float _cur;

        while (Mathf.Abs(_gap) > 0.01f && _sign == Mathf.Sign(_gap))
        {
            if (isSmooth)
                _cur = _gap * moveSpeed;
            else
                _cur = _sign * moveSpeed;

            Camera.main.orthographicSize += _cur * Time.deltaTime;
            _gap -= _cur * Time.deltaTime;
            yield return null;
        }
    }
}
