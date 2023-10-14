using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArchedProjectile : EnemyProjectile
{
    // Start is called before the first frame update
    protected Vector2 targetPoint;
    protected Vector2 archDir;
    protected bool continueArch;
    protected float archDuration;
    private float initialV;
    private float initialTime;
    private float decay;
    private void Start()
    {

    }
    protected override void MoveProjectileByDir()
    {
        initialV -= decay * Time.deltaTime * 0.5f;
        Vector2 newDir = dir.normalized * Speed * Time.deltaTime;
        Vector2 modifiedArch = archDir.normalized * initialV * Time.deltaTime;
        newDir += modifiedArch;
        initialV -= decay * Time.deltaTime * 0.5f;
        gameObject.transform.Translate(newDir);
    }
    public override void Init(Vector2 dir, Vector3 spawnPos, LayerMask Target, int damage, float duration, float speed, float knockbackForce, Transform shooter)
    {
        this.dir = dir;
        transform.position = spawnPos;
        SpriteObj.rotation = UtilityFunction.LookAt2D(Vector3.zero, dir);
        if (col2d)
        {
            col2d.includeLayers = terrainMask + Target;
            rb.includeLayers = terrainMask + Target;
            col2d.excludeLayers = ~col2d.includeLayers;
            rb.excludeLayers = ~rb.includeLayers;
        }        
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        Duration = duration;
        Speed = speed;
        this.shooter = shooter;
        StartLifetime();
    }
    public void InitArch(Vector2 archDir ,Vector2 targetPoint, float initialVelocty,bool fixedArrival, bool continueArch = false)
    {
        initialTime = Time.time;
        this.archDir = archDir;
        this.targetPoint = targetPoint;
        this.continueArch = continueArch;
        if (fixedArrival)
        {
            archDuration = Speed * 0.5f;
            Speed = ((targetPoint - (Vector2)transform.position).magnitude / Speed);
        }
        else
            archDuration = ((targetPoint - (Vector2)transform.position).magnitude / Speed) * 0.5f;
        initialV = initialVelocty;
        decay = initialV / archDuration;
        Invoke("SpawnAcid", archDuration * 2);
    }

    public void SpawnAcid()
    {
        Speed = 0;
        initialV = 0;
        decay = 0;
        //PoolSelf();
    }
    public void Pause()
    {
        Debug.Log(initialV);    
        Debug.Break();
    }


}
