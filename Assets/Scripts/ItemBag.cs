using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemBag
{

}

public abstract class CollisionItem
{
    public abstract void Collision();
}

public class UpdateItem
{

}


public class Homing : UpdateItem
{

}


public class Fire : CollisionItem
{
    public override void Collision() {
    }
}

public class Water : CollisionItem
{
    public override void Collision()
    {
    }
}

public class Faint : CollisionItem
{
    public override void Collision()
    {
    }
}