using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrailObj : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerTrailEffect owner;

    public SpriteRenderer render;

    private MaterialPropertyBlock block;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetAlpha(float finalAlpha, float duration, bool removeSelf = false)
    {
        render.DOFade(finalAlpha, duration).OnComplete(() =>
         {
             if (removeSelf)
                 OnComplete();
         });
    }

    public void PerfectDodge(float duration, bool removeSelf = false)
    {
        SetAlpha(0.75f);
        SetAlpha(0.25f, duration);
        transform.DOShakePosition(duration, 0.6f, 12, 90,false,false,ShakeRandomnessMode.Harmonic).OnComplete(() => 
        {
            if (removeSelf)
                OnComplete();
        });
    }

    public void CounterEffect(Vector2 point, float duration, bool removeSelf = false)
    {
        float dur = duration * 0.5f;
        SetAlpha(0.75f);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOMove(point + (Vector2)transform.position, dur).SetEase(Ease.InSine));
        seq.Append(render.DOFade(0, dur).SetEase(Ease.OutCubic));
        seq.OnComplete(() =>
        {
            if (removeSelf)
                OnComplete();
        });
        
        //SetAlpha(0, duration);
    }

    public void SetSprite(Sprite sprite)
    {
        render.sprite = sprite;
    }

    public void SetColourAndBlend(float blend, Color color)
    {
        block.SetTexture("_MainTex", render.sprite.texture);
        block.SetColor("_FlashColour", color);
        block.SetFloat("_FlashAmount", blend);
        render.SetPropertyBlock(block);
    }

    public void SetXScale(Vector3 xScale)
    {
        transform.localScale = xScale;
    }

    private void OnComplete()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            owner.AddTrailObj(this);
        }
    }

    public void SetAlpha(float a)
    {
        Color temp = render.color;
        temp.a = a;
        render.color = temp;
    }

    public void SetOwner(PlayerTrailEffect owner)
    {
        this.owner = owner;
        block = new MaterialPropertyBlock();
    }
}
