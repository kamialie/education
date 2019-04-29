using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Interaction
{
    public InputAction inputAction;
    [TextArea]
    public string textRespond; // respond given to the player when he applies action to an item
    public ActionResponse actionResponse;
}
