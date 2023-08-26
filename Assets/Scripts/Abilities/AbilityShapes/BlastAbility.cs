using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class BlastAbility : AbilityBase
{
    [SerializeField][ReadOnly]
    private BlastVariantSO selected;
    [SerializeField]
    private PolygonCollider2D col2D;

    private Coroutine blastCoroutine;

    private Vector2[] corners = new Vector2[4];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void CastAbility()
    {
        selected = selectedAbility as BlastVariantSO;
        transform.position = initialPos;
        RotateSelf();
        col2D.SetPath(0, selected.initialShape);
        Array.Copy(selected.initialShape, corners,4);
        blastCoroutine = StartCoroutine(StartBlastExpand());
    }

    private void RotateSelf()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private IEnumerator StartBlastExpand()
    {
        float startTime = Time.time;
        Vector2 endCornerRight = selected.finalShape[0];
        Vector2 startRight = col2D.points[0];
        Vector2 endCornerLeft = selected.finalShape[1];
        Vector2 startLeft = col2D.points[1];
        for (float timer = 0; timer < selected.speed; timer += Time.deltaTime)
        {
            float ratio = (Time.time - startTime) / selected.speed;
            if (Vector2.Distance(col2D.points[0], endCornerRight) > 0.1f)
            {
                corners[0] = Vector2.Lerp(startRight, endCornerRight, ratio);
            }
            else
            {
                corners[0] = selected.finalShape[0];
            }
            if (Vector2.Distance(col2D.points[1], endCornerLeft) > 0.1f)
            {
                corners[1] = Vector2.Lerp(startLeft, endCornerLeft, ratio);
            }
            else
            {
                corners[1] = selected.finalShape[1];
            }
            col2D.SetPath(0, corners);
            yield return null;
        }
        StopExpand();
    }

    private void StopExpand()
    {
        if (blastCoroutine != null)
        {
            blastCoroutine = null;
        }
    }

    protected override void InvokePoolSelf(object sender, Timer.OnTimeIsZeroEventArgs e)
    {
        if (e.timerSlot == 0)
        {
            poolSelf();
            base.InvokePoolSelf(sender, e);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
