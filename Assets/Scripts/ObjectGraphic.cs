using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectGraphic : MonoBehaviour
{
    [SerializeField] bool transparentForPlayer = false;
    [SerializeField] bool fadeWhenActivate = false;

    SpriteRenderer renderer;
    Vector2 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>(); 
        originalPosition = GetComponentInParent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sortingOrder = (int)(-transform.position.y * 100f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collide");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("exit");
    }

    public void Activate(bool boolean)
    {
        if (fadeWhenActivate)
        {
            if (boolean)
            {
                renderer.DOFade(0.0f, 0.0f);
                renderer.DOFade(1.0f, 0.5f);
            }
            else
            {
                renderer.DOFade(0.0f, 0.5f);
            }
        }
    }
}
