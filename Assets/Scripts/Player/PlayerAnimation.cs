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
    private Transform Sprite;
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
        if (PCM.system.GetState() == playerState.idle)
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
        int direction = (int)(CustomMath.ClampedDirection(Vector2.right, PCM.control.lastDirection, false) / 45);
        if (Sprite.localScale.x == 1 && (direction == 3 || direction == 4|| direction == 5))
        {
            Sprite.localScale = new Vector3(-1f, 1, 1);
        }
        else if (Sprite.localScale.x == -1f && !(direction == 3 || direction == 4 || direction == 5))
        {
            Sprite.localScale = new Vector3(1f, 1, 1);
        }
        switch (direction)
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
        int direction = (int)(CustomMath.ClampedDirection(Vector2.right, PCM.control.lastDirection,true) / 90);
        if (Sprite.localScale.x == 1 && (direction == 2))
        {
            Sprite.localScale = new Vector3(-1f, 1, 1);
        }
        else if (Sprite.localScale.x == -1f && !(direction == 2))
        {
            Sprite.localScale = new Vector3(1f, 1, 1);
        }
        switch (direction)
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
    }

}
