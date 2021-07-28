using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] EnvironmentManager lightmanager;
    [SerializeField] float EndInteractTime = 2.0f;
    [SerializeField] bool turnOn;

    Interact interactsource;

    [Header("Audio")]
    [SerializeField] string onLightSE = "Light_Switch";
    [SerializeField] string offLightSE = "Light_Switch";

    private void Start()
    {
        if (lightmanager == null)
        {
            lightmanager = FindObjectOfType<EnvironmentManager>();
        }
    }

    public void SwitchLight(Interact source)
    {
        turnOn = !turnOn;
        lightmanager.SwitchLightBulb(turnOn);
        interactsource = source;
        StartCoroutine(EndInteract(EndInteractTime));

        if (turnOn)
        {
            AkSoundEngine.PostEvent(onLightSE, gameObject);
        }
        else
        {
            AkSoundEngine.PostEvent(offLightSE, gameObject);
        }
    }

    private IEnumerator EndInteract(float delay)
    {
        yield return new WaitForSeconds(delay);

        interactsource.EndInteract();
    }
}

