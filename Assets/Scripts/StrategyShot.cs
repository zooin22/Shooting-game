//Template Method Pattern 사용

using UnityEngine;

public abstract class StrategyShot : BaseObject
{
    protected Vector3 direction; // 총알 박향 벡터
    protected Quaternion lookRotation; // 총알 방향으로 이미지 회전
    protected Bullet bulletT;// 총알 타입
    protected float speed = 5; // 총알 속도
    
    public StrategyShot(ref Bullet bulletT) //생성자
    {
        this.bulletT = bulletT;
    }

    public virtual void ShotStart(Transform center, Vector3 dest) {// center 에서 dest 방향 벡터로 발사
        direction = (new Vector3(dest.x, dest.y, 0) - center.position).normalized;
        lookRotation = BulletCal.GetRotFromVector(direction);
    }

    public virtual void ShotEnd(Transform center, Vector3 dest)
    {

    }
    public virtual void InitBullet(GameObject bulletObj, Vector3 position,Vector3 direction, Quaternion rotation) // 총알 초기화
    {
        Bullet mBullet = bulletT.Clone(); // 총알 속성 스크립트
        mBullet.SetGameObject(bulletObj); // 총알 속성 스크립트에 총알 정보 전달
        bulletObj.transform.position = position; // 총알 위지 설정
        bulletObj.transform.rotation = rotation; // 총알 각도 설정
        bulletObj.AddComponent<Property>().SetBullet(mBullet, direction, position); // 총알 발사.
        bulletObj.SetActive(true); // 트루
    }
}

class PlainShot : StrategyShot // 기본 샷 1발짜리
{
    public PlainShot(Bullet bullet) : base(ref bullet)
    {
    }

#region override
    public override void ShotStart(Transform center, Vector3 dest) //발사
    {
        base.ShotStart(center, dest);
        GameObject bullet = ObjectPool.current.GetPooledObject();
        if (bullet == null || bullet.GetComponent<Property>() != null) return;
        InitBullet(bullet, center.position, direction, lookRotation);
    }
#endregion
}

class SpreadShot : StrategyShot // 퍼치는 샷 num발 짜리 angle각도
{ 
    private int num = 4; // 발사 갯수
    private float angle = 60f; // 발사 각도

    public SpreadShot(Bullet bullet, int num,float angle) : base(ref bullet)
    {
        this.num = num;
        this.angle = angle;
    }

#region override
    public override void ShotStart(Transform center, Vector3 dest)//발사
    {
        base.ShotStart(center, dest);
        float shotAngle = -angle / 2;
        float addAngle = angle / (num-1);
        for (int i = 0; i < num; i++)
        {
            Vector3 directionR = BulletCal.VectorRotate(direction, shotAngle); // angle만큼 회전된 값.
            Quaternion lookRotationR = BulletCal.GetRotFromVector(directionR);

            GameObject bullet = ObjectPool.current.GetPooledObject();
            if (bullet == null || bullet.GetComponent<Property>() != null) return;
            InitBullet(bullet, center.position, directionR, lookRotationR);
            shotAngle += addAngle;
        }
    }
#endregion
}

class OnlyOneShot : StrategyShot // 기본 샷 1발짜리
{
    GameObject bullet;
    Vector3 oldCenter;
    Vector3 oldDirection;
    public OnlyOneShot(Bullet bullet) : base(ref bullet)
    {
    }

    #region override
    public override void ShotStart(Transform center, Vector3 dest) //발사
    {
        base.ShotStart(center, dest);
        if (bullet == null)
        {
            oldCenter = center.position;
            oldDirection = dest;
            bullet = ObjectPool.current.GetPooledObject();
            if (bullet == null) return;
            InitBullet(bullet, center.position, direction, lookRotation);
        }
        if (oldDirection != dest || oldCenter != center.position)
        {
            InitBullet(bullet, center.position, direction, lookRotation);
        }
    }
    public override void ShotEnd(Transform center, Vector3 dest) //발사
    {
        if (bullet != null)
        {
            ObjectPool.current.FreeObject(bullet);
            bullet = null;
        }
    }

    public override void InitBullet(GameObject bulletObj, Vector3 position, Vector3 direction, Quaternion rotation) // 총알 초기화 Template Method
    {
        Bullet mBullet = bulletT.Clone(); // 총알 속성 스크립트
        mBullet.SetGameObject(bulletObj); // 총알 속성 스크립트에 총알 정보 전달
        bulletObj.transform.position = position; // 총알 위치 설정
        bulletObj.transform.rotation = rotation; // 총알 각도 설정
        if (bulletObj.GetComponent<Property>() == null)
            bulletObj.AddComponent<Property>().SetBullet(mBullet, direction, position); // 총알 발사.
        else
            bulletObj.GetComponent<Property>().SetBullet(mBullet, direction, position); // 총알 발사.
        bulletObj.SetActive(true); // 트루
    }
    #endregion
}