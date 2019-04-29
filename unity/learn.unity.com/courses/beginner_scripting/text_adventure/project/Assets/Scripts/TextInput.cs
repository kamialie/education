using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for unity InputField

public class TextInput : MonoBehaviour // take care of user input
{
    public InputField inputField;

    GameController controller;

    private void Awake()
    {
        controller = GetComponent<GameController>();
        inputField.onEndEdit.AddListener(AcceptStringInput); // add a listener to an event
    }

    void AcceptStringInput(string userInput)
    {
        userInput = userInput.ToLower();
        controller.LogStringWithReturn(userInput); // log user's input

        char[] delimiterCharacters = { ' ' }; // split method needs array of delimiters as a parameter
        string[] separatedInputWords = userInput.Split(delimiterCharacters); // separate user input

        for (int i = 0; i < controller.inputActions.Length; i++)
        {
            InputAction inputAction = controller.inputActions[i]; // check first word accoring to game grammar
            if (inputAction.keyWord == separatedInputWords[0]) // if matches
            {
                inputAction.RespondToInput(controller, separatedInputWords);
            }
        }

        InputComplete();
    }

    void InputComplete() // when user is done with input
    {
        controller.DisplayLoggedText(); // display logs
        inputField.ActivateInputField(); // return focus to input field, because its going away after you press return key
        inputField.text = null; // empty input field
    }
}
