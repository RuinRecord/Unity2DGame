using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    private const float DEFAULT_PRINT_TIME = 0.075f;

    [SerializeField]
    private GameObject tooltip;

    [SerializeField]
    private TMP_Text tooltipText;

    public void ShowTooltip(string info)
    {
        tooltip.SetActive(true);
        StartCoroutine(SetTooltipText(info));
    }

    public void CloseTooltip()
        => tooltip.SetActive(false);

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
