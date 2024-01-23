using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractUICtrl : MonoBehaviour
{
    private const float DEFAULT_PRINT_TIME = 0.05f;

    [Header("[ 상호작용 대화창 전용 변수 ]")]

    [SerializeField]
    private GameObject interaction_panel;


    /// <summary> 상화작용: 대화를 보여주는 텍스트 </summary>
    [SerializeField]
    private TMP_Text interaction_infoText;


    /// <summary> 상화작용: 모든 대화가 끝나면 다음을 넘길 수 있음을 보여주는 텍스트 </summary>
    [SerializeField]
    private GameObject interaction_next_object;




    [Header("[ 플레이어 대화창 전용 변수 ]")]

    [SerializeField]
    private GameObject player_panel;


    /// <summary> 플레이어: 왼쪽 프로필 이미지 </summary>
    [SerializeField]
    private Image player_leftImage;


    /// <summary> 플레이어: 오른쪽 프로필 이미지 </summary>
    [SerializeField]
    private Image player_rightImage;


    /// <summary> 플레이어: 대화를 보여주는 텍스트 </summary>
    [SerializeField]
    private TMP_Text player_infoText;


    /// <summary> 플레이어: 모든 대화가 끝나면 다음을 넘길 수 있음을 보여주는 텍스트 </summary>
    [SerializeField]
    private GameObject player_next_object;


    /// <summary> 최근 대화 시스템에서 다루던 캐비넷 오브젝트 </summary>
    private Cabinet currentCabinet;


    /// <summary> 최근 대화 시스템에서 다루던 상호작용 오브젝트 </summary>
    private InteractionObject currentObject;


    /// <summary> 최근 대화 시스템에서 다루던 대화 리스트 </summary>
    private DialogSet[] currentDialogs;


    /// <summary> 최근 대화 시스템에서 다루던 대화 리스트의 위치 </summary>
    private int currentIdx;


    /// <summary> 최근 대화 출력 중인 코루틴 </summary>
    private Coroutine currentInfoCo;


    /// <summary> 현재 대화 시스템을 진행할 수 있는 상황인지에 대한 여부</summary>
    public bool isDialog;


    /// <summary> 하나의 대화(현재)를 모두 출력한 상태인지에 대한 여부 </summary>
    private bool isDoneOne;


    /// <summary> 대화 시스템에 등록된 대화 리스트를 모두 출력한 상태인지에 대한 여부 </summary>
    private bool isDoneAll;


    public void Init()
    {
        currentIdx = 0;
        currentInfoCo = null;
        isDialog = false;
        isDoneOne = false;
        isDoneAll = false;

        interaction_panel.SetActive(false);
        player_panel.SetActive(false);
    }


    private void Update()
    {
        if (!isDialog)
            return; // 상호작용 메세지가 작동 중이 아니라면 아래 기능을 수행하지 않음

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isDoneAll)
            {
                // 아직 모든 대화가 끝나지 않았다면
                if (!isDoneOne)
                {
                    // 아직 하나의 대화가 끝나지 않았다면

                    if (CutSceneCtrl.isCutSceneOn)
                        return; // 컷씬 전용 대사면 상호작용 불가능

                    // 현재 대화 중지
                    StopCoroutine(currentInfoCo);

                    // 현재 대화 바로 출력
                    SetInfoText(currentDialogs[currentIdx].GetDialogType(), currentDialogs[currentIdx].GetWords());

                    // 넘기기 아이콘 출력
                    SetNextActive(currentDialogs[currentIdx].GetDialogType(), false);

                    currentIdx++;
                    if (CheckLeftDialog())
                        // 아직 대화가 남음 => 변수 설정
                        isDoneOne = true; 
                    else
                        // 모든 대화가 끝남 => 변수 설정
                        isDoneAll = true; 
                }
                else
                {
                    // 하나의 대화가 모두 끝났다면

                    if (CheckLeftDialog())
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
                currentCabinet = null;
                currentObject = null;
                StartCoroutine(DelayedSetInteractOn(false));

                interaction_panel.SetActive(false);
                player_panel.SetActive(false);

                // 태그 기능 해제
                PlayerTag.instance.isCanTag = true;

                // 만약 연출용 대화였다면
                if (CutSceneCtrl.isCutSceneOn)
                    CutSceneCtrl.instance.isDialogDone = true;
            }
        }
    }


    /// <summary>
    /// 상호작용 오브젝트로 대화 시스템을 시작하는 함수이다.
    /// </summary>
    /// <param name="interactionObject">출력을 수행할 상호작용 오브젝트</param>
    public void StartDialog(InteractionObject interactionObject)
    {
        // 태그 기능 잠금
        PlayerTag.instance.isCanTag = false;

        // 최근 상호작용 메세지 및 변수 설정
        currentCabinet = null;
        currentObject = interactionObject;
        currentDialogs = currentObject.GetDialogs().ToArray();
        currentIdx = 0;

        // 출력 시작
        currentInfoCo = StartCoroutine(ShowInfoText(currentDialogs[currentIdx]));
        StartCoroutine(DelayedSetInteractOn(true));
    }



    /// <summary>
    /// 상호작용 오브젝트로 대화 시스템을 시작하는 함수이다.
    /// </summary>
    /// <param name="interactionObject">출력을 수행할 상호작용 오브젝트</param>
    public void StartDialog(Cabinet cabinet)
    {
        // 태그 기능 잠금
        PlayerTag.instance.isCanTag = false;

        // 최근 상호작용 메세지 및 변수 설정
        currentCabinet = cabinet;
        currentObject = currentCabinet.InteractionOb;
        currentDialogs = currentObject.GetDialogs().ToArray();
        currentIdx = 0;

        // 출력 시작
        currentInfoCo = StartCoroutine(ShowInfoText(currentDialogs[currentIdx]));
        StartCoroutine(DelayedSetInteractOn(true));
    }


    /// <summary>
    /// 대사로 대화 시스템을 시작하는 함수이다.
    /// </summary>
    /// <param name="dialogs">출력을 수행할 대사</param>
    public void StartDialog(DialogSet[] dialogs)
    {
        // 태그 기능 잠금
        PlayerTag.instance.isCanTag = false;

        // 최근 상호작용 메세지 및 변수 설정
        currentCabinet = null;
        currentObject = null;
        currentDialogs = dialogs;
        currentIdx = 0;

        // 출력 시작
        currentInfoCo = StartCoroutine(ShowInfoText(currentDialogs[currentIdx]));
        StartCoroutine(DelayedSetInteractOn(true));
    }


    /// <summary>
    /// 하나의 대화를 출력하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_dialog">출력될 하나의 대화</param>
    IEnumerator ShowInfoText(DialogSet _dialog)
    {
        DialogType dialogType = _dialog.GetDialogType();
        string words = _dialog.GetWords();
        float printTime = _dialog.GetPrintTime();

        if (printTime == 0f)
            printTime = DEFAULT_PRINT_TIME;

        isDoneOne = false;
        SetDialogPanel(dialogType);
        if (dialogType.Equals(DialogType.Player))
        {
            player_leftImage.sprite = _dialog.GetLeftSprite();
            player_rightImage.sprite = _dialog.GetRightSprite();

            if (player_leftImage.sprite != null)
            {
                player_leftImage.GetComponent<RectTransform>().sizeDelta = _dialog.GetLeftSprite().rect.size;
                player_leftImage.color = Color.white;
            }
            else
            {
                player_leftImage.color = new Color(1f, 1f, 1f, 0f);
            }

            if (player_rightImage.sprite != null)
            {
                player_rightImage.GetComponent<RectTransform>().sizeDelta = _dialog.GetRightSprite().rect.size;
                player_rightImage.color = Color.white;
            }
            else
            {
                player_rightImage.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        // 만약 오디오 클립이 있다면 출력
        AudioClip audioClip = _dialog.GetAudioClip();
        if (audioClip != null)
            GameManager._sound.PlaySE(audioClip);

        // 현재 여주라면
        if (PlayerTag.playerType.Equals(PlayerType.WOMEN))
        {
            // 아이템 체크 및 획득
            if (CheckDropItem())
            {
                currentObject.DropItem();

                // 획득 사운드 출력
                GameManager._sound.PlaySE("아이템획득");
            }

            // 아이템 체크 및 획득
            if (CheckDropRecord())
            {
                currentObject.DropRecord();
                currentCabinet.SetAnimOfGetItem();

                EventCtrl.Instance.CheckEvent(EventType.Interact);

                // 획득 사운드 출력
                GameManager._sound.PlaySE("아이템획득");
            }
        }

        // 한글자씩 천천히 출력
        foreach (var ch in words)
        {
            AddInfoText(dialogType, ch.ToString());
            yield return new WaitForSeconds(printTime);
        }

        // 넘기기 아이콘 출력
        SetNextActive(dialogType, true);
        ++currentIdx;

        if (CheckLeftDialog())
            // 아직 대화가 남음 => 변수 설정
            isDoneOne = true;
        else
            // 모든 대화가 끝남 => 변수 설정
            isDoneAll = true;
    }


    /// <summary> 대사가 아직 존재하는지에 대한 여부를 반환하는 함수이다. </summary>
    private bool CheckLeftDialog()
        => currentIdx < currentDialogs.Length && !string.IsNullOrEmpty(currentDialogs[currentIdx].GetWords());


    /// <summary> 아이템 획득이 가능한지 체크하는 함수이다. </summary>
    private bool CheckDropItem() 
        => currentIdx == currentDialogs.Length - 1 && currentObject != null && currentObject.hasItem;


    /// <summary> 조사일지 획득이 가능한지 체크하는 함수이다. </summary>
    private bool CheckDropRecord()
        => currentIdx == currentDialogs.Length - 1 && currentObject != null && currentObject.hasRecord;


    /// <summary> 페널 타입에 따른 페널 설정 함수이다. </summary>
    private void SetDialogPanel(DialogType dialogType)
    {
        interaction_panel.SetActive(dialogType.Equals(DialogType.Interaction));
        player_panel.SetActive(dialogType.Equals(DialogType.Player));
        SetInfoText(dialogType, "");
        SetNextActive(dialogType, false);
    }


    /// <summary> 페널 타입에 따른 다음 알림 버튼을 끄거나 키는 함수이다. </summary>
    private void SetNextActive(DialogType dialogType, bool isActive)
    {
        if (dialogType.Equals(DialogType.Interaction))
            interaction_next_object.SetActive(isActive);
        else if (dialogType.Equals(DialogType.Player))
            player_next_object.SetActive(isActive);
    }


    /// <summary> 페널 타입에 따른 텍스트 설정 함수이다. </summary>
    private void SetInfoText(DialogType dialogType, string text)
    {
        if (dialogType.Equals(DialogType.Interaction))
            interaction_infoText.SetText(text);
        else if (dialogType.Equals(DialogType.Player))
            player_infoText.SetText(text);
    }


    /// <summary> 페널 타입에 따른 텍스트 추가 함수이다. </summary>
    private void AddInfoText(DialogType dialogType, string text)
    {
        if (dialogType.Equals(DialogType.Interaction))
            interaction_infoText.text += text;
        else if (dialogType.Equals(DialogType.Player))
            player_infoText.text += text;
    }


    /// <summary>
    /// 1 프레임 뒤에 isInteractOn 변수를 후처리하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_isOn"></param>
    IEnumerator DelayedSetInteractOn(bool _isOn)
    {
        yield return new WaitForEndOfFrame();

        isDialog = _isOn;
    }
}
