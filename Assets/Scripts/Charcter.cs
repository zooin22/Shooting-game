using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Charcter : MonoBehaviour
{
    protected enum State { IDLE, DASH, KNOCKBACK, DEAD }
    protected enum Direction { LEFT, RIGHT, UP, DOWN, UPLEFT, UPRIGHT, DOWNLEFT, DOWNRIGHT }
    protected float speed;
    protected int maxHp;
    protected int hp;
    protected State state;
    protected Vector3 lastPosition;
    protected WeaponBag weaponBag;
    protected Weapon weapon;
    protected Vector3 aim;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AudioSource audioSource;

    public Transform arm;
    public SpriteRenderer gunBody;
    public Transform gunBarrel;

    public Weapon GetWeapon() { return weapon; }
    public void SetWeapon(Weapon weapon) { this.weapon = weapon; gunBody.sprite = weapon.GetSprite(); }

    public abstract void Attacked(int damage, Vector2 pushDirection, float disctance); //피격
    protected abstract void Aiming(); //조준
    protected abstract void Move(); //이동
    protected abstract void Action(); //행동
    protected abstract void Shot(); //발사
    protected abstract void Death(); //사망
}
