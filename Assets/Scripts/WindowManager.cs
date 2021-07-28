using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<string, Window> windowList = new Dictionary<string, Window>();
    private Transform canvas;
    private GameObject windowObject;
    private bool isInitialized;

    public void Initialization()
    {
        if (isInitialized) return;

        canvas = GameObject.Find("Canvas").transform;
        windowObject = Resources.Load("WindowPrefab", typeof(GameObject)) as GameObject;

        isInitialized = true;
    }

    public void CreateWindow(string name, Vector2 pos, Vector2 size)
    {
        if (!windowList.ContainsKey(name))
        {
            GameObject obj = Instantiate(windowObject, canvas);
            Window comp = obj.GetComponent<Window>();

            // rename
            obj.name = "[" + name + "] window";

            obj.GetComponent<RectTransform>().anchoredPosition = pos;
            obj.GetComponent<RectTransform>().sizeDelta = size;
            obj.SetActive(false);

            if (comp)
            {
                windowList.Add(name, comp);
                comp.Initialize();
            }
        }
        else
        {
            // window already exist.
            Debug.Log("Window <color=teal>" + name + "</color> already exist.");
        }
    }

    public void Open(string name, float time)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            // set size to 0
            comp.GetComponent<RectTransform>().sizeDelta = new Vector2(0, comp.GetComponent<RectTransform>().sizeDelta.y);
            // enable object
            comp.gameObject.SetActive(true);
            comp.SetWindowState(Window.WindowState.opening);
            if (time > 0.0f)
            {
                // resize over time
                comp.ResizeX(comp.GetWindowSize().x, time);
            }
            else
            {
                // instant open
                comp.ResizeX(comp.GetWindowSize().x);
            }
        }
    }

    public void Close(string name, float time, bool destroy)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetWindowState(Window.WindowState.closing);
            if (time > 0.0f)
            {
                comp.ResizeX(0.0f, time);
                comp.SetActiveAfterDelay(false, time);
            }
            else
            {
                comp.ResizeX(0.0f);
                comp.gameObject.SetActive(false);
            }

            if (destroy)
            {
                Destroy(comp.gameObject, time);
                windowList.Remove(name);
            }
        }
    }

    // ====================================Text====================================

    /// <param name="interval"> if set to 0 or below means instant </param>
    public void SetText(string name, string text, float interval)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            if (interval > 0.0f)
            {
                comp.SetText(text, interval);
            }
            else
            {
                comp.SetText(text);
            }
        }
    }

    public void SetText(string name, string text)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetText(text);
        }
    }

    public void SkipTextWriter(string name)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SkipText();
        }
    }

    public void SetTextSize(string name, float size)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetTextSize(size);
        }
    }
    public void SetTextMargin(string name, Vector4 vec)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetTextMargin(vec);
        }
    }

    public void SetTextColor(string name, Color color)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetTextColor(color);
        }
    }

    public void SetTextOffset(string name, Vector2 offset)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetTextOffset(offset);
        }
    }

    public void SetTextAlignment(string name, CustomTextAlignment alignment)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetTextAlignment(alignment);
        }
    }
    public void SetTextWrappingMode(string name, bool enable)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetTextWrappingMode(enable);
        }
    }

    public void SetTextEnableSE(string name, bool enable)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.SetTextEnableSE(enable);
        }
    }

    public TMP_Text AddNewText(string name, string text, Vector2 location, float size, Color color)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            // check if the text is out of window
            if (location.x <= 0 || location.y <= 0
                || location.x >= comp.GetWindowSize().x || location.y >= comp.GetWindowSize().y)
            {
                Debug.Log(location + " text is out of window");
                return null;
            }
            return comp.AddNewText(text, location, size, color);
        }

        return null;
    }

    public TMP_Text GetMainText(string name)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            return comp.GetMainText();
        }

        return null;
    }

    // ====================================Image====================================
    public void AddNewImage(string name, string path, Vector2 location, Vector2 size, bool behindWindow = false, float fadeTime = 0.0f)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            comp.AddNewImage(path, location, size, behindWindow);
        }
    }

    // ====================================Get====================================
    public bool IsWindowExist(string name)
    {
        return windowList.ContainsKey(name);
    }

    public bool IsTypeWriting(string name)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            return comp.GetIsTypeWriting();
        }

        return false;
    }

    public bool IsWindowOpen(string name)
    {
        Window comp;
        if (GetReference(name, out  comp))
        {
            return comp.GetIsOpen();
        }

        return false;
    }

    public Window.WindowState GetWindowState(string name)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            return comp.GetState();
        }

        return Window.WindowState.none;
    }

    public Window GetWindowObject(string name)
    {
        Window comp;
        if (GetReference(name, out comp))
        {
            return comp;
        }

        return null;
    }

    private bool GetReference(string name, out Window component)
    {
        bool rtn = false;

        // get reference
        if (windowList.ContainsKey(name))
        {
            Window comp = windowList[name];
            component = comp;
            rtn = true;
        }
        else
        {
            component = null;
            Debug.Log("Window <color=teal>" + name + "</color> not found.");
        }

        return rtn;
    }
}
