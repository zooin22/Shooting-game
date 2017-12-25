using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Charcter : BaseObject
{
    protected enum State {IDLE,WALK,ROLL,DIE}
    protected enum Direction {LEFT,RIGHT,UP,DOWN,UPLEFT,UPRIGHT,DOWNLEFT,DOWNRIGHT}
    protected float speed;
    protected int maxHp;
    protected int hp;
    protected bool isAlive;
    protected State state;
    protected Vector3 lastPosition;

    protected abstract void Aiming(); //조준
    protected abstract void Attacked(int damage); //피격
    protected abstract void Move(); //이동
    protected abstract void Action(); //행동
    protected abstract void Shot(); //발사
    protected abstract void Death(); //사망
}
