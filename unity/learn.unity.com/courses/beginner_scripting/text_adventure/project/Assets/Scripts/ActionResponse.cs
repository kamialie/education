using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionResponse : ScriptableObject
{
    public string requiredString; // check if something is possible

    public abstract bool DoActionResponse(GameController controller); // better to pass any scene references to function in abstract class
}
