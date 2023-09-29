using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTag : MonoBehaviour
{
    /// <summary> 현재 태그 중인 플레이어 타입 </summary>
    public static PlayerType playerType;


    /// <summary> 현재 태그 선택 중인지에 대한 여부 </summary>
    public static bool isTagOn;


    /// <summary> 남주인공 및 여주인공의 카메라 </summary>
    [SerializeField]
    private Camera cameraM, cameraW;


    /// <summary> 태그 연출을 위한 애니메이션 </summary>
    [SerializeField]
    private Animator tagAnimator;


    /// <summary> 현재 태그 가능 상태인지에 대한 여부 </summary>
    private bool isCanTag;


    // Start is called before the first frame update
    void Start()
    {
        playerType = PlayerType.WOMEN;
        isTagOn = false;
        isCanTag = true;

        tagAnimator.gameObject.SetActive(false);
        SetCameraRect(playerType, isTagOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCanTag && Input.GetKeyDown(KeyCode.Tab))
        {
            // 태그 패널 열기
            isCanTag = false;
            ShowTagPanel();
        }
    }

    /// <summary>
    /// 게임 카메라를 설정하는 함수이다.
    /// </summary>
    /// <param name="_playerType">플레이어 타입</param>
    /// <param name="_isTagOn">태그 상태</param>
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

    /// <summary>
    /// 태그 패널 UI를 출력하는 함수이다.
    /// </summary>
    private void ShowTagPanel()
    {
        isTagOn = true;
        tagAnimator.gameObject.SetActive(true);

        // 현재 플레이어가 움직이는 중이라면 멈추도록 명령
        PlayerCtrl.instance.StopMove();

        // 태그 선택 카메라로 설정
        SetCameraRect(playerType, isTagOn);
    }

    /// <summary>
    /// (버튼 이벤트 함수) 플레이어 태그 패널을 클릭했을 때, 해당 플레이어로 설정하는 함수이다.
    /// </summary>
    public void OnClickTagPanel()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return; // 선택된 오브젝트가 없음

        var uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uibox == null)
            return; // uibox가 없는 오브젝트를 선택

        // 태그 패널 닫기
        tagAnimator.gameObject.SetActive(false);

        // 플레이어 타입 설정
        switch (uibox.index)
        {
            case 0: playerType = PlayerType.MEN; break;
            case 1: playerType = PlayerType.WOMEN; break;
        }
        isTagOn = false;
        isCanTag = true;

        // 변경된 플레이어 타입에 따라 카메라 설정
        SetCameraRect(playerType, isTagOn);
    }
}
