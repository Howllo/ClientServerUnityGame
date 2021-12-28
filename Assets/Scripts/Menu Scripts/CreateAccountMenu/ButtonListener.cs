using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour
{
    //Private Variables
    [SerializeField] private Button loginSignupButton;
    [SerializeField] private Button guessLoginButton;
    [SerializeField] private Button termsMenuCloseButton;
    [SerializeField] private Button termsMenuCloseButtonGuess;
    [SerializeField] private Button signupMenuCloseButton;
    [SerializeField] private Button confirmedButton;
    private bool onClickClose;
    private string currentStatus;

    //Public Variables
    public bool isGuessClicked;
    public bool isLoginSignupClicked;

    void Start()
    {
        isGuessClicked = false;
        isLoginSignupClicked = false;
        onClickClose = true;
    }

    void Update()
    {
        confirmedButton.onClick.AddListener(onClickConfirm);

        if (isLoginSignupClicked || isGuessClicked)
        {
            termsMenuCloseButton.onClick.AddListener(closeButton);
            signupMenuCloseButton.onClick.AddListener(closeButton);
            termsMenuCloseButtonGuess.onClick.AddListener(closeButton);
        }

        if (onClickClose)
        {
            if (PlayerPrefs.GetString(currentStatus) == "loginNormal")
                loginSignupOnClick();
            if (PlayerPrefs.GetString(currentStatus) == "guessLogin")
                guessAccountOnClick();
        }
    }

    void guessAccountOnClick()
    {
        isGuessClicked = true;
        isLoginSignupClicked = false;
        onClickClose = false;
        return;
    }

    void loginSignupOnClick()
    {
        isLoginSignupClicked = true;
        isGuessClicked = false;
        onClickClose = false;
        return;
    }

    void closeButton()
    {
        onClickClose = true;
        isGuessClicked = false;
        isLoginSignupClicked = false;
    }

    void onClickConfirm()
    {
        isGuessClicked = false;
        isLoginSignupClicked = false;
        onClickClose = true;
    }
}
