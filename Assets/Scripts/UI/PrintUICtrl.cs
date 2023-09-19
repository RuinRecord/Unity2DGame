using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 출력 관련 UI 컨트롤러 클래스이다.
/// </summary>
public class PrintUICtrl : MonoBehaviour
{
    private const float CAPTURE_CAMERA_IN_TIME = 0.5f;

    /// <summary> 조사 상호작용 오브젝트 생성 위치 조정 벡터 </summary>
    private static Vector3 capture_upVec = Vector3.up * 0.75f;


    /// <summary> PrintUICtrl 싱글톤 </summary>
    private static PrintUICtrl Instance;
    public static PrintUICtrl instance
    {
        set 
        {
            if (Instance == null)
                Instance = value; 
        }
        get { return Instance; }
    }


    /// <summary> 조사 상호작용 오브젝트와 충돌할 때 나오는 UI 이미지 </summary>
    [SerializeField]
    private RectTransform captureInfo;


    /// <summary> 현재 상호작용중인(충돌한) 오브젝트 </summary>
    private CaptureObject selected_captureObject;


    /// <summary> 조사 카메라 연출 애니메이션 </summary>
    [SerializeField]
    private Animation captureCameraAnim;


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        captureCameraAnim.gameObject.SetActive(true);

        CaptureInfoOff();
    }


    void Update()
    {
        if (selected_captureObject != null) 
        {
            // 현재 선택된 조사 가능 물체 존재
            // -> 실시간으로 이미지 UI를 물체 위에 위치 시키기 (카메라가 이동하기 때문에 조정)
            captureInfo.position = Camera.main.WorldToScreenPoint(selected_captureObject.transform.position + capture_upVec);
        }
    }


    /// <summary>
    /// 현재 상호작용 중인 오브젝트를 'selectedObject'로 설정하고 조사 가능 UI 이미지를 켜는 함수이다.
    /// </summary>
    /// <param name="_selectedObject">충돌한 상호작용 오브젝트</param>
    public void CaptureInfoOn(CaptureObject _selectedObject)
    {
        captureInfo.gameObject.SetActive(true);

        selected_captureObject = _selectedObject;
        captureInfo.position = Camera.main.WorldToScreenPoint(_selectedObject.transform.position + capture_upVec);
    }


    /// <summary>
    /// 조사 가능 UI 이미지 끄는 함수이다.
    /// </summary>
    public void CaptureInfoOff()
    {
        captureInfo.gameObject.SetActive(false);

        selected_captureObject = null;
    }

    /// <summary>
    /// 카메라 UI가 켜지도록 하는 코루틴 함수이다.
    /// </summary>

    public IEnumerator CaptureCameraIn()
    {
        yield return new WaitForSeconds(CAPTURE_CAMERA_IN_TIME);
        // 'CAPTURE_CAMERA_IN_TIME' 시간이 지나면

        // 애니메이션 시작 (여주인공의 사진 촬영 애니메이션이 끝나고 카메라 UI 애니메이션 등장)
        captureCameraAnim.Play("Camera_In");

        // 조사 이벤트 등록
        MapCtrl.instance.AddCapture(selected_captureObject.code);

        // 조사 조작 가능
        PlayerCtrl.instance.isCanCapture = true;
    }

    /// <summary>
    /// 카메라 UI가 꺼지도록 하는 코루틴 함수이다.
    /// </summary>
    public IEnumerator CaptureCameraOut()
    {
        // 애니메이션 바로 끄기
        captureCameraAnim.Play("Camera_Out");

        yield return new WaitForSeconds(CAPTURE_CAMERA_IN_TIME);
        // 'CAPTURE_CAMERA_IN_TIME' 시간이 지나면

        // 이동 및 조사 조작 가능
        PlayerCtrl.instance.isCanMove = true;
        PlayerCtrl.instance.isCanCapture = true;
    }
}
