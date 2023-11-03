using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheetah : Enemy
{
    private CheetahScriptableObject CheetahSO;
    private float swipeRange;
    private float swipeDistance;
    private float swipeDuration;
    private bool isSwiping;
    private Pool<EnemyProjectile> daggerPool;
    [SerializeField, Space(20),Header("Cheetah")]
    private Transform daggerSpawnPoint;
    [SerializeField]
    private GameObject daggerPF;
    [SerializeField]
    private int daggerAmount;
    [SerializeField, Range(0,45)]
    private float degreeOffset;
    [SerializeField]
    private float daggerSpeed;
    [SerializeField, Space(20)]
    private int swipeAmount;
    [SerializeField]
    private Collider2D swipeHitbox;
    [SerializeField]
    private float multiSwipeDelay;
    [SerializeField]
    private Collider2D hitbox;
    private LayerMask initial;


    protected override void Start()
    {
        base.Start();
        GameManager.Instance.PoolingManager.FindPool(daggerPF,out daggerPool);
        initial = col2D.excludeLayers;
    }
    protected override void SetInheritanceSO()
    {
        CheetahSO = SO as CheetahScriptableObject;
    }

    public override void SetStatsFromScriptableObject()
    {
        base.SetStatsFromScriptableObject();
        swipeRange = CheetahSO.swipeRange;
        swipeDistance = CheetahSO.swipeDistance;
        swipeDuration = CheetahSO.swipeDuration;
    }

    protected override void DetermineAttackPathing()
    {
        switch (CurrentAttack)
        {
            case 1:
                Vector3 targetpoint = targetTr.position;
                Vector3 minimumRange = transform.position - targetpoint;
                minimumRange = minimumRange.normalized * swipeRange;
                targetpoint += minimumRange;
                SetDestination(targetpoint);
                break;
            case 2:
                SetDestination(transform.position);
                Debug.Log("Attempting to Shockwave");
                break;
            case 3:
                SetDestination(transform.position);
                Debug.Log("Attempting to Stomp");
                break;
        }
    }
    #region Attack1
    protected override void Attack1()
    {
        dir = (targetTr.position - transform.position).normalized;
        StartCoroutine(Swipe());
    }
    private IEnumerator Swipe()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.Swipe);
        isSwiping = true;
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        col2D.excludeLayers = TargetLayer;
        swipeHitbox.includeLayers = TargetLayer;
        swipeHitbox.enabled = true;
        float startTime = Time.time;
        Vector2 endPos = (Vector2)transform.position + (dir * swipeDistance);
        Vector2 startPos = transform.position;
        Vector2 dashDirection = endPos - startPos;
        Vector2 size = ((CapsuleCollider2D)hitbox).size;
        for (float timer = 0; timer < swipeDuration; timer += Time.deltaTime)
        {

            float ratio = (Time.time - startTime) / swipeDuration;
            //float cubic = Mathf.Sin((ratio * Mathf.PI) * 0.5f);
            Vector2 nextPosition = Vector2.Lerp(startPos, endPos, ratio);
            if (Physics2D.CircleCast(transform.position + (Vector3)col2D.offset, ((CircleCollider2D)col2D).radius, dashDirection, Vector2.Distance(transform.position, nextPosition), TerrainLayers))
            {
                break;
            }
            if (!isSwiping)
                break;
            if (Vector2.Distance(transform.position, endPos) > 0.1f)
            {
                transform.position = nextPosition;
            }
            else
            {
                transform.position = endPos;
            }
            yield return null;
        }
        isSwiping = false;
        Swiping = null;
        col2D.excludeLayers = initial;
        swipeHitbox.enabled = false;
        swipeHitbox.includeLayers = 0;
    }
    #endregion

    #region Attack2
    protected override void Attack2()
    {
        dir = (targetTr.position - transform.position).normalized;
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        float startingAngle = degreeOffset * ((float)daggerAmount * 0.5f - 0.5f) * -1;
        for ( int i = 0; i < daggerAmount; i++)
        {
            GameManager.Instance.AudioManager.PlaySound(AudioRef.KnifeThrow);
            Vector2 newDir = CustomMath.RotateByEularAngles(dir, startingAngle + (i * degreeOffset));
            EnemyProjectile temp = daggerPool.GetPooledObj(out bool initial);
            if (initial)
            {
                temp.NewInstance();
            }
            temp.Init(newDir, daggerSpawnPoint.position, TargetLayer, Attack2Damage, Attack2Duration, daggerSpeed, AttackKnockback2, transform);
        }
    }
    #endregion

    #region Attack3
    private Coroutine multiSwipe;
    protected override void Attack3()
    {
        multiSwipe = StartCoroutine(MultiSwipes());
    }
    private int swipesDone = 0;
    private Coroutine Swiping;
    private IEnumerator MultiSwipes()
    {
        while (swipesDone < swipeAmount)
        {
            if (Swiping == null)
            {
                enemyAnimation.overrideAnim = true;
                float time = 0;
                if (swipesDone != 0)
                {
                    while (time < multiSwipeDelay)
                    {
                        dir = (targetTr.position - transform.position).normalized;
                        (enemyAnimation as CheetahAnimation).ChargingThree(dir);
                        yield return null;
                        time += Time.deltaTime;
                    }
                }
                dir = (targetTr.position - transform.position).normalized;
                (enemyAnimation as CheetahAnimation).AttackingThree(dir);
                Swiping = StartCoroutine(Swipe());
                while (Swiping != null)
                {
                    yield return null;
                }
                swipesDone++;
            }
            yield return null;
        }
        swipesDone = 0;
        enemyAnimation.overrideAnim = false;
        multiSwipe = null;
    }
    #endregion

    #region trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger || !swipeHitbox.enabled)
            return;
        PlayerSystem foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (foundTarget.GetState() == playerState.perfectDodge)
            {
                Debug.Log("perfectDodge");
                isSwiping = false;
                InterruptAttack();                
                foundTarget.Counter();
            }
            else
            {
                DoDamage(foundTarget);
            }
        }
    }
    #endregion
    public override void InterruptAttack()
    {
        if(Swiping != null)
        {
            StopCoroutine(Swiping);
            isSwiping = false;
            Swiping = null;
            col2D.excludeLayers = initial;
            swipeHitbox.enabled = false;
            swipeHitbox.includeLayers = 0;
        }
        if (multiSwipe != null)
        {
            StopCoroutine(multiSwipe);
            swipesDone = 0;
            enemyAnimation.overrideAnim = false;
            multiSwipe = null;
        }
        enemyAnimation.overrideAnim = false;
        base.InterruptAttack();
    }

    public override void PoolSelf()
    {
        hitbox.enabled = false;
        base.PoolSelf();
    }
}
