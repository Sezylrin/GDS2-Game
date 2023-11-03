using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private PlayerComponentManager PCM;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float footstepSoundSpeed;
    private bool left = false;
    void Start()
    {
        
    }
    private Coroutine PlayFootStepCoroutine;
    // Update is called once per frame
    void Update()
    {
        if (PCM.system.GetState() == playerState.hit)
        {
            HitStunned();
        }
        else if (PCM.system.GetState() == playerState.abilityStart)
        {
            WindUp();
        }
        else if (PCM.system.GetState() == playerState.abilityCast)
        {
            Attack();
        }
        else if (PCM.system.GetState() == playerState.dashing)
        {
            lastDirection = CustomMath.GetDirection(Vector2.right, PCM.control.direction, false);
            Dash(lastDirection);
        }
        else if (PCM.system.GetState() == playerState.idle)
        {
            IdleMovement();
        }
        else if( PCM.system.GetState() == playerState.moving)
        {
            Moving();
            PlayFootStep();
        }
        else
        {
            IdleMovement();
        }
    }
    private void PlayFootStep()
    {
        if (PlayFootStepCoroutine == null)
            PlayFootStepCoroutine = StartCoroutine(PlayFootStep(footstepSoundSpeed));
    }

    private IEnumerator PlayFootStep(float time)
    {
        if (left)
        {
            left = !left;
            GameManager.Instance.AudioManager.PlaySound(AudioRef.FootStepGrassL, false, 0.7f);
        }
        else
        {
            left = !left;
            GameManager.Instance.AudioManager.PlaySound(AudioRef.FootStepGrassR, false, 0.7f);
        }
        yield return new WaitForSeconds(time);
        PlayFootStepCoroutine = null;
    }
    private void Moving()
    {
        lastDirection = CustomMath.GetDirection(Vector2.right, PCM.control.lastDirection, false);
        anim.Play("Run");
        anim.SetFloat("Run", (float)lastDirection / 8f);
    }
    private void IdleMovement()
    {
        lastDirection = CustomMath.GetDirection(Vector2.right, PCM.control.lastDirection, false);
        anim.Play("Idle");
        anim.SetFloat("Idle", (float)lastDirection / 8f);
    }
    float lastDirection;
    private void HitStunned()
    {
        Vector2 knockback = PCM.system.hitDir;
        float dir;
        if (knockback.magnitude > 0.1f)
            dir = CustomMath.GetDirection(Vector2.right, knockback, false);
        else
            dir = ((lastDirection + 4) % 8);
        anim.SetFloat("Hit", dir / 8f);
        anim.Play("Hit");
    }

    private void WindUp()
    {
        lastDirection = CustomMath.GetDirection(Vector2.right, PCM.abilities.castDir, false);
        anim.SetFloat("Charge", lastDirection / 8f);
        anim.Play("Casting");
    }

    private void Attack()
    {
        lastDirection = CustomMath.GetDirection(Vector2.right, PCM.abilities.castDir, false);
        switch (PCM.abilities.castType)
        {
            case AbilityType.dash:
                Dash(lastDirection);
                break;
            case AbilityType.AOE:
                anim.SetFloat("Punching", lastDirection / 8f);
                anim.Play("PunchingDown");
                break;
            default:
                anim.SetFloat("Punching", lastDirection / 8f);
                anim.Play("Punching");
                break;
        }
    }

    private void AOEAttack()
    {
        
    }

    private void BlastAndProjAttack()
    {

    }
    private void Dash(float dashDir)
    {
        anim.Play("Run");
        anim.SetFloat("Run", dashDir / 8f);
    }

}
