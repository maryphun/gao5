using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueWangZong : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] bool isNewsPaperPassedToday;

    public void TriggerDialogue()
    {
        if (!isNewsPaperPassedToday)
        {
            Transform player = ReferenceManager.Instance.GetPlayer().transform;
            ReferenceManager.Instance.GetDialogueManager().RegisterNewDialogue("早安，您的报纸。", player.transform, new Vector2(0.0f, 500.0f));
            ReferenceManager.Instance.GetDialogueManager().RegisterNewDialogue("早安，刘先生。", transform, new Vector2(0.0f, 500.0f));

            isNewsPaperPassedToday = true;
        }
    }
}
