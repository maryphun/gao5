using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Selection : MonoBehaviour
{
    [SerializeField] GameObject selectionOrigin;
    [SerializeField] Color selectionColor = Color.yellow;
    [SerializeField] float selectionFadeTime = 0.4f;
    [SerializeField] float textFadeTime = 0.5f;

    private List<Transform> selectionList;
    private bool activated = false;
    private int selectIndex = 0;
    private bool selected = false;
    private int resultIndex = 0;
    private bool inAnimation = false;
    

    public void Initialization(List<string> selectList, float startDelay = 0.0f)
    {
        if (selectList.Count <= 0 || selectList.Count > 3)
        {
            Debug.Log("Selection list out of bound!");
            return;
        }

        // initialize
        selectionList = new List<Transform>();
        activated = false;
        selected = false;
        inAnimation = false;
        selectIndex = selectList.Count-1;

        // instantiate
        for (int i = 0; i < selectList.Count; i++)
        {
            // create copy
            var tmp = Instantiate(selectionOrigin, transform);

            // get reference
            RectTransform rect = tmp.GetComponent<RectTransform>();
            TMP_Text text = tmp.GetComponentInChildren<TMP_Text>();

            // set position
            rect.anchoredPosition = new Vector2(0, -(40 * selectList.Count) + i * 80);

            tmp.GetComponent<Image>().DOBlendableColor(selectionColor, 0f);
            tmp.GetComponent<Image>().DOFade(0.0f, 0.0f);

            text.SetText(selectList[i]);
            text.color = new Color(1, 1, 1, 0);

            tmp.SetActive(true);

            selectionList.Add(tmp.transform);
        }

        // activate
        StartCoroutine(DelayActivation(startDelay));
    }

    IEnumerator DelayActivation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // show text
        for (int i = 0; i < selectionList.Count; i++)
        {
            selectionList[i].GetComponentInChildren<TMP_Text>().DOFade(1.0f, textFadeTime);
        }

        // highlight the default selection
        selectionList[selectIndex].GetComponent<Image>().DOFade(1.0f, selectionFadeTime);

        // get reference
        RectTransform rect = selectionList[selectIndex].GetComponent<RectTransform>();
        TMP_Text text = selectionList[selectIndex].GetComponentInChildren<TMP_Text>();

        rect.sizeDelta = new Vector2(text.GetRenderedValues().x + 100f, rect.sizeDelta.y);

        // allow update
        activated = true;
    }

    private void Update()
    {
        if (!activated && selected) return;

        int oldIndex = selectIndex;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectIndex++;
            if (selectIndex >= selectionList.Count)
            {
                selectIndex = 0;
            }
            AkSoundEngine.PostEvent("UI_Selection", gameObject);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectIndex--;
            if (selectIndex < 0)
            {
                selectIndex = selectionList.Count-1;
            }
            AkSoundEngine.PostEvent("UI_Selection", gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            resultIndex = selectIndex;
            selected = true;
            StartCoroutine(PlaySelectAnimation(resultIndex, 0.4f));
            AkSoundEngine.PostEvent("UI_Confirm", gameObject);
        }

        if (oldIndex != selectIndex)
        {
            selectionList[oldIndex].GetComponent<Image>().DOFade(0.0f, selectionFadeTime);
            selectionList[selectIndex].GetComponent<Image>().DOFade(1.0f, selectionFadeTime);


            // get reference
            RectTransform rect = selectionList[selectIndex].GetComponent<RectTransform>();
            TMP_Text text = selectionList[selectIndex].GetComponentInChildren<TMP_Text>();

            rect.sizeDelta = new Vector2(text.GetRenderedValues().x + 100f, rect.sizeDelta.y);
        }
    }

    IEnumerator PlaySelectAnimation(int index, float animationTime)
    {
        // flag
        inAnimation = true;

        // move selected box
        //selectionList[index].GetComponent<RectTransform>().DOLocalMoveY(Screen.height / 5f, animationTime);

        // hide other text
        for (int i = 0; i < selectionList.Count; i++)
        {
            if (i != index)
            {
                selectionList[i].GetComponentInChildren<TMP_Text>().DOFade(0.0f, animationTime);
            }
        }

        // initialization
        bool isHiding = false;
        float timeElapsed = 0.0f;

        // reference
        Image backColor = selectionList[index].Find("Fill").GetComponent<Image>();
        Image frame = selectionList[index].GetComponent<Image>();
        TMP_Text text = selectionList[index].GetComponentInChildren<TMP_Text>();

        // setup
        backColor.color = new Color(frame.color.r, frame.color.g, frame.color.b, 0.1f);

        // loop
        while (timeElapsed <= animationTime)
        {
            isHiding = !isHiding;

            float alpha = isHiding ? 0.1f : 0.0f;
            backColor.DOFade(alpha, 0.05f);

            timeElapsed += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        backColor.DOFade(0.1f, Time.fixedDeltaTime);

        yield return new WaitForSeconds(animationTime/2f);

        // end
        backColor.DOFade(0.0f, selectionFadeTime);
        frame.DOFade(0.0f, selectionFadeTime);
        text.DOFade(0.0f, textFadeTime);


        // flag
        inAnimation = false;
    }

    public void SetSelectionColor(Color newcolor)
    {
        selectionColor = newcolor;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public int GetResult()
    {
        return resultIndex;
    }

    public bool InAnimation()
    {
        return inAnimation;
    }
}
