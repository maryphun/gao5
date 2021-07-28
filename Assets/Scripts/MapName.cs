using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class MapName : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] float displayTime = 1.5f;
    [SerializeField] float displayBackAlpha = 0.2f;
    [SerializeField] float displayFadeTime = 1.5f;

    [SerializeField] Image alpha;
    [SerializeField] TMP_Text text;

    bool isShowing = false;

    [SerializeField, Range(0.0f, 1.0f)] float timeLeft;
    
    public void ShowName(string name)
    {
        if (isShowing)
        {
            timeLeft = displayTime;
            text.text = name;
            return;
        }

        alpha.DOKill(true);
        text.DOKill(true);
        alpha.DOFade(displayBackAlpha, 0.0f);
        text.text = name;
        text.DOFade(1.0f, 0.0f);
        isShowing = true;

        StartCoroutine(HideName(displayTime));
    }

    private IEnumerator HideName(float delay)
    {
        timeLeft = displayTime;

        while (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        isShowing = false;
        alpha.DOFade(0.0f, displayFadeTime);
        text.DOFade(0.0f, displayFadeTime);

        yield return new WaitForSeconds(displayFadeTime);

        if (!isShowing)
        {
            text.text = string.Empty;
        }
    }
}
