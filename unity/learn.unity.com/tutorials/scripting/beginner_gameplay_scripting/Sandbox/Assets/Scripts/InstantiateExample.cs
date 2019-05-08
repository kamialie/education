using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateExample : MonoBehaviour
{
    //public Rigidbody cube;

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Instantiate(cube);
    //    }
    //}

    public Rigidbody cube;
    public Transform sphere;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Rigidbody cubeInstance;
            cubeInstance = Instantiate(cube, sphere.position + new Vector3(0, 0, 2), sphere.rotation) as Rigidbody;
            cubeInstance.AddForce(sphere.forward * 1000);
        }
    }
}
