using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class DashAbility : AbilityBase
{
    [SerializeField, ReadOnly]
    private DashVariantSO selected;

    [SerializeField]
    private CapsuleCollider2D col2D;
    [SerializeField]
    private Transform Animation;
    // Start is called before the first frame update

    protected override void CastAbility()
    {
        selected = selectedAbility as DashVariantSO;
        col2D.size = selected.hitboxSize;
        GameManager.Instance.PCM.control.Dash(selected.distance, selected.lifeTime, direction, selected.color.color, 0.5f);
        Animation.rotation = CustomMath.LookAt2D(direction);
        base.CastAbility();
    }

    private void Update()
    {
        transform.position = followTR.position;
    }
}
