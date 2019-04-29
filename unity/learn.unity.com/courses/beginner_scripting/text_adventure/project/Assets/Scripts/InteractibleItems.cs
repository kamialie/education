using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleItems : MonoBehaviour
{
    public List<InteractibleObject> usableItemList;
    public Dictionary<string, string> examineDictionary = new Dictionary<string, string>(); // not need to hide, as unity doesnt serialize dictionaries
    public Dictionary<string, string> takeDictionary = new Dictionary<string, string>();

    [HideInInspector] public List<string> nounsInRoom = new List<string>(); // dynamically built list of objects in a room

    Dictionary<string, ActionResponse> useDictionary = new Dictionary<string, ActionResponse>();
    List<string> nounsInInventory = new List<string>(); // holder of objects in inventory
    GameController controller;

    private void Awake()
    {
        controller = GetComponent<GameController>();
    }

    public string GetObjectsNotInInventory(Room currentRoom, int i)
    {
        InteractibleObject interactibleInRoom = currentRoom.interactibleObjectsInRoom[i];

        if (!nounsInInventory.Contains(interactibleInRoom.noun)) // if object is not in inventory, we are safe to display it available in a room
        {
            nounsInRoom.Add(interactibleInRoom.noun);
            return interactibleInRoom.description;
        }
        return null;
    }

    public void AddActionResponsesToUseDictionary()
    {
        for (int i = 0; i < nounsInInventory.Count; i++)
        {
            string noun = nounsInInventory[i];

            InteractibleObject interactibleObjectInInventory = GetInteractibleObjectFromUsableList(noun);
            if (interactibleObjectInInventory == null)
                continue;

            for (int j = 0; j < interactibleObjectInInventory.interactions.Length; j++)
            {
                Interaction interaction = interactibleObjectInInventory.interactions[j];

                if (interaction.actionResponse == null)
                    continue;

                if (!useDictionary.ContainsKey(noun))
                {
                    useDictionary.Add(noun, interaction.actionResponse);
                }
            }
        }
    }

    InteractibleObject GetInteractibleObjectFromUsableList(string noun)
    {
        for (int i = 0; i < usableItemList.Count; i++)
        {
            if (usableItemList[i].noun == noun)
            {
                return usableItemList[i];
            }
        }
        return null;
    }

    public void DisplayInventory()
    {
        controller.LogStringWithReturn("You look in your backpack, inside you have: ");
        for (int i = 0; i < nounsInInventory.Count; i++)
        {
            controller.LogStringWithReturn(nounsInInventory[i]);
        }
    }

    public void ClearCollection()
    {
        examineDictionary.Clear();
        takeDictionary.Clear();
        nounsInRoom.Clear();
    }

    public Dictionary<string, string> Take (string[] separatedInputWords)
    {
        string noun = separatedInputWords[1];

        if (nounsInRoom.Contains(noun)) // if input object is in the room
        {
            nounsInInventory.Add(noun); // add to inventory
            AddActionResponsesToUseDictionary(); // rebuild action dictionary
            nounsInRoom.Remove(noun); // remove from room
            return takeDictionary;
        }
        else
        {
            controller.LogStringWithReturn("There is no " + noun + " here to take.");
            return null;
        }
    }

    public void UseItem(string[] separatedInputWords)
    {
        string nounToUse = separatedInputWords[1];

        if (nounsInInventory.Contains(nounToUse)) // check if its in inventory
        {
            if (useDictionary.ContainsKey(nounToUse)) // check if its in action dictionary
            {
                bool actionResult = useDictionary[nounToUse].DoActionResponse(controller);
                if (!actionResult) // check available response
                {
                    controller.LogStringWithReturn("Hmm. Nothing happens.");
                }
            }
            else // if not in action dictionary
            {
                controller.LogStringWithReturn("You can't use the " + nounToUse);
            }
        } // if not in inventory
        else
        {
            controller.LogStringWithReturn("There is no " + nounToUse + " in your inventory to use");
        }
    }
}
