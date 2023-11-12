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

    }

    private void SetRecordContent()
    {

    }

    private void OnItemSlot()
    {
        UIBox uiBox = CheckUIBoxOnClick();
        if (uiBox == null)
            return;

        itemInfoOb.SetActive(true);
        
        int itemCode = uiBox.index;
        ItemSO itemData = GameManager._data.itemDatas[itemCode];
        Sprite itemSprite = itemData.item_sprite;

        itemInfoImage.sprite = itemSprite;
        itemInfoImage.GetComponent<RectTransform>().sizeDelta 
            = new Vector2(itemSprite.rect.width, itemSprite.rect.height).normalized * itemInfoImageSize;
        itemInfoText.SetText($"{itemData.item_name}\n\n <size=70%>{itemData.item_info}");
    }

    private UIBox CheckUIBoxOnClick()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return null;
        return EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
    }
}
