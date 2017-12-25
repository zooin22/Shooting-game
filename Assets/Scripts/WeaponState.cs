using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class WeaponState
{
    public enum Owner { PLAYER, ENEMY }
    public enum EBullet { BaseBullet }
    public enum EShotMode { SingleShot, SpreadShot, ChargeShot }
    public enum EMouseMode { NormalMode, ChargeMode }
    public enum EBulletProperty { BaseNormalUpdateProperty, HomingProperty, BaseNormalCollisionProperty, BasePierceProperty, BaseBouncyProperty, FreezeProperty, BurnProperty }

    public static Bullet GetBullet(EBullet eBullet)
    {
        switch (eBullet)
        {
            default:
            case EBullet.BaseBullet:
                return new BaseBullet();
        }
    }
    public static ShotMode GetShotMode(EShotMode eShotMode)
    {
        switch (eShotMode)
        {
            default:
            case EShotMode.SingleShot:
                return new SingleShot();
            case EShotMode.SpreadShot:
                return new SpreadShot();
            case EShotMode.ChargeShot:
                return new ChargeShot();
        }
    }
    public static BulletProperty GetBulletProperty(EBulletProperty eBulletProperty)
    {
        switch (eBulletProperty)
        {
            default:
            case EBulletProperty.BaseNormalCollisionProperty:
                return new BaseNormalCollisionProperty();
            case EBulletProperty.BaseNormalUpdateProperty:
                return new BaseNormalUpdateProperty();
            case EBulletProperty.BasePierceProperty:
                return new BasePierceProperty();
            case EBulletProperty.BaseBouncyProperty:
                return new BaseBouncyProperty();
            case EBulletProperty.BurnProperty:
                return new BurnProperty();
            case EBulletProperty.FreezeProperty:
                return new FreezeProperty();
            case EBulletProperty.HomingProperty:
                return new HomingProperty();
        }
    }
    public static BulletProperty[] GetBulletProperty(EBulletProperty[] eBulletProperty)
    {
        int length = eBulletProperty.Length;
        BulletProperty[] temp = new BulletProperty[length];
        for (int i = 0; i < length; i++)
        {
            switch (eBulletProperty[i])
            {
                default:
                case EBulletProperty.BaseNormalCollisionProperty:
                    temp[i] = new BaseNormalCollisionProperty();
                    break;
                case EBulletProperty.BaseNormalUpdateProperty:
                    temp[i] = new BaseNormalUpdateProperty();
                    break;
                case EBulletProperty.BasePierceProperty:
                    temp[i] = new BasePierceProperty();
                    break;
                case EBulletProperty.BaseBouncyProperty:
                    temp[i] = new BaseBouncyProperty();
                    break;
                case EBulletProperty.BurnProperty:
                    temp[i] = new BurnProperty();
                    break;
                case EBulletProperty.FreezeProperty:
                    temp[i] = new FreezeProperty();
                    break;
                case EBulletProperty.HomingProperty:
                    temp[i] = new HomingProperty();
                    break;
            }
        }
        return temp;
    }
    public static MouseMode GetMouseMode(EMouseMode eMouseMode)
    {
        switch (eMouseMode)
        {
            default:
            case EMouseMode.NormalMode:
                return new NormalMode();
            case EMouseMode.ChargeMode:
                return new ChargeMode();
        }
    }
}