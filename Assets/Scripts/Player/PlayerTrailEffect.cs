using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTrailEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject trailObj;
    [SerializeField]
    private SpriteRenderer rend;
    [SerializeField]
    private Transform sprite;

    private Stack<TrailObj> trails = new Stack<TrailObj>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Countered(0.3f, true);
        }
    }
    public void PerfectDodge(float duration, bool removeSelf = false)
    {
        for (int i = 0; i < 5; i++)
        {
            TrailObj objOne = GetObj();
            SetSprite(objOne);
            objOne.PerfectDodge(duration, removeSelf);
        }
    }

    public void Countered(float duration, bool removeSelf = false)
    {
        Vector2[] dirs =
           {new Vector2(1, 1) ,
            new Vector2(-1, 1) ,
            new Vector2(-1, -1) ,
            new Vector2(1, -1) };
        for (int i = 0; i < 4; i++)
        {
            TrailObj obj = GetObj();
            SetSprite(obj);
            obj.CounterEffect(dirs[i].normalized * 1.4f, duration, removeSelf);
        }
    }

    public void DashAfterImage(float dashDuration, int afterImageAmount)
    {
        StartCoroutine(SpawnAfterImage(dashDuration, afterImageAmount));
    }

    private IEnumerator SpawnAfterImage(float dashDuration, int imageAmount)
    {
        float time = 0;
        float interval = dashDuration / imageAmount;
        int imageSpawned = 0;
        while (time < dashDuration)
        {
            if (time >= interval * imageSpawned && imageSpawned < imageAmount)
            {
                TrailObj temp = GetObj();
                SetSprite(temp);
                temp.SetAlpha(0f, 0.6f, true);
                imageSpawned++;
            }

            time += Time.deltaTime;
            yield return null;
        }
    }
    private TrailObj GetObj()
    {
        if (trails.Count > 0)
        {
            trails.Peek().gameObject.SetActive(true);
            return trails.Pop();
        }
        else
        {
            TrailObj temp = Instantiate(trailObj, transform.position, Quaternion.identity).GetComponent<TrailObj>();
            temp.SetOwner(this);
            return temp;
        }
    }

    private void SetSprite(TrailObj obj, float alpha = 1)
    {
        obj.transform.position = transform.position;
        obj.SetAlpha(alpha);
        obj.SetSprite(rend.sprite);
        obj.SetXScale(sprite.localScale);
    }

    public void AddTrailObj(TrailObj obj)
    {
        trails.Push(obj);
    }
}
