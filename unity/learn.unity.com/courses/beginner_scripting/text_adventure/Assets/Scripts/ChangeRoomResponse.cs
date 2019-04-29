using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TextAdventure/ActionResponse/ChangeRoom")]
public class ChangeRoomResponse : ActionResponse
{
    public Room roomToChange;

    public override bool DoActionResponse(GameController controller)
    {
        if (controller.roomNavigation.currentRoom.roomName == requiredString) // check if we are in the right room
        {
            controller.roomNavigation.currentRoom = roomToChange; // change room
            controller.DisplayRoomText(); // display what happened
            return true; // succeed
        }
        return false;
    }
}
