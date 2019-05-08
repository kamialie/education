using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickExample : MonoBehaviour
{
    void OnMouseDown()
    {
        this.GetComponent<Rigidbody>().AddForce(-transform.forward * 500f);
        this.GetComponent<Rigidbody>().useGravity = true;
    }
}
