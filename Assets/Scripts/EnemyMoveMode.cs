using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMoveMode
{

    public abstract void Move(Enemy enemy, Transform transform, Transform target, float speed);
    public abstract void Stop(Enemy enemy);
}

public class SimpleMoveMode : EnemyMoveMode
{
    public override void Move(Enemy enemy, Transform transform, Transform target, float speed)
    {
        enemy.GetComponent<Unit>().UpdatedPath();
        //transform.Translate((target.position - transform.position).normalized * Time.deltaTime * speed);
    }
    public override void Stop(Enemy enemy)
    {
        enemy.GetComponent<Unit>().StopMove();
    }
}


public class StopMoveMode : EnemyMoveMode
{
    public override void Move(Enemy enemy, Transform transform, Transform target, float speed)
    {
        transform.Translate((target.position - transform.position).normalized * Time.deltaTime * speed);
    }
    public override void Stop(Enemy enemy)
    {
    }
}

