using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    ObjectPool objectPool;

	// Use this for initialization
	void Start () {
        objectPool = PoolGroup.instance.GetObjectPool(Pool.ENEMY);
        for(int i = 0; i < 10; i++)
        {
            GameObject enemy = objectPool.GetPooledObject();
            enemy.transform.position = new Vector3(i*1, 0,0);
            enemy.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update () {
    }
}
