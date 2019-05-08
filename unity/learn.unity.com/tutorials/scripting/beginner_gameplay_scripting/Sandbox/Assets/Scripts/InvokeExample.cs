using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeExample : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        //Invoke("SpawnObject", 2);
        InvokeRepeating("SpawnObject", 2, 1);
    }

    void SpawnObject()
    {
        Instantiate(target, new Vector3(5, 0, 0), Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            CancelInvoke();
        }
    }
}
