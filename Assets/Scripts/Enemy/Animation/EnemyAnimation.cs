using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected SpriteRenderer rend;
    [SerializeField]
    protected SpriteRenderer comboRend;
    [SerializeField]
    protected Animator comboAnim;
    [SerializeField]
    protected Enemy enemy;
    [SerializeField, SerializedDictionary("Animation,String")]
    public int lastDir { get; private set; } = 0;
    public bool overrideAnim = false;
    // Start is called before the first frame update
    // Update is called once per frame
    protected virtual void Update()
    {
        DecideAnimation();
        DetermineLayer();

    }

    private void DetermineLayer()
    {
        if (GameManager.Instance.PlayerTransform)
        {
            if (GameManager.Instance.PlayerTransform.position.y > transform.position.y)
            {
                rend.sortingOrder = 30;
                comboRend.sortingOrder = 31;
            }
            else
            {
                rend.sortingOrder = 10;
                comboRend.sortingOrder = 11;
            }
                
        }
    }

    protected virtual void DecideAnimation()
    {
        if (overrideAnim)
            return;
        Vector2 dir = enemy.NextPathPoint();
        if (enemy.InAttack)
        {
            AttackAnimation(lastDir);
        }
        else if (enemy.WindingUp)
        {
            lastDir = CustomMath.GetDirection(Vector2.right, enemy.AimAtPlayer());
            WindUpAnimation(lastDir);
        }
        else if (dir.magnitude > 0.1f)
        {
            WalkAnimation(CustomMath.GetDirection(Vector2.right, dir));
        }
        else if (dir.magnitude <= 0.1f)
        {
            IdleAnimation(lastDir);
        }
    }
    private void WalkAnimation(float direction)
    {
        anim.SetFloat("Walk", direction / 4f);
        anim.Play("Walk");
        lastDir = (int)direction;
    }

    private void IdleAnimation(float direction)
    {
        anim.SetFloat("Idle", direction / 4f);
        anim.Play("Idle");
    }

    public void PlayComboAnimation(Combos comboToPlay)
    {
        comboAnim.Play(comboToPlay.ToString());
    }

    public void StopShockAnim()
    {
        comboAnim.Play("Nothing");
    }

    private void WindUpAnimation(float direction)
    {
        anim.SetFloat("Charge", direction / 4f);
        switch (enemy.CurrentAttack)
        {
            case 1:
                anim.Play("ChargingOne");
                break;
            case 2:
                anim.Play("ChargingTwo");
                break;
            case 3:
                anim.Play("ChargingThree");
                break;
        }
    }

    private void AttackAnimation(float direction)
    {
        anim.SetFloat("Attack", direction / 4f);
        switch (enemy.CurrentAttack)
        {
            case 1:
                anim.Play("AttackingOne");
                break;
            case 2:
                anim.Play("AttackingTwo");
                break;
            case 3:
                anim.Play("AttackingThree");
                break;
        }
    }
    public void ReturnToIdle()
    {
        anim.SetFloat("Idle", lastDir / 4f);
        anim.Play("Idle");
    }
}
