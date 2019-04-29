using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TextAdventure/Room")] // create blueprint in a sub menu i n Create 
public class Room : ScriptableObject
{
    [TextArea] // make input area bigger for the next variable
    public string describtion;
    public string roomName;
    public Exit[] exits; // array of exits
    public InteractibleObject[] interactibleObjectsInRoom; // array of objects in a room

}
