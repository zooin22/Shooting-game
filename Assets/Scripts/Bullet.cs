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
    public virtual Vector3 Getdirection()
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
    int bounces;

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
        line.startWidth = width;
        line.startWidth = width;
        bounces = bulletStrategy.Coliision();
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
        bulletStrategy.InitMemberValue();
        LaserBeam(gameObject.transform.position, direction, 0, range);
    }

    private void LaserBeam(Vector3 src, Vector3 dir, int count, float distance)
    {
        Vector2 position = src;
        Vector2 direction = dir;
        Vector2 lastPosition = position;
        int vertCount = 1;
        int bounceCount = 0;
        bool loop = true;
        line.positionCount = vertCount;
        line.SetPosition(vertCount - 1, position);

        while (loop)
        {
            if (bounceCount >= bounces)
            {
                loop = false;
            }
            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance);

            if (hit == true)
            {
                //hit
                Vector2 hitPosition = hit.point;
                Vector2 hitNormal = hit.normal;
                distance -= hit.distance;
                position = hitPosition;

                Wall wall = hit.collider.GetComponent<Wall>();
                if (wall != null)
                {
                    if (wall.reflect == false)
                    {
                        vertCount += 3;
                        line.positionCount = vertCount;
                        line.SetPosition(vertCount - 3, Vector2.MoveTowards(position, lastPosition, 0.001f));
                        line.SetPosition(vertCount - 2, position);
                        line.SetPosition(vertCount - 1, position);
                        break;
                    }
                }

                Vector2 reflection = Vector2.Reflect(direction, hitNormal);

                direction = reflection;

                bounceCount++;
                vertCount += 3;
                line.positionCount = vertCount;
                line.SetPosition(vertCount - 3, Vector2.MoveTowards(position, lastPosition, 0.001f));
                line.SetPosition(vertCount - 2, position);
                line.SetPosition(vertCount - 1, position);

                lastPosition = position;

                position = position + direction * 0.001f;
            }
            else
            {
                // No hit
                vertCount++;
                line.positionCount = vertCount;
                line.SetPosition(vertCount - 1, position + (direction * distance));
                loop = false;
            }
        }
         
        #endregion
    }
}