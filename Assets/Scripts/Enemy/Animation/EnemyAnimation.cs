using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class EnemyAnimation : MonoBehaviour
{
    public enum AnimationMovements
    {
        idleR,
        idleL,
        idleU,
        idleD,
        walkR,
        walkL,
        walkU,
        walkD
    }
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected SpriteRenderer rend;
    [SerializeField]
    protected Enemy enemy;
    [SerializeField, SerializedDictionary("Animation,String")]
    protected SerializedDictionary<AnimationMovements, string> clips;
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
                rend.sortingOrder = 30;
            else
                rend.sortingOrder = 10;
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
        switch (direction)
        {
            case 0:
                anim.Play(clips[AnimationMovements.walkR]);
                break;
            case 1:
                anim.Play(clips[AnimationMovements.walkU]);
                break;
            case 2:
                anim.Play(clips[AnimationMovements.walkL]);
                break;
            case 3:
                anim.Play(clips[AnimationMovements.walkD]);
                break;
        }
        lastDir = direction;
    }

    private void IdleAnimation(int direction)
    {
        switch (direction)
        {
            case 0:
                anim.Play(clips[AnimationMovements.idleR]);
                break;
            case 1:
                anim.Play(clips[AnimationMovements.idleU]);
                break;
            case 2:
                anim.Play(clips[AnimationMovements.idleL]);
                break;
            case 3:
                anim.Play(clips[AnimationMovements.idleD]);
                break;
        }
    }
}
