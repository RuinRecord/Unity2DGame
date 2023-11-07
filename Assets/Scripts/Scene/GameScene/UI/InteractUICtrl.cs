using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractUICtrl : MonoBehaviour
{
    static private InteractUICtrl Instance;

    static public InteractUICtrl instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }

        get { return Instance; }
    }

    private const string FADE_IN_ANIM = "Interact_In";

    private const string FADE_OUT_ANIM = "Interact_Out";

    private const float FADE_TIME = 0.2f;

    private const float DEFAULT_PRINT_TIME = 0.05f;


    /// <summary> 페이드 애니메이션을 수행하는 컴포넌트 </summary>
    [SerializeField]
    private Animation anim;


    /// <summary> 대화를 보여주는 텍스트 </summary>
    [SerializeField]
    private TMP_Text infoText;


    /// <summary> 대화 SE 효과를 출력을 담당하는 컴포넌ㅌ, </summary>
    [SerializeField]
    private new AudioSource audio;


    /// <summary> 모든 대화가 끝나면 다음을 넘길 수 있음을 보여주는 텍스트 </summary>
    [SerializeField]
    private GameObject next_object;


    /// <summary> 최근 대화 시스템에서 다루던 대화 리스트 </summary>
    private PlayerDialog[] currentDialogs;


    /// <summary> 최근 대화 시스템에서 다루던 대화 리스트의 위치 </summary>
    private int currentIdx;


    /// <summary> 최근 대화 출력 중인 코루틴 </summary>
    private Coroutine currentInfoCo;


    /// <summary> 현재 대화 시스템을 진행할 수 있는 상황인지에 대한 여부</summary>
    public bool isInteractOn;


    /// <summary> 하나의 대화(현재)를 모두 출력한 상태인지에 대한 여부 </summary>
    private bool isDoneOne;


    /// <summary> 대화 시스템에 등록된 대화 리스트를 모두 출력한 상태인지에 대한 여부 </summary>
    private bool isDoneAll;


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        currentIdx = 0;
        currentInfoCo = null;
        isInteractOn = false;
        isDoneOne = false;
        isDoneAll = false;
        next_object.SetActive(false);
    }


    private void Update()
    {
        if (!isInteractOn)
            return; // 상호작용 메세지가 작동 중이 아니라면 아래 기능을 수행하지 않음

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isDoneAll)
            {
                // 아직 모든 대화가 끝나지 않았다면
                if (!isDoneOne)
                {
                    // 아직 하나의 대화가 끝나지 않았다면

                    // 현재 대화 중지
                    StopCoroutine(currentInfoCo);

                    // 현재 대화 바로 출력
                    infoText.SetText(currentDialogs[currentIdx++].GetWords());

                    // 넘기기 아이콘 출력
                    next_object.SetActive(true);

                    if (CheckDialog())
                        // 아직 대화가 남음 => 변수 설정
                        isDoneOne = true; 
                    else
                        // 모든 대화가 끝남 => 변수 설정
                        isDoneAll = true; 
                }
                else
                {
                    // 하나의 대화가 모두 끝났다면

                    // 넘기기 아이콘 출력
                    next_object.SetActive(true);

                    if (CheckDialog())
                        // 아직 대화가 남음 => 다음 대화 출력
                        currentInfoCo = StartCoroutine(ShowInfoText(currentDialogs[currentIdx])); 
                    else
                        // 모든 대화가 끝남 => 변수 설정
                        isDoneAll = true; 
                }
            }
            else
            {
                // 모든 대화가 끝났다면 => 창 닫기
                isDoneOne = isDoneAll = false;
                next_object.SetActive(false);
                StartCoroutine(DelayedSetInteractOn(false));

                anim.Play(FADE_OUT_ANIM);
                anim[FADE_OUT_ANIM].speed = 1f / FADE_TIME;
            }
        }
    }


    /// <summary>
    /// 대화 시스템을 시작하는 함수이다.
    /// </summary>
    /// <param name="_dialogs">출력될 모든 대화 리스트</param>
    public void StartDialog(PlayerDialog[] _dialogs)
    {
        // 대화창이 켜지는 애니메이션 수행
        anim.Play(FADE_IN_ANIM);
        anim[FADE_IN_ANIM].speed = 1f / FADE_TIME;

        // 최근 상호작용 메세지 및 변수 설정
        currentDialogs = _dialogs;

        isInteractOn = true;
        currentIdx = 0;

        // 출력 시작
        currentInfoCo = StartCoroutine(ShowInfoText(currentDialogs[currentIdx]));
    }


    /// <summary>
    /// 하나의 대화를 출력하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_dialog">출력될 하나의 대화</param>
    IEnumerator ShowInfoText(PlayerDialog _dialog)
    {
        infoText.SetText("");
        isDoneOne = false;
        next_object.SetActive(false);

        // 만약 오디오 클립이 있다면 출력
        AudioClip audioClip = _dialog.GetAudioClip();
        if (audioClip != null)
            PlayAudio(audioClip);

        // 대화를 한 문자씩 천천히 출력
        string words = _dialog.GetWords();
        float printTime = _dialog.GetPrintTime();
        if (printTime == 0) printTime = DEFAULT_PRINT_TIME;

        foreach (var ch in words)
        {
            infoText.text += ch;
            yield return new WaitForSeconds(printTime);
        }

        // 넘기기 아이콘 출력
        next_object.SetActive(true);
        ++currentIdx;

        if (CheckDialog())
            // 아직 대화가 남음 => 변수 설정
            isDoneOne = true;
        else
            // 모든 대화가 끝남 => 변수 설정
            isDoneAll = true;
    }


    public void PlayAudio(AudioClip _audioClip)
    {
        audio.clip = _audioClip;
        audio.Play();
    }


    /// <summary> 대사가 아직 존재하는지에 대한 여부를 반환하는 함수이다. </summary>
    private bool CheckDialog() => currentIdx < currentDialogs.Length && !string.IsNullOrEmpty(currentDialogs[currentIdx].GetWords());


    /// <summary>
    /// 0.1초 뒤에 isInteractOn 변수를 후처리하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_isOn"></param>
    IEnumerator DelayedSetInteractOn(bool _isOn)
    {
        yield return new WaitForSeconds(0.1f);

        isInteractOn = _isOn;
    }
}
