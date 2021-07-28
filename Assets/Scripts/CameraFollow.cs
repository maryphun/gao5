using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float delay = 1.0f;
    [SerializeField] Vector2 offset;
    [SerializeField] float minimumY, maximumY;

    Vector2 originalPosition;
    float valueY;
    private void Awake()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            enabled = false;
            return;
        }

        transform.DOMoveX(target.position.x + offset.x, delay);
        transform.DOMoveY(originalPosition.y + offset.y, delay);
        //transform.DOMoveY(originalPosition.y - minimumY + (valueY * maximumY), delay);
    }

    public void SetCameraFollowTarget(Transform trsfm)
    {
        target = trsfm;
        enabled = true;
    }

    public void SetCameraFollowDelay(float time)
    {
        delay = time;
    }

    public void SetCameraOffset(Vector2 offs)
    {
        offset = offs;
    }

    public void SetCameraOffsetX(float x)
    {
        offset = new Vector2(x, offset.y);
    }

    public void SetCameraOffsetY(float y)
    {
        offset = new Vector2(offset.x, y);
    }

    public void SetCameraValueY(float value)
    {
        valueY = Mathf.Clamp(value, -1f, 1f);
    }

    public void ResetOriginalPosition(Vector2 newPos)
    {
        originalPosition = newPos;
    }
}
