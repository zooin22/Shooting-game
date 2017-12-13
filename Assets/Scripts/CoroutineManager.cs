using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour {
    public static CoroutineManager instance;
    IEnumerator reloadRoutine;
    // Use this for initialization
    void Start () {
        instance = this;
    }

    public void RateOfFire(Weapon weapon) // 리로드 코루틴을 위한 함수 코루틴의 겨우 ref와 out을 허락하지 않으므로 class값을 넘겨 레퍼런스를 받음
    {
        reloadRoutine = RateOfFireRoutine(weapon);
        StartCoroutine(reloadRoutine);
    }

    private IEnumerator RateOfFireRoutine(Weapon weapon) // 리로드 코루틴을 위한 코루틴 함수
    {
        yield return new WaitForSeconds(weapon.GetRateOfFireTime());
        weapon.SetGunState(GunState.IDLE);
    }

    public void Reload(Weapon weapon) // 리로드 코루틴을 위한 함수 코루틴의 겨우 ref와 out을 허락하지 않으므로 class값을 넘겨 레퍼런스를 받음
    {
        reloadRoutine = ReloadRoutine(weapon);
        StartCoroutine(reloadRoutine);
    }

    private IEnumerator ReloadRoutine(Weapon weapon) // 리로드 코루틴을 위한 코루틴 함수
    {
        yield return new WaitForSeconds(weapon.GetReloadTime());
        weapon.SetAmmoCapacity();
        weapon.SetAmmo();
        weapon.SetGunState(GunState.IDLE);
    }

    public void SwitchWeapon(float reloadTime, WeaponBag weaponBag)
    {
        weaponBag.Init();
        StartCoroutine(SwitchWeaponRouine(reloadTime, weaponBag));
    }
    private IEnumerator SwitchWeaponRouine(float reloadTime, WeaponBag weaponBag) // 리로드 코루틴을 위한 코루틴 함수
    {
        yield return new WaitForSeconds(reloadTime);
        weaponBag.SwapWeapon();
    }

}

