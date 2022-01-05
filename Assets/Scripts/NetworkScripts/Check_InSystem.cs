using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using DataStoringIDError;
using TitleDateInfo;

public class Check_InSystem : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GetAccountInfoScript getAccountInfoScript;
    [SerializeField] private ReceivedItemScript receivedItemScript;

    [Header("What Type of Popup")]
    public string PlayerTrackerName = "";
    public string TitleDataTrackerName = "";
    public string WeeklyOrMonthly = "";

    [Header("Popup Info")]
    public GameObject checkinPopup;
    public GameObject informationPopup;
    public GameObject InfoPackagePopup;
    public string[] Catagory = { "Items", "Characters" };
    [SerializeField] private Button getReceivedButton;
    [SerializeField] private Image displayImage, displayImage_BundleOne, displayImage_BundleTwo;
    [SerializeField] private TextMeshProUGUI itemInformation, itemCount, itemName;
    [SerializeField] private TextMeshProUGUI itemInformation_BundleOne, itemCount_BundleOne, itemName_BundleOne;
    [SerializeField] private TextMeshProUGUI itemInformation_BundleTwo, itemCount_BundleTwo, itemName_BundleTwo;
    [SerializeField] private List<TextMeshProUGUI> textInfo = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> buttonImages = new List<Image>();
    [SerializeField] private List<Button> getItemInfoButton = new List<Button>();
    private Dictionary<CatalogItem, int> bundleHolder = new Dictionary<CatalogItem, int>();
    private string GetTitleJsonData = "";
    private List<string> tempList = new List<string>();
    private DateTime AfterDate;
    private int p = 0;


    private void Awake()
    {
        inventorySystem.PlayerInventory();

        //Only get CatagoryItems once per day. Reduce the amount of API calls.
        if(!PlayerPrefs.HasKey("NextTime"))
        {
            inventorySystem.GetCatagoryItems();
            PlayerPrefs.SetString("NextTime", DateTime.Now.AddDays(1).ToString());
        } else
        {
            int CheckTime = DateTime.Compare(DateTime.Now, Convert.ToDateTime(PlayerPrefs.GetString("NextTime")));
            if(CheckTime >= 0)
                inventorySystem.GetCatagoryItems();
        }
    }

    private void Start()
    {
        GetTitleDataInformation();
        getReceivedButton.onClick.AddListener(GetPlayerCheckinReward);
        
        for(int i = 0; i < getItemInfoButton.Count; i++)
        {
            getItemInfoButton[i].onClick.AddListener(OnClickInfo);
        }

        //Set all image and count automatically.
        foreach (var text in textInfo)
        {
            tempList.Add(GetCorrectRewardSwitch((uint)p));
            bundleHolder = new Dictionary<CatalogItem, int>(inventorySystem.GetBundleCount(tempList[p]));
            if (bundleHolder.Count > 1)
            {
                foreach (var item in DataStoring.catalogItems)
                {
                    if (item.ItemId.Equals(tempList[p]))
                    {
                        text.text = "1";
                        getItemInfoButton[p].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item.ItemImageUrl);
                        getItemInfoButton[p].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Day " + p + 1;
                        getItemInfoButton[p].transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            } else if (bundleHolder.Count == 1)
            {
                foreach(var item in bundleHolder)
                {
                    text.text = item.Value.ToString();
                    getItemInfoButton[p].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item.Key.ItemImageUrl);
                    getItemInfoButton[p].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Day " + p + 1;
                    getItemInfoButton[p].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            p++;
        }
    }

    public void OnClickInfo()
    {
        string stringNum = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(stringNum);
        uint buttonNumber = Convert.ToUInt32(stringNum);

        foreach (var item in DataStoring.playerInventory)
        {

        }
    }

    private void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayerPrefs.GetString("PlayerAccountID")
        }, results =>
        {
            GetJson getJson = JsonUtility.FromJson<GetJson>(results.Data[PlayerTrackerName].Value.ToString());
            AfterDate = Convert.ToDateTime(getJson.LoginAfter);
            PlayerPrefs.SetString("LoginAfter", AfterDate.ToString());
        }, OnError);
    }

    private void GetTitleDataInformation()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest()
        {
            Keys = new List<string>
            {
                TitleDataTrackerName,
            }
        }, results =>
        {
            GetTitleJsonData = results.Data[TitleDataTrackerName];
        }, OnError);
    }

    //Cloud script | Grants items if after x amount of time.
    private void GetPlayerCheckinReward()
    {
        string PlayerAccountID = PlayerPrefs.GetString("PlayerAccountID");
        getReceivedButton.interactable = false;
        try
        {
            if(PlayerPrefs.HasKey("LoginAfter"))
                AfterDate = Convert.ToDateTime(PlayerPrefs.GetString("LoginAfter"));
        } catch (Exception e)
        {
            Debug.Log(e);
        }

        //Return if the date is time for the next reward to prevent unnecessary API calls.
        if (AfterDate != null)
        {
            int dateResults = DateTime.Compare(DateTime.Now, AfterDate);
            if (dateResults < 0)
            {
                getReceivedButton.interactable = true;
                return;
            }
        }

        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "CheckinFunction",
            FunctionParameter = new { 
                TitleDataTrackerName, 
                PlayerTrackerName, 
                WeeklyOrMonthly, 
                PlayerAccountID 
            },
            GeneratePlayStreamEvent = true
        }, results =>
        {
            GetJson retrieveFromJson = JsonUtility.FromJson<GetJson>(results.FunctionResult.ToString());
            List<string> holderItemCountList = new List<string>();
            AfterDate = Convert.ToDateTime(retrieveFromJson.LoginAfter);
            getReceivedButton.interactable = true;
            StartCoroutine(receivedItemScript.GetRewardPopup(retrieveFromJson.GrantedItem));
            inventorySystem.PlayerInventory();
            getAccountInfoScript.GetPlayerInventory();
            GetPlayerData();
        }, OnError);
    }

    //Get the reward information.
    private string GetCorrectRewardSwitch(uint DailyRewards)
    {
        string reward = "";
        GetTitleData titleData = JsonUtility.FromJson<GetTitleData>(GetTitleJsonData);

        switch (DailyRewards)
        {
            case 0: reward = titleData.Day1.Reward; break;
            case 1: reward = titleData.Day2.Reward; break;
            case 2: reward = titleData.Day3.Reward; break;
            case 3: reward = titleData.Day4.Reward; break;
            case 4: reward = titleData.Day5.Reward; break;
            case 5: reward = titleData.Day6.Reward; break;
            case 6: reward = titleData.Day7.Reward; break;
            case 7: reward = titleData.Day8.Reward; break;
            case 8: reward = titleData.Day9.Reward; break;
            case 9: reward = titleData.Day10.Reward; break;
            case 10: reward = titleData.Day11.Reward; break;
            case 11: reward = titleData.Day12.Reward; break;
            case 12: reward = titleData.Day13.Reward; break;
            case 13: reward = titleData.Day14.Reward; break;
            case 14: reward = titleData.Day15.Reward; break;
            case 15: reward = titleData.Day16.Reward; break;
            case 16: reward = titleData.Day17.Reward; break;
            case 17: reward = titleData.Day18.Reward; break;
            case 18: reward = titleData.Day19.Reward; break;
            case 19: reward = titleData.Day20.Reward; break;
            case 20: reward = titleData.Day21.Reward; break;
            case 21: reward = titleData.Day22.Reward; break;
            case 22: reward = titleData.Day23.Reward; break;
            case 23: reward = titleData.Day24.Reward; break;
            case 24: reward = titleData.Day25.Reward; break;
            case 25: reward = titleData.Day26.Reward; break;
            case 26: reward = titleData.Day27.Reward; break;
            case 27: reward = titleData.Day28.Reward; break;
            case 28: reward = titleData.Day29.Reward; break;
            case 29: reward = titleData.Day30.Reward; break;
            case 30: reward = titleData.Day31.Reward; break;
        }

        return reward;
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error);
    }
}

[Serializable]
public class GetJson
{
    public uint HighestStreak;
    public string LoginAfter;
    public string GrantedItem;
}





////Display the Amount in Inventory - Single Item
//foreach (var items in DataStoring.catalogItems)
//{

//    holderString = GetCorrectRewardSwitch(buttonNumber);
//    if (items.ItemId == holderString
//            {
//        //Doubles
//        if (items.Bundle.BundledItems[0] != items.Bundle.BundledItems[items.Bundle.BundledItems.Count - 1])
//        {
//            itemList.Clear();
//            //Get Item Amount
//            for (int i = 0; i < items.Bundle.BundledItems.Count; i++)
//            {
//                if (items.Bundle.BundledItems.Contains(items.Bundle.BundledItems[0]))
//                    j++;
//                else if (items.Bundle.BundledItems.Contains(items.Bundle.BundledItems[items.Bundle.BundledItems.Count - 1]))
//                    c++;
//            }
//            itemList.Add(items.Bundle.BundledItems[0], j);
//            itemList.Add(items.Bundle.BundledItems[items.Bundle.BundledItems.Count - 1], c);

//            foreach (var item in DataStoring.playerInventory)
//            {
//                if (item.Key.ItemId.ToString() == items.ItemId)
//                {
//                    itemInformation_BundleOne.text = items.Description;
//                    displayImage_BundleOne.sprite = Resources.Load<Sprite>(items.ItemImageUrl);
//                    itemCount_BundleOne.text = item.Key.RemainingUses.ToString();
//                    itemName_BundleOne.text = item.Key.DisplayName.ToString();
//                    break;
//                }
//            }
//            foreach (var item in DataStoring.playerInventory)
//            {
//                if (item.Key.ItemId.ToString() == items.ItemId)
//                {
//                    itemInformation_BundleTwo.text = items.Description;
//                    displayImage_BundleTwo.sprite = Resources.Load<Sprite>(items.ItemImageUrl);
//                    itemCount_BundleTwo.text = item.Key.RemainingUses.ToString();
//                    itemName_BundleTwo.text = item.Key.DisplayName.ToString();
//                    InfoPackagePopup.SetActive(true);
//                    break;
//                }
//            }
//            break;
//        }//Single
//        else
//        {
//            itemList.Clear();
//            itemList.Add(items.Bundle.BundledItems[0], items.Bundle.BundledItems.Count);
//            foreach (var item in DataStoring.playerInventory)
//            {
//                if (item.Key.ItemId.ToString() == items.ItemId)
//                {
//                    itemInformation.text = items.Description;
//                    displayImage.sprite = Resources.Load<Sprite>(items.ItemImageUrl);
//                    itemCount.text = item.Key.RemainingUses.ToString();
//                    itemName.text = item.Key.DisplayName.ToString();
//                    informationPopup.SetActive(true);
//                    break;
//                }
//            }
//            break;
//        }
//    }
//}