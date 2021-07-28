using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] RoomType targetRoom;
    [SerializeField] Vector2 targetPosition;
    [SerializeField] CharacterFacing direction = CharacterFacing.Max;
    [SerializeField] float EndInteractTime = 2.0f;
    [SerializeField] float teleportTime = 1.5f;
    [SerializeField] float newCameraOffsetY = 0.5f;

    [SerializeField] bool withFade = false;

    [SerializeField] bool enableX = true;
    [SerializeField] bool enableY = true;

    [SerializeField] EnvironmentManager environmentMng;
    [SerializeField] PlayerController player;
    [SerializeField] MapName mapNameDisplay;
    [SerializeField] Image screenAlpha;

    [SerializeField] ObjectGraphic fadeInDoor;
    [SerializeField] ObjectGraphic fadeOutDoor;

    [SerializeField] string mapName;

    Interact interactSource;

    private void Start()
    {
        if (environmentMng == null)
        {
            environmentMng = FindObjectOfType<EnvironmentManager>();
        }
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
        if (mapNameDisplay == null)
        {
            mapNameDisplay = FindObjectOfType<MapName>();
        }
        if (screenAlpha == null)
        {
            Debug.Log("<color=red>Screen Alpha Not Found!!!</color>");
        }
    }

    public void SwitchRoom(Interact source)
    { 
        if (!enableX) targetPosition.x = 0.0f;
        if (!enableY) targetPosition.y = 0.0f;

        float time = 0.0f;
        if (withFade)
        {
            time = 0.4f;
            screenAlpha.DOFade(1.0f, time);
        }
        interactSource = source;
        StartCoroutine(Transition(time));
    }

    private IEnumerator Transition(float delay)
    {
        if (delay > 0.0f)
        {
            yield return new WaitForSeconds(delay);
        }

        player.Teleport(targetPosition.x, targetPosition.y, teleportTime);
        if (direction != CharacterFacing.Max) player.ChangeFaceDirection(direction);
        player.SetCameraOffsetY(newCameraOffsetY);
        environmentMng.SwitchRoom(targetRoom);
        player.EndInteractWithDelay(EndInteractTime);
        player.InteractUnregister(interactSource);
        if (withFade)
        {
            screenAlpha.DOFade(0.0f, EndInteractTime);
        }

        if (fadeInDoor != null)
        {
            fadeInDoor.Activate(false);
        }
        if (fadeOutDoor != null)
        {
            fadeOutDoor.Activate(true);
        }

        // show map name
        if (mapName.Length > 0)
        {
            mapNameDisplay.ShowName(mapName);
        }

        yield break;
    }
}
