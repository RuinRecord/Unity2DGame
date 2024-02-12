using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureUICtrl : MonoBehaviour
{
    private const float CAPTURE_CAMERA_IN_TIME = 0.5f;


    /// <summary> 현재 조사 오브젝트와 충돌한 오브젝트 </summary>
    private CaptureObject selected_captureObject;


    /// <summary> 조사 카메라 연출 애니메이션 </summary>
    [SerializeField]
    private Animation captureCameraAnim;


    public void Init()
    {
        captureCameraAnim.gameObject.SetActive(true);

        CaptureInfoOff();
    }

    public void CaptureInfoOn(CaptureObject _selectedObject)
        => selected_captureObject = _selectedObject;


    public void CaptureInfoOff()
        => selected_captureObject = null;


    public IEnumerator CaptureCameraIn()
    {
        yield return new WaitForSeconds(CAPTURE_CAMERA_IN_TIME);
        // 'CAPTURE_CAMERA_IN_TIME' 시간이 지나면

        if (selected_captureObject != null && !GameManager.Data.player.CheckHasCapture(selected_captureObject.Code))
        {
            // 애니메이션 시작 (여주인공의 사진 촬영 애니메이션이 끝나고 카메라 UI 애니메이션 등장)
            captureCameraAnim.Play("Camera_In");

            // 인벤토리에 등록
            GameManager.Data.player.AddCapture(selected_captureObject.Code);

            PlayerCtrl.Instance.IsCameraOn = true;
        }

        // 이동 및 조사 조작 가능
        PlayerCtrl.Instance.IsCanMove = true;
        PlayerCtrl.Instance.IsCanInteract = true;
        PlayerCtrl.Instance.IsCanCapture = true;
        PlayerCtrl.Instance.IsCanInven = true;

        PlayerCtrl.Instance.State = PlayerState.IDLE;
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
        PlayerCtrl.Instance.IsCanMove = true;
        PlayerCtrl.Instance.IsCanInteract = true;
        PlayerCtrl.Instance.IsCanCapture = true;
        PlayerCtrl.Instance.IsCanInven = true;

        PlayerCtrl.Instance.State = PlayerState.IDLE;

        EventCtrl.Instance.CheckEvent(EventType.Capture);
    }
}
