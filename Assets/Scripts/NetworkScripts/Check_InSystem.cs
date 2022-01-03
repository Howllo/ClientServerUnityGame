using System;
using System.Linq;
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

public class Check_InSystem : MonoBehaviour, IPointerUpHandler
{
    [Header("Scripts")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GetAccountInfoScript getAccountInfoScript;
    [SerializeField] private ReceivedItemScript receivedItemScript;

    [Header("What Type of Popup")]
    public string PlayerTrackerName = "";
    public string TitleDataTrackerName = "";
    public string WeeklyOrMonthly = "";
    public string[] Catagory = { "Items", "Characters" };
    public string BundleClass = "bundlePackage";

    [Header("Popup Info")]
    public GameObject checkinPopup, informationPopup, InfoPackagePopup;
    [SerializeField] private Button getReceivedButton;
    [SerializeField] private Image displayImage, displayImage_BundleOne, displayImage_BundleTwo;
    [SerializeField] private TextMeshProUGUI itemInformation, itemCount, itemName;
    [SerializeField] private TextMeshProUGUI itemInformation_BundleOne, itemCount_BundleOne, itemName_BundleOne;
    [SerializeField] private TextMeshProUGUI itemInformation_BundleTwo, itemCount_BundleTwo, itemName_BundleTwo;
    [SerializeField] private List<TextMeshProUGUI> textInfo = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> buttonImages = new List<Image>();
    [SerializeField] private List<Button> getItemInfoButton = new List<Button>();
    private string GetTitleJsonData = "";
    private uint highestStreak = 0;
    private DateTime AfterDate;

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
        getReceivedButton.onClick.AddListener(GetPlayerWeekly);
    }

    public void OnClickInfo(string RewardInformationm)
    {
        string stringNum = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(stringNum);
        uint buttonNumber = Convert.ToUInt32(stringNum);
        Dictionary<string, int> itemList = new Dictionary<string, int>();
        int j = 0, c = 0;

        //Display the Amount in Inventory - Single Item
        foreach(var items2 in DataStoring.catalogItemsCombined)
        {
            if(items2.ItemId == GetCorrectRewardSwitch(buttonNumber).ToString())
            {
                items2.Bundle.BundledItems.Sort();
                //Doubles
                if(items2.Bundle.BundledItems.First() != items2.Bundle.BundledItems.Last())
                {
                    itemList.Clear();
                    //Get Item Amount
                    for (int i = 0; i < items2.Bundle.BundledItems.Count; i++)
                    {
                        if (items2.Bundle.BundledItems.Contains(items2.Bundle.BundledItems.First()))
                            j++;
                        else if (items2.Bundle.BundledItems.Contains(items2.Bundle.BundledItems.Last()))
                            c++;
                    }
                    itemList.Add(items2.Bundle.BundledItems.First(), j);
                    itemList.Add(items2.Bundle.BundledItems.Last(), c);

                    foreach (var item in DataStoring.playerInventory)
                    {
                        if (item.Key.ItemId.ToString() == items2.ItemId)
                        {
                            itemInformation_BundleOne.text = items2.Description;
                            displayImage_BundleOne.sprite = Resources.Load<Sprite>(items2.ItemImageUrl);
                            itemCount_BundleOne.text = item.Key.RemainingUses.ToString();
                            itemName_BundleOne.text = item.Key.DisplayName.ToString();
                            break;
                        }
                    }
                    foreach (var item in DataStoring.playerInventory)
                    {
                        if (item.Key.ItemId.ToString() == items2.ItemId)
                        {
                            itemInformation_BundleTwo.text = items2.Description;
                            displayImage_BundleTwo.sprite = Resources.Load<Sprite>(items2.ItemImageUrl);
                            itemCount_BundleTwo.text = item.Key.RemainingUses.ToString();
                            itemName_BundleTwo.text = item.Key.DisplayName.ToString();
                            InfoPackagePopup.SetActive(true);
                            break;
                        }
                    }
                    break;
                }//Single
                else
                {
                    itemList.Clear();
                    itemList.Add(items2.Bundle.BundledItems[0], items2.Bundle.BundledItems.Count());
                    foreach (var item in DataStoring.playerInventory)
                    {
                        if (item.Key.ItemId.ToString() == items2.ItemId)
                        {
                            itemInformation.text = items2.Description;
                            displayImage.sprite = Resources.Load<Sprite>(items2.ItemImageUrl);
                            itemCount.text = item.Key.RemainingUses.ToString();
                            itemName.text = item.Key.DisplayName.ToString();
                            informationPopup.SetActive(true);
                            break;
                        }
                    }
                    break;
                }
            }
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
            Keys = null
        }, results =>
        {
            GetTitleJsonData = results.Data[TitleDataTrackerName].ToString();
        }, OnError);
    }

    //Cloud script | Grants items if after x amount of time.
    private void GetPlayerWeekly()
    {
        string PlayerAccountID = PlayerPrefs.GetString("PlayerAccountID");
        ItemInstance tempInstance = null;
        int tempCount = 0;

        try
        {
            if(PlayerPrefs.GetString("LoginAfter") != null)
                AfterDate = Convert.ToDateTime(PlayerPrefs.GetString("LoginAfter"));
        } catch (Exception e)
        {
            Debug.Log(e);
        }

        //Return if Now is less than AfterDate
        if (AfterDate != null)
        {
            int dateResults = DateTime.Compare(DateTime.Now, AfterDate);
            if (dateResults < 0)
                return;
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
            highestStreak = retrieveFromJson.HighestStreak;
            AfterDate = Convert.ToDateTime(retrieveFromJson.LoginAfter);
            getAccountInfoScript.GetPlayerInventory();
            foreach(var item in DataStoring.catalogItemsCombined)
            {
                if (item. Key.ToString() == retrieveFromJson.GrantedItem)
                    tempInstance = item.Key;
                if(item.Key.BundleContents.First() == item.Key.BundleContents.Last())
                    tempCount = item.Key.BundleContents.Count();
            }
            if(tempInstance != null)
                receivedItemScript.GetRewardPopup(tempInstance, tempCount);
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
            case 0: reward = titleData.day1.Reward; break;
            case 1: reward = titleData.day2.Reward; break;
            case 2: reward = titleData.day3.Reward; break;
            case 3: reward = titleData.day4.Reward; break;
            case 4: reward = titleData.day5.Reward; break;
            case 5: reward = titleData.day6.Reward; break;
            case 6: reward = titleData.day7.Reward; break;
            case 7: reward = titleData.day8.Reward; break;
            case 8: reward = titleData.day9.Reward; break;
            case 9: reward = titleData.day10.Reward; break;
            case 10: reward = titleData.day11.Reward; break;
            case 11: reward = titleData.day12.Reward; break;
            case 12: reward = titleData.day13.Reward; break;
            case 13: reward = titleData.day14.Reward; break;
            case 14: reward = titleData.day15.Reward; break;
            case 15: reward = titleData.day16.Reward; break;
            case 16: reward = titleData.day17.Reward; break;
            case 17: reward = titleData.day18.Reward; break;
            case 18: reward = titleData.day19.Reward; break;
            case 19: reward = titleData.day20.Reward; break;
            case 20: reward = titleData.day21.Reward; break;
            case 21: reward = titleData.day22.Reward; break;
            case 22: reward = titleData.day23.Reward; break;
            case 23: reward = titleData.day24.Reward; break;
            case 24: reward = titleData.day25.Reward; break;
            case 25: reward = titleData.day26.Reward; break;
            case 26: reward = titleData.day27.Reward; break;
            case 27: reward = titleData.day28.Reward; break;
            case 28: reward = titleData.day29.Reward; break;
            case 29: reward = titleData.day30.Reward; break;
            case 30: reward = titleData.day31.Reward; break;
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