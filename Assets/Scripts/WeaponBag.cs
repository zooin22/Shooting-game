using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBag 
{
    List<Weapon> weapons;
    Player player;
    private int currentIndex;
    private float switchTime;
    public WeaponBag(Player player)
    {
        this.player = player;
        weapons = new List<Weapon>();
        weapons.Add(new NoWeapon(0, 0, 0, 0, 0, 0, 0));
        currentIndex = 0;
        switchTime = 1;
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
        if (player.GetWeapon().GetGunState() == GunState.RELOAD || player.GetWeapon().GetGunState() == GunState.SWITCH)
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
        player.GetWeapon().SetGunState(GunState.SWITCH);
        player.GetWeapon().MouseUp(player.gameObject.transform, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0))); // Weapon에 현재 위치, 목표 위치 전달
        CoroutineManager.instance.SwitchWeapon(switchTime,this);
        Debug.Log(currentIndex);
    }

    public void SwapWeapon()
    {
        player.SetWeapon(weapons[currentIndex]);
        player.GetWeapon().SetGunState(GunState.IDLE);
    }
}