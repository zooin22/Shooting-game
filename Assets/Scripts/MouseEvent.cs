using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class MouseMode
{
    public Weapon weapon;

    public void Init(Weapon weapon) { this.weapon = weapon; }

    public abstract void MouseDown(Transform position, Vector2 dest);

    public abstract void MouseUp(Transform position, Vector2 dest);
}

class NormalMode : MouseMode
{
    public override void MouseDown(Transform position, Vector2 dest)
    {
        if (0 >= weapon.GetAmmo() || weapon.GetGunState() != GunState.IDLE)
            return;
        weapon.MinusAmmo();
        weapon.RateOfFire();
        weapon.GetShotMode().ShotStart(position, dest); // StrategyShot에 따른 발사.
    }
    public override void MouseUp(Transform position, Vector2 dest) { }
}

class ChargeMode : MouseMode
{
    public override void MouseDown(Transform position, Vector2 dest)
    {
        if (0 >= weapon.GetAmmo() || weapon.GetGunState() != GunState.IDLE)
            return;
        weapon.MinusAmmo();
        weapon.GetShotMode().ShotStart(position, dest);
    }

    public override void MouseUp(Transform position, Vector2 dest)
    {
        weapon.GetShotMode().ShotEnd(position, dest);
    }
}