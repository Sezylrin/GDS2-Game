using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class ProjectileAbility : AbilityBase
{
    // Start is called before the first frame update
    [SerializeField][ReadOnly]
    private ProjectileVariantSO selected;

    [SerializeField]
    private Transform sprite;

    [SerializeField]
    private Rigidbody2D rb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void CastAbility()
    {
        selected = selectedAbility as ProjectileVariantSO;
        LaunchTowardsTarget();
        base.CastAbility();
    }

    private void LaunchTowardsTarget()
    {
        transform.position = initialPos;
        rb.velocity = direction.normalized * selected.speed;
        sprite.rotation = CustomMath.LookAt2D(direction);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (CurrentPierce <= 0 || collision.CompareTag(Tags.T_Terrain))
        {
            PoolSelf();
        }
    }
}
