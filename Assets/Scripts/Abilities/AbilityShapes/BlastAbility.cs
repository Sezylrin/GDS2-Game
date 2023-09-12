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
        //angle -= 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private IEnumerator StartBlastExpand()
    {
        float startTime = Time.time;
        for (float timer = 0; timer < selected.speed; timer += Time.deltaTime)
        {
            float ratio = (Time.time - startTime) / selected.speed;
            for (int i = 0; i < 4; i++)
            {
                if (Vector2.Distance(col2D.points[i], selected.finalShape[i]) > 0.1f)
                {
                    corners[i] = Vector2.Lerp(selected.initialShape[i],selected.finalShape[i], ratio);
                }
                else
                {
                    corners[i] = selected.finalShape[i];
                }
            }
            col2D.SetPath(0, corners);
            yield return null;
        }
        if(selected.speed == 0)
        {
            col2D.SetPath(0, selected.finalShape);
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

    protected override void InvokePoolSelf(object sender, EventArgs e)
    {
        PoolSelf();
        base.InvokePoolSelf(sender, e);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
