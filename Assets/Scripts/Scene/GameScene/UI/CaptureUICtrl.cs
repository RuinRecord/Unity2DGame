using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureUICtrl : MonoBehaviour
{
    private const float CAPTURE_CAMERA_IN_TIME = 0.5f;

    [SerializeField] private Image polaroid;

    [SerializeField] private Animation captureCameraAnim;


    public void Init()
    {
        captureCameraAnim.gameObject.SetActive(true);
        polaroid.color = new Color(1f, 1f, 1f, 0f);
    }

    public IEnumerator CaptureCameraIn()
    {
        yield return new WaitForSeconds(CAPTURE_CAMERA_IN_TIME);
        // 'CAPTURE_CAMERA_IN_TIME' 시간이 지나면

        var captureOb = PlayerCtrl.Instance.CurrentCaptureOb;

        if (captureOb != null && !GameManager.Data.player.CheckHasCapture(captureOb.Code))
        {
            // 애니메이션 시작 (여주인공의 사진 촬영 애니메이션이 끝나고 카메라 UI 애니메이션 등장)
            captureCameraAnim.Play("Camera_In");

            Sprite sprite = GameManager.Data.captureDatas[captureOb.Code].Polaroid;
            polaroid.sprite = sprite;
            polaroid.GetComponent<RectTransform>().sizeDelta  = new Vector2(sprite.rect.width, sprite.rect.height);

            // 인벤토리에 등록
            GameManager.Data.player.AddCapture(captureOb.Code);

            PlayerCtrl.Instance.IsCameraOn = true;
        }

        UIManager.PlayerUI.SetKeyOnHUD(PlayerFunction.Capture);

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

        UIManager.PlayerUI.SetKeyOnHUD(PlayerFunction.Capture);

        // 이동 및 조사 조작 가능
        PlayerCtrl.Instance.IsCanMove = true;
        PlayerCtrl.Instance.IsCanInteract = true;
        PlayerCtrl.Instance.IsCanCapture = true;
        PlayerCtrl.Instance.IsCanInven = true;

        PlayerCtrl.Instance.State = PlayerState.IDLE;

        EventCtrl.Instance.CheckEvent(EventTiming.Capture);
    }
}
