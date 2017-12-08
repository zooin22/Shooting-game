using UnityEngine;
using System.Collections;

public enum ShotType { CHARGED, SEMIAUTOMATIC, AUTOMATIC, BURST }
public enum GunState { IDLE, SHOT, RELOAD, CHARGE }

public abstract class Weapon : BaseObject // 무기 클래스
{
    protected StrategyShot strategyShot;
    protected Bullet bullet;
    protected GunState gunState;
    protected ShotType shotType;
    protected readonly int damage;//데미지
    protected int ammoCapacity; //탄약 량
    protected readonly int magazine; // 탄창 크기
    protected int ammo; // 현재 탄약
    protected readonly float reloadTime; // 리로드 속도
    protected readonly float range; // 사거리 
    protected readonly float rateOfFire; // 연사속도 
    protected readonly float bulletSpeed; // 총알속도 
    #region setter
    public void SetAmmo()
    {
        ammo = magazine;
    }
    public void SetAmmoCapacity()
    {
        ammoCapacity--;
    }
    public void SetGunState()
    {
        gunState = GunState.IDLE;
    }
    #endregion
    #region getter
    public int GetAmmo()
    {
        return ammo;
    }   
    public int GetDamage()
    {
        return damage;
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
    #endregion

    public Weapon(int damage,int ammoCapacity,int magazine,float reloadTime,float range,float rateOfFire,float bulletSpeed) // 생성자
    {
        this.gunState = GunState.IDLE;
        this.damage = damage;
        this.ammoCapacity = ammoCapacity;
        this.magazine = magazine;
        this.ammo = magazine;
        this.reloadTime = reloadTime;
        this.range = range;
        this.rateOfFire = rateOfFire;
        this.bulletSpeed = bulletSpeed;
    }

    public virtual void Stop(Transform position, Vector3 dest)
    {
    }
    public virtual void MouseDown(Transform position, Vector3 dest) {
        if (0 >= ammo || gunState != GunState.IDLE || gunState == GunState.SHOT)
            return;
        ammo--;
        RateOfFire();
        strategyShot.ShotStart(position, dest); // StrategyShot에 따른 발사.
    }
    public abstract void MouseUp(Transform position, Vector3 dest);
    #region coroutine
    public virtual void RateOfFire()
    {
        if (gunState == GunState.RELOAD || gunState == GunState.SHOT)
            return;
        gunState = GunState.SHOT;
        CoroutineManager.instance.RateOfFire(this);
    }

    public void Reload()
    {
        if (ammo == magazine || ammoCapacity <= 0)
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
    public NoWeapon(int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed) :
        base(damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
    {
    }

    #region override
    public override void Stop(Transform position, Vector3 dest)
    {
    }
    public override void MouseDown(Transform position, Vector3 dest)
    {
    }
    public override void MouseUp(Transform position, Vector3 dest)
    {
    }
    #endregion
}

class BaseGun : Weapon
{
    public BaseGun(int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire,float bulletSpeed) : 
        base(damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
    {
        bullet = new NormalBullet(new BaseBullet(damage, bulletSpeed, range));
        strategyShot = new PlainShot(bullet);
    }

    #region override
    //public override void Shot(Transform _position, Vector3 _dest)
    //{
    //    if (0 >= ammo)
    //        return;
    //    strategyShot.Shot(_position, _dest); // StrategyShot에 따른 발사.
    //}
    public override void MouseUp(Transform position, Vector3 dest) {
    }
    #endregion
}

class BounceGun : Weapon
{
    public BounceGun(int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire,float bulletSpeed, int bounceTime) : 
        base(damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
    {
        bullet = new BouncedBullet(new BaseBullet(damage, bulletSpeed, range), bounceTime);
        strategyShot = new PlainShot(bullet);
    }

    #region override
    //public override void Shot(Transform _position, Vector3 _dest)
    //{
    //    if (0 >= ammo)
    //        return;
    //    strategyShot.Shot(_position, _dest); // StrategyShot에 따른 발사.
    //}
    public override void MouseUp(Transform position, Vector3 dest)
    {
    }
    #endregion
}

class ShotGun : Weapon
{
    public ShotGun(int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed) :
        base(damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
    {
        bullet = new NormalBullet(new BaseBullet(damage, bulletSpeed, range));
        strategyShot = new SpreadShot(bullet,5,60);
    }

    #region override
    //public override void Shot(Transform _position, Vector3 _dest)
    //{
    //    if (0 >= ammo)
    //        return;
    //    strategyShot.Shot(_position, _dest); // StrategyShot에 따른 발사.
    //}
    public override void MouseUp(Transform position, Vector3 dest)
    {
    }
    #endregion
}

class LaserGun : Weapon
{
    public LaserGun(int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire , float bulletSpeed) : 
        base(damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed)
    {
        bullet = new LineBullet(new BouncedBullet(new BaseBullet(damage, bulletSpeed,range),5),0.1f);
        strategyShot = new OnlyOneShot(bullet);
    }

    #region override
    public override void Stop(Transform position, Vector3 dest)
    {
        strategyShot.ShotEnd(position, dest); // StrategyShot에 따른 발사.
    }
    public override void MouseDown(Transform position, Vector3 dest)
    {
        if (0 >= ammo || gunState != GunState.IDLE || gunState == GunState.RELOAD)
            return;
        strategyShot.ShotStart(position, dest); // StrategyShot에 따른 발사.
    }
    public override void MouseUp(Transform position, Vector3 dest)
    {
        strategyShot.ShotEnd(position, dest); // StrategyShot에 따른 발사.
    }
    #endregion
}