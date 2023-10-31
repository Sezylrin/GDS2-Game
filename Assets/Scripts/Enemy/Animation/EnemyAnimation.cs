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
        Vector2 dir = enemy.NextPathPoint();
        if (enemy.InAttack)
        {

        }
        else if (enemy.WindingUp)
        {
            lastDir = CustomMath.GetDirection(Vector2.right, enemy.AimAtPlayer());
            IdleAnimation(lastDir);
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
    private void WalkAnimation(int direction)
    {
        anim.SetFloat("Walk", (float)direction / 4f);
        anim.Play("Walk");
        lastDir = direction;
    }

    private void IdleAnimation(int direction)
    {
        anim.SetFloat("Idle", (float)direction / 4f);
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
}
