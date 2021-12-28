using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ButtonDisable : MonoBehaviour
{
    //Private Variables
    [SerializeField] private Button confirmButtonPopup;
    [SerializeField] private Button confirmButtonGuess;
    [SerializeField] private ButtonListener buttonListener;
    [SerializeField] private CheckToggle toggleLogin;
    [SerializeField] private CheckToggle toggleGuess;
    private bool isGuessClicked;
    private bool loginSignupButtonBool;

    public bool buttonLoginEnable = false;
    public bool buttonGuessEnable = false;

    void Start()
    {
        isGuessClicked = buttonListener.isGuessClicked;
        loginSignupButtonBool = buttonListener.isLoginSignupClicked;
    }

    void Update()
    {
        buttonLoginEnable = toggleLogin.isToggle;
        buttonGuessEnable = toggleGuess.isToggle;

        //Enable/Disable Button Login Signup
        if (buttonLoginEnable)
            confirmButtonPopup.interactable = true;
        else if (!buttonLoginEnable)
            confirmButtonPopup.interactable = false;

        //Enable/Disable Button Guess Signup
        if (buttonGuessEnable)
            confirmButtonGuess.interactable = true;
        else if (!buttonGuessEnable)
            confirmButtonGuess.interactable = false;
    }
}
