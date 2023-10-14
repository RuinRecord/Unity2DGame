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

    [SerializeField]
    private Animation anim;

    [SerializeField]
    private TMP_Text infoText;

    [SerializeField]
    private GameObject next_object;

    private Dialog[] currentDialogs;
    private int currentIdx;
    private Coroutine currentInfoCo;

    public bool isInteractOn;
    private bool isDoneOne;
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
                    infoText.SetText(currentDialogs[currentIdx++].dialog);

                    // 넘기기 아이콘 출력
                    next_object.SetActive(true);

                    if (currentIdx < currentDialogs.Length)
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

                    if (currentIdx < currentDialogs.Length)
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

    public void StartDialog(Dialog[] _dialogs)
    {
        // 현재 플레이어가 움직이는 중이라면 멈추도록 명령
        PlayerCtrl.instance.StopMove();

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

    IEnumerator ShowInfoText(Dialog _dialog)
    {
        infoText.SetText("");
        isDoneOne = false;
        next_object.SetActive(false);

        foreach (var ch in _dialog.dialog)
        {
            infoText.text += ch;
            yield return new WaitForSeconds(_dialog.print_time);
        }

        // 넘기기 아이콘 출력
        next_object.SetActive(true);

        if (++currentIdx < currentDialogs.Length)
            // 아직 대화가 남음 => 변수 설정
            isDoneOne = true;
        else
            // 모든 대화가 끝남 => 변수 설정
            isDoneAll = true;
    }

    IEnumerator DelayedSetInteractOn(bool _isOn)
    {
        yield return new WaitForSeconds(0.1f);

        isInteractOn = _isOn;
    }
}
