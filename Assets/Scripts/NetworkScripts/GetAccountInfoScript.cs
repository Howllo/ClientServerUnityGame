using System;
using System.Collections;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using DataStoringIDError;

public class GetAccountInfoScript : MonoBehaviour
{
    //Public Variables
    [Header("Variables")]
    public bool recievedCurrency = false;
    public bool recievedAccountExp = false;
    public bool isServerStillLoading = true;

    //Private Variables
    [Header("Variables")]
    [SerializeField] private int getStamina = 0;
    [SerializeField] private int getEtherCredits = 0;
    [SerializeField] private int getAstralCredits = 0;
    [SerializeField] private RememberMeEncryption rememberMeEncryption;
    [SerializeField] private GameObject sessionExpired;
    [SerializeField] private InventorySystem inventorySystem;

    //Private TextMeshPro
    [Header("TextMesh")]
    [SerializeField] private TextMeshProUGUI agencyResourcesText, agencyResourceRecruitment;
    [SerializeField] private TextMeshProUGUI etherCreditText, etherCreditCreditRecruitment;
    [SerializeField] private TextMeshProUGUI astralCreditText, astralCreditRecruitment;
    [SerializeField] private TextMeshProUGUI playerUsernameText;
    [SerializeField] private TextMeshProUGUI getPlayerLevelText;

    private void Awake()
    {
        if (getPlayerLevelText != null)
            StartCoroutine(WaitForInventoryFinish());
        if(PlayerPrefs.GetString("WhatLoginWasUsed") != "guessLogin")
            GetDecrypedEmail();
        else if(PlayerPrefs.GetString("WhatLoginWasUsed") == "guessLogin")
                CustomGetUserDisplayName();
    }

    private void Update()
    {
        if (recievedCurrency)
        {
            recievedCurrency = false;
            if(inventorySystem != null)
                inventorySystem.PlayerInventory();
        }
        if (recievedAccountExp)
        {
            recievedAccountExp = false;
            GetPlayerAccountLevel();
        }
    }

    private IEnumerator WaitForInventoryFinish()
    {
        yield return new WaitUntil(() => inventorySystem.isInventoryLoadingFinished == true);
        GetPlayerInventory();
        GetPlayerAccountLevel();
    }

    private void GetPlayerInventory()
    {
        try
        {
            getStamina = DataStoring.VirtualCurrency["AR"];
            agencyResourcesText.text = getStamina.ToString() + "/64";
            getEtherCredits = DataStoring.VirtualCurrency["EC"];
            etherCreditText.text = getEtherCredits.ToString();
            getAstralCredits = DataStoring.VirtualCurrency["AC"];
            getAstralCredits += DataStoring.VirtualCurrency["FA"];
            astralCreditText.text = getAstralCredits.ToString();
        } catch (Exception ex) { Debug.Log(ex); }
    }

    private void GetDecrypedEmail()
    {
        StartCoroutine(rememberMeEncryption.SingleDecrpytionOffline(PlayerPrefs.GetString("PlayerEmail"), (callback) =>
        {
            GetPlayerDisplayName(callback.ToString());
        }));
    }

    private void GetPlayerDisplayName(string GetPlayerEmail)
    {
        var request = new GetAccountInfoRequest()
        {
            PlayFabId = PlayerPrefs.GetString("PlayerAccountID"),
            Email = GetPlayerEmail
        };
        PlayFabClientAPI.GetAccountInfo(request, OnAccountInfoResult, OnError);
    }

    private void CustomGetUserDisplayName()
    {
        var request = new GetAccountInfoRequest()
        {
            PlayFabId = PlayerPrefs.GetString("PlayerAccountID")
        };
        PlayFabClientAPI.GetAccountInfo(request, OnAccountInfoResult, OnError);
    }

    private void OnAccountInfoResult(GetAccountInfoResult results)
    {
        string[] str;
        if(playerUsernameText != null)
        {
            if(PlayerPrefs.GetString("WhatLoginWasUsed") != "guessLogin")
            {
                str = results.AccountInfo.TitleInfo.DisplayName.Split('#');
                playerUsernameText.text = str[0];
            } 
            else 
                playerUsernameText.text = results.AccountInfo.TitleInfo.DisplayName;
        }
    }

    private void GetPlayerAccountLevel()
    {
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "GetPlayerAcccountLevel",
            FunctionParameter = PlayerPrefs.GetString("PlayerAccountID"),
            GeneratePlayStreamEvent = true
        }, PlayerLevelResults, OnError);
    }

    private void PlayerLevelResults(ExecuteFunctionResult result)
    {
        try
        {
            getPlayerLevelText.text = "Agency Rank: " + result.FunctionResult.ToString();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        isServerStillLoading = false;
    }

    private void OnError(PlayFabError error)
    {
        if(error.HttpCode == 1100 || error.HttpCode == 1074)
        {
            sessionExpired.SetActive(true);
        }

        Debug.Log(error.ToString());
    }
}