using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour { // 오브젝트 풀링을 위한 클래스

    public GameObject pooledObject; // 오브젝트
    public int pooledAmount = 20; // 오브젝트 수
    public bool willGrow = true; // 오브젝트 제한 여부

    List<GameObject> pooledObjects;

    // Use this for initialization
    void Awake () {
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
	}
    public GameObject GetPooledObject() // Pool에서 오브젝트 가져오기
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].GetComponent<Property>() == null)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow) // 오브젝트 더 생성.
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    public void FreeObject(GameObject gameObject) // 오브젝트 삭제
    {
        gameObject.SetActive(false);
        DestroyComponent(gameObject);
    }
    public void DestroyComponent(GameObject gameObject)
    {
        if (gameObject.GetComponent<Property>() != null)
            gameObject.GetComponent<Property>().Destroy();
        if(gameObject.GetComponent<LineRenderer>() != null)
            gameObject.GetComponent<LineRenderer>().positionCount = 0;
    }
}
