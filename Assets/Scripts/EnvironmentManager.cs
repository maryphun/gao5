using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum RoomType
{ 
    PLAYERROOM,
    CORRIDOR4F,
    CORRIDOR3F,
    CORRIDOR2F,
    WANG4B,
    MAX,
    NONE,
}

public class EnvironmentManager : MonoBehaviour
{
    [System.Serializable]
    public struct RoomComponent
    {
        public SpriteRenderer[] morning;
        public SpriteRenderer[] night;
        public SpriteRenderer lightbulb;
        public SpriteRenderer roomAlpha;
    }

    [Header("Room")]
    [SerializeField] GameObject playerRoom;
    [SerializeField] GameObject corridor4f;
    [SerializeField] GameObject corridor3f;
    [SerializeField] GameObject corridor2f;
    [SerializeField] GameObject wang4b;

    [Header("Room Component")]
    [SerializeField] RoomComponent playerRoomComponent;
    [SerializeField] RoomComponent corridor4fComponent;
    [SerializeField] RoomComponent corridor3fComponent;
    [SerializeField] RoomComponent corridor2fComponent;
    [SerializeField] RoomComponent wang4bComponent;

    [Header("Specific Reference")]
    [SerializeField] SpriteRenderer alphaExceptCalender;

    [Header("Parameter")]
    [SerializeField] float lightBulbTime = 2f;

    [Header("Audio")]
    [SerializeField] string playerRoomAmbient = "WallClock";

    private bool isNight = false;
    private RoomType currentRoom = RoomType.NONE;
    RoomComponent currentRoomComponent;

    bool[] lightOn;

    // Start is called before the first frame update
    void Start()
    {
        // default
        lightOn = new bool[(int)RoomType.MAX];
        SwitchRoom(RoomType.PLAYERROOM, true);
        Morning();
    }

    public void SwitchLightBulb(bool boolean)
    {
        lightOn[(int)currentRoom] = boolean;
        if (boolean)
        {
            StartCoroutine(TurnOnLightBulbPattern(lightBulbTime/2f));
        }
        else
        {
            currentRoomComponent.lightbulb.DOFade(0.0f, lightBulbTime);
            if (isNight)
            {
                currentRoomComponent.night[1].DOFade(1.0f, lightBulbTime);
                currentRoomComponent.night[2].DOFade(0.0f, lightBulbTime);
                currentRoomComponent.roomAlpha.DOFade(0.7f, lightBulbTime);
            }
            else // morning
            {
                currentRoomComponent.roomAlpha.DOFade(0.0f, lightBulbTime);
                currentRoomComponent.morning[0].DOFade(0.0f, lightBulbTime);
            }
        }
    }

    private IEnumerator TurnOnLightBulbPattern(float time)
    {
        currentRoomComponent.lightbulb.DOFade(Random.Range(0.2f, 0.5f), 0.0f);

        float alp = 0.0f;

        for (int i = 0; i < 8; i++)
        {
            if (!lightOn[(int)currentRoom]) yield break;
            yield return new WaitForSeconds(time / 30.0f);
            alp = alp == 0.0f ? Random.Range(0.2f, 0.5f) : 0.0f;
            currentRoomComponent.lightbulb.DOFade(alp, 0.0f);
        }

        if (!lightOn[(int)currentRoom]) yield break;
        yield return new WaitForSeconds(time / 2.50f);

        if (!lightOn[(int)currentRoom]) yield break;
        currentRoomComponent.lightbulb.DOFade(1.0f, time / 5.0f);

        if (isNight)
        {
            currentRoomComponent.night[1].DOFade(0.0f, time / 5.0f);
            currentRoomComponent.night[2].DOFade(1.0f, time / 5.0f);
            currentRoomComponent.roomAlpha.DOFade(0.0f, time / 5.0f);
        }
        else // morning
        {
            currentRoomComponent.morning[0].DOFade(1.0f, time / 5.0f);
        }
    }

    public void Night()
    {
        isNight = true;
        foreach (SpriteRenderer sprite in currentRoomComponent.morning)
        {
            sprite.DOFade(0.0f, 0.0f);
        }

        currentRoomComponent.roomAlpha.DOFade(0.7f, 0.0f);

        SpriteRenderer tmp;
        if (lightOn[(int)currentRoom])
        {
            tmp = currentRoomComponent.night[1];
            if (tmp != null)
            {
                tmp.DOFade(0.0f, 0.0f);
            }
            tmp = currentRoomComponent.night[2];
            if (tmp != null)
            {
                tmp.DOFade(1.0f, 0.0f);
            }
            tmp = currentRoomComponent.roomAlpha;
            if (tmp != null)
            {
                tmp.DOFade(0.0f, 0.0f);
            }
        }
        else
        {
            tmp = currentRoomComponent.night[1];
            if (tmp != null)
            {
                tmp.DOFade(1.0f, 0.0f);
            }
            tmp = currentRoomComponent.night[2];
            if (tmp != null)
            {
                tmp.DOFade(0.0f, 0.0f);
            }
        }
    }
    public void Morning()
    {
        isNight = false;
        foreach (SpriteRenderer sprite in currentRoomComponent.night)
        {
            sprite.DOFade(0.0f, 0.0f);
        }

        currentRoomComponent.roomAlpha.DOFade(0.0f, 0.0f);

        SpriteRenderer tmp;
        if (lightOn[(int)currentRoom])
        {
            tmp = currentRoomComponent.morning[0];
            if (tmp != null)
            {
                tmp.DOFade(1.0f, 0.0f);
            }
        }
        else
        {
            tmp = currentRoomComponent.morning[0];
            if (tmp != null)
            {
                tmp.DOFade(0.0f, 0.0f);
            }
        }
    }

    public void SwitchRoom(RoomType target, bool isInitiate = false)
    {
        // initiate variable
        GameObject[] rooms = GetRoomListArray();
        RoomComponent[] roomComponents = GetRoomComponentListArray();

        // swap sorting order to back
        for (int i = 0; i < (int)RoomType.MAX; i++)
        {
            if (i == (int)currentRoom || (isInitiate && i != (int)target))
            {
                Transform env = rooms[i].transform.Find("Environment");
                SwapChildSorting(rooms[i].transform, true);
                SwapChildSorting(env, true);
                Transform roomAlpha = rooms[i].transform.Find("room-alpha");
                roomAlpha.GetComponent<SpriteRenderer>().DOFade(1.0f, 0.5f);
            }
        }

        // swap sorting order to front
        for (int i = 0; i < (int)RoomType.MAX; i++)
        {
            if (i == (int)target)
            {
                Transform env = rooms[i].transform.Find("Environment");
                SwapChildSorting(rooms[i].transform, false);
                SwapChildSorting(env, false);
                Transform roomAlpha = rooms[i].transform.Find("room-alpha");
            }
        }

        // set object active
        SetRoomObjectActive(currentRoom, false);
        SetRoomObjectActive(target, true);

        currentRoom = target;

        // swap component
        currentRoomComponent = roomComponents[(int)target];

        // initiate night or morning
        if (isNight)
        {
            Night();
        }
        else
        {
            Morning();
        }

        // Ambient
        AkSoundEngine.StopAll(gameObject);
        if (currentRoom == RoomType.PLAYERROOM)
        {
            AkSoundEngine.PostEvent(playerRoomAmbient, gameObject);
        }
    }

    private void SetRoomObjectActive(RoomType roomType, bool boolean)
    {
        // initiate variable
        GameObject[] rooms = GetRoomListArray();

        // search the specific room
        for (int i = 0; i < (int)RoomType.MAX; i++)
        {
            if (i == (int)roomType)
            {
                // objects
                Transform objParent = rooms[i].transform.Find("Objects");
                objParent.gameObject.SetActive(boolean);

                // collisions
                Transform collisions = rooms[i].transform.Find("Collisions");
                collisions.gameObject.SetActive(boolean);
            }
        }
    }

    private void SwapChildSorting(Transform parent, bool GoToBack)
    {
        List<SpriteRenderer> conv = new List<SpriteRenderer>();

        if (parent.GetComponent<SpriteRenderer>())
        {
            conv.Add(parent.GetComponent<SpriteRenderer>());
        }

        foreach (Transform t in parent)
        {
            SpriteRenderer renderer = t.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                conv.Add(renderer);
            }
        }

        if (GoToBack)
        {
            ReplaceSortingLayer(conv, "BackObject", "BackObjectOtherRoom");
            ReplaceSortingLayer(conv, "Object", "ObjectOtherRoom");
            ReplaceSortingLayer(conv, "FrontObject", "FrontObjectOtherRoom");
            ReplaceSortingLayer(conv, "Shadow", "ShadowOtherRoom");
        }
        else
        {
            ReplaceSortingLayer(conv, "ShadowOtherRoom", "Shadow");
            ReplaceSortingLayer(conv, "FrontObjectOtherRoom", "FrontObject");
            ReplaceSortingLayer(conv, "ObjectOtherRoom", "Object");
            ReplaceSortingLayer(conv, "BackObjectOtherRoom", "BackObject");
        }
    }

    private void ReplaceSortingLayer(List<SpriteRenderer> list, string from, string to)
    {
        foreach (SpriteRenderer renderer in list)
        {
            if (renderer.sortingLayerName == from)
            {
                renderer.sortingLayerName = to;
            }
        }
    }

    private GameObject[] GetRoomListArray()
    {
        GameObject[] rooms = new GameObject[(int)RoomType.MAX];
        rooms[0] = playerRoom;
        rooms[1] = corridor4f;
        rooms[2] = corridor3f;
        rooms[3] = corridor2f;
        rooms[4] = wang4b;

        return rooms;
    }

    private RoomComponent[] GetRoomComponentListArray()
    {
        RoomComponent[] roomComponents = new RoomComponent[(int)RoomType.MAX];
        roomComponents[0] = playerRoomComponent;
        roomComponents[1] = corridor4fComponent;
        roomComponents[2] = corridor3fComponent;
        roomComponents[3] = corridor2fComponent;
        roomComponents[4] = wang4bComponent;

        return roomComponents;
    }
}
