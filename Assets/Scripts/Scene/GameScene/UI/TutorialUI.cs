using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;

public class TutorialUI : MonoBehaviour
{
    private const float DEFAULT_PRINT_TIME = 0.075f;

    [SerializeField] private GameObject tooltip;

    [SerializeField] private Button tooltipButton;

    private Image tooltipButtonImage;

    [SerializeField] private TMP_Text tooltipText;

    private string currentInfo;

    private void Start()
    {
        tooltipButtonImage = tooltipButton.GetComponent<Image>();
        tooltipText.SetText("알려드릴 정보가 없습니다");

        OnTooltip();
    }

    public void ShowTooltip(string info)
    {
        currentInfo = info;
        OnTooltip();
        StartCoroutine(SetTooltipText(info));
    }

    public void OnTooltip()
    {
        if (!string.IsNullOrEmpty(currentInfo))
            tooltipText.SetText(currentInfo);
        ActiveTooltip(true);
        ActiveTooptipButton(true);
    }

    public void CloseTooltip()
    {
        ActiveTooltip(false);
        ActiveTooptipButton(false);
    }

    public void OnOffTooltip()
    {
        GameManager.Sound.PlaySE("UI클릭");
        if (tooltip.activeSelf)
            CloseTooltip();
        else
            OnTooltip();
    }

    private void ActiveTooltip(bool isActice) => tooltip.SetActive(isActice);

    private void ActiveTooptipButton(bool isActice) => tooltipButtonImage.color = (isActice) ? new Color(0.7f, 0.9f, 0.9f, 1f) : new Color(0.7f, 0.9f, 0.9f, 0.2f);

    IEnumerator SetTooltipText(string text)
    {
        string words = "";

        // 한글자씩 천천히 출력
        foreach (var ch in text)
        {
            words += ch;
            tooltipText.SetText(words);
            // GameManager._sound.PlaySE("");
            yield return new WaitForSeconds(DEFAULT_PRINT_TIME);
        }
    }
}
