using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;
    public Transform target;
    public Sprite sprite;
    ObjectPool objectPool;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnEnemy(Sprite sprite, Vector2 position, int size, int hp, float speed)
    {
        GameObject enemy = objectPool.GetPooledObject();
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemy.transform.position = position;
        enemy.transform.localScale = new Vector2(size, size);
        enemyScript.Init(sprite,target,hp,speed);
        enemy.SetActive(true);

    }

    // Use this for initialization
    void Start () {
        objectPool = PoolGroup.instance.GetObjectPool(Pool.ENEMY);
    }

    // Update is called once per frame
    void Update () {
    }
}
