using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintUICtrl : MonoBehaviour
{
    private static Vector3 capture_upVec = Vector3.up * 0.75f;
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

    [SerializeField]
    private RectTransform captureInfo;

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
            // ���� ���õ� ���� ���� ��ü ����
            // -> UI�� ��ü ���� ��ġ ��Ű��
            captureInfo.position = Camera.main.WorldToScreenPoint(selected_captureObject.transform.position + capture_upVec);
        }
    }

    public void CaptureInfoOn(GameObject _selectedObject)
    {
        captureInfo.gameObject.SetActive(true);

        selected_captureObject = _selectedObject;
        captureInfo.position = Camera.main.WorldToScreenPoint(_selectedObject.transform.position + capture_upVec);
    }

    public void CaptureInfoOff()
    {
        captureInfo.gameObject.SetActive(false);

        selected_captureObject = null;
    }
}
