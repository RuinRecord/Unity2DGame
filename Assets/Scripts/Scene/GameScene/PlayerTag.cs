using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTag : MonoBehaviour
{
    /// <summary> 페이드 애니메이션 진행 시간 </summary>
    private const float FADE_TIME = 0.5f;


    /// <summary> 페이드 아웃 애니메이션 이름 </summary>
    private const string FADE_OUT_ANIM_NAME = "PlayerTag_FadeOut";


    /// <summary> 페이드 인 애니메이션 이름 </summary>
    private const string FADE_IN_ANIM_NAME = "PlayerTag_FadeIn";


    /// <summary> 현재 태그 중인 플레이어 타입 </summary>
    public static PlayerType PlayerType;


    /// <summary> 현재 태그 기능이 동작 중인가? </summary>
    public static bool IsTagOn;


    /// <summary> 현재 태그 가능 상태인지에 대한 여부 </summary>
    public bool IsCanTag;


    /// <summary> 현재 태그 패널이 동작 중인가? </summary>
    private bool isPanelOn;


    /// <summary> 남주인공 및 여주인공의 카메라 </summary>
    [SerializeField]
    private Camera cameraM, cameraW;


    /// <summary> 태그 연출을 위한 애니메이션 </summary>
    [SerializeField]
    private Animation tagAnim;


    /// <summary> 태그 프레임 UI 오브젝트 </summary>
    [SerializeField]
    private GameObject tag_frame;


    /// <summary> PlayerTag 싱글톤 </summary>
    private static PlayerTag instance;
    public static PlayerTag Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }


    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        PlayerType = PlayerType.WOMEN;
        isPanelOn = false;
        IsCanTag = true;

        tagAnim.gameObject.SetActive(false);
        tag_frame.SetActive(false);
        SetCameraRect(PlayerType, isPanelOn);
        UIManager.PlayerUI.SetPlayerUIAll(PlayerType);
    }


    // Update is called once per frame
    void Update()
    {
        if (!CheckCanTag())
            return;

        if (IsCanTag && Input.GetKeyDown(KeyCode.Tab))
        {
            // 태그 패널 열기
            IsCanTag = false;
            ShowTagPanel();
        }
    }

    /// <summary>
    /// 현재 플레이어 태그가 가능한 지 체크하는 함수이다.
    /// </summary>
    private bool CheckCanTag()
    {
        if (GameManager.Change.IsChanging)
            return false; // 현재 씬 및 위치 전환 중이면 동작 불가

        if (CutSceneCtrl.IsCutSceneOn)
            return false; // 컷씬이 진행중이면 동작 불가

        if (UIManager.InteractUI.IsDialog)
            return false; // 현재 상호작용 대화 시스템이 작동 중이면 동작 불가

        if (PlayerCtrl.Instance.IsMoving)
            return false; // 플레이어가 이동 중이면 동작 불가

        return true;
    }



    /// <summary>
    /// 게임 카메라를 설정하는 함수이다.
    /// </summary>
    /// <param name="_playerType">플레이어 타입</param>
    /// <param name="_isPanelOn">태그 상태</param>
    private void SetCameraRect(PlayerType _playerType, bool _isPanelOn)
    {
        if (_isPanelOn)
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
        IsTagOn = true;
        isPanelOn = true;

        // 애니메이션 패널 켜기
        tagAnim.gameObject.SetActive(true);

        // 페이드 애니메이션 시작
        StartCoroutine(Fade(0));
    }


    /// <summary>
    /// (버튼 이벤트 함수) 플레이어 태그 패널을 클릭했을 때, 해당 플레이어로 설정하는 함수이다.
    /// </summary>
    public void OnClickTagPanel()
    {
        if (!isPanelOn)
            return; // space key 중복 처리 예외

        if (EventSystem.current.currentSelectedGameObject == null)
            return; // 선택된 오브젝트가 없음

        var uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uibox == null)
            return; // uibox가 없는 오브젝트를 선택

        // 플레이어 타입 설정
        switch (uibox.index)
        {
            case 0: PlayerType = PlayerType.MEN; break;
            case 1: PlayerType = PlayerType.WOMEN; break;
        }
        isPanelOn = false;

        // 페이드 애니메이션 시작
        StartCoroutine(Fade(1));
    }


    /// <summary>
    /// 페이드 애니메이션을 수행하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_type">페이드 이벤트 타입(0 = 패널 열기 / 1 = 패널 닫기)</param>
    /// <returns></returns>
    IEnumerator Fade(int _type)
    {
        // 페이드 인 애니메이션 시작
        tagAnim.Play(FADE_OUT_ANIM_NAME);
        tagAnim[FADE_OUT_ANIM_NAME].speed = 1f / FADE_TIME;
        
        yield return new WaitForSeconds(FADE_TIME);
        // 페이드 인 애니메이션 종료

        // 페이드 아웃 애니메이션 시작
        tagAnim.Play(FADE_IN_ANIM_NAME);
        tagAnim[FADE_IN_ANIM_NAME].speed = 1f / FADE_TIME;

        // 페이드 아웃 시작 시 기능 처리
        switch (_type)
        {
            case 0:
                tag_frame.SetActive(true);
                UIManager.Instance.SetActiveUI(false);
                break;
            case 1:
                tag_frame.SetActive(false);
                UIManager.Instance.SetActiveUI(true);
                UIManager.PlayerUI.SetPlayerUIAll(PlayerType);
                break;
        }

        SetCameraRect(PlayerType, isPanelOn);

        yield return new WaitForSeconds(FADE_TIME);
        // 페이드 아웃 애니메이션 종료

        // 페이드 완전 종료 시 기능 처리

        switch (_type)
        {
            case 1:
                // 태그 패널 닫기
                tagAnim.gameObject.SetActive(false);
                IsTagOn = false;
                IsCanTag = true;
                break;
        }
    }
}
