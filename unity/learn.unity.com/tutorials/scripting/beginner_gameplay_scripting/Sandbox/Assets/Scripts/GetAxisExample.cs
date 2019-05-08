using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAxisExample : MonoBehaviour
{
    public float range;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float xPos = h * range;
        float yPos = v * range;

        transform.position = new Vector3(xPos, yPos, 0);
    }
}
