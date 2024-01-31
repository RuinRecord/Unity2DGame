using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTag : MonoBehaviour
{
    private const float FADE_TIME = 0.5f;
    private const string FADE_OUT_ANIM_NAME = "PlayerTag_FadeOut";
    private const string FADE_IN_ANIM_NAME = "PlayerTag_FadeIn";

    public static PlayerType PlayerType;
    public static bool IsTagOn;

    public bool IsCanTag;
    private bool isPanelOn;

    [SerializeField] private Animation tagAnim;

    [SerializeField] private GameObject tagFrame;


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
        tagFrame.SetActive(false);

        CameraCtrl.Instance.SetCameraRect(isPanelOn);
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
            OnOffTagPanel();
        }
    }

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

    private void OnOffTagPanel()
    {
        IsTagOn = true;
        isPanelOn = !isPanelOn;

        // 애니메이션 패널 켜기
        tagAnim.gameObject.SetActive(true);

        // 페이드 애니메이션 시작
        StartCoroutine(Tag());
    }

    public void OnSwitchMode()
    {
        if (PlayerType == PlayerType.MEN)
        {
            PlayerType = PlayerType.WOMEN;
            CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
        }
        else if (PlayerType == PlayerType.WOMEN)
        {
            PlayerType = PlayerType.MEN;
            CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerM);
        }
    }

    public void OnClickTagPanel()
    {
        if (!isPanelOn)
            return; // space key 중복 처리 예외

        isPanelOn = false;

        // 페이드 애니메이션 시작
        StartCoroutine(Tag());
    }

    IEnumerator Tag()
    {
        // 페이드 아웃 애니메이션 시작
        tagAnim.Play(FADE_OUT_ANIM_NAME);
        tagAnim[FADE_OUT_ANIM_NAME].speed = 1f / FADE_TIME;
        
        yield return new WaitForSeconds(FADE_TIME);
        // 페이드 아웃 애니메이션 종료

        // 페이드 인 애니메이션 시작
        tagAnim.Play(FADE_IN_ANIM_NAME);
        tagAnim[FADE_IN_ANIM_NAME].speed = 1f / FADE_TIME;

        // 페이드 인 시작 시 기능 처리
        if (isPanelOn)
        {
            tagFrame.SetActive(true);
            UIManager.Instance.SetActiveUI(false);
        }
        else
        {
            tagFrame.SetActive(false);
            UIManager.Instance.SetActiveUI(true);
            UIManager.PlayerUI.SetPlayerUIAll(PlayerType);
        }

        CameraCtrl.Instance.SetCameraRect(isPanelOn);

        yield return new WaitForSeconds(FADE_TIME);
        // 페이드 아웃 애니메이션 종료

        // 페이드 완전 종료 시 기능 처리
        if (isPanelOn)
        {
            IsTagOn = true;
            IsCanTag = true;
        }
        else
        {
            IsTagOn = false;
            IsCanTag = true;
            tagAnim.gameObject.SetActive(false);
        }
    }
}
