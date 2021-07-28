using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : Singleton<ReferenceManager>
{
    private PlayerController player;
    private DialogueManager dialogueManager;
    private GameStateManager gameStateManager;

    public void Init()
    {
        player = FindObjectOfType<PlayerController>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        gameStateManager = FindObjectOfType<GameStateManager>();
    }

    public PlayerController GetPlayer()
    {
        return player;
    }

    public DialogueManager GetDialogueManager()
    {
        return dialogueManager;
    }

    public GameStateManager GetGameStateManager()
    {
        return gameStateManager;
    }
}
