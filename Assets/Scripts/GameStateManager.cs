using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] PlayerController player;
    // Start is called before the first frame update
    void Awake()
    {
        ReferenceManager.Instance.Init();
        WindowManager.Instance.Initialization();
        //DialogueManager
    }

    private void Start()
    {
        dialogueManager.RegisterNewNarrative("早安，我的名字叫做刘连。我是这栋大楼的送报员，每天早上都会去分送报纸给各户人家。");
        dialogueManager.RegisterNewNarrative("今天的报纸也来了。");
        dialogueManager.RegisterNewNarrative("去送报纸吧。");
        player.SetInteractMode(true);
    }
}
