using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InvenUICtrl : MonoBehaviour
{
    private readonly float itemSlotImageSize = 200;

    private readonly float itemInfoImageSize = 350;

    private readonly float galleryAnimTime = 0.75f;

    private readonly string galleryAnimName_In = "CaptureCard_In";

    private readonly string galleryAnimName_Out = "CaptureCard_Out";

    [SerializeField]
    private GameObject InvenObject;

    [SerializeField]
    private UIBox[] menus;

    [SerializeField]
    private GameObject[] contents;

    public bool isOnInven;

    [SerializeField]
    private int ContentIndex;

    private int contentIndex
    {
        set 
        { 
            ContentIndex = value;
            SetMenus(ContentIndex);
            SetContents(ContentIndex);
        }
        get { return ContentIndex; }
    }



    [Header("[ Item Fields ]")]
    [SerializeField]
    private GameObject itemSlot;

    [SerializeField]
    private Transform itemContentTr;

    [SerializeField]
    private GameObject itemInfoOb;

    [SerializeField]
    private Image itemInfoImage;

    [SerializeField]
    private TMP_Text itemInfoText;



    [Header("[ Gallery Fields ]")]
    [SerializeField]
    private GameObject captureSlot;

    [SerializeField]
    private Transform captureContentTr;

    [SerializeField]
    private Animation galleryAnim;

    [SerializeField]
    private Image captureCardImage;

    [SerializeField]
    private TMP_Text captureCardText;

    private bool isCanTouchCard;



    [Header("[ Record Fields ]")]
    [SerializeField]
    private GameObject recordSlot;

    [SerializeField]
    private Transform recordContentTr;

    [SerializeField]
    private GameObject recordInfoOb;

    [SerializeField]
    private TMP_Text recordInfoText;



    public void Init()
    {
        isOnInven = false;
        contentIndex = 0;
        InvenObject.SetActive(false);
    }

    public void OnOffInven()
    {
        isOnInven = !isOnInven;
        contentIndex = 0;
        InvenObject.SetActive(isOnInven);

        GameManager._sound.PlaySE("가방여닫기");
    }

    public void SetMenus(int index)
    {
        for (int i = 0; i < menus.Length; i++)
            menus[i].images[0].color = Color.white * 0.4f;
        menus[index].images[0].color = Color.white * 0.8f;
    }

    public void SetContents(int index)
    {
        for (int i = 0; i < contents.Length; i++)
            contents[i].SetActive(false);
        contents[index].SetActive(true);

        itemInfoOb.SetActive(false);
        galleryAnim.gameObject.SetActive(false);
        recordInfoOb.SetActive(false);

        switch (index)
        {
            case 0: SetItemContent(); break;    // Item Setting
            case 1: SetGalleryContent(); break; // Gallery Setting
            case 2: SetRecordContent(); break;  // Record Setting
        }
    }

    public void OnMenuButton()
    {
        UIBox uiBox = CheckUIBoxOnClick();
        if (uiBox == null)
            return;

        contentIndex = uiBox.index;
        GameManager._sound.PlaySE("UI클릭");
    }

    private void SetItemContent()
    {
        var pre_objects = itemContentTr.GetComponentsInChildren<UIBox>();
        foreach (var ob in pre_objects)
            Destroy(ob.gameObject);

        List<int> items = GameManager._data.player.hasItems;
        foreach (var itemCode in items)
        {
            UIBox uIBox = Instantiate(itemSlot, itemContentTr).GetComponent<UIBox>();
            Sprite itemSprite = GameManager._data.itemDatas[itemCode].item_sprite;

            uIBox.images[0].GetComponent<RectTransform>().sizeDelta = 
                new Vector2(itemSprite.rect.width, itemSprite.rect.height).normalized * itemSlotImageSize;
            uIBox.images[0].sprite = itemSprite;
            uIBox.button.onClick.AddListener(OnItemSlot);
            uIBox.index = itemCode;
        }
    }

    private void SetGalleryContent()
    {
        var pre_objects = captureContentTr.GetComponentsInChildren<UIBox>();
        foreach (var ob in pre_objects)
            Destroy(ob.gameObject);

        List<int> captures = GameManager._data.player.hasCaptures;
        foreach (var captureCode in captures)
        {
            UIBox uIBox = Instantiate(captureSlot, captureContentTr).GetComponent<UIBox>();
            Sprite itemSprite = GameManager._data.captureDatas[captureCode].capture_sprite;

            uIBox.images[0].sprite = itemSprite;
            uIBox.button.onClick.AddListener(OnCaptureSlot);
            uIBox.index = captureCode;
        }
    }

    private void SetRecordContent()
    {
        var pre_objects = recordContentTr.GetComponentsInChildren<UIBox>();
        foreach (var ob in pre_objects)
            Destroy(ob.gameObject);

        List<int> records = GameManager._data.player.hasRecords;
        foreach (var recordCode in records)
        {
            UIBox uIBox = Instantiate(recordSlot, recordContentTr).GetComponent<UIBox>();

            uIBox.tmp_texts[0].SetText(GameManager._data.recordDatas[recordCode].record_name);
            uIBox.button.onClick.AddListener(OnRecordSlot);
            uIBox.index = recordCode;
        }
    }

    private void OnItemSlot()
    {
        UIBox uiBox = CheckUIBoxOnClick();
        if (uiBox == null)
            return;

        itemInfoOb.SetActive(true);
        GameManager._sound.PlaySE("UI클릭");

        int itemCode = uiBox.index;
        ItemSO itemData = GameManager._data.itemDatas[itemCode];
        Sprite itemSprite = itemData.item_sprite;

        itemInfoImage.sprite = itemSprite;
        itemInfoImage.GetComponent<RectTransform>().sizeDelta 
            = new Vector2(itemSprite.rect.width, itemSprite.rect.height).normalized * itemInfoImageSize;
        itemInfoText.SetText($"{itemData.item_name}\n\n <size=70%>{itemData.item_info}");
    }

    private void OnCaptureSlot()
    {
        UIBox uiBox = CheckUIBoxOnClick();
        if (uiBox == null)
            return;

        GameManager._sound.PlaySE("UI클릭");

        isCanTouchCard = false;
        galleryAnim.gameObject.SetActive(true);
        galleryAnim.Play(galleryAnimName_In);
        galleryAnim[galleryAnimName_In].speed = 1f / galleryAnimTime;
        Invoke("EndGalleryAnimIn", galleryAnimTime);

        int captureCode = uiBox.index;
        CaptureSO captureData = GameManager._data.captureDatas[captureCode];
        Sprite itemSprite = captureData.capture_sprite;

        captureCardImage.sprite = itemSprite;
        captureCardText.SetText($"{captureData.capture_info}");
    }

    private void OnRecordSlot()
    {
        UIBox uiBox = CheckUIBoxOnClick();
        if (uiBox == null)
            return;

        recordInfoOb.SetActive(true);
        GameManager._sound.PlaySE("UI클릭");

        int recordCode = uiBox.index;
        RecordSO recordData = GameManager._data.recordDatas[recordCode];

        itemInfoText.SetText($"{recordData.record_name}\n\n <size=70%>{recordData.record_info}");
    }

    public void OnCaptureCard()
    {
        if (!isCanTouchCard) return;
        isCanTouchCard = false;
        galleryAnim.Play(galleryAnimName_Out);
        GameManager._sound.PlaySE("UI클릭");
    }

    public void EndGalleryAnimIn()
    {
        isCanTouchCard = true;
    }

    private UIBox CheckUIBoxOnClick()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return null;
        return EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
    }
}
