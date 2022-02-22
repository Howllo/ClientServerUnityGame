using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using DataStoringIDError;
using Newtonsoft.Json;
using Newtonsoft;

public class GachaRollScript : MonoBehaviour
{
    [SerializeField] private InstaniatingUISystem instaniatingUI;


    [SerializeField] private Button rollOneRecruitmentButton, rollTenRecruitmentButton;
    [Tooltip("Enter the table ID from playfab.")]
    [SerializeField] private string WhatBanner = "";
    [SerializeField] private string AgentOrOperator = "";

    private void Start()
    {
        rollOneRecruitmentButton.onClick.AddListener(GachaRollOne);
        rollTenRecruitmentButton.onClick.AddListener(GachaRollTen);
    }

    private void GachaRollOne()
    {
        string PlayerID = PlayerPrefs.GetString("PlayerAccountID");
        int Amount = 1;

        foreach(var items in DataStoring.playerInventory)
        {
            if(items.ItemId == "recruitmentFolder")
            {
                if(items.RemainingUses < Amount)
                {
                    return;
                }
            }
        }

        //Gacha System
        PlayFabCloudScriptAPI.ExecuteFunction(new PlayFab.CloudScriptModels.ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
              Id = PlayFabSettings.staticPlayer.EntityId,
              Type = PlayFabSettings.staticPlayer.EntityType
            },
            FunctionName = "GachaFunction",
            FunctionParameter = new
            {
                WhatBanner,
                PlayerID,
                Amount
            },
            GeneratePlayStreamEvent = true
        }, results =>
        {
            string itemKey = "";
            JsonInformation jsonInfo = new JsonInformation();
            //itemKey = JsonConvert.DeserializeObject(results.FunctionResult.ToString());

            instaniatingUI.GetRewardPopup(itemKey);

            //TODO: Create animation for all different types for walk in through door. 4,5,6 all have special animations.
            //If player pulls a duplicate display items on the right side of the screen that was given during the duplicate.
            //Check the stars amount for each pull.
        }, OnError);
    }

    private void GachaRollTen()
    {
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.ToString());
    }
}

public class JsonInformation
{
    List<ItemInstance> IncomingItems;
}

