using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAudio : MonoBehaviour
{
    [SerializeField] string audioName;

    public void PlayAudio()
    {
        AkSoundEngine.PostEvent(audioName, gameObject);
    }
}
