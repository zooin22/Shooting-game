using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Charcter
{
    public Transform arm;
    public SpriteRenderer gunBody;
    public Transform gunBarrel;
    public Sprite sprite;
    public Transform target;
    Vector3 aim;

    WeaponBag weaponBag;
    Weapon weapon;

    public void Init(Sprite sprite, Transform target,int hp, float speed)
    {
        this.GetComponent<SpriteRenderer>().sprite = sprite;
        this.target = target;
        isAlive = true;
    }

    #region override
    protected override void Aiming()
    {
        aim = target.position;
        arm.rotation = BulletCal.GetRotFromVector(aim - this.transform.position);
        arm.Rotate(new Vector3(0, 0, -90));
    }
    protected override void Attacked(int damage)
    {
        this.hp -= damage;
    }
    protected override void Move()
    {
        transform.Translate((target.position-this.transform.position).normalized * Time.deltaTime);
    }
    protected override void Shot()
    {

    }
    protected override void Action()
    {

    }
    protected override void Death()
    {
        isAlive = false;
    }
    #endregion
    #region UnityFunction
    private void Update()
    {
        Aiming();
        Shot();
    }

    private void FixedUpdate()
    {
        Move();
        Action();
    }
    #endregion
}
