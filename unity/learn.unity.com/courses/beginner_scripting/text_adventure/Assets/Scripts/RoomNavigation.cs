using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{
    public Room currentRoom;

    Dictionary<string, Room> exitDictionary = new Dictionary<string, Room>(); // dictionary to match command with exits

    GameController controller;

    void Awake()
    {
        controller = GetComponent<GameController>(); // get refenece to GameController in a current object
    }

    public void UnpackExitsInRoom() // add exits to controllers list of possible actions
    {
        for (int i = 0; i < currentRoom.exits.Length; i++)
        {
            exitDictionary.Add(currentRoom.exits[i].keyString, currentRoom.exits[i].valueRoom); // create key-value pair
            controller.interactionDescriptionInRoom.Add(currentRoom.exits[i].exitDescription); // loop through and add describtions
        }
    }

    public void AttemptToChangeRooms(string directionNoun) // check input command
    {
        if (exitDictionary.ContainsKey(directionNoun)) // if it matches change current room
        {
            currentRoom = exitDictionary[directionNoun];
            controller.LogStringWithReturn("You head off to the " + directionNoun); // tell user about his action
            controller.DisplayRoomText(); // display new room description
        }
        else
        {
            controller.LogStringWithReturn("There is no path to the " + directionNoun); // on nonvalid input
        }
    }

    public void ClearExits()
    {
        exitDictionary.Clear();
    }
}
