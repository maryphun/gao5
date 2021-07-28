using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] private LayerMask collideMask;
    [SerializeField] private Vector2 cameraOffset;

    [Header("References")]
    [SerializeField] CharacterGraphic graphic;

    [Header("Audio")]
    [SerializeField] string walkSound = "PLYR_Walking";
    [SerializeField,Range(0.1f, 0.5f)] float SEInterval = 0.2f;

    private Collider2D collider;
    private CameraFollow mainCamera;
    [SerializeField] private List<Interact> interactList = new List<Interact>();
    private bool interactMode = false;
    private float walkSoundIntervalCount;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        interactList.Clear();
        mainCamera = Camera.main.GetComponent<CameraFollow>();
        mainCamera.SetCameraFollowTarget(transform);
        walkSoundIntervalCount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool interactKey = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space);

        // disable controls during interact mode
        if (interactMode)
        {
            movInput = Vector2.zero;
            interactKey = false;
        }

        // calculate new position
        if (movInput.magnitude > 0.0f)
        {
            Vector2 newPos = transform.position;
            newPos += movInput * baseMoveSpeed * Time.deltaTime;

            // check if target position is movable
            Move(newPos, movInput);

            // flip
            if (movInput.x > 0.0f)
            {
                graphic.ChangeFacing(CharacterFacing.Right);
                mainCamera.SetCameraOffsetX(cameraOffset.x + 0.5f);
            }
            else if (movInput.x < 0.0f)
            {
                graphic.ChangeFacing(CharacterFacing.Left);
                mainCamera.SetCameraOffsetX(cameraOffset.x + -0.5f);
            }
            else if (movInput.y < 0.0f)
            {
                graphic.ChangeFacing(CharacterFacing.Bottom);
                mainCamera.SetCameraOffsetX(cameraOffset.x);
            }
            else if (movInput.y > 0.0f)
            {
                graphic.ChangeFacing(CharacterFacing.Top);
                mainCamera.SetCameraOffsetX(cameraOffset.x);
            }

            // audio
            walkSoundIntervalCount += Time.deltaTime;
            if (walkSoundIntervalCount > SEInterval)
            {
                walkSoundIntervalCount = 0.0f;
                AkSoundEngine.PostEvent(walkSound, gameObject);
            }
        }
        else
        {
            walkSoundIntervalCount = 0;
        }

        // camera
        mainCamera.SetCameraOffsetY(cameraOffset.y + transform.position.y / 7.5f);

        // animation
        graphic.Run(movInput.magnitude > 0.0f);

        // interact
        if (interactKey && interactList.Count > 0)
        {
            float range = 1000.0f;
            Interact nearestObj = interactList[0];
            foreach (Interact obj in interactList)
            {
                // compare
                float tmp = obj.GetRange(transform.position);
                if (tmp < range)
                {
                    range = tmp;
                    nearestObj = obj;
                }
            }
            nearestObj.InteractTrigger();
            interactMode = true;
}
    }

    private void Move(Vector2 newPosition, Vector2 direction)
    {
        Vector2 mag = newPosition - new Vector2(transform.position.x, transform.position.y);
        Vector2 colliderPos = new Vector2(transform.position.x, transform.position.y) + collider.offset;
        Vector2 adjustedPos = new Vector2(colliderPos.x, colliderPos.y) + mag;

        // collision check before actually move the player
        Vector2 destinationTop = new Vector2(adjustedPos.x + (direction.x * collider.bounds.extents.x), colliderPos.y + collider.bounds.extents.y - 0.05f);
        Vector2 destinationMid = new Vector2(adjustedPos.x + (direction.x * collider.bounds.extents.x), colliderPos.y);
        Vector2 destinationBottom = new Vector2(adjustedPos.x + (direction.x * collider.bounds.extents.x), colliderPos.y - collider.bounds.extents.y + 0.05f);

        // assign new position to the character (x)
        if (!CollisionCheck(destinationTop, collideMask)     // collision with wall
            && !CollisionCheck(destinationBottom, collideMask)
            && !CollisionCheck(destinationMid, collideMask))
        {
            transform.position = new Vector2(newPosition.x, transform.position.y);
        }


        // collision check before actually move the player
        Vector2 destinationLeft = new Vector2(colliderPos.x - collider.bounds.extents.x, adjustedPos.y + (direction.y * collider.bounds.extents.y));
        Vector2 destinationMiddle = new Vector2(colliderPos.x, (adjustedPos.y + (direction.y * collider.bounds.extents.y)));
        Vector2 destinationRight = new Vector2(colliderPos.x + collider.bounds.extents.x, adjustedPos.y + (direction.y * collider.bounds.extents.y));

        // assign new position to the character (y)
        if (!CollisionCheck(destinationLeft, collideMask)     // collision with wall
            && !CollisionCheck(destinationRight, collideMask)
            && !CollisionCheck(destinationMiddle, collideMask))
        {
            transform.position = new Vector2(transform.position.x, newPosition.y);
        }

        Debug.DrawLine(adjustedPos, destinationMiddle);
    }

    private bool CollisionCheck(Vector2 point, LayerMask layerMask)
    {
        return (Physics2D.OverlapPoint(point, layerMask));
    }

    public void InteractRegister(Interact obj)
    {
        if (!interactList.Contains(obj))
        {
            interactList.Add(obj);
        }
    }

    public void InteractUnregister(Interact obj)
    {
        if (interactList.Contains(obj))
        {
            interactList.Remove(obj);
        }
    }
    public bool IsInteracting()
    {
        return interactMode;
    }

    public void SetInteractMode(bool boolean)
    {
        interactMode = boolean;
    }

    public void Teleport(float x, float y, float time)
    {
        Vector2 target;
        if (x == 0)
        {
            target.x = transform.position.x;
        }
        else
        {
            target.x = x;
        }
        if (y == 0)
        {
            target.y = transform.position.y;
        }
        else
        {
            target.y = y;
        }

        transform.DOMove(target, time);
    }

    public void ChangeFaceDirection(CharacterFacing direction)
    {
        graphic.ChangeFacing(direction);
    }

    public void SetCameraOffsetY(float y)
    {
        cameraOffset.y = y;
    }

    public void EndInteractWithDelay(float time)
    {
        StartCoroutine(EndInteract(time));
    }

    private IEnumerator EndInteract(float delay)
    {
        yield return new WaitForSeconds(delay);

        SetInteractMode(false);
    }
}
