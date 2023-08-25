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
    [SerializeField][ReadOnly]
    private int maxCombo;
    [ReadOnly][SerializeField]
    private int currentCombo;
    [SerializeField]
    private float comboEndLag;

    [Header("General varibles")]
    [SerializeField]
    private PlayerController playerControl;
    [SerializeField]
    private Transform centre;
    [SerializeField]
    private Timer timers;
    [SerializeField]
    private PolygonCollider2D[] lightHitboxes = new PolygonCollider2D[3];
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
        timers = TimerManager.instance.GenerateTimers(typeof(coolDownTimers), gameObject);
        playerControl = GetComponent<PlayerController>();
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
            currentAttackStage = attackStage.attackEnd;
            playerControl.SetIsAttackEnd(true);
            playerControl.SetIsAttacking(false);
            if (currentCombo < maxCombo)
                timers.SetTime((int)coolDownTimers.lightEndLag, lightAttackEndLag);
            else
                timers.SetTime((int)coolDownTimers.lightEndLag, comboEndLag);
        }
        else if (timers.IsTimeZero((int)coolDownTimers.lightEndLag) && currentAttackStage.Equals(attackStage.attackEnd))
        {
            currentAttackStage = attackStage.noAttack;
            currentCombo = 0;
            playerControl.SetIsAttacking(false);
            playerControl.SetIsAttackEnd(false);
        }
    }

    public void LightAttack()
    {
        actionState[] unAllowed = { actionState.dashing};
        if (playerControl.CheckStates(unAllowed))
            return;
        if (currentCombo >= maxCombo || currentAttackStage == attackStage.attackStart)
            return;
        playerControl.RemoveBufferInput();
        // initiate light attack
        // quick attack towards mouse
        playerControl.SetIsAttacking(true);
        playerControl.SetIsAttackEnd(false);
        UniqueLightAttack();
        currentCombo++;
        timers.SetTime((int)coolDownTimers.lightAttackDuration, lightAttackDuration);
        currentAttackStage = attackStage.attackStart;
    }

    private void UniqueLightAttack()
    {
        // moves user in that direction via force
        Vector2 dir = (playerControl.mousePos - (Vector2)transform.position).normalized;
        playerControl.rb.velocity = dir * lightAttackPullDist;
        centre.eulerAngles = new Vector3(0, 0, CustomMath.ClampedDirection(Vector2.up, dir));
        lightHitboxes[currentCombo].enabled = true;
        // play animation and state control in inherited classes

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void ResetTimer()
    {
        timers.ResetToZero();
    }
}
