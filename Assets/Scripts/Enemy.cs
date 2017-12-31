using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Charcter
{
    public Sprite sprite;
    public Transform target;
    public bool isMoving;
    EnemyMoveMode enemyMoveMode;

    public void Init(Sprite sprite, Weapon weapon, Transform target, int hp, float speed, EnemyState.EEnemyMoveMode eEnemyMoveMode)
    {
        this.GetComponent<Unit>().Init(target,speed);
        this.GetComponent<SpriteRenderer>().sprite = sprite;
        this.target = target;
        this.hp = hp;
        this.speed = speed;
        this.weapon = weapon;
        weaponBag = new WeaponBag(this);
        weaponBag.Add(weapon);
        weaponBag.WheelWeapon(false);
        enemyMoveMode = EnemyState.GetEnemyMoveMode(eEnemyMoveMode);
        isAlive = true;
        isMoving = true;
    }
    private void Remove() // 게임 풀 오브젝트 false
    {
        PoolGroup.instance.GetObjectPool(Pool.ENEMY).FreeObject(this.gameObject, Pool.ENEMY);
    }
    private Collider2D Raycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(gunBarrel.position, (aim - gunBarrel.position).normalized, weapon.GetRange()*0.9f, -1537);
        Debug.DrawLine(gunBarrel.position, hit.point, Color.cyan);
        return hit.collider;
    }
    #region override
    public override void Attacked(int damage)
    {
        this.hp -= damage;
        if (hp < 0)
            Death();
    }
    protected override void Aiming()
    {
        aim = target.position;
        arm.rotation = BulletCal.GetRotFromVector(aim - this.transform.position);
        arm.Rotate(new Vector3(0, 0, -90));
        Collider2D collider2D = Raycast();
        if (collider2D != null)
        {
            if (collider2D.CompareTag("Player"))
                isMoving = false;
            else
                isMoving = true;
        }
        else
        {
            isMoving = true;
        }
    }
    protected override void Move()
    {
        if (!isMoving)
        {
            enemyMoveMode.Stop(this);
            return;
        }
        enemyMoveMode.Move(this, this.transform, target, speed);
    }
    protected override void Shot()
    {
        if (isMoving)
            return;
        weapon.MouseDown(gunBarrel, aim); // Weapon에 현재 위치, 목표 위치 전달
    }
    protected override void Action()
    {

    }
    protected override void Death()
    {
        isAlive = false;
        Remove();
        Debug.Log("Death");
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
