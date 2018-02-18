using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonManager<T> {

    private JsonManager<T> instance;

    public JsonManager<T> GetInstance()
    {
        if (instance == null)
        {
            instance = new JsonManager<T>();
        }
        return instance;
    }

    T data;
    string s;

    public void ToJson(T _data)
    {
        data = _data;
        s = JsonUtility.ToJson(_data);
        Debug.Log(s);
    }

	//// Use this for initialization
	//void Start () {
 //       //Sprite[] abilityIconsAtlas = Resources.LoadAll<Sprite>("Sprites/pack");
 //       //Weapon weapon = new Weapon(abilityIconsAtlas[0], WeaponState.Owner.PLAYER, 5, 50, 1000, 100, 100, 0.1f, 10, 0.1f, WeaponState.EBullet.BaseBullet , WeaponState.EMouseMode.ChargeMode, WeaponState.EShotMode.ChargeShot,
 //       //   WeaponState.EBulletProperty.BaseNormalUpdateProperty, WeaponState.EBulletProperty.BaseNormalCollisionProperty,
 //       //   WeaponState.EBulletProperty.FreezeProperty, WeaponState.EBulletProperty.HomingProperty);
 //       //string s = JsonUtility.ToJson(weapon);
 //       //Debug.Log(s);
 //   }

}
