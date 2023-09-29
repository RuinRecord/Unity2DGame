using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTag : MonoBehaviour
{
    public static PlayerType playerType;

    public static bool isTagOn;

    [SerializeField]
    private Camera cameraM, cameraW;

    [SerializeField]
    private Animator cameraAnimator;

    private bool isCanTag;

    // Start is called before the first frame update
    void Start()
    {
        playerType = PlayerType.WOMEN;
        isTagOn = false;
        isCanTag = true;

        cameraAnimator.gameObject.SetActive(false);
        SetCameraRect(playerType, isTagOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCanTag && Input.GetKeyDown(KeyCode.Tab))
        {
            isCanTag = false;
            ShowTagPanel();
        }
    }

    private void SetCameraRect(PlayerType _playerType, bool _isTagOn)
    {
        if (_isTagOn)
        {
            // Tag Panel 선택 UI에 맞춰 카메라 설정
            cameraM.gameObject.SetActive(true);
            cameraW.gameObject.SetActive(true);
            cameraM.rect = new Rect(0f, 0f, 0.5f, 1f);
            cameraW.rect = new Rect(0.5f, 0f, 1f, 1f);
        }
        else
        {
            // Tag 선택 완료 => 플레이어 타입에 맞춰 카메라 설정
            switch (_playerType)
            {
                case PlayerType.MEN:
                    cameraM.gameObject.SetActive(true);
                    cameraW.gameObject.SetActive(false);
                    cameraM.rect = new Rect(0f, 0f, 1f, 1f);
                    break;
                case PlayerType.WOMEN:
                    cameraM.gameObject.SetActive(false);
                    cameraW.gameObject.SetActive(true);
                    cameraW.rect = new Rect(0f, 0f, 1f, 1f);
                    break;
            }
        }
    }

    private void ShowTagPanel()
    {
        isTagOn = true;
        cameraAnimator.gameObject.SetActive(true);
        PlayerCtrl.instance.StopMove();
        SetCameraRect(playerType, isTagOn);
    }

    public void OnClickTagPanel()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return; // 선택된 오브젝트가 없음

        var uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uibox == null)
            return; // uibox가 없는 오브젝트를 선택

        // 플레이어 카메라 전환
        cameraAnimator.gameObject.SetActive(false);
        switch (uibox.index)
        {
            case 0: playerType = PlayerType.MEN; break;
            case 1: playerType = PlayerType.WOMEN; break;
        }
        isTagOn = false;
        isCanTag = true;
        SetCameraRect(playerType, isTagOn);
    }
}
