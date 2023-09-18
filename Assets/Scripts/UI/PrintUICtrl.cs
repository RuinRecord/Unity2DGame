using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 출력 관련 UI 컨트롤러 클래스이다.
/// </summary>
public class PrintUICtrl : MonoBehaviour
{
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
    private GameObject selected_captureObject;


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
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
    public void CaptureInfoOn(GameObject _selectedObject)
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
}
