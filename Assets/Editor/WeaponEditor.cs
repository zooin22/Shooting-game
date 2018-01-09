using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class WeaponEditor : EditorWindow {

    GameObject obj;
    string name;
    Sprite sprite;
    Sprite bulletSprite;
    WeaponState.Owner owner;
    int damage;
    int ammoCapacity;
    int magazine;
    float reloadTime;
    float range;
    float rateOfFire;
    float bulletSpeed;
    float minuteOfAngle;
    float knockBack;
    int num = 1;
    float angle = 60;
    int maxCharged = 2;
    WeaponState.EBullet eBullet;
    WeaponState.EBulletProperty eUpdateProperty;
    WeaponState.EBulletProperty eCollisionProperty;
    WeaponState.EBulletProperty[] eBulletProperty;
    int numOfBulletProperty;
    WeaponState.EMouseMode eMouseMode;
    WeaponState.EShotMode eShotMode;
    
    [MenuItem("Custom/Weapon")]
    static public void ShowWindow()
    {
        // 윈도우 생성
        WeaponEditor window = (WeaponEditor)EditorWindow.GetWindow(typeof(WeaponEditor));
    }
     
    void OnGUI()
    {
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
        obj = (GameObject)EditorGUILayout.ObjectField( "Find Dependency", obj, typeof(GameObject));
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
        bulletSprite = (Sprite)EditorGUILayout.ObjectField("BulletSprite", bulletSprite, typeof(Sprite), allowSceneObjects: true);
        sprite = (Sprite)EditorGUILayout.ObjectField("WeaponSprite", sprite, typeof(Sprite), allowSceneObjects: true);
        name = EditorGUILayout.TextField("Name", name);
        owner = (WeaponState.Owner)EditorGUILayout.EnumPopup("Owner", owner);
        damage = EditorGUILayout.IntField("Damage", damage);
        ammoCapacity = EditorGUILayout.IntField("AmmoCapacity", ammoCapacity);
        magazine = EditorGUILayout.IntField("Magazine", magazine);
        reloadTime = EditorGUILayout.FloatField("ReloadTime", reloadTime);
        range = EditorGUILayout.FloatField("Range", range);
        rateOfFire = EditorGUILayout.FloatField("RateOfFire", rateOfFire);
        bulletSpeed = EditorGUILayout.FloatField("BulletSpeed", bulletSpeed);
        minuteOfAngle = EditorGUILayout.FloatField("MinuteOfAngle", minuteOfAngle);
        knockBack = EditorGUILayout.FloatField("KnockBack", knockBack);
        switch (eShotMode)
        {
            default:
            case WeaponState.EShotMode.SingleShot:
                break;
            case WeaponState.EShotMode.SpreadShot:
                num = EditorGUILayout.IntField("Num", num);
                angle = EditorGUILayout.FloatField("Angle", angle);
                break;
            case WeaponState.EShotMode.ChargeShot:
                maxCharged = EditorGUILayout.IntField("MaxCharged", maxCharged);
                break;
        }
        eBullet = (WeaponState.EBullet)EditorGUILayout.EnumPopup("Bullet", eBullet);
        eMouseMode = (WeaponState.EMouseMode)EditorGUILayout.EnumPopup("MouseMode", eMouseMode);
        eShotMode = (WeaponState.EShotMode)EditorGUILayout.EnumPopup("ShotMode", eShotMode);
        eUpdateProperty = (WeaponState.EBulletProperty)EditorGUILayout.EnumPopup("UpdateProperty", eUpdateProperty);
        eCollisionProperty = (WeaponState.EBulletProperty)EditorGUILayout.EnumPopup("CollisionProperty", eCollisionProperty);
        EditorGUI.BeginChangeCheck();
        numOfBulletProperty = EditorGUILayout.IntField("Num of BulletProperty", numOfBulletProperty);
        if (EditorGUI.EndChangeCheck())
        {           
            eBulletProperty = new WeaponState.EBulletProperty[numOfBulletProperty];
        }
        for (int i = 0; i < numOfBulletProperty; i++)
        {
            eBulletProperty[i] = (WeaponState.EBulletProperty)EditorGUILayout.EnumPopup("BulletProperty", eBulletProperty[i]);
        }
        if (GUILayout.Button("Create"))
        {
            GameObject weaponObj = Instantiate(Resources.Load("Prefabs/Item") as GameObject,Vector3.zero,Quaternion.identity,null);
            weaponObj.name = name;
            weaponObj.AddComponent<WeaponItemWrapper>().SetItem(name, sprite, bulletSprite, owner,
                damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed, minuteOfAngle, knockBack,
                num,angle,maxCharged,
                eBullet, eMouseMode, eShotMode, eUpdateProperty, eCollisionProperty,
                eBulletProperty);
        }
        if (GUILayout.Button("Save"))
        {
            StringBuilder stringBuilder;
            try
            {
                stringBuilder = new StringBuilder(File.ReadAllText(Application.dataPath + @"/weaponData.json"));

            }
            catch
            {
                stringBuilder = new StringBuilder();
            }
            if (obj != null)
            {
                if (obj.GetComponent<WeaponItemWrapper>() != null)
                {
                    stringBuilder.Append("\n");
                    stringBuilder.Append(JsonUtility.ToJson(obj.GetComponent<WeaponItemWrapper>()));
                    File.WriteAllText(Application.dataPath + @"/weaponData.json", stringBuilder.ToString());
                }
            }
            else
            {
                GameObject weaponObj = Instantiate(Resources.Load("Prefabs/Item") as GameObject, Vector3.zero, Quaternion.identity, null);
                weaponObj.name = name;
                weaponObj.AddComponent<WeaponItemWrapper>().SetItem(name, sprite, bulletSprite, owner,
                    damage, ammoCapacity, magazine, reloadTime, range, rateOfFire, bulletSpeed, minuteOfAngle, knockBack,
                    num, angle, maxCharged,
                    eBullet, eMouseMode, eShotMode, eUpdateProperty, eCollisionProperty,
                    eBulletProperty);
                stringBuilder.Append("\n");
                stringBuilder.Append(JsonUtility.ToJson(weaponObj.GetComponent<WeaponItemWrapper>()));
                File.WriteAllText(Application.dataPath + @"/weaponData.json", stringBuilder.ToString());
                DestroyImmediate(weaponObj);
            }
        }
    }

}
