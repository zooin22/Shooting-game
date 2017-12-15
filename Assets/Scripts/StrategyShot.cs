//Template Method Pattern 사용

using UnityEngine;

public abstract class ShotStrategy : BaseObject
{
    protected Vector3 direction; // 총알 박향 벡터
    protected Quaternion lookRotation; // 총알 방향으로 이미지 회전
    protected Bullet bulletT;// 총알 타입
    protected float speed = 5; // 총알 속도
    
    public ShotStrategy(ref Bullet bulletT) //생성자
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
    public void InitBullet(GameObject bulletObj, Vector3 position,Vector3 direction, Quaternion rotation) // 총알 초기화
    {
        Bullet bullet = bulletT.Clone(); // 총알 속성 스크립트
        bullet.SetGameObject(bulletObj); // 총알 속성 스크립트에 총알 정보 전달
        bulletObj.transform.position = position; // 총알 위지 설정
        bulletObj.transform.rotation = rotation; // 총알 각도 설정
        bulletObj.transform.localScale = new Vector3(1, 1, 0);
        bulletObj.AddComponent<Property>().SetBullet(bullet, direction, position); // 총알 발사.
        bulletObj.SetActive(true); // 트루
        SubInitBullet(bullet, bulletObj);
    }
    protected abstract void SubInitBullet(Bullet bullet, GameObject bulletObj);
}

class PlainShot : ShotStrategy // 기본 샷 1발짜리
{
    public PlainShot(Bullet bullet) : base(ref bullet)
    {
    }

    #region override
    protected override void SubInitBullet(Bullet bullet, GameObject bulletObj)
    {
    }
    public override void ShotStart(Transform center, Vector3 dest) //발사
    {
        base.ShotStart(center, dest);
        GameObject bullet = PoolGroup.instance.GetObjectPool(Pool.BULLET).GetPooledObject();
        if (bullet == null || bullet.GetComponent<Property>() != null) return;
        InitBullet(bullet, center.position, direction, lookRotation);
    }
    #endregion
}

class SpreadShot : ShotStrategy // 퍼치는 샷 num발 짜리 angle각도
{ 
    private int num = 4; // 발사 갯수
    private float angle = 60f; // 발사 각도

    public SpreadShot(Bullet bullet, int num,float angle) : base(ref bullet)
    {
        this.num = num;
        this.angle = angle;
    }

    #region override
    protected override void SubInitBullet(Bullet bullet, GameObject bulletObj)
    {
    }
    public override void ShotStart(Transform center, Vector3 dest)//발사
    {
        base.ShotStart(center, dest);
        float shotAngle = -angle / 2;
        float addAngle = angle / (num-1);
        for (int i = 0; i < num; i++)
        {
            Vector3 directionR = BulletCal.VectorRotate(direction, shotAngle); // angle만큼 회전된 값.
            Quaternion lookRotationR = BulletCal.GetRotFromVector(directionR);

            GameObject bullet = PoolGroup.instance.GetObjectPool(Pool.BULLET).GetPooledObject();
            if (bullet == null || bullet.GetComponent<Property>() != null) return;
            InitBullet(bullet, center.position, directionR, lookRotationR);
            shotAngle += addAngle;
        }
    }
    #endregion
}

class ChargeShot : ShotStrategy // 기본 샷 1발짜리
{
    float charged;
    int maxCharged;
    bool isCharge;

    public ChargeShot(Bullet bullet,int maxCharged) : base(ref bullet)
    {
        this.charged = 1;
        this.maxCharged = maxCharged;
        this.isCharge = false;
    }

    #region override
    protected override void SubInitBullet(Bullet bullet, GameObject bulletObj)
    {
        bullet.SetChargeDamage((int)charged);
        bulletObj.transform.localScale *= (int)charged;
    }
    public override void ShotStart(Transform center, Vector3 dest) //발사
    {
        charged += Time.deltaTime;
        if (maxCharged <= charged)
            charged = maxCharged;
        isCharge = true;
    }
    public override void ShotEnd(Transform center, Vector3 dest)
    {
        if (isCharge)
        {
            base.ShotStart(center, dest);
            GameObject bullet = PoolGroup.instance.GetObjectPool(Pool.BULLET).GetPooledObject();
            if (bullet == null || bullet.GetComponent<Property>() != null) return;
            InitBullet(bullet, center.position, direction, lookRotation);
            charged = 1;
            isCharge = false;
        }
    }
    #endregion
}
