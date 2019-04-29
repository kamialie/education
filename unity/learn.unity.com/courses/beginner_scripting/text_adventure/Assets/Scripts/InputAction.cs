using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputAction : ScriptableObject
{
    public string keyWord; // input to respond to

    public abstract void RespondToInput(GameController controller, string[] separatedInputWords);
}
