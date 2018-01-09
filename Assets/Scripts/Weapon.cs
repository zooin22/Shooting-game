using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public enum ShotType { CHARGED, SEMIAUTOMATIC, AUTOMATIC, BURST }
public enum GunState { IDLE, SHOT, RELOAD, CHARGE, SWITCH }

[System.Serializable]
public class Weapon : Item// 무기 클래스
{
    public string name;
    public Sprite sprite;
    public Sprite bulletSprite;
    public ShotMode shotMode;
    public BulletProperty bulletProperties;
    public MouseMode mouseEvent;
    public Bullet bullet;
    public GunState gunState;
    public ShotType shotType;
    public int ammoCapacity; //탄약 량
    public int magazine; // 탄창 크기
    public int ammo; // 현재 탄약
    public float reloadTime; // 리로드 속도
    public float rateOfFire; // 연사속도 
    private int num = 4; // 발사 갯수
    private float angle = 60f; // 발사 각도
    private int maxCharged = 3;
    private float range;

    #region setter
    public void SetAmmo()
    {
        ammo = magazine;
    }
    public void SetAmmoCapacity()
    {
        ammoCapacity--;
    }
    public void SetGunState(GunState gunState)
    {
        this.gunState = gunState;
    }
    #endregion
    #region getter
    public Sprite GetSprite()
    {
        return sprite;
    }
    public int GetAmmo()
    {
        return ammo;
    }
    public int GetAmmoCapacity()
    {
        return ammoCapacity;
    }
    public int GetMagazine()
    {
        return magazine;
    }
    public GunState GetGunState()
    {
        return gunState;
    }
    public float GetRateOfFireTime()
    {
        return rateOfFire;
    }
    public float GetReloadTime()
    {
        return reloadTime;
    }
    public ShotMode GetShotMode()
    {
        return shotMode;
    }
    public float GetRange() { return range; }
    #endregion

    public Weapon()
    {

    }
    public Weapon(string name,Sprite sprite, Sprite bulletSprite, WeaponState.Owner owner,
        int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed, float minuteOfAngle, float knockBack,
        int num, float angle, int maxCharged,
        WeaponState.EBullet eBullet, WeaponState.EMouseMode eMouseMode, WeaponState.EShotMode eShotMode, WeaponState.EBulletProperty eUpdateProperty, WeaponState.EBulletProperty eCollisionProperty,
        params WeaponState.EBulletProperty[] eBulletProperty) // 생성자
    {
        this.name = name;
        this.sprite = sprite;
        this.bulletSprite = bulletSprite;
        this.gunState = GunState.IDLE;
        this.ammoCapacity = ammoCapacity;
        this.magazine = magazine;
        this.ammo = magazine;
        this.reloadTime = reloadTime;
        this.rateOfFire = rateOfFire;
        this.num = num;
        this.angle = angle;
        this.maxCharged = maxCharged;
        this.range = range; 
        this.bullet = WeaponState.GetBullet(eBullet);
        this.bullet.Init(owner, damage, bulletSpeed, range, knockBack, bulletSprite);
        this.bullet.Property((UpdateProperty)WeaponState.GetBulletProperty(eUpdateProperty));
        this.bullet.Property((CollisionProperty)WeaponState.GetBulletProperty(eCollisionProperty));
        this.bullet.BaseProperty(WeaponState.GetBulletProperty(eBulletProperty));
        this.mouseEvent = WeaponState.GetMouseMode(eMouseMode);
        mouseEvent.Init(this);
        this.shotMode = WeaponState.GetShotMode(eShotMode);
        this.shotMode.InitBullet(ref bullet, bulletSpeed, minuteOfAngle);
        SubInit(eShotMode, shotMode);
    }
    public void SubInit(WeaponState.EShotMode eShotMode,ShotMode shotMode)
    {
        switch (eShotMode)
        {
            default:
            case WeaponState.EShotMode.SingleShot:
                break;
            case WeaponState.EShotMode.SpreadShot:
                SpreadShot spreadShot = (SpreadShot)shotMode;
                spreadShot.Init(num, angle);
                break;
            case WeaponState.EShotMode.ChargeShot:
                ChargeShot chargeShot = (ChargeShot)shotMode;
                chargeShot.Init(maxCharged);
                break;
        }
    }
    public void MinusAmmo()
    {
        ammo--;
    }
    public virtual void MouseDown(Transform position, Vector2 dest)
    {
        mouseEvent.MouseDown(position, dest);
    }
    public virtual void MouseUp(Transform position, Vector2 dest)
    {
        mouseEvent.MouseUp(position, dest);
    }
    #region coroutine
    public virtual void RateOfFire()
    {
        gunState = GunState.SHOT;
        CoroutineManager.instance.RateOfFire(this);
    }
    public void Reload()
    {
        if (GunState.IDLE != gunState || ammo == magazine || ammoCapacity <= 0)
            return;
        gunState = GunState.RELOAD;
        CoroutineManager.instance.Reload(this);
    }
    #endregion
    //private IEnumerator ReloadRoutine(float time) //코루틴의 경우 this값이 null일 경우 사용이 불가.
    //{
    //    yield return new WaitForSeconds(time);
    //    ammoCapacity--;
    //    ammo = magazine;
    //    gunState = GunState.IDLE;
    //}
}

class NoWeapon : Weapon
{
    #region override
    public override void MouseDown(Transform position, Vector2 dest)
    {
    }
    public override void MouseUp(Transform position, Vector2 dest)
    {
    }
    #endregion
}

//class BaseGun : Weapon
//{
//    public BaseGun(WeaponState.Owner owner, int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire,float bulletSpeed) : 
//        base(owner, damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
//    {
//        bullet = new BaseBullet(owner, damage, bulletSpeed, range);
//        bullet.Property(new BaseBouncyProperty());
//        bullet.BaseProperty(new HomingProperty(bulletSpeed));
//        //bullet.BaseProperty(new FreezeProperty(), new BurnProperty());
//        shotMode = new SingleShot(bullet,0.1f);
//    }

//    #region override
//    #endregion
//}

//class BounceGun : Weapon
//{
//    public BounceGun(WeaponState.Owner owner, int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed, int bounceTime) :
//        base(owner, damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
//    {
//        bullet = new BaseBullet(owner, damage, bulletSpeed, range);
//        bullet.Property(new BaseBouncyProperty());
//        shotMode = new SingleShot(bullet,0);
//    }

//    #region override
//    #endregion
//}

//class ShotGun : Weapon
//{
//    public ShotGun(WeaponState.Owner owner, int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed) :
//        base(owner, damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
//    {
//        bullet = new BaseBullet(owner, damage, bulletSpeed, range);
//        bullet.Property(new BaseBouncyProperty());
//        shotMode = new SpreadShot(bullet, 0, 5, 60);
//    }

//    #region override
//    #endregion
//}

//class ChargeGun : Weapon
//{
//    public ChargeGun(WeaponState.Owner owner, int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed) :
//        base(owner, damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
//    {
//        bullet = new BaseBullet(owner, damage, bulletSpeed, range);
//        bullet.Property(new BaseBouncyProperty());
//        shotMode = new ChargeShot(bullet, 2);
//    }

//    #region override
//    public override void MouseDown(Transform position, Vector2 dest)
//    {
//        if (0 >= ammo || gunState == GunState.SHOT || gunState == GunState.RELOAD || gunState == GunState.SWITCH)
//            return;
//        ammo--;
//        shotMode.ShotStart(position, dest); // shotMode에 따른 발사.
//    }
//    public override void MouseUp(Transform position, Vector2 dest)
//    {
//        shotMode.ShotEnd(position, dest); // StrategyShot에 따른 발사.
//    }
//    #endregion
//}

//class LaserGun : Weapon
//{
//    public LaserGun(int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed) :
//        base(damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
//    {
//        bullet = new LineBullet(damage, bulletSpeed, range, 0.1f, 1);
//        bulletStrategy = new BouncedBulletStrategy(3);

//        bullet.SetBulletStrategy(bulletStrategy);
//        shotMode = new SingleShot(bullet);
//    }

//    #region override
//    public override void MouseDown(Transform position, Vector3 dest)
//    {
//        if (0 >= ammo || gunState != GunState.IDLE)
//            return;
//        ammo--;
//        shotMode.ShotStart(position, dest); // shotMode에 따른 발사.
//    }
//    #endregion
//}
