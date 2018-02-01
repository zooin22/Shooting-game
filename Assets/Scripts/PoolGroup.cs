using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pool { BULLET, ENEMY }

public class PoolGroup : MonoBehaviour{
    public static PoolGroup instance;

    public ObjectPool[] objectPool;

    private void Awake()
    {
        instance = this;
    }

    public ObjectPool GetObjectPool(Pool pool)
    {
        switch (pool)
        {
            case Pool.BULLET:
                return objectPool[0];
            case Pool.ENEMY:
                return objectPool[1];
            default:
                return null;                    
        }
    }
}
