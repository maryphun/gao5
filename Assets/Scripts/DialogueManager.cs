using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject SelectionPrefab;
    [SerializeField] PlayerController player;
    [SerializeField] Canvas SelectionCanvas;
    [SerializeField] int textSize = 36;
    [SerializeField] float textInterval = 0.075f;
    [SerializeField] Image narrativeAlpha;
    [SerializeField] TMP_Text narrativeText;

    [Header("Audio")]
    [SerializeField] string soundName = "Text_Appear";
    [SerializeField] int wordPerSE = 1;

    Selection currentSelection;
    private bool isInDialogue;
    private string currentWindowBoxName;

    // narrative
    private bool narrativeMode;
    private bool isTypeWrting;
    private bool isSkipTypeWriter;
    private bool isPlayTypeWriterSE;
    private int typewriteCnt;

    public enum DialogueType
    {
        ChatboxWithSelection,
        ChatboxNoSelection,
        Narrative,
        SelectionOnly,
        Max,
    }

    public struct RegisteredDialogue
    {
        public DialogueType type;
        public string text;
        public Transform fromObject;
        public bool enablePlaySE;
        public Vector2 offset;
    }

    List<RegisteredDialogue> registeredDialogueList;

    private void Awake()
    {
        narrativeMode = false;
        isInDialogue = false;
        narrativeAlpha.color = new Color(0, 0, 0, 0);
        narrativeText.text = string.Empty;
        currentWindowBoxName = string.Empty;
        registeredDialogueList = new List<RegisteredDialogue>();
    }

    private void Update()
    {
        if (!isInDialogue)
        {
            if (registeredDialogueList.Count == 0)
            {
                return;
            }
            else
            {
                // a new dialogue registered.
                isInDialogue = true;

                switch (registeredDialogueList[0].type)
                {
                    case DialogueType.Narrative:
                        EnterNarrativeMode();
                        isPlayTypeWriterSE = registeredDialogueList[0].enablePlaySE;
                        StartCoroutine(SetNarrativeTextLoop(registeredDialogueList[0].text, textInterval));
                        break;
                    case DialogueType.ChatboxNoSelection:
                        isPlayTypeWriterSE = registeredDialogueList[0].enablePlaySE;
                        CreateDialogueBox(registeredDialogueList[0].text, registeredDialogueList[0].fromObject, registeredDialogueList[0].offset);
                        break;
                    default:
                        // non-valid dialogue type.
                        isInDialogue = false;
                        Debug.Log("Non-valid dialogue type have been registered. (" + registeredDialogueList[0].type + ")");
                        break;
                }

                // remove from the list.
                registeredDialogueList.RemoveAt(0);
            }
        }

        bool pressed = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);

        // narrative
        if (pressed)
        {
            if (isTypeWrting)
            {
                // skip text
                isSkipTypeWriter = true;
            }
            else
            {
                // next text
                isSkipTypeWriter = false;
                isInDialogue = false;
                if (CheckNextDialogue() == false)
                {
                    player.SetInteractMode(false);
                }
            }
        }
    }

    // return false if there are no more dialogue left.
    private bool CheckNextDialogue()
    {
        if (registeredDialogueList.Count == 0)
        {
            if (narrativeMode)
            {
                ExitNarrativeMode();
                return false;
            }
            else
            {
                WindowManager.Instance.Close(currentWindowBoxName, 0.5f, true);
            }
        }
        else
        {
            if (narrativeMode)
            {
                if (registeredDialogueList[0].type != DialogueType.Narrative)
                {
                    ExitNarrativeMode();
                }
            }
            else
            {
                WindowManager.Instance.Close(currentWindowBoxName, 0.5f, true);
                if (registeredDialogueList[0].type == DialogueType.Narrative)
                {
                    EnterNarrativeMode();
                }
            }

            return true;
        }

        return false;
    }

    public void RegisterNewChoice(List<string> selections)
    {
        var obj = Instantiate(SelectionPrefab, SelectionCanvas.transform);
        currentSelection = obj.GetComponent<Selection>();
        currentSelection.Initialization(selections, 0.5f);
    }

    public void RegisterNewDialogue(string text, Vector2 worldPosition, bool playSE = true)
    {
        RegisteredDialogue tmp;
        tmp.type = DialogueType.ChatboxNoSelection;
        tmp.fromObject = null;
        tmp.text = text;
        tmp.enablePlaySE = playSE;
        tmp.offset = worldPosition;
        registeredDialogueList.Add(tmp);
    }

    public void RegisterNewDialogue(string text, Transform character, Vector2 offset, bool playSE = true)
    {
        RegisteredDialogue tmp;
        tmp.type = DialogueType.ChatboxNoSelection;
        tmp.fromObject = character;
        tmp.text = text;
        tmp.enablePlaySE = playSE;
        tmp.offset = offset;
        registeredDialogueList.Add(tmp);
    }

    public void CreateDialogueBox(string text, Transform character, Vector2 offset)
    {
        // calculate window size base on text count
        float spacePerText = (textSize * 1.38888888889f);
        float textCount = (text.Length + 1);
        Vector2 windowSize = new Vector2(spacePerText * textCount, 100f);
        windowSize = AutoResizeWindow(windowSize);

        Vector2 tmpPos = new Vector2(Camera.main.WorldToScreenPoint(character.position).x - Screen.width / 2f,
                                     Camera.main.WorldToScreenPoint(character.position).y - Screen.height / 2f);
        Debug.Log(tmpPos);
        tmpPos.y += windowSize.y / 2f;
        tmpPos += offset;
        currentWindowBoxName = "dialogue" + text.Substring(0, 5);
        WindowManager.Instance.CreateWindow(currentWindowBoxName, tmpPos, windowSize);
        WindowManager.Instance.Open(currentWindowBoxName, 0.5f);
        WindowManager.Instance.SetText(currentWindowBoxName, text, textInterval);
        WindowManager.Instance.SetTextAlignment(currentWindowBoxName, CustomTextAlignment.topLeft);
        WindowManager.Instance.SetTextColor(currentWindowBoxName, Color.yellow);
        WindowManager.Instance.SetTextSize(currentWindowBoxName, textSize);
        WindowManager.Instance.SetTextMargin(currentWindowBoxName, new Vector4(14, 0, 0, 0));

        WindowManager.Instance.AddNewImage(currentWindowBoxName, "arrow", new Vector2(windowSize.x / 2f, -8f), new Vector2(50f, 50f), true);

        // Play Audio
        AkSoundEngine.PostEvent("Dialogue_Appear", gameObject);
    }

    public void RegisterNewNarrative(string text, bool playSE = true)
    {
        RegisteredDialogue tmp;
        tmp.type = DialogueType.Narrative;
        tmp.fromObject = null;
        tmp.text = text;
        tmp.enablePlaySE = playSE;
        tmp.offset = Vector2.zero;
        registeredDialogueList.Add(tmp);

        // force update
        //Update();
    }

    private void EnterNarrativeMode()
    {
        if (narrativeMode) return;

        narrativeMode = true;

        // reset
        narrativeText.text = string.Empty;

        // fade
        narrativeText.DOFade(1.0f, 0.2f);
        narrativeAlpha.DOFade(0.7f, 1.0f);
    }

    private void ExitNarrativeMode()
    {
        if (!narrativeMode) return;

        narrativeMode = false;
        narrativeText.DOFade(0.0f, 0.5f);
        narrativeAlpha.DOFade(0.0f, 0.5f);
    }

    private int GetResult()
    {
        if (currentSelection.IsSelected())
        {
            return -1;
        }

        return currentSelection.GetResult();
    }

    private Vector2 AutoResizeWindow(Vector2 original)
    {
        Vector2 rtn = original;

        while (rtn.x > 1000.0f)
        {
            rtn.x /= 2f;
            rtn.y += 85.0f;
        }

        return rtn;
    }

    private IEnumerator SetNarrativeTextLoop(string newText, float interval)
    {
        int wordCount = newText.Length - 1;
        int currentCount = 0;
        string patternDetect = string.Empty;
        string text = string.Empty;
        bool[] highlight = new bool[3];
        for (int i = 0; i < 3; i++) highlight[i] = false;

        isTypeWrting = true;
        typewriteCnt = 0;

        while (currentCount <= wordCount)
        {
            // initiate wait time
            float waitTime = isSkipTypeWriter ? 0.0f : interval;

            //don't add text yet if the window is not fully open
            if (!narrativeMode || narrativeAlpha.color.a < 0.7f)
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }

            text = text + newText[currentCount];

            // update text
            narrativeText.text = text;

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
            if (isPlayTypeWriterSE && !isSkipTypeWriter)
            {
                if (typewriteCnt % wordPerSE == 0)
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

}
