using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
public class Attacks : MonoBehaviour
{
    [Header("Light Attack Stats")]
    [SerializeField]
    private float lightAttackDuration;
    [SerializeField]
    private float lightAttackEndLag;
    [SerializeField]
    private float lightAttackPullDist;
    [SerializeField]
    private int[] lightAttackDamage = new int[3];
    [SerializeField]
    private int[] lightAttackStagger = new int[3];
    [SerializeField]
    private float[] lightAttackKnockBack = new float[3];
    [SerializeField][ReadOnly]
    private int maxCombo;
    [ReadOnly][SerializeField]
    private int currentCombo;
    [SerializeField]
    private float comboEndLag;

    [Header("General varibles")]
    [SerializeField]
    private PlayerComponentManager PCM; 
    [SerializeField]
    private Transform centre;
    [SerializeField]
    private Timer timers;
    [SerializeField]
    private PolygonCollider2D[] lightHitboxes = new PolygonCollider2D[3];
    /// <summary>
    /// the amount of time to reduce the ability point regen delay
    /// </summary>
    [SerializeField]
    private float reduceDelay;
    
    [SerializeField]
    private SpriteRenderer[] hitboxSprite = new SpriteRenderer[3];

    private attackStage currentAttackStage;
    private enum coolDownTimers : int
    {
        lightAttackDuration,
        lightEndLag
    }
    private enum attackStage
    {
        noAttack,
        attackStart,
        attackEnd
    }
    // Start is called before the first frame update
    void Start()
    {
        timers = GameManager.Instance.TimerManager.GenerateTimers(typeof(coolDownTimers), gameObject);
        maxCombo = lightHitboxes.Length;
    }

    // Update is called once per frame
    void Update()
    {
        AttackLag();
    }

    private void AttackLag()
    {
        if (timers.IsTimeZero((int)coolDownTimers.lightAttackDuration) && currentAttackStage.Equals(attackStage.attackStart))
        {
            lightHitboxes[currentCombo - 1].enabled = false;
            hitboxSprite[currentCombo - 1].enabled = false;
            currentAttackStage = attackStage.attackEnd;
            PCM.control.SetIsAttackEnd(true);
            PCM.control.SetIsAttacking(false);
            if (currentCombo < maxCombo)
                timers.SetTime((int)coolDownTimers.lightEndLag, lightAttackEndLag);
            else
                timers.SetTime((int)coolDownTimers.lightEndLag, comboEndLag);
        }
        else if (timers.IsTimeZero((int)coolDownTimers.lightEndLag) && currentAttackStage.Equals(attackStage.attackEnd))
        {
            currentAttackStage = attackStage.noAttack;
            currentCombo = 0;
            PCM.control.SetIsAttacking(false);
            PCM.control.SetIsAttackEnd(false);
        }
    }

    public void LightAttack()
    {
        playerState[] unAllowed = { playerState.dashing, playerState.abilityCast, playerState.hit, playerState.perfectDodge};
        if (PCM.control.CheckStates(unAllowed) || PCM.system.isCountered)
            return;
        if (currentCombo >= maxCombo || currentAttackStage == attackStage.attackStart)
            return;
        PCM.control.RemoveBufferInput();
        // initiate light attack
        // quick attack towards mouse
        PCM.control.SetIsAttacking(true);
        PCM.control.SetIsAttackEnd(false);
        UniqueLightAttack();
        currentCombo++;
        timers.SetTime((int)coolDownTimers.lightAttackDuration, lightAttackDuration);
        currentAttackStage = attackStage.attackStart;
    }

    private void UniqueLightAttack()
    {
        // moves user in that direction via force
        Vector2 dir = (PCM.control.mousePos - (Vector2)transform.position).normalized;
        PCM.control.rb.velocity = dir * lightAttackPullDist;
        centre.eulerAngles = new Vector3(0, 0, CustomMath.ClampedDirection(Vector2.up, dir, false));
        lightHitboxes[currentCombo].enabled = true;
        hitboxSprite[currentCombo].enabled = true;
        // play animation and state control in inherited classes

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        IDamageable foundEnemy;
        if (UtilityFunction.FindComponent(collision.transform, out foundEnemy))
        {
            if (!(foundEnemy as Enemy))
            {
                return;
            }
            foundEnemy.TakeDamage(lightAttackDamage[currentCombo - 1] + GameManager.Instance.StatsManager.attackDamageModifier, lightAttackStagger[currentCombo - 1],ElementType.noElement);
            foundEnemy.AddForce((collision.transform.position - transform.position).normalized * lightAttackKnockBack[currentCombo - 1]);

            //enable quicker point regen rate
        }
    }

    public void ResetTimer()
    {
        timers.ResetToZero();
    }
}
