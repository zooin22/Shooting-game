using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet
{
    protected GameObject gameObject; // Bullet 게임 오브젝트
    protected Sprite sprite;
    protected CollisionProperty baseCollision; // 총알 기본 속성 Normal, Bounce, Pierce
    protected UpdateProperty baseUpdate; // 기본 Update
    //protected CollisionProperty[] collisionPropertis; // 기본 속성
    protected List<CollisionProperty> collisionPropertis;
    protected int collisionLength;
    protected List<UpdateProperty> updatePropertis;
    protected int updateLength;
    protected List<CollisionProperty> addedCollisionPropertis; // 추가 속성
    protected int addedCollisionLength;
    protected List<UpdateProperty> addedUpdatePropertis; // 추가 속성
    protected int addedUpdateLength;

    protected WeaponState.Owner owner; // 소유자 
    protected int damage; // 데미지
    protected float speed; // 속도
    protected float range; // 사거리
    protected Vector3 direction; // 방향
    protected Vector3 shotPos; // 시작 벡터
    protected float distance = 0; // 움직인 거리 Time.deltaTime * speed
   
    public Bullet()
    {
        baseCollision = new BaseNormalCollisionProperty();
        baseUpdate = new BaseNormalUpdateProperty();

        updatePropertis = new List<UpdateProperty>();
        collisionPropertis = new List<CollisionProperty>();
    }
    public void Init(WeaponState.Owner owner, int damage, float speed, float range , Sprite bulletSprite)
    {
        this.owner = owner;
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.sprite = bulletSprite;
    }
    protected bool CheckDistance()
    {
        if (distance > range)
            return false;
        return true;
    }
    public WeaponState.Owner GetOwner() { return owner; }
    public void Remove() // 게임 풀 오브젝트 false
    {
        PoolGroup.instance.GetObjectPool(Pool.BULLET).FreeObject(this.gameObject);
    }
    public void Update()
    {
        distance += speed * Time.deltaTime;
        if (!CheckDistance())
            Remove();
        baseUpdate.Update();
        for (int i = 0; i < updateLength; i++)
        {
            updatePropertis[i].Update();
        }
        //gameObject.transform.Translate(Vector3.down * Time.deltaTime * speed); // direction 벡터로 speed * Time.deltaTime 만큼 이동
        //gameObject.transform.position = gameObject.transform.position + direction * speed * Time.deltaTime; // direction 벡터로 speed * Time.deltaTime 만큼 이동
    }
    #region getter
    public Sprite GetSprite() { return sprite; }
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
    public void SetChargeDamage(int i) // 데미지 증가
    {
        this.damage *= (i + 1);
    }
    public void SetDirection(Vector3 direction) // 방향 선택
    {
        this.direction = direction;
    }
    public void SetGameObject(GameObject gameObject) // 게임 오브젝트 셋팅
    {
        this.gameObject = gameObject;
        PropertyInit(baseCollision);
        PropertyInit(baseUpdate);

        if (collisionPropertis != null)
            for (int i = 0; i < collisionPropertis.Count; i++)
                PropertyInit(collisionPropertis[i]);
        if (updatePropertis != null)
            for (int i = 0; i < updatePropertis.Count; i++)
                PropertyInit(updatePropertis[i]);

        if (WeaponState.Owner.PLAYER == owner)
            gameObject.layer = 9;
        else
            gameObject.layer = 10;
    }
    public void SetBullet(Vector3 direction, Vector3 shotPos) // 방향, 속도 초기화
    {
        this.direction = direction.normalized;
        this.shotPos = shotPos;
    }
    #endregion
    #region properties
    public void BaseProperty(params BulletProperty[] bulletProperty) // 총알 기본 속성, Fire, Freeze
    {
        if (bulletProperty == null)
            return;
        for(int i=0;i< bulletProperty.Length; i++)
        {
            UpdateProperty mUpdateProperty = bulletProperty[i] as UpdateProperty;
            CollisionProperty mCollisionProperty = bulletProperty[i] as CollisionProperty;

            if(mCollisionProperty != null)
            {
                collisionPropertis.Add(mCollisionProperty);
            }
            else
            {
                updatePropertis.Add(mUpdateProperty);
            }
        }
        collisionLength = collisionPropertis.Count;
        updateLength = updatePropertis.Count;
    }
    //Update
    private void PropertyInit(UpdateProperty bulletProperty)
    {
        bulletProperty.Init(gameObject, this);
        bulletProperty.Init();
    }
    public void Property(UpdateProperty updateProperty) // 총알 기본 충돌 속성, Normal Bounce, Pierce
    {
        this.baseUpdate = updateProperty.Clone();
    }
    public void BaseProperty(List<UpdateProperty> bulletProperty) // 총알 기본 속성, Fire, Freeze
    {
        if (bulletProperty == null)
            return;
        updateLength = bulletProperty.Count;
        for (int i = 0; i < updateLength; i++)
        {
            updatePropertis.Add(bulletProperty[i].Clone());
        }
    }

    //Collision
    private void PropertyInit(CollisionProperty bulletProperty)
    {
        bulletProperty.Init(gameObject, this);
    }
    public void Property(CollisionProperty collsionProperty) // 총알 기본 충돌 속성, Normal Bounce, Pierce
    {
        this.baseCollision = collsionProperty.Clone();
    }
    public void BaseProperty(List<CollisionProperty> bulletProperty) // 총알 기본 속성, Fire, Freeze
    {
        if (bulletProperty == null)
            return;
        collisionLength = bulletProperty.Count;
        for (int i = 0; i < collisionLength; i++)
            collisionPropertis.Add(bulletProperty[i].Clone());
    }
    #endregion
    #region override
    public abstract Bullet Clone();
    public abstract void OnCollisionEnter2D(Collision2D coll);
    #endregion
   
}

class BaseBullet : Bullet // 노말 총알 - 데미지.
{
    #region override
    public override Bullet Clone()
    {
        Bullet clone = new BaseBullet();
        clone.Init(owner, damage, speed, range, sprite);
        clone.Property(baseCollision);
        clone.Property(baseUpdate);
        clone.BaseProperty(collisionPropertis);
        clone.BaseProperty(updatePropertis);
        return clone;
    }

    public override void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
            return;
        baseCollision.Collision(ref coll);
        for(int i = 0; i < collisionLength; i++)
        {
            collisionPropertis[i].Collision(ref coll);
        }
    }
    #endregion
}

//class LineBullet : Bullet // 레이저
//{
//    LineRenderer line;
//    int life;
//    float width;
//    int bounces;
//    bool isPierce;

//    public LineBullet(int damage, float speed, float range, float width, int life) : base(damage, speed, range)
//    {
//        this.width = width;
//        this.life = life;
//    }
//    #region override
//    public override Bullet Clone()
//    {
//        Bullet clone = new LineBullet(damage, speed, range, width,life);
//        clone.SetBulletStrategy(bulletStrategy);
//        return clone;
//    }
//    public override void SetProperties()
//    {
//        line = this.gameObject.GetComponent<LineRenderer>();
//        line.startWidth = width;
//        line.startWidth = width;
//        bounces = bulletStrategy.Coliision();
//        switch (bounces) // BulletStrategy
//        {
//            case -1: // 관통 탄이면
//                isPierce = true;
//                break;
//            case 0:// 노말 탄이면
//                isPierce = false;
//                break;
//            default:
//                isPierce = false;
//                break;
//        }
//    }// 개별 속성 설정
//    public override void OnCollisionEnter2D(Collision2D coll)
//    {
//        // If a missile hits this object
//        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet")) // 플레이어 or bullet에는 충돌하지 않음
//            return;
//        // 미사일 히트!
//    }
//    public override void Update()
//    {
//        if (!CheckDistance() || life <= 0)
//        {
//            Remove();
//            return;
//        }
//        life--;
//        bulletStrategy.InitMemberValue();
//        LaserBeam(gameObject.transform.position, direction, range);
//    }
//    #endregion
//    private void LaserBeam(Vector3 src, Vector3 dir, float distance)
//    {
//        Vector2 position = src;
//        Vector2 direction = dir;
//        Vector2 lastPosition = position;
//        int vertCount = 1;
//        int bounceCount = 0;
//        bool loop = true;
//        line.positionCount = vertCount;
//        line.SetPosition(vertCount - 1, position);
//        do
//        {          
//            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance);

//            if (hit == true)
//            {
//                //hit
//                Vector2 hitPosition = hit.point;
//                Vector2 hitNormal = hit.normal;
//                distance -= hit.distance;
//                position = hitPosition;

//                Wall wall = hit.collider.GetComponent<Wall>();
//                if (wall != null)// 벽과 충돌했을 때
//                {
//                    if (wall.reflect == false)
//                    {
//                        vertCount += 3;
//                        line.positionCount = vertCount;
//                        line.SetPosition(vertCount - 3, Vector2.MoveTowards(position, lastPosition, 0.001f));
//                        line.SetPosition(vertCount - 2, position);
//                        line.SetPosition(vertCount - 1, position);
//                        break;
//                    }
//                    Vector2 reflection = Vector2.Reflect(direction, hitNormal);

//                    direction = reflection;

//                    bounceCount++;
//                    vertCount += 3;
//                    line.positionCount = vertCount;
//                    line.SetPosition(vertCount - 3, Vector2.MoveTowards(position, lastPosition, 0.001f));
//                    line.SetPosition(vertCount - 2, position);
//                    line.SetPosition(vertCount - 1, position);

//                    lastPosition = position;

//                    position = position + direction * 0.001f;
//                    if (bounceCount > bounces)
//                    {
//                        loop = false;
//                    }
//                }
//                else// 벽이 아닌 것과 충돌 했을 때
//                {

//                    vertCount += 3;
//                    line.positionCount = vertCount;
//                    line.SetPosition(vertCount - 3, Vector2.MoveTowards(position, lastPosition, 0.001f));
//                    line.SetPosition(vertCount - 2, position);
//                    line.SetPosition(vertCount - 1, position);

//                    lastPosition = position;

//                    position = position + direction * 0.001f;
//                    if(!isPierce) //통과 기능이 없을때
//                    {
//                        loop = false;
//                    }
//                }
//            }
//            else
//            {
//                // No hit
//                vertCount++;
//                line.positionCount = vertCount;
//                line.SetPosition(vertCount - 1, position + (direction * distance));
//                loop = false;
//            }
//        } while (loop);
//    }

//}