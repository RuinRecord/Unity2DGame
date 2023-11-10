using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvenUICtrl : MonoBehaviour
{
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
    }

    public void OnMenuButton()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return;

        var uiBox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uiBox == null)
            return;

        contentIndex = uiBox.index;
    }
}
