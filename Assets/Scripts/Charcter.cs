using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Charcter : BaseObject
{
    protected enum State {IDLE,WALK,ROLL,DIE}
    protected enum Direction {LEFT,RIGHT,UP,DOWN,UPLEFT,UPRIGHT,DOWNLEFT,DOWNRIGHT}
    protected float speed;
    protected int hp;
    protected State state;
    protected Vector3 oldPos;

    protected abstract void Move();
    protected abstract void Action();
    protected abstract void Shot();
}
