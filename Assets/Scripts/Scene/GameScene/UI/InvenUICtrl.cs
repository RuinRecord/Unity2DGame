using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InvenUICtrl : MonoBehaviour
{
    private readonly float itemSlotImageSize = 150;
    private readonly float itemInfoImageSize = 400;
    private readonly float galleryAnimTime = 0.75f;
    private readonly string galleryAnimName_In = "CaptureCard_In";
    private readonly string galleryAnimName_Out = "CaptureCard_Out";

    [SerializeField] private GameObject InvenObject;
    [SerializeField] private UIBox[] menus;
    [SerializeField] private GameObject[] contents;
    [SerializeField] private int contentIndex;
    [SerializeField] private Sprite[] menuOnSprites;
    [SerializeField] private Sprite[] menuOffSprites;
    public bool IsOnInven;

    [Header("[ Item Fields ]")]
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private Transform itemContentTr;
    [SerializeField] private GameObject itemInfoOb;
    [SerializeField] private Image itemInfoImage;
    [SerializeField] private TMP_Text itemInfoText;

    [Header("[ Gallery Fields ]")]
    [SerializeField] private Transform captureContentTr;
    [SerializeField] private GameObject captureNextPageButton;
    [SerializeField] private GameObject capturePrePageButton;
    [SerializeField] private TMP_Text capturePageText;
    [SerializeField] private Animation galleryAnim;
    [SerializeField] private Image captureCardImage;
    [SerializeField] private TMP_Text captureCardText;
    private bool isCanTouchCard;
    private int galleryPage;

    [Header("[ Record Fields ]")]
    [SerializeField] private GameObject recordSlot;
    [SerializeField] private Transform recordContentTr;
    [SerializeField] private Sprite recordSelected, recordNotSelected;
    [SerializeField] private GameObject recordInfoOb;
    [SerializeField] private TMP_Text recordInfoText;

    public void Init()
    {
        IsOnInven = false;
        contentIndex = 0;
        galleryPage = 0;
        InvenObject.SetActive(false);
        SetMenus(contentIndex);
        SetContents(contentIndex);
    }

    public void OnOffInven()
    {
        IsOnInven = !IsOnInven;
        contentIndex = 0;
        galleryPage = 0;
        InvenObject.SetActive(IsOnInven);
        SetMenus(contentIndex);
        SetContents(contentIndex);

        GameManager.Sound.PlaySE("가방여닫기");

        if (!IsOnInven)
            EventCtrl.Instance.CheckEvent(EventType.Inventory);
    }

    public void SetMenus(int index)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (i == index)
                menus[i].images[0].sprite = menuOnSprites[i];
            else
                menus[i].images[0].sprite = menuOffSprites[i];
        }
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
        SetMenus(contentIndex);
        SetContents(contentIndex);
        GameManager.Sound.PlaySE("UI클릭");
    }

    private void SetItemContent()
    {
        var pre_objects = itemContentTr.GetComponentsInChildren<UIBox>();
        foreach (var ob in pre_objects)
            Destroy(ob.gameObject);

        List<int> items = GameManager.Data.player.hasItems;
        foreach (var itemCode in items)
        {
            UIBox uIBox = Instantiate(itemSlot, itemContentTr).GetComponent<UIBox>();
            Sprite itemSprite = GameManager.Data.itemDatas[itemCode].itemSprite;

            uIBox.images[0].enabled = false;
            uIBox.images[1].GetComponent<RectTransform>().sizeDelta = 
                new Vector2(itemSprite.rect.width, itemSprite.rect.height).normalized * itemSlotImageSize;
            uIBox.images[1].sprite = itemSprite;
            uIBox.button.onClick.AddListener(OnItemSlot);
            uIBox.index = itemCode;
        }
    }

    private void SetGalleryContent()
    {
        UIBox[] captureSlots = captureContentTr.GetComponentsInChildren<UIBox>();
        List<int> captures = GameManager.Data.player.hasCaptures;
        galleryPage = 0;
        SetGalleryPageButton();

        for (int i = 0; i < captureSlots.Length; i++)
        {
            UIBox uIBox = captureSlots[i + galleryPage * 6];

            if (i + galleryPage * 6 < captures.Count)
            {
                int idx = captures[i + galleryPage * 6];
                
                Sprite itemSprite = GameManager.Data.captureDatas[idx].captureSprite;

                uIBox.images[0].sprite = itemSprite;
                uIBox.images[0].color = Color.white;
                uIBox.button.enabled = true;
                uIBox.button.onClick.AddListener(OnCaptureSlot);
                uIBox.index = idx;
            }
            else
            {
                // 해당하는 갤러리가 없음 => 비활성화
                uIBox.images[0].sprite = null;
                uIBox.images[0].color = new Color(0f, 0f, 0f, 0f);
                uIBox.button.enabled = false;
                uIBox.index = -1;
            }
        }
    }

    private void SetRecordContent()
    {
        UIBox[] pre_objects = recordContentTr.GetComponentsInChildren<UIBox>();
        foreach (var ob in pre_objects)
        {
            ob.images[0].sprite = recordNotSelected;
            ob.tmp_texts[0].color = Color.white;
        }

        foreach (var ob in pre_objects)
            Destroy(ob.gameObject);

        List<int> records = GameManager.Data.player.hasRecords;
        foreach (var recordCode in records)
        {
            UIBox uIBox = Instantiate(recordSlot, recordContentTr).GetComponent<UIBox>();

            uIBox.images[0].sprite = recordNotSelected;
            uIBox.tmp_texts[0].color = Color.white;
            uIBox.tmp_texts[0].SetText(GameManager.Data.recordDatas[recordCode].record_name);
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
        var pre_objects = itemContentTr.GetComponentsInChildren<UIBox>();
        foreach (var ob in pre_objects)
            ob.images[0].enabled = false;
        GameManager.Sound.PlaySE("UI클릭");

        int itemCode = uiBox.index;
        ItemSO itemData = GameManager.Data.itemDatas[itemCode];
        Sprite itemSprite = itemData.itemSprite;

        uiBox.images[0].enabled = true;
        itemInfoImage.sprite = itemSprite;
        itemInfoImage.GetComponent<RectTransform>().sizeDelta 
            = new Vector2(itemSprite.rect.width, itemSprite.rect.height).normalized * itemInfoImageSize;
        itemInfoText.SetText($"{itemData.itemName}\n\n <size=70%>{itemData.itemInfo}");
    }

    private void OnCaptureSlot()
    {
        UIBox uiBox = CheckUIBoxOnClick();
        if (uiBox == null)
            return;

        GameManager.Sound.PlaySE("UI클릭");

        isCanTouchCard = false;
        galleryAnim.gameObject.SetActive(true);
        galleryAnim.Play(galleryAnimName_In);
        galleryAnim[galleryAnimName_In].speed = 1f / galleryAnimTime;
        Invoke("EndGalleryAnimIn", galleryAnimTime);

        int captureCode = uiBox.index;
        CaptureSO captureData = GameManager.Data.captureDatas[captureCode];
        Sprite itemSprite = captureData.captureSprite;

        captureCardImage.sprite = itemSprite;
        captureCardText.SetText($"{captureData.captureInfo}");
    }

    public void OnRecordSlot()
    {
        UIBox uiBox = CheckUIBoxOnClick();
        if (uiBox == null)
            return;

        recordInfoOb.SetActive(true);
        UIBox[] pre_objects = recordContentTr.GetComponentsInChildren<UIBox>();
        foreach (var ob in pre_objects)
        {
            ob.images[0].sprite = recordNotSelected;
            ob.tmp_texts[0].color = Color.white;
        }
        GameManager.Sound.PlaySE("UI클릭");

        int recordCode = uiBox.index;
        uiBox.images[0].sprite = recordSelected;
        uiBox.tmp_texts[0].color = Color.black;
        RecordSO recordData = GameManager.Data.recordDatas[recordCode];

        recordInfoText.SetText($"{recordData.record_name}\n\n <size=70%>{recordData.record_info}");
    }

    public void SetGalleryPageButton()
    {
        int maxPage = (GameManager.Data.player.hasCaptures.Count - 1) / 6;

        capturePrePageButton.SetActive(true);
        captureNextPageButton.SetActive(true);
        if (galleryPage == 0)
            capturePrePageButton.SetActive(false);
        if (GameManager.Data.player.hasCaptures.Count == 0 || galleryPage == maxPage)
            captureNextPageButton.SetActive(false);

        capturePageText.SetText($"( {galleryPage + 1} / { maxPage + 1 } )");
    }

    public void OnGalleryPreButton()
    {
        galleryPage--;
        SetGalleryContent();
        GameManager.Sound.PlaySE("UI클릭");
    }

    public void OnGalleryNextButton()
    {
        galleryPage++;
        SetGalleryContent();
        GameManager.Sound.PlaySE("UI클릭");
    }

    public void OnCaptureCard()
    {
        if (!isCanTouchCard) return;
        isCanTouchCard = false;
        galleryAnim.Play(galleryAnimName_Out);
        GameManager.Sound.PlaySE("UI클릭");
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
