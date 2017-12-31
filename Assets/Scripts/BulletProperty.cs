using UnityEngine;

[System.Serializable]
public abstract class BulletProperty
{
    public GameObject bulletObj;
    public Bullet bullet;
    protected WeaponState.Owner owner;

    public virtual void Init(GameObject bulletObj, Bullet bullet) { this.bulletObj = bulletObj; this.bullet = bullet; this.owner = bullet.GetOwner(); }
}

public abstract class UpdateProperty : BulletProperty
{
    public abstract void Init();
    public abstract UpdateProperty Clone();
    public abstract void Update();
}

class BaseNormalUpdateProperty : UpdateProperty
{
    private float speed;

    public override void Init()
    {
        base.Init(bulletObj,bullet);
        this.speed = bullet.GetSpeed();
    }
    public override UpdateProperty Clone()
    {
        return new BaseNormalUpdateProperty();
    }
    public override void Update()
    {
        bulletObj.transform.Translate(Vector3.down * Time.deltaTime * speed); // direction 벡터로 speed * Time.deltaTime 만큼 이동
    }
}

class HomingProperty : UpdateProperty
{
    ObjectPool objectPool;
    private float rocketTurnSpeed;
    private float timerSinceLaunch;
    public HomingProperty()
    {
        rocketTurnSpeed = 50.0f;
        timerSinceLaunch = 0;
    }
    public override void Init()
    {
        base.Init(bulletObj, bullet);
        owner = bullet.GetOwner();
        if (WeaponState.Owner.PLAYER == owner)
        {
            objectPool = PoolGroup.instance.GetObjectPool(Pool.ENEMY);
        }
        else
        {
            objectPool = null;
        }
    }
    public override UpdateProperty Clone()
    {
        return new HomingProperty();
    }
    public override void Update()
    {
        timerSinceLaunch += Time.deltaTime;
        Vector3 directionTarget;
        if (WeaponState.Owner.PLAYER == owner)
        {
            directionTarget = objectPool.GetNeareastObject(bulletObj.transform.position);
        }
        else
        {
            directionTarget = EnemyManager.instance.target.position - bulletObj.transform.position;
        }
        if (!directionTarget.Equals(Vector3.zero))
        {
            if (timerSinceLaunch > 1)
            {
                if (directionTarget.sqrMagnitude > 50)
                {
                    rocketTurnSpeed = 90.0f;
                }
                else
                {
                    //if close to target
                    if (directionTarget.sqrMagnitude < 10)
                    {
                        rocketTurnSpeed = 360.0f;
                    }
                }
            }
            directionTarget.Normalize();
            bulletObj.transform.rotation = Quaternion.RotateTowards(bulletObj.transform.rotation, BulletCal.GetRotFromVector(directionTarget), Time.deltaTime * rocketTurnSpeed * timerSinceLaunch);
        }
    }
}

public abstract class CollisionProperty : BulletProperty
{
    public abstract CollisionProperty Clone();
    public abstract void Collision(ref Collision2D coll);
}

class BaseNormalCollisionProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new BaseNormalCollisionProperty();
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Item") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        switch (owner)
        {
            default:
            case WeaponState.Owner.PLAYER:
                if (coll.transform.CompareTag("Player")) // 플레이어에는 충돌하지 않음
                    return;
                if (coll.transform.GetComponent<Enemy>())
                {
                    coll.transform.GetComponent<Enemy>().Attacked(bullet.GetDamage());
                }
                break;
            case WeaponState.Owner.ENEMY:
                if (coll.transform.CompareTag("Enemy")) // 플레이어에는 충돌하지 않음
                    return;
                break;
        }
        bullet.Remove();
    }
}

class BasePierceProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new BasePierceProperty();
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        if (coll.transform.CompareTag("Wall"))
        {
            bullet.Remove();
            return;
        }
        switch (owner)
        {
            default:
            case WeaponState.Owner.PLAYER:
                if (coll.transform.CompareTag("Player")) // 플레이어에는 충돌하지 않음
                    return;
                if (coll.transform.GetComponent<Enemy>())
                {
                    coll.transform.GetComponent<Enemy>().Attacked(bullet.GetDamage());
                }
                break;
            case WeaponState.Owner.ENEMY:
                if (coll.transform.CompareTag("Enemy")) // 플레이어에는 충돌하지 않음
                    return;
                break;
        }        
    }
}

class BaseBouncyProperty : CollisionProperty
{
    int bounceCount;
    int bounces;
    public BaseBouncyProperty()
    {
        bounces = 2;
        bounceCount = 0;
    }

    public override CollisionProperty Clone()
    {
        return new BaseBouncyProperty();       
    }
    public override void Collision(ref Collision2D coll)
    {
        // If a missile hits this object
        if (coll.transform.CompareTag("Item") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        switch (owner)
        {
            default:
            case WeaponState.Owner.PLAYER:
                if (coll.transform.CompareTag("Player")) // 플레이어에는 충돌하지 않음
                    return;
                if (coll.transform.GetComponent<Enemy>())
                {
                    coll.transform.GetComponent<Enemy>().Attacked(bullet.GetDamage());
                }
                break;
            case WeaponState.Owner.ENEMY:
                if (coll.transform.CompareTag("Enemy")) // 플레이어에는 충돌하지 않음
                    return;
                break;
        }
        if (bounces <= bounceCount)  // 5번 다 튕길 경우 삭제
        {
            bullet.Remove();
            return;
        }
        bounceCount++;
        ContactPoint2D[] contact = coll.contacts;
        if (contact.Length > 0)
        {
            bullet.SetDirection(Vector3.Reflect(bullet.Getdirection(), contact[0].normal)); // 진행방향을 반사각으로 바꿈.
            bulletObj.transform.rotation = BulletCal.GetRotFromVector(bullet.Getdirection()); // 반사각에 따른 진행방향 이미지 회전
        }
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
        if (coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        switch (owner)
        {
            default:
            case WeaponState.Owner.PLAYER:
                if (coll.transform.CompareTag("Player")) // 플레이어에는 충돌하지 않음
                    return;
                break;
            case WeaponState.Owner.ENEMY:
                if (coll.transform.CompareTag("Enemy")) // 플레이어에는 충돌하지 않음
                    return;
                break;
        }
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
        if (coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        switch (owner)
        {
            default:
            case WeaponState.Owner.PLAYER:
                if (coll.transform.CompareTag("Player")) // 플레이어에는 충돌하지 않음
                    return;
                break;
            case WeaponState.Owner.ENEMY:
                if (coll.transform.CompareTag("Enemy")) // 플레이어에는 충돌하지 않음
                    return;
                break;
        }
        Debug.Log(coll.gameObject.name + " Burn");
    }
}

