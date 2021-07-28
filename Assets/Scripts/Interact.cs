using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class Interact : MonoBehaviour
{
    enum InteractType
    {
        Collider,
        Range,
        PositionX,
        Default = Range,
    }

    public PlayerController player;
    [SerializeField] private bool oneTimeOnly = false;
    [SerializeField] private InteractType interactType = InteractType.Default;
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private float offsetY = 0.8f;
    [SerializeField] private float offsetX = 0.0f;
    [SerializeField] private float dialogueOffsetY = 150f;
    [SerializeField] private float floatSpeed = 2.0f;
    [SerializeField] private float floatOffset = 0.1f;

    [SerializeField] UnityEvent OnInteracted;

    DialogueManager dialogueManager;
    SpriteRenderer UI;
    Vector2 originalPosition;
    bool isInRange = false;
    float floatingCount;
    bool interacted = false;

    Collider2D interactCollider;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        UI = GetComponent<SpriteRenderer>();

        originalPosition = GetComponentInParent<Transform>().position; 
        originalPosition = originalPosition + GetComponentInParent<Collider2D>().offset;

        if (interactType == InteractType.Collider)
        {
            interactCollider = GetComponent<Collider2D>();
            if (interactCollider == null)
            {
                interactType = InteractType.Default;
            }
        }

        isInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInRange() && !player.IsInteracting())
        {
            if (!isInRange)
            {
                isInRange = true;
                transform.localPosition = new Vector2(0.0f, offsetY);
                floatingCount = 0.0f;
                UI.DOFade(1.0f, 0.5f);

                player.InteractRegister(this);
            }
        }
        else
        {
            if (isInRange)
            {
                isInRange = false;
                UI.DOFade(0.0f, 0.5f);

                player.InteractUnregister(this);
            }
        }

        if (!isInRange) return;

        // move
        floatingCount += Time.deltaTime * floatSpeed;
        transform.localPosition = new Vector2(0.0f + offsetX, offsetY + (Mathf.Sin(floatingCount) * floatOffset));
    }

    public float GetRange(Vector2 compare)
    {
        return Vector2.Distance(compare, originalPosition);
    }

    private bool IsInRange()
    {
        bool rtn = false;

        if (interactType == InteractType.Range)
        {
            rtn = GetRange(player.transform.position) < interactRange;
        }
        else if (interactType == InteractType.Collider)
        {
            rtn = player.GetComponent<Collider2D>().IsTouching(interactCollider);
        }
        else if (interactType == InteractType.PositionX)
        {
            rtn = Mathf.Abs(player.transform.position.x - originalPosition.x) < interactRange;
        }

        if (interacted && oneTimeOnly)
        {
            rtn = false;
        }

        return rtn;
    }

    public void InteractTrigger()
    {
        OnInteracted.Invoke();
        interacted = true;
    }

    public void EndInteract()
    {
        if (player.IsInteracting())
        {
            player.SetInteractMode(false);
        }
    }
}
