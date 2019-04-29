using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // add for Text variable

public class GameController : MonoBehaviour
{
    public Text displayedText;
    public InputAction[] inputActions; // array to compate input to

    [HideInInspector] public RoomNavigation roomNavigation; // make public so that other scripts can access it, but hide in Inspector
    [HideInInspector] public List<string> interactionDescriptionInRoom = new List<string>(); // List of all possible actions in a room
    [HideInInspector] public InteractibleItems interactibleItems;

    List<string> actionLog = new List<string>(); // list to gather all data to be displayed on the screen
    
     void Awake()
    {
        interactibleItems = GetComponent<InteractibleItems>(); // get reference to items
        roomNavigation = GetComponent<RoomNavigation>(); // initiate variable
    }

    private void Start() // start all processes
    {
        DisplayRoomText(); // room describtion first
        DisplayLoggedText(); // logs after
    }

    public void DisplayLoggedText() // function to display logs
    {

        string logAsText = string.Join("\n", actionLog.ToArray()); // join list member together

        displayedText.text = logAsText; // display on the screen by passing value to Text
    }

    public void DisplayRoomText()
    {
        ClearCollectionForNewRoom(); // clear everything before unpacking current room

        UnpackRoom(); // call to gather all exit from the current room

        string joinedInteractionDescriptions = string.Join("\n", interactionDescriptionInRoom.ToArray());

        string combineText = roomNavigation.currentRoom.describtion + "\n" + joinedInteractionDescriptions;

        LogStringWithReturn(combineText); // add describtion to log list
    }

    void UnpackRoom()
    {
        roomNavigation.UnpackExitsInRoom();
        PrepareObjectToTakeOrExamin(roomNavigation.currentRoom);
    }

    void PrepareObjectToTakeOrExamin(Room currentRoom)
    {
        for (int i = 0; i < currentRoom.interactibleObjectsInRoom.Length; i++)
        {
            string desctiptionNotInInventory = interactibleItems.GetObjectsNotInInventory(currentRoom, i);
            if (desctiptionNotInInventory != null) // check if item returned is not in inventory
            {
                interactionDescriptionInRoom.Add(desctiptionNotInInventory); // add to describtion list
            }

            InteractibleObject interactibleInRoom = currentRoom.interactibleObjectsInRoom[i];

            for (int j = 0; j < interactibleInRoom.interactions.Length; j++) // loop through interactions of objects
            {
                Interaction interaction = interactibleInRoom.interactions[j];
                if (interaction.inputAction.keyWord == "examine")
                {
                    interactibleItems.examineDictionary.Add(interactibleInRoom.noun, interaction.textRespond);
                }

                else if (interaction.inputAction.keyWord == "take")
                {
                    interactibleItems.takeDictionary.Add(interactibleInRoom.noun, interaction.textRespond);
                }
            }
        }
    }

    public string TestVerbDictionaryWithNoun(Dictionary<string, string> verbDictionary, string verb, string noun) // check if action is in verbDictionary (set of possible action)
    {
        if (verbDictionary.ContainsKey(noun))
        {
            return verbDictionary[noun];
        }

        return "You can't " + verb + " " + noun;
    }

    void ClearCollectionForNewRoom()
    {
        interactibleItems.ClearCollection();
        interactionDescriptionInRoom.Clear();
        roomNavigation.ClearExits();
    }

    public void LogStringWithReturn(string stringToAdd) // function to add to log list
    {
        actionLog.Add(stringToAdd + "\n");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
