using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class AreaAbility : AbilityBase
{
    // Start is called before the first frame update
    [SerializeField][ReadOnly]
    private AOEVariantSO selected;

    [SerializeField]
    private CircleCollider2D col2D;
    [SerializeField]
    private Transform AOETr;
    [SerializeField]
    private SpriteRenderer rend;

    private Coroutine aoeCoroutine;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void CastAbility()
    {
        selected = selectedAbility as AOEVariantSO;
        transform.position = initialPos;
        SpawnAoeEffect();
        //polish
        base.CastAbility();
    }

    private void SpawnAoeEffect()
    {
        if (selected.growthSpeed != 0)
        {
            aoeCoroutine = StartCoroutine(StartAoeExpand());
        }
        else
        {
            col2D.radius = selected.radius;
        }
    }

    private IEnumerator StartAoeExpand()
    {
        float startTime = Time.time;
        float endRadius = selected.radius;
        for (float timer = 0; timer < selected.growthSpeed; timer += Time.deltaTime)
        {
            float ratio = (Time.time - startTime) / selected.growthSpeed;
            col2D.radius = ratio * endRadius;
            if (timer + Time.deltaTime >= selected.growthSpeed)
                col2D.radius = endRadius;
            float radius = col2D.radius;
            yield return null;
        }
        StopExpand();
    }

    private void StopExpand()
    {
        if (aoeCoroutine != null)
        {
            aoeCoroutine = null;
        }
    }

    private void StopExpand(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
        StopExpand();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
