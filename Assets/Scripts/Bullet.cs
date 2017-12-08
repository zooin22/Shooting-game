//Decorator Pattern 사용

using System;
using UnityEngine;

public abstract class Bullet
{
    public GameObject gameObject;
    protected int damage; // 데미지
    protected float speed; // 속도
    protected float range; // 사거리
    protected Vector3 direction; // 방향
    protected Vector3 shotPos; // 시작 벡터

    public float GetSpeed()
    {
        return speed;
    }
    public float GetRange()
    {
        return range;
    }
    public void SetGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    public void SetBullet(Vector3 direction,Vector3 shotPos) // 방향, 속도 초기화
    {
        this.direction = direction.normalized;
        this.shotPos = shotPos;
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
#region override
    public abstract Bullet Clone();
    public abstract void SetProperties();
    public abstract void OnCollisionEnter2D(Collision2D coll);
    public abstract void Update();
    public abstract void Collision();
#endregion
}

class BaseBullet : Bullet // 노말 총알 - 데미지.
{
    public BaseBullet(int damage,float speed,float range) {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
    }
    #region override
    public override Bullet Clone()
    {
        return new BaseBullet(damage, speed, range);
    }
    public override void SetProperties()
    {
    }
    public override void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
    }
    public override void Update()
    {
    }
    public override void Collision()
    {
        Debug.Log("Base");
    }
    #endregion
}

abstract class BulletDecorator : Bullet
{
    Bullet bulletDecorator;
    public BulletDecorator(Bullet bulletDecorator) 
    {
        this.bulletDecorator = bulletDecorator;
        this.speed = bulletDecorator.GetSpeed();
        this.range = bulletDecorator.GetRange();
    }
    #region override
    public override Bullet Clone()
    {
        Bullet mCBullet = bulletDecorator.Clone();
        return mCBullet;
    }
    public override void OnCollisionEnter2D(Collision2D coll)
    {
        bulletDecorator.OnCollisionEnter2D(coll);
        return;
    }
    public override void SetProperties()
    {
    }
    public override void Update()
    {
        bulletDecorator.Update();
    }
    public override void Collision()
    {
        bulletDecorator.Collision();
    }
    #endregion
}

class NormalBullet : BulletDecorator // 노말 총알 - 아무 능력 없음.
{
    public NormalBullet(Bullet cBullet) : base(cBullet) { }
#region override
    public override Bullet Clone()
    {
        Bullet mCBullet = base.Clone();
        return new NormalBullet(mCBullet);
    }
   
    public override void OnCollisionEnter2D(Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        Remove(); // 충돌 시 삭제
        //Collision();
        return;
    }
    public override void Update()
    {
        if (!CheckDistance())
        {
            Remove();
        }
        gameObject.transform.position = gameObject.transform.position + direction * speed * Time.deltaTime; // direction 벡터로 speed * Time.deltaTime 만큼 이동
    }
    public override void Collision()
    {
        Debug.Log("Normal");
    }
#endregion
}

class BouncedBullet : BulletDecorator // 튕기는 총알
{
    int bounceTime = 5; // Bounce 횟수 5번

    public BouncedBullet(Bullet cBullet,int bounceTime) : base(cBullet) {
        this.bounceTime = bounceTime;
    }

    public void SetBounceTime(int _bounceTime)
    {
        bounceTime = _bounceTime;
    }
    #region override
    public override Bullet Clone()
    {
        Bullet mCBullet = base.Clone();

        return new BouncedBullet(mCBullet,bounceTime);
    }
    public override void SetProperties()
    {
    }
    public override void OnCollisionEnter2D(Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        if (bounceTime < 0)  // 5번 다 튕길 경우 삭제
        {
            Remove();
            return;
        }
        ContactPoint2D[] contact = coll.contacts;

        direction = Vector3.Reflect(direction, contact[0].normal); // 진행방향을 반사각으로 바꿈.
        gameObject.transform.rotation = BulletCal.GetRotFromVector(direction); // 반사각에 따른 진행방향 이미지 회전
        bounceTime--;
        //Collision();
    }
    public override void Update()
    {
        if (!CheckDistance())
        {
            Remove();
        }
        gameObject.transform.position = gameObject.transform.position + direction * speed * Time.deltaTime; // direction 벡터로 speed * Time.deltaTime 만큼 이동
    }
    public override void Collision()
    {
        //base.Collision();
        Debug.Log("Bounced");
    }
    #endregion
}

class LineBullet : BulletDecorator // 튕기는 총알
{
    LineRenderer line;
    float Width;
    public LineBullet(Bullet cBullet, float Width) : base(cBullet)
    {
        this.Width = Width;
    }

    #region override
    public override Bullet Clone()
    {
        Bullet mCBullet = base.Clone();

        return new LineBullet(mCBullet, Width);
    }
    public override void SetProperties()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = Width;
        line.startWidth = Width;
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
                    Collision();
                    line.SetPosition(1, hit[i].point);
                }
            }
        }
    }
    public override void Collision()
    {
        base.Collision();
        Debug.Log("Bounced");
    }
    #endregion
}
