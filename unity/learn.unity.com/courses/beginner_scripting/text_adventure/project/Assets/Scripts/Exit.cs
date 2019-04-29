using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // dont need to inherit from MonoBehavior
public class Exit
{
    public string keyString; // keyword we are looking for
    public string exitDescription; // its description
    public Room valueRoom;
}
