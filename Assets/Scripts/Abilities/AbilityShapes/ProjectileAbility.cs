using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : AbilityBase
{
    // Start is called before the first frame update
    [SerializeField]
    private CircleCollider2D col2D;
    [SerializeField]
    private ProjectileElementSO selected;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void CastAbility()
    {
        selected = selectedAbility as ProjectileElementSO;
        LaunchTowardsTarget();

    }

    protected override void InvokePoolSelf(object sender, Timer.OnTimeIsZeroEventArgs e)
    {
        if (e.timerSlot == 0)
        {
            Debug.Log("pooled self");
            poolSelf();
            base.InvokePoolSelf(sender, e);
        }
    }

    private void LaunchTowardsTarget()
    {
        transform.position = initialPos;
        rb.velocity = direction.normalized * selected.speed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (CurrentPierce <= 0 || collision.CompareTag(Tags.T_Terrain))
        {
            poolSelf();
        }
    }
}
