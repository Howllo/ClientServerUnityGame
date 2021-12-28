using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProfanityFilter;
using PlayFab;
using PlayFab.ClientModels;

public class AccountSystem : MonoBehaviour
{
    //Private Variables
    public bool IsLoggedIn;

    //Register Variables
    [Header("Register")]
    [SerializeField] private GameObject signUpPopup;
    [SerializeField] private GameObject registerEmailInputGO;
    [SerializeField] private GameObject registerPasswordInputGO;
    [SerializeField] private TMP_InputField registerEmailInputField;
    [SerializeField] private TMP_InputField registerPasswordInputField;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button registerCloseButton;

    [Header("Login")]
    [SerializeField] private TMP_InputField loginEmailInputField;
    [SerializeField] private TMP_InputField loginPasswordInputField;
    [SerializeField] private Button forgotPasswordButton;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private Button loginButton;
    [SerializeField] private GameObject loginPopup;
    [SerializeField] private TextMeshProUGUI getAccountID;
    [SerializeField] private GameObject getAccountIDGO;
    [SerializeField] private Button loginCloseButton;
    [SerializeField] private Button confirmTosGuessButton;

    [Header("AccountInfo")]
    public string getUsername;
    public string accountInfoID;
    public string currentSessionTicket;
    public string currentGUID;

    //Username Variables
    [Header("Username")]
    [SerializeField] private GameObject usernamePopup;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private string registerUsernameInput;
    [SerializeField] private Button userNameButton;
    [SerializeField] private Button touchToStart;
    [SerializeField] private Button pressAnyKey;
    [SerializeField] private Button pressAnyButton;

    [Header("Guess Login")]
    [SerializeField] private Button guessLoginButton;

    [Header("ServerSelection")]
    [SerializeField] private Button globalServerButton;
    [SerializeField] private Button europeServerButton;
    [SerializeField] private Button asiaServerButton;
    [SerializeField] private Button koreaServerButton;
    [SerializeField] private GameObject serverSelectionPopup;

    [Header("ForgotPassword")]
    [SerializeField] private TMP_InputField resetPasswordInputField;
    [SerializeField] private string resetPasswordInput;
    [SerializeField] private Button resetPasswordButton;
    [SerializeField] private GameObject forgotPasswordPopup;
    [SerializeField] private GameObject resettingPasswordPopup;

    [Header("Logout")]
    [SerializeField] private GameObject loginButtons;
    [SerializeField] private GameObject topRightCanvas;
    [SerializeField] private Button logOutButton;

    //Error Variables
    [Header("Error Variables")]
    [SerializeField] private GameObject errorPopup;
    [SerializeField] private GameObject signupErrorGO;
    [SerializeField] private GameObject tooManyTextErrorPassowrdGameObject;
    [SerializeField] private GameObject tooLittleTextErrorPasswordGO;
    [SerializeField] private GameObject badEmailType;
    [SerializeField] private TextMeshProUGUI signupError;

    //Other Game Objects
    [Header("Other Game Objects")]
    [SerializeField] private GameObject afterSlashScreenCanvas;
    [SerializeField] private GameObject dimmerCanvas;
    [SerializeField] private GameObject menuLoginButtons;
    [SerializeField] private GameObject menuTouchToStart;
    [SerializeField] private GameObject menuPressAnykey;
    [SerializeField] private GameObject menuPressAnyButton;
    [SerializeField] private Toggle showPasswordToggle;

    [Header("RememberMe")]
    [SerializeField] private RememberMeEncryption rememberMeEncryption;
    private bool RememberMeBool = false;
    private bool hasRan = false;
    private bool isNewPlayer = false; 

    private void Start()
    {
        //Auto-Login - This is a remember me system.
        if (PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.GetString("RememberMe") == "true" && !IsLoggedIn)
        {
            StartCoroutine(GetTotalTime());
            if (!IsLoggedIn)
                StartCoroutine(rememberMeEncryption.GetRememberLogin());
            else
                rememberMeEncryption.DecrptionOfAccount(PlayerPrefs.GetString("PlayerEmail"), PlayerPrefs.GetString("PlayerPass"));
        }

        //Auto-Login - This is for guess accounts.
        if(PlayerPrefs.GetString("WhatLoginWasUsed") == "guessLogin")
            GuessLogin64BitRememberMe();

        //Button Listeners
        registerButton.onClick.AddListener(RegisterNewAccountRequest);
        userNameButton.onClick.AddListener(DisplayNameRegisterRequest);
        loginButton.onClick.AddListener(LoginGameRequest);
        resetPasswordButton.onClick.AddListener(RequestResetPassword);
        logOutButton.onClick.AddListener(LogoutOfGame);
        touchToStart.onClick.AddListener(SaveLoginInformation);
        pressAnyKey.onClick.AddListener(SaveLoginInformation);
        pressAnyButton.onClick.AddListener(SaveLoginInformation);
        loginCloseButton.onClick.AddListener(ResetLoginFields);
        registerCloseButton.onClick.AddListener(ResetRegisterFields);
        //globalServerButton.onClick.addListener();
        //europeServerButton.onClick.AddListener();
        //asiaServerButton.onClick.AddListener();
        //koreaServerButton.onClick.AddListener();

#if UNITY_64
        confirmTosGuessButton.onClick.AddListener(GuessLoginRequest64Bit);
#endif
#if UNITY_IOS
        confirmTosGuessButton.onClick.AddListener(GuessLoginRequestIOSDevice);
#endif
#if UNITY_ANDRIOD
        confirmTosGuessButton.onClick.AddListener(GuessLoginRequestAndriod);
#endif
#if UNITY_XBOXONE
        confirmTosGuessButton.onClick.AddListener(GuessLoginRequestXboxOne);
#endif
#if UNITY_PS5
        confirmTosGuessButton.onClick.AddListener(GuessLoginRequestPSN);
#endif
#if UNITY_SWITCH
        confirmTosGuessButton.onClick.AddListener(GuessLoginRequestSwitch);
#endif
    }

    private void Update()
    {
        if (IsLoggedIn && !hasRan)
        {
            rememberMeEncryption.GetPlayerTitleID();
            hasRan = true;
        }
    }

    //If the account does not connect to the server, unhide login.
    private IEnumerator GetTotalTime()
    {
        dimmerCanvas.SetActive(false);
        loginButtons.SetActive(false);
        loginButtons.SetActive(false);
        yield return new WaitForSeconds(6f);
        if (!IsLoggedIn)
            loginButtons.SetActive(true);
    }

    /// <summary>
    /// If remember is checked encrypt the information.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SaveEncryptedAccount()
    {
        yield return new WaitUntil(() => IsLoggedIn);
        rememberMeEncryption.EncrptAccount(loginEmailInputField.text, loginPasswordInputField.text);
    }

    private void RegisterNewAccountRequest()
    {
        bool doesEmailHaveAt = false;
        bool doesEmailHavePeriod = false;

        //Information Checks
        if(registerPasswordInputField.text.Length <= 5)
        {
            errorPopup.SetActive(true);
            tooLittleTextErrorPasswordGO.SetActive(true);
            return;
        } 

        if (registerPasswordInputField.text.Length > 100)
        {
            errorPopup.SetActive(true);
            tooManyTextErrorPassowrdGameObject.SetActive(true);
            return;
        }

        //Email Check
        for(int i = 0; i < registerEmailInputField.text.Length; i++)
        {
            char c = registerEmailInputField.text[i];
            if (c == '@')
                doesEmailHaveAt = true;
            if (doesEmailHaveAt)
                if (c == '.')
                    doesEmailHavePeriod = true;
        }

        if (!doesEmailHaveAt || !doesEmailHavePeriod)
        {
            errorPopup.SetActive(true);
            badEmailType.SetActive(true);
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = registerEmailInputField.text,
            Password = registerPasswordInputField.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SaveRegisterInformation();
        PlayerPrefs.SetString("WhatLoginWasUsed", "register");
        currentSessionTicket = result.SessionTicket;
        getAccountID.text = "ID:" + result.PlayFabId.ToString();
        getAccountIDGO.SetActive(true);
        IsLoggedIn = true;
        usernamePopup.SetActive(true);
        signUpPopup.SetActive(false);
    }

    public void DisplayNameRegisterRequest()
    {
        string str = "";
        string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_, ";
        string randomChar = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        Profanity profanity = new Profanity();

        //Check the for special characters.
        for (int i = 0; i < usernameInputField.text.Length; i++)
            for (int k = 0; k < specialChar.Length; k++)
                if (usernameInputField.text[i] == specialChar[k])
                    return;

        if (PlayerPrefs.GetString("WhatLoginWasUsed") == "guessLogin")
        {
            int rnd = Random.Range(0, 1);
            if (rnd == 0)
                str = "iderror#";
            else if (rnd == 1)
                str = "iderrortd#";
        }
        else if (PlayerPrefs.GetString("WhatLoginWasUsed") == "register")
            str = usernameInputField.text + "#";

        for (int i = 0; i < Random.Range(4,6); i++)
            str += randomChar[Random.Range(0, 46)];

        //Check the profanity list.
        for (int i = 0; i < profanity.filterList.Length; i++)
            if (usernameInputField.text.ToLower().Contains(profanity.filterList[i]))
                return;

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = str
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUserSuccess, OnError);
    }

    private void OnUserSuccess(UpdateUserTitleDisplayNameResult results)
    {
        topRightCanvas.SetActive(true);
        usernamePopup.SetActive(false);
        dimmerCanvas.SetActive(false);
        loginButtons.SetActive(false);
#if UNITY_EDITOR
        menuPressAnykey.SetActive(true);
#endif
#if UNITY_IOS
        menuTouchToStart.SetActive(true);
#endif
#if UNITY_ANDRIOD
        menuTouchToStart.SetActive(true);
#endif
#if UNITY_64
        menuPressAnykey.SetActive(true);
#endif
#if UNITY_XBOXONE
        menuPressAnyButton.SetActive(true);
#endif
#if UNITY_PS5
        menuPressAnyButton.SetActive(true);
#endif
#if UNITY_SWITCH
        menuPressAnyButton.SetActive(true);
#endif
    }

#region Login Into Game
private void LoginGameRequest()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = loginEmailInputField.text,
            Password = loginPasswordInputField.text,
            TitleId = "7ED66"
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }
    
    //Remember Me Login System
    public void LoginThroughRememberMe(string emailRME, string passRME)
    {
        var request = new LoginWithEmailAddressRequest()
        {
            Email = emailRME,
            Password = passRME,
            TitleId = "7ED66",
        };
        RememberMeBool = true;
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

#if UNITY_64
    private void GuessLoginRequest64Bit()
    {
        Guid getNewGUID = Guid.NewGuid();
        currentGUID = getNewGUID.ToString();
        var request = new LoginWithCustomIDRequest
        {
            TitleId = "7ED66",
            CreateAccount = true,
            CustomId = getNewGUID.ToString(),
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginGuesssSuccess, OnError);
        isNewPlayer = true;
    }

    private void GuessLogin64BitRememberMe()
    {
        string getGUID = PlayerPrefs.GetString("PlayerGUID");
        var request = new LoginWithCustomIDRequest
        {
            TitleId = "7ED66",
            CreateAccount = false,
            CustomId = getGUID.ToString()
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginGuesssSuccess, OnError);
    }
#endif

#if UNITY_IOS
    private void GuessLoginRequestIOSDevice()
    {
        var request = new LoginWithIOSDeviceIDRequest
        {
            TitleId = "7ED66",
            CreateAccount = true,
            DeviceId = UIDevice.CurrentDevice.IdentifierForVendor.ToString(),
            OS = "IOSDevice"
        };
        PlayFabClientAPI.LoginWithIOSDeviceID(request, OnLoginGuesssSuccess, OnError);
        isNewPlayer = true;
    }

    private void GuessLoginRememberMeIOSDevice()
    {
        var request = new LoginWithIOSDeviceIDRequest
        {
            TitleId = "7ED66",
            CreateAccount = false,
            DeviceId = UIDevice.CurrentDevice.IdentifierForVendor.ToString(),
            OS = "IOSDevice"
        };
        PlayFabClientAPI.LoginWithIOSDeviceID(request, OnLoginGuesssSuccess, OnError);
    }
#endif

#if UNITY_ANDRIOD
    private void GuessLoginRequestAndriod()
    {
        var request = new LoginWithAndroidDeviceIDRequest
        {
            TitleId = "7ED66",
            CreateAccount = true,
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier.ToString(),
            OS = "AndroidDevice"
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginGuesssSuccess, OnError);
        isNewPlayer = true;
    }

    private void GuessLoginRememberMeAndriod()
    {
        var request = new LoginWithAndroidDeviceIDRequest
        {
            TitleId = "7ED66",
            CreateAccount = false,
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier.ToString(),
            OS = "AndroidDevice"
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginGuesssSuccess, OnError);
    }
#endif

#if UNITY_XBOXONE //Fix this
    private void GuessLoginRequestXboxOne()
    {
        var request = new LoginWithXboxRequest
        {
            TitleId = "7ED66",
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithXbox(request, OnLoginGuesssSuccess, OnError);
        isNewPlayer = true;
    }

    private void GuessLoginRememberMeXboxOne()
    {
        var request = new LoginWithXboxRequest
        {
            TitleId = "7ED66",
            CreateAccount = false,
        };
        PlayFabClientAPI.LoginWithXbox(request, OnLoginGuesssSuccess, OnError);
    }
#endif

#if UNITY_PS5 //Fix This
    private void GuessLoginRequestPSN()
    {
        var request = new LoginWithPSNRequest
        {
            TitleId = "7ED66",
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithPSN(request, OnLoginGuesssSuccess, OnError);
        isNewPlayer = true;
    }

    private void GuessLoginRememberMePSN()
    {
        var request = new LoginWithPSNRequest
        {
            TitleId = "7ED66",
            CreateAccount = false,
        };
        PlayFabClientAPI.LoginWithPSN(request, OnLoginGuesssSuccess, OnError);
    }
#endif

#if UNITY_SWITCH //Fix this
    private void GuessLoginRequestSwitch(){
            var request = new LoginWithNintendoSwitchDeviceIdRequest
        {
            TitleId = "7ED66",
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithNintendoSwitchDeviceId(request, OnLoginGuesssSuccess, OnError);
        isNewPlayer = true;
    }

        private void GuessLoginRememberMeSwitch(){
            var request = new LoginWithNintendoSwitchDeviceIdRequest
        {
            TitleId = "7ED66",
            CreateAccount = false,
        };
        PlayFabClientAPI.LoginWithNintendoSwitchDeviceId(request, OnLoginGuesssSuccess, OnError);
    }
#endif

    private void OnLoginSuccess(LoginResult result)
    {
        PlayerPrefs.SetString("WhatLoginWasUsed", "loginNormal");
        currentSessionTicket = result.SessionTicket;
        getAccountID.text = "ID: " + result.PlayFabId.ToString();
        accountInfoID = result.PlayFabId.ToString();
        getAccountIDGO.SetActive(true);
        IsLoggedIn = true;
        topRightCanvas.SetActive(true);
        loginPopup.SetActive(false);
        dimmerCanvas.SetActive(false);
        loginButtons.SetActive(false);

#if UNITY_EDITOR
        menuPressAnykey.SetActive(true);
#endif
#if UNITY_IOS
        menuTouchToStart.SetActive(true);
#endif
#if UNITY_ANDRIOD
        menuTouchToStart.SetActive(true);
#endif
#if UNITY_64
        menuPressAnykey.SetActive(true);
#endif
#if UNITY_XBOXONE
        menuPressAnyButton.SetActive(true);
#endif
#if UNITY_PS5
        menuPressAnyButton.SetActive(true);
#endif
#if UNITY_SWITCH
        menuPressAnyButton.SetActive(true);
#endif
        SaveLoginInformation();
    }

    private void OnLoginGuesssSuccess(LoginResult result)
    {
        PlayerPrefs.SetString("WhatLoginWasUsed", "guessLogin");
        if(isNewPlayer)
            DisplayNameRegisterRequest();
        currentSessionTicket = result.SessionTicket;
        getAccountID.text = "ID:" + result.PlayFabId.ToString();
        accountInfoID = result.PlayFabId.ToString();
        getAccountIDGO.SetActive(true);
        IsLoggedIn = true;
        topRightCanvas.SetActive(true);
        loginPopup.SetActive(false);
        loginButtons.SetActive(false);
        SaveLoginGuessInformation();
    }
#endregion

    private void RequestResetPassword()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = resetPasswordInput,
            TitleId = "7ED66",
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnResetSendSuccess, OnError);
    }

    private void OnResetSendSuccess(SendAccountRecoveryEmailResult results)
    {
        forgotPasswordPopup.SetActive(false);
        resettingPasswordPopup.SetActive(true);
    }

    private void ResetLoginFields()
    {
        loginEmailInputField.text = "";
        loginPasswordInputField.text = "";
        rememberMeToggle.isOn = false;
        showPasswordToggle.isOn = false;
    }

    private void ResetRegisterFields()
    {
        registerEmailInputField.text = "";
        registerPasswordInputField.text = "";
        rememberMeToggle.isOn = false;
        showPasswordToggle.isOn = false;
    }

    /// <summary>
    /// Logout of the game and remove all credentials. 
    /// </summary>
    public void LogoutOfGame()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteKey("PlayerEmail");
        PlayerPrefs.DeleteKey("PlayerPass");
        PlayerPrefs.DeleteKey("PlayerAccountID");
        PlayerPrefs.DeleteKey("PlayerSessionTicket");
        PlayerPrefs.DeleteKey("GetGUID");
        PlayerPrefs.DeleteKey("Key");
        PlayerPrefs.DeleteKey("RememberMe");
        IsLoggedIn = false;
        getAccountIDGO.SetActive(false);
        topRightCanvas.SetActive(false);
        loginButtons.SetActive(true);
        menuPressAnyButton.SetActive(false);
        menuPressAnykey.SetActive(false);
        menuTouchToStart.SetActive(false);
        ResetLoginFields();
    }

    private void SaveLoginInformation()
    {
        PlayerPrefs.SetString("PlayerAccountID", accountInfoID);
        PlayerPrefs.SetString("PlayerSessionTicket", currentSessionTicket);
        if (rememberMeToggle.isOn && !RememberMeBool)
        {
            StartCoroutine(SaveEncryptedAccount());
            PlayerPrefs.SetString("RememberMe", "true");
        }
        else if (!RememberMeBool && PlayerPrefs.GetString("WhatLoginWasUsed") != "guessLogin")
        {
            StartCoroutine(rememberMeEncryption.EncrptSingleInfo(loginEmailInputField.text, (callback) =>
            {
                PlayerPrefs.SetString("PlayerEmail", callback);
            }, true));
        }
        PlayerPrefs.Save();
    }

    private void SaveLoginGuessInformation()
    {
        PlayerPrefs.SetString("PlayerSessionTicket", currentSessionTicket);
        if (isNewPlayer)
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.SetFloat("soundVolume", 1);
            PlayerPrefs.SetFloat("UIVolume", 1);
            PlayerPrefs.SetFloat("voiceoverVolume", 1);
            PlayerPrefs.SetString("PlayerAccountID", accountInfoID);

#if UNITY_64
            PlayerPrefs.SetString("PlayerGUID", currentGUID);
#endif
        }
        PlayerPrefs.Save();

#if UNITY_EDITOR
        menuPressAnykey.SetActive(true);
#endif
#if UNITY_IOS
        menuTouchToStart.SetActive(true);
#endif
#if UNITY_ANDRIOD
        menuTouchToStart.SetActive(true);
#endif
#if UNITY_64
        menuPressAnykey.SetActive(true);
#endif
#if UNITY_XBOXONE
        menuPressAnyButton.SetActive(true);
#endif
#if UNITY_PS5
        menuPressAnyButton.SetActive(true);
#endif
#if UNITY_SWITCH
        menuPressAnyButton.SetActive(true);
#endif
    }

    //Not Finished.
    public void SaveRegisterInformation()
    {
        PlayerPrefs.SetFloat("musicVolume", 1);
        PlayerPrefs.SetFloat("soundVolume", 1);
        PlayerPrefs.SetFloat("UIVolume", 1);
        PlayerPrefs.SetFloat("voiceoverVolume", 1);
        PlayerPrefs.SetString("PlayerAccountID", accountInfoID);
        PlayerPrefs.SetString("PlayerSessionTicket", currentSessionTicket);
        rememberMeEncryption.EncrptSingleInfo(registerEmailInputField.text, (callback) =>
        {
            PlayerPrefs.SetString("PlayerEmail", callback);
        });
        PlayerPrefs.Save();
    }

    private void OnError(PlayFabError error)
    {
        string errorText = error.ToString();
        errorText = errorText.Substring(25);

        errorPopup.SetActive(true);
        signupError.text = errorText;
        signupErrorGO.SetActive(true);
        guessLoginButton.interactable = true; 

        Debug.Log(error.GenerateErrorReport());
    }
}