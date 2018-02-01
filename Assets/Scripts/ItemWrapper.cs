using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ItemType { Weapon }

[Serializable]
public abstract class ItemWrapper : MonoBehaviour // Item Wraaper
{
    public Item item;
    public ItemType itemType;
    public string name;
    public Sprite sprite;
    
    public Item PickItem()
    {
        return item;
    }
    public void DropItem()
    {

    }
}

[Serializable]
public class WeaponItemWrapper : ItemWrapper
{
    public Sprite bulletSprite;
    public WeaponState.Owner owner;
    public int damage;
    public int ammoCapacity;
    public int magazine;
    public float reloadTime;
    public float range;
    public float rateOfFire;
    public float bulletSpeed;
    public float minuteOfAngle;
    public float knockBack;

    public int num;
    public float angle;
    public int maxCharged;

    public WeaponState.EBullet eBullet;
    public WeaponState.EBulletProperty eUpdateProperty;
    public WeaponState.EBulletProperty eCollisionProperty;
    public WeaponState.EBulletProperty[] eBulletProperty;
    public WeaponState.EMouseMode eMouseMode;
    public WeaponState.EShotMode eShotMode;

    public void Start()
    {
        CallItem(name, sprite, bulletSprite, owner,
              damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed, minuteOfAngle, knockBack,
              num, angle, maxCharged,
              eBullet, eMouseMode, eShotMode, eUpdateProperty, eCollisionProperty,
              eBulletProperty);
    }

    public void SetItem(string name, Sprite sprite, Sprite bulletSprite, WeaponState.Owner owner,
       int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed, float minuteOfAngle, float knockBack,
       int num, float angle, int maxCharged,
       WeaponState.EBullet eBullet, WeaponState.EMouseMode eMouseMode, WeaponState.EShotMode eShotMode, WeaponState.EBulletProperty eUpdateProperty, WeaponState.EBulletProperty eCollisionProperty,
       params WeaponState.EBulletProperty[] eBulletProperty)
    {
        this.name = name;
        this.sprite = sprite;
        this.bulletSprite = bulletSprite;
        this.owner = owner;
        this.damage = damage;
        this.ammoCapacity = ammoCapacity;
        this.magazine = magazine;
        this.reloadTime = reloadTime;
        this.range = range;
        this.rateOfFire = rateOfFire;
        this.bulletSpeed = bulletSpeed;
        this.minuteOfAngle = minuteOfAngle;
        this.knockBack = knockBack;
        this.num = num;
        this.angle = angle;
        this.maxCharged = maxCharged;
        this.eBullet = eBullet;
        this.eUpdateProperty = eUpdateProperty;
        this.eCollisionProperty = eCollisionProperty;
        this.eBulletProperty = eBulletProperty;
        this.eMouseMode = eMouseMode;
        this.eShotMode = eShotMode;
        CallItem(name, sprite, bulletSprite, owner,
             damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed, minuteOfAngle, knockBack,
             num, angle, maxCharged,
             eBullet, eMouseMode, eShotMode, eUpdateProperty, eCollisionProperty,
             eBulletProperty);
    }

    public void CallItem(string name, Sprite sprite, Sprite bulletSprite, WeaponState.Owner owner,
        int damage, int ammoCapacity, int magazine, float reloadTime, float range, float rateOfFire, float bulletSpeed, float minuteOfAngle, float knockBack,
        int num, float angle, int maxCharged,
        WeaponState.EBullet eBullet, WeaponState.EMouseMode eMouseMode, WeaponState.EShotMode eShotMode, WeaponState.EBulletProperty eUpdateProperty, WeaponState.EBulletProperty eCollisionProperty,
        params WeaponState.EBulletProperty[] eBulletProperty)
    {
        itemType = ItemType.Weapon;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        item = new Weapon(name, sprite, bulletSprite, owner, damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed, minuteOfAngle, knockBack,
            num, angle, maxCharged,
            eBullet, eMouseMode, eShotMode, eUpdateProperty, eCollisionProperty,
           eBulletProperty);
    }
}
