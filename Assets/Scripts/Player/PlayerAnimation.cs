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
        else if (PCM.system.GetState() == playerState.idle)
        {
            IdleMovement();
        }
        else if( PCM.system.GetState() == playerState.moving)
        {
            Moving();
            PlayFootStep();
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
            GameManager.Instance.AudioManager.PlaySound(AudioRef.FootStepGrassL);
        }
        else
        {
            left = !left;
            GameManager.Instance.AudioManager.PlaySound(AudioRef.FootStepGrassR);
        }
        yield return new WaitForSeconds(time);
        PlayFootStepCoroutine = null;
    }
    private void Moving()
    {
        lastDirection = CustomMath.GetDirection(Vector2.right, PCM.control.lastDirection, false);
        switch (lastDirection)
        {
            case 0:
                anim.Play("PlayerRunningE");
                break;
            case 1:
                anim.Play("PlayerRunningNE");
                break;
            case 2:
                anim.Play("PlayerRunningN");
                break;
            case 3:
                anim.Play("PlayerRunningNW");
                break;
            case 4:
                anim.Play("PlayerRunningW");
                break;
            case 5:
                anim.Play("PlayerRunningSW");
                break;
            case 6:
                anim.Play("PlayerRunningS");
                break;
            case 7:
                anim.Play("PlayerRunningSE");
                break;
        }
    }
    private void IdleMovement()
    {
        lastDirection = CustomMath.GetDirection(Vector2.right, PCM.control.lastDirection);

        switch (lastDirection)
        {
            case 0:
                anim.Play("IdleEast");
                break;
            case 1:
                anim.Play("IdleNorth");
                break;
            case 2:
                anim.Play("IdleWest");
                break;
            case 3:
                anim.Play("IdleSouth");
                break;
        }
        lastDirection *= 2;
    }
    int lastDirection;
    private void HitStunned()
    {
        Vector2 knockback = PCM.system.hitDir;
        int dir;
        if (knockback.magnitude > 0.1f)
            dir = CustomMath.GetDirection(Vector2.right, knockback, false);
        else
            dir = ((lastDirection + 4) % 8);
        switch (dir)
        {
            case 0:
                anim.Play("HitGoingE");
                break;
            case 1:
                anim.Play("HitGoingNE");
                break;
            case 2:
                anim.Play("HitGoingN");
                break;
            case 3:
                anim.Play("HitGoingNW");
                break;
            case 4:
                anim.Play("HitGoingW");
                break;
            case 5:
                anim.Play("HitGoingSW");
                break;
            case 6:
                anim.Play("HitGoingS");
                break;
            case 7:
                Debug.Log("the one failing");
                anim.Play("HitGoingSE");
                break;
        }
    }

}
