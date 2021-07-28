using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
 

public class bindAlpha : MonoBehaviour
{
    [SerializeField] SpriteRenderer target;

    SpriteRenderer self;

    private void Awake()
    {
        self = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            self.DOFade(target.color.a, 0.0f);
        }
    }
}
