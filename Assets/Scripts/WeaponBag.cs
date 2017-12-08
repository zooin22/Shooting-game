using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class WeaponBag 
{
    List<Weapon> weapons;
    GameObject player;
    public WeaponBag(GameObject player)
    {
        this.player = player;
        weapons = new List<Weapon>();
        weapons.Add(new NoWeapon(0, 0, 0, 0, 0, 0, 0));
    }

    public Weapon Init()
    {
        return weapons[0];
    }

    public void Add(Weapon weapon)
    {
        weapons.Add(weapon);
    }

    public void SwapWeapon(ref Weapon weapon,int i)
    {
        if (i >= weapons.Count || weapon.GetGunState() == GunState.RELOAD)
            return;
        weapon.MouseUp(player.transform, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0))); // Weapon에 현재 위치, 목표 위치 전달
        weapon = weapons[i];
    }
}