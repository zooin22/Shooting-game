using UnityEngine;

public abstract class BulletProperty
{
    protected GameObject bulletObj;
    protected Bullet bullet;

    public void Init(GameObject bulletObj, Bullet bullet) { this.bulletObj = bulletObj; this.bullet = bullet; }
    public abstract CollisionProperty Clone();
}

public abstract class UpdateProperty : BulletProperty
{
    public abstract void Update();
}

public abstract class CollisionProperty : BulletProperty
{
    public abstract void Collision(ref Collision2D coll);
}

class NormalProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new NormalProperty();
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        bullet.Remove();
    }
}

class BouncyProperty : CollisionProperty
{
    int bounceCount;
    int bounces;
    public BouncyProperty()
    {
        bounces = 2;
        bounceCount = 0;
    }

    public override CollisionProperty Clone()
    {
        return new BouncyProperty();       
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        if (bounces <= bounceCount)  // 5번 다 튕길 경우 삭제
        {
            bullet.Remove();
            return;
        }
        bounceCount++;
        ContactPoint2D[] contact = coll.contacts;

        bullet.SetDirection(Vector3.Reflect(bullet.Getdirection(), contact[0].normal)); // 진행방향을 반사각으로 바꿈.
        bulletObj.transform.rotation = BulletCal.GetRotFromVector(bullet.Getdirection()); // 반사각에 따른 진행방향 이미지 회전
    }
}

class PierceProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new PierceProperty();
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
    }
}

class FreezeProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new FreezeProperty();
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        Debug.Log(coll.gameObject.name + " Freeze");
    }
}

class BurnProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new BurnProperty();
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        Debug.Log(coll.gameObject.name + " Burn");
    }
}

