using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPool : MonoBehaviour
{
    public GameObject prefab;

    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    public GameObject GetObject()
    {
        GameObject spawnedGameObject;

        if (inactiveInstances.Count > 0)
        {
            spawnedGameObject = inactiveInstances.Pop();
        }
        else
        {
            spawnedGameObject = (GameObject)GameObject.Instantiate(prefab);
            PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
            pooledObject.pool = this;
        }

        spawnedGameObject.SetActive(true);
        return spawnedGameObject;
    }

    public void ReturnObject(GameObject toReturn)
    {
        PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

        if(pooledObject != null && pooledObject.pool == this)
        {
            toReturn.SetActive(false);

            inactiveInstances.Push(toReturn);
        }
        else // error case
        {
            Destroy(toReturn);
        }
    }
}

public class PooledObject : MonoBehaviour
{
    public SimpleObjectPool pool;
}