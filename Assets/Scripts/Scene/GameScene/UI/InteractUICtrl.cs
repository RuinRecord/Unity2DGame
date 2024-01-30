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

    /// <summary> 현재 대화 시스템을 진행할 수 있는 상황인지에 대한 여부</summary>
    public bool IsDialog;


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


    /// <summary> 최근 대화 시스템에서 다루던 상호작용 오브젝트 </summary>
    private InteractionObject currentObject;


    /// <summary> 최근 대화 시스템에서 다루던 대화 리스트 </summary>
    private DialogSet[] currentDialogs;


    /// <summary> 최근 대화 시스템에서 다루던 대화 리스트의 위치 </summary>
    private int currentIdx;


    /// <summary> 최근 대화 출력 중인 코루틴 </summary>
    private Coroutine currentInfoCo;


    /// <summary> 하나의 대화(현재)를 모두 출력한 상태인지에 대한 여부 </summary>
    private bool isDoneOne;


    /// <summary> 대화 시스템에 등록된 대화 리스트를 모두 출력한 상태인지에 대한 여부 </summary>
    private bool isDoneAll;

    private bool isItemEventCheckOn;
    private bool isRecordEventCheckOn;


    public void Init()
    {
        currentIdx = 0;
        currentInfoCo = null;
        IsDialog = false;
        isDoneOne = false;
        isDoneAll = false;
        isItemEventCheckOn = false;
        isRecordEventCheckOn = false;

        interaction_panel.SetActive(false);
        player_panel.SetActive(false);
    }


    private void Update()
    {
        if (!IsDialog)
            return; // 상호작용 메세지가 작동 중이 아니라면 아래 기능을 수행하지 않음

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isDoneAll)
            {
                // 아직 모든 대화가 끝나지 않았다면
                if (!isDoneOne)
                {
                    // 아직 하나의 대화가 끝나지 않았다면

                    if (CutSceneCtrl.IsCutSceneOn)
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
                currentObject = null;
                StartCoroutine(DelayedSetInteractOn(false));

                interaction_panel.SetActive(false);
                player_panel.SetActive(false);

                // 태그 기능 해제
                PlayerTag.Instance.IsCanTag = true;

                if (isItemEventCheckOn)
                {
                    isItemEventCheckOn = false;
                    EventCtrl.Instance.CheckEvent(EventType.GetItem);
                }

                // 이벤트 체크
                if (isRecordEventCheckOn)
                {
                    isRecordEventCheckOn = false;
                    EventCtrl.Instance.CheckEvent(EventType.GetRecord);
                }

                // 만약 연출용 대화였다면
                if (CutSceneCtrl.IsCutSceneOn)
                    CutSceneCtrl.Instance.IsDialogDone = true;
            }
        }
    }


    public void StartDialog(InteractionObject interactionObject)
    {
        // 태그 기능 잠금
        PlayerTag.Instance.IsCanTag = false;

        // 최근 상호작용 메세지 및 변수 설정
        currentObject = interactionObject;
        currentDialogs = currentObject.Dialogs.ToArray();
        currentIdx = 0;

        // 출력 시작
        currentInfoCo = StartCoroutine(ShowInfoText(currentDialogs[currentIdx]));
        StartCoroutine(DelayedSetInteractOn(true));
    }



    public void StartDialog(DialogSet[] dialogs)
    {
        // 태그 기능 잠금
        PlayerTag.Instance.IsCanTag = false;

        // 최근 상호작용 메세지 및 변수 설정
        currentObject = null;
        currentDialogs = dialogs;
        currentIdx = 0;

        // 출력 시작
        currentInfoCo = StartCoroutine(ShowInfoText(currentDialogs[currentIdx]));
        StartCoroutine(DelayedSetInteractOn(true));
    }


    IEnumerator ShowInfoText(DialogSet dialog)
    {
        DialogType _dialogType = dialog.GetDialogType();
        string _words = dialog.GetWords();
        float _printTime = dialog.GetPrintTime();

        if (_printTime == 0f)
            _printTime = DEFAULT_PRINT_TIME;

        isDoneOne = false;
        SetDialogPanel(_dialogType);
        if (_dialogType.Equals(DialogType.Player))
        {
            player_leftImage.sprite = dialog.GetLeftSprite();
            player_rightImage.sprite = dialog.GetRightSprite();

            if (player_leftImage.sprite != null)
            {
                player_leftImage.GetComponent<RectTransform>().sizeDelta = dialog.GetLeftSprite().rect.size;
                player_leftImage.color = Color.white;
            }
            else
            {
                player_leftImage.color = new Color(1f, 1f, 1f, 0f);
            }

            if (player_rightImage.sprite != null)
            {
                player_rightImage.GetComponent<RectTransform>().sizeDelta = dialog.GetRightSprite().rect.size;
                player_rightImage.color = Color.white;
            }
            else
            {
                player_rightImage.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        // 만약 오디오 클립이 있다면 출력
        AudioClip _audioClip = dialog.GetAudioClip();
        if (_audioClip != null)
            GameManager.Sound.PlaySE(_audioClip);

        // 현재 여주라면
        if (PlayerTag.PlayerType.Equals(PlayerType.WOMEN))
        {
            // 아이템 체크 및 획득
            if (CheckDropItem())
            {
                currentObject.DropItem();

                // 이벤트 여부 체크
                isItemEventCheckOn = true;
            }

            // 아이템 체크 및 획득
            if (CheckDropRecord())
            {
                currentObject.DropRecord();
                ((Cabinet)currentObject)?.SetAnimOfGetItem();

                // 이벤트 여부 체크
                isRecordEventCheckOn = true;
            }
        }

        // 한글자씩 천천히 출력
        foreach (var _ch in _words)
        {
            AddInfoText(_dialogType, _ch.ToString());
            yield return new WaitForSeconds(_printTime);
        }

        // 넘기기 아이콘 출력
        SetNextActive(_dialogType, true);
        ++currentIdx;

        if (CheckLeftDialog())
            // 아직 대화가 남음 => 변수 설정
            isDoneOne = true;
        else
            // 모든 대화가 끝남 => 변수 설정
            isDoneAll = true;
    }

    private bool CheckLeftDialog() => currentIdx < currentDialogs.Length && !string.IsNullOrEmpty(currentDialogs[currentIdx].GetWords());


    private bool CheckDropItem()  => currentIdx == currentDialogs.Length - 1 && currentObject != null && currentObject.HasItem;


    private bool CheckDropRecord() => currentIdx == currentDialogs.Length - 1 && currentObject != null && currentObject.HasRecord;


    private void SetDialogPanel(DialogType dialogType)
    {
        interaction_panel.SetActive(dialogType.Equals(DialogType.Interaction));
        player_panel.SetActive(dialogType.Equals(DialogType.Player));
        SetInfoText(dialogType, "");
        SetNextActive(dialogType, false);
    }


    private void SetNextActive(DialogType dialogType, bool isActive)
    {
        if (dialogType.Equals(DialogType.Interaction))
            interaction_next_object.SetActive(isActive);
        else if (dialogType.Equals(DialogType.Player))
            player_next_object.SetActive(isActive);
    }


    private void SetInfoText(DialogType dialogType, string text)
    {
        if (dialogType.Equals(DialogType.Interaction))
            interaction_infoText.SetText(text);
        else if (dialogType.Equals(DialogType.Player))
            player_infoText.SetText(text);
    }


    private void AddInfoText(DialogType dialogType, string text)
    {
        if (dialogType.Equals(DialogType.Interaction))
            interaction_infoText.text += text;
        else if (dialogType.Equals(DialogType.Player))
            player_infoText.text += text;
    }

    
    IEnumerator DelayedSetInteractOn(bool isOn)
    {
        yield return new WaitForEndOfFrame();

        IsDialog = isOn;
    }
}
