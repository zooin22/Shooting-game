using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBag 
{
    List<Weapon> weapons;
    Charcter charcter;
    private int currentIndex;
    private float switchTime;
    public WeaponBag(Charcter charcter)
    {
        this.charcter = charcter;
        weapons = new List<Weapon>();
        weapons.Add(new NoWeapon());
        currentIndex = 0;
        switchTime = 0.2f;
    }

    public Weapon Init()
    {
        return weapons[0];
    }

    public void Add(Weapon weapon)
    {
        weapons.Add(weapon);
    }

    public void WheelWeapon(bool up)
    {
        if (charcter.GetWeapon().GetGunState() == GunState.RELOAD || charcter.GetWeapon().GetGunState() == GunState.SWITCH)
            return;
        if (!up)
        {
            currentIndex++;
            if (currentIndex >= weapons.Count)
                currentIndex = 0;
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = weapons.Count - 1;
        }
        charcter.GetWeapon().SetGunState(GunState.SWITCH);
        charcter.GetWeapon().MouseUp(charcter.gameObject.transform, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0))); // Weapon에 현재 위치, 목표 위치 전달
        CoroutineManager.instance.SwitchWeapon(switchTime,this);
    }

    public void SwapWeapon()
    {
        charcter.SetWeapon(weapons[currentIndex]);
        charcter.GetWeapon().SetGunState(GunState.IDLE);
    }
}