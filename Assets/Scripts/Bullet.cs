//Decorator Pattern 사용

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet
{
    public GameObject gameObject;
    protected BulletStrategy bulletStrategy;
    protected int damage; // 데미지
    protected float speed; // 속도
    protected float range; // 사거리
    protected Vector3 direction; // 방향
    protected Vector3 shotPos; // 시작 벡터
    protected List<Bullet> properties;

    #region getter
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public float GetRange()
    {
        return range;
    }
    public Vector3 Getdirection()
    {
        return direction;
    }
    #endregion
    #region setter
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
    public void SetBulletStrategy(BulletStrategy bulletStrategy)
    {
        this.bulletStrategy = bulletStrategy;
    }
    public void SetGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        bulletStrategy.SetGameObject(gameObject,this);
    }
    public void SetBullet(Vector3 direction,Vector3 shotPos) // 방향, 속도 초기화
    {
        this.direction = direction.normalized;
        this.shotPos = shotPos;
    }
    #endregion
    public Bullet(int damage, float speed, float range)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
    }
    protected bool CheckDistance()
    {
        if (Vector3.Distance(gameObject.transform.position, shotPos) > range)
            return false;
        return true;
    }
    public void Remove() // 게임 풀 오브젝트 false
    {
        ObjectPool.current.FreeObject(this.gameObject);
    }
    public virtual void Update()
    {
        if (!CheckDistance())
            Remove();
        gameObject.transform.position = gameObject.transform.position + direction * speed * Time.deltaTime; // direction 벡터로 speed * Time.deltaTime 만큼 이동
    }
    #region override
    public abstract Bullet Clone();
    public abstract void SetProperties();
    public abstract void OnCollisionEnter2D(Collision2D coll);
    #endregion
}

class BaseBullet : Bullet // 노말 총알 - 데미지.
{
    public BaseBullet(int damage, float speed, float range) : base(damage, speed, range) { }
    #region override
    public override Bullet Clone()
    {
        Bullet clone = new BaseBullet(damage, speed, range);
        clone.SetBulletStrategy(bulletStrategy.Clone());
        return clone;
    }
    public override void SetProperties()
    {
    }
    public override void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        bulletStrategy.Coliision(ref coll);
    }
    #endregion
}

class LineBullet : Bullet // 레이저
{
    LineRenderer line;
    float width;
    public LineBullet(int damage, float speed, float range, float width) : base(damage, speed, range)
    {
        this.width = width;
    }
    #region override
    public override Bullet Clone()
    {
        Bullet clone = new LineBullet(damage, speed, range, width);
        clone.SetBulletStrategy(bulletStrategy);
        return clone;
    }
    public override void SetProperties()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = width;
        line.startWidth = width;
    }
    public override void OnCollisionEnter2D(Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        // 미사일 히트!
    }
    public override void Update()
    {
        if (!CheckDistance())
        {
            return;
        }
        RaycastHit2D[] hit;
        Ray2D ray = new Ray2D(gameObject.transform.position, direction);
        line.SetPosition(0, gameObject.transform.position);

        hit = Physics2D.RaycastAll(ray.origin,direction,range);
        if(hit.Length == 0)
        {
            line.SetPosition(1, ray.GetPoint(range));
        }
        else
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider)
                {
                    line.SetPosition(1, hit[i].point);
                }
            }
        }
    }
    #endregion
}