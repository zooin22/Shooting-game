using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class BulletStrategy
{
    protected GameObject gameObject;
    protected Bullet bullet;

    public void SetGameObject(GameObject gameObject,Bullet bullet)
    {
        this.gameObject = gameObject;
        this.bullet = bullet;
    }

    public void Remove() // 게임 풀 오브젝트 false
    {
        ObjectPool.current.FreeObject(gameObject);
    }

    public abstract BulletStrategy Clone();
    public virtual void InitMemberValue() { }
    public abstract void Coliision(ref Collision2D coll);
    public virtual int Coliision() { return 1; }
}

class NormalBulletStrategy : BulletStrategy // 노말 총알 - 아무 능력 없음.
{
    #region override
    public override BulletStrategy Clone()
    {
        BulletStrategy bulletStrategy = new NormalBulletStrategy();
        return bulletStrategy;
    }
    public override void Coliision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        Remove(); // 충돌 시 삭제
        return;
    }
    #endregion
}

class BouncedBulletStrategy : BulletStrategy// 튕기는 총알
{
    int bounceTime = 5; // Bounce 횟수 5번
    int bounced;
    public BouncedBulletStrategy(int bounceTime)
    {
        this.bounceTime = bounceTime;
        this.bounced = 0;
    }
    public override void InitMemberValue()
    {
        bounced = 0;
    }
    public override BulletStrategy Clone()
    {
        BulletStrategy bulletStrategy = new BouncedBulletStrategy(bounceTime);
        return bulletStrategy;
    }
    public override void Coliision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        if (bounceTime <= bounced)  // 5번 다 튕길 경우 삭제
        {
            Remove();
            return;
        }
        bounced++;
        ContactPoint2D[] contact = coll.contacts;

        bullet.SetDirection(Vector3.Reflect(bullet.Getdirection(), contact[0].normal)); // 진행방향을 반사각으로 바꿈.
        gameObject.transform.rotation = BulletCal.GetRotFromVector(bullet.Getdirection()); // 반사각에 따른 진행방향 이미지 회전
    }
    public override int Coliision()
    {         // If a missile hits this object
        return bounceTime;
    }
}
