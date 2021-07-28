using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CustomTextAlignment
{
    topLeft,
    topCenter,
    topRight,
    middleLeft,
    center,
    middleRight,
    bottomLeft,
    bottomCenter,
    bottomRight
}

public class Window : MonoBehaviour
{
    public enum WindowState
    {
        none = -1,
        opening,
        opened,
        closing,
        closed
    }

    [SerializeField] private Image baseImage;
    private WindowState windowState;
    private RectTransform rect;
    private Vector2 windowSize;
    private TMP_Text dialogText;
    private bool isPlayTypeWriterSE;
    private bool isInitialized;
    private bool isTypeWrting;
    private bool isSkipTypeWriter;
    private int typewriteCnt;

    private List<TMP_Text> extraTextList = new List<TMP_Text>();
    private List<Image> extraImageList = new List<Image>();

    public void Initialize()
    {
        if (isInitialized) return;

        rect = GetComponent<RectTransform>();
        windowSize = rect.sizeDelta;
        dialogText = GetComponentInChildren<TMP_Text>(true);
        dialogText.enabled = false;
        extraTextList.Clear();
        windowState = WindowState.closed;
        isPlayTypeWriterSE = true;

        isInitialized = true;
    }

    public void ResizeX(float sizeX, float time)
    {
        StartCoroutine(Resize(new Vector2(sizeX, rect.sizeDelta.y), time));
    }

    public void ResizeX(float sizeX)
    {
        rect.sizeDelta = new Vector2(sizeX, rect.sizeDelta.y);
        if (GetIsOpen())
        {
            DoneOpen();
        }
        else
        {
            DoneClose();
        }
    }

    private IEnumerator Resize(Vector2 target, float time)
    {
        Vector2 origin = GetComponent<RectTransform>().sizeDelta;
        float lerp = 0;

        while (lerp < time)
        {
            lerp = Mathf.Clamp(lerp + Time.deltaTime, 0.0f, time);
            rect.sizeDelta = Vector2.Lerp(origin, target, ParametricBlend(lerp/time));
            yield return new WaitForEndOfFrame();
        }

        rect.sizeDelta = target;

        if (GetIsOpen())
        {
            DoneOpen();
        }
        else
        {
            DoneClose();
        }
    }

    public void SetSizeAfterDelay(Vector2 size, float time)
    {
        StartCoroutine(SetSizeAfterDelayLoop(size, time));
    }

    private IEnumerator SetSizeAfterDelayLoop(Vector2 size, float time)
    {
        yield return new WaitForSeconds(time);

        rect.sizeDelta = size;
    }

    public void SetActiveAfterDelay(bool active, float time)
    {
        StartCoroutine(SetActiveAfterDelayLoop(active, time));
    }

    private IEnumerator SetActiveAfterDelayLoop(bool active, float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(active);
    }

    // formulae for ease in and ease out
    private float ParametricBlend(float t)
    {
        float sqt = t * t;
        return sqt / (2.0f * (sqt - t) + 1.0f);
    }

    public bool GetIsOpen()
    {
        return windowState == WindowState.opened || windowState == WindowState.opening;
    }

    public WindowState GetState()
    {
        return windowState;
    }

    private void DoneOpen()
    {
        windowState = WindowState.opened;

        // show extra texts
        dialogText.enabled = true;
        EnableAllEntraText(true);
        EnableAllEntraImage(true);
    }

    private void DoneClose()
    {
        windowState = WindowState.closed;
    }

    public void SetWindowState(WindowState state)
    {
        windowState = state;

        dialogText.gameObject.SetActive(GetIsOpen());

        if (!GetIsOpen())
        {
            // hide extra texts
            dialogText.enabled = false;
            EnableAllEntraText(false);
            EnableAllEntraImage(false);
        }
    }

    public Vector2 GetWindowSize()
    {
        return windowSize;
    }

    // Text
    public void SetTextSize(float newSize)
    {
        dialogText.fontSize = newSize;
    }
    public void SetTextColor(Color color)
    {
        dialogText.color = color;
    }

    public void SetTextMargin(Vector4 vec)
    {
        dialogText.margin = vec;
    }

    public void SetText(string newText)
    {
        dialogText.SetText(newText);
    }

    public void SetText(string newText, float interval)
    {
        if (isTypeWrting) return;

        StartCoroutine(SetTextLoop(newText, interval));
    }

    public TMP_Text GetMainText()
    {
        return dialogText;
    }

    private IEnumerator SetTextLoop(string newText, float interval)
    {
        int wordCount = newText.Length-1;
        int currentCount = 0;
        string patternDetect = string.Empty;
        string text = string.Empty;
        bool[] highlight = new bool[3];
        for (int i = 0; i < 3; i++) highlight[i] = false;

         isTypeWrting = true;

        while (currentCount <= wordCount)
        {
            // initiate wait time
            float waitTime = isSkipTypeWriter ?  0.0f : interval;

            //don't add text yet if the window is not fully open
            if (windowState != WindowState.opened)
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }

            // process text
            if (CheckHighlightOrange(newText[currentCount].ToString()))
            {
                if (!highlight[0])
                {
                    text = text + "<color=orange>";
                }
                else
                {
                    text = text + "</color>";
                }
                highlight[0] = !highlight[0];
            }
            else if (CheckHighlightTeal(newText[currentCount].ToString()))
            {
                if (!highlight[1])
                {
                    text = text + "<color=green>";
                }
                else
                {
                    text = text + "</color>";
                }
                highlight[1] = !highlight[1];
            }
            else if (CheckHighlightYellow(newText[currentCount].ToString()))
            {
                if (!highlight[2])
                {
                    text = text + "<color=yellow>";
                }
                else
                {
                    text = text + "</color>";
                }
                highlight[2] = !highlight[2];
            }
            else
            {
                text = text + newText[currentCount];
            }

            // update text
            if (highlight[0] || highlight[1] || highlight[2])
            {
                SetText(text + "</color>");
            }
            else
            {
                SetText(text);
            }

            // check pattern
            patternDetect = patternDetect + newText[currentCount];

            if (CheckPatterns(patternDetect, waitTime, out float newWaitTime))
            {
                patternDetect = string.Empty;
                waitTime = newWaitTime;
            }

            // update count at the last
            currentCount++;

            // play SE
            if (isPlayTypeWriterSE)
            {
                if (typewriteCnt % 2 == 0)
                {
                    AkSoundEngine.PostEvent("Text_Appear", gameObject);
                }
                typewriteCnt++;
            }
            yield return new WaitForSeconds(waitTime);
        }

        isTypeWrting = false;
        isSkipTypeWriter = false;
    }

    public void SkipText()
    {
        if (isTypeWrting)
        {
            isSkipTypeWriter = true;
        }
    }

    public bool GetIsTypeWriting()
    {
        if (isSkipTypeWriter) return false;

        return isTypeWrting;
    }

    private bool CheckPatterns(string pattern, float originalWaitTime, out float newWaitTime)
    {
        newWaitTime = originalWaitTime;

        if (CheckComma(pattern))
        {
            newWaitTime = originalWaitTime * 3.5f;
            return true;
        }

        if (CheckSpace(pattern))
        {
            newWaitTime = 0.0f;
            return true;
        }

        if (CheckPeriod(pattern))
        {
            newWaitTime = originalWaitTime * 7.5f;
            return true;
        }

        return false;
    }

    private bool CheckComma(string pattern)
    {
        return (pattern.Contains(",") || pattern.Contains("、") || pattern.Contains("，"));
    }

    private bool CheckSpace(string pattern)
    {
        return (pattern.Contains(" "));
    }

    private bool CheckPeriod(string pattern)
    {
        return (pattern.Contains(".") || pattern.Contains("。") || pattern.Contains("?") || pattern.Contains("!"));
    }

    private void CheckHighlights(string pattern)
    {
        
    }

    private bool CheckHighlightOrange(string pattern)
    {
        return (pattern.Contains("+"));
    }

    private bool CheckHighlightTeal(string pattern)
    {
        return (pattern.Contains("%"));
    }

    private bool CheckHighlightYellow(string pattern)
    {
        return (pattern.Contains("$"));
    }

    public void SetTextOffset(Vector2 offset)
    {
        dialogText.GetComponent<RectTransform>().anchoredPosition = offset;
    }

    public void SetTextAlignment(CustomTextAlignment alignment)
    {
        switch (alignment)
        {
            case CustomTextAlignment.topLeft:
                dialogText.alignment = TextAlignmentOptions.TopLeft;
                break;
            case CustomTextAlignment.topCenter:
                dialogText.alignment = TextAlignmentOptions.TopGeoAligned;
                break;
            case CustomTextAlignment.topRight:
                dialogText.alignment = TextAlignmentOptions.TopRight;
                break;
            case CustomTextAlignment.middleLeft:
                dialogText.alignment = TextAlignmentOptions.Left;
                break;
            case CustomTextAlignment.center:
                dialogText.alignment = TextAlignmentOptions.Center;
                break;
            case CustomTextAlignment.middleRight:
                dialogText.alignment = TextAlignmentOptions.Right;
                break;
            case CustomTextAlignment.bottomLeft:
                dialogText.alignment = TextAlignmentOptions.BottomLeft;
                break;
            case CustomTextAlignment.bottomCenter:
                dialogText.alignment = TextAlignmentOptions.BottomGeoAligned;
                break;
            case CustomTextAlignment.bottomRight:
                dialogText.alignment = TextAlignmentOptions.BottomRight;
                break;
        }
    }

    public void SetTextWrappingMode(bool enable)
    {
        dialogText.enableWordWrapping = enable;
    }
    public void SetTextEnableSE(bool enable)
    {
        isPlayTypeWriterSE = enable;
    }


    // Add new texts. return reference
    public TMP_Text AddNewText(string newText, Vector2 location, float size, Color color)
    {
        // create text
        TMP_Text tmp = Instantiate(dialogText.gameObject, transform).GetComponent<TMP_Text>();
        extraTextList.Add(tmp);

        Vector2 bottomleft = -GetWindowSize() / 2f;
        tmp.GetComponent<RectTransform>().anchoredPosition = bottomleft + location;
        tmp.fontSize = size;
        tmp.text = newText;
        tmp.color = color;
        tmp.gameObject.SetActive(windowState == WindowState.opened);

        return tmp;
    }

    public void EnableAllEntraText(bool boolean)
    {
        foreach (TMP_Text txt in extraTextList)
        {
            txt.gameObject.SetActive(boolean);
        }
    }

    public void AddNewImage(string path, Vector2 location, Vector2 size, bool behindWindow)
    {
        // create text
        Image tmp = Instantiate(baseImage.gameObject, transform).GetComponent<Image>();
        extraImageList.Add(tmp);

        Vector2 bottomleft = -GetWindowSize() / 2f;
        tmp.GetComponent<RectTransform>().anchoredPosition = bottomleft + location;
        tmp.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        tmp.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        tmp.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        tmp.GetComponent<RectTransform>().sizeDelta = size;

        tmp.color = Color.white;
        tmp.sprite = Resources.Load<Sprite>(path);

        if (behindWindow) tmp.transform.SetSiblingIndex(0);

        tmp.gameObject.SetActive(windowState == WindowState.opened);
    }


    public void EnableAllEntraImage(bool boolean)
    {
        foreach (Image img in extraImageList)
        {
            img.gameObject.SetActive(boolean);
        }
    }
}
