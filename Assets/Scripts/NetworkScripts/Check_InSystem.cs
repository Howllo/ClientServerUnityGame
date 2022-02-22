using System;
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
using Newtonsoft.Json;

public class Check_InSystem : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GetAccountInfoScript getAccountInfoScript;
    [SerializeField] private InstaniatingUISystem receivedItemScript;
    [SerializeField] private AudioManagement audioManagement;

    [Header("What Type of Popup")]
    public string PlayerTrackerName = "";
    public string TitleDataTrackerName = "";
    public string WeeklyOrMonthly = "";

    [Header("Popup Info")]
    public GameObject checkinPopup;
    public GameObject informationPopup;

    public string[] Catagory = { "Items", "Characters" };
    [SerializeField] private Button getReceivedButton;
    [SerializeField] private Button recievedReward;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI itemInformation;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private List<TextMeshProUGUI> textInfo = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> buttonImages = new List<Image>();
    [SerializeField] private List<GameObject> getDayRibbon = new List<GameObject>();
    [SerializeField] private List<TextMeshProUGUI> getDayText = new List<TextMeshProUGUI>();
    [SerializeField] private List<Button> getItemInfoButton = new List<Button>();
    private List<string> tempList = new List<string>();
    private List<GetTitleData> titleData = new List<GetTitleData>();
    private DateTime AfterDate;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("HighestStreak"))
        {
            if (PlayerPrefs.GetInt("HighestStreak") >= 6)
            {
                PlayerPrefs.SetString("LastWeekly", TitleDataTrackerName);
            }
        }

        //If the first last weekly does not equal the new weekly reset FirstTimeCheckin.
        if (PlayerPrefs.HasKey("LastWeekly"))
        {
            if(PlayerPrefs.GetString("lastWeekly") != TitleDataTrackerName)
            {
                PlayerPrefs.SetString("FirstTimeCheckin", "true");
            }
        }
    }

    private void Start()
    {
        getReceivedButton.onClick.AddListener(GetPlayerCheckinReward);
        SetCheckinInformation();
        recievedReward.onClick.AddListener(DelayStampAfterRecievedItem);

        if (PlayerPrefs.HasKey("FirstTimeCheckin"))
        {
            if (PlayerPrefs.GetString("FirstTimeCheckin").Equals("false"))
            {
                int intOne = PlayerPrefs.GetInt("HighestStreak");
                for (int i = 0; i < getItemInfoButton.Count; i++)
                {
                    if (intOne >= i)
                    {
                        getItemInfoButton[i].transform.GetChild(3).gameObject.SetActive(true);
                    }
                    else if (intOne < i)
                    {
                        break;
                    }
                }
            }
        }
    }

    //Set everything that has to do with the checkin.
    //This mean image, items, amount, days, and set buttons.
    private void SetCheckinInformation()
    {
        for (int i = 0; i < getItemInfoButton.Count; i++)
        {
            getItemInfoButton[i].onClick.AddListener(OnClickInfo);
        }

        try
        {
            //Set Image, Date, and Count automatically.
            for (int p = 0; p < getItemInfoButton.Count; p++)
            {
                tempList.Add(GetCorrectRewardSwitch(p));
                Dictionary<CatalogItem, int> bundleHolder = new Dictionary<CatalogItem, int>(inventorySystem.GetBundleCount(tempList[p]));
                
                if (bundleHolder != null)
                {
                    foreach (var item in bundleHolder)
                    {
                        textInfo[p].text = item.Value.ToString();
                        buttonImages[p].sprite = Resources.Load<Sprite>(item.Key.ItemImageUrl);
                        getDayText[p].text = $"Day {p+1}";
                        getDayRibbon[p].SetActive(true);
                    }
                }
            }
        } catch (Exception ex) { Debug.Log(ex); }
    }

    /// <summary>
    /// Get inventory count of items that the player has and display the item information.
    /// </summary>
    private void OnClickInfo()
    {
        string stringNum = EventSystem.current.currentSelectedGameObject.name;;
        string GetRewardName = GetCorrectRewardSwitch(Convert.ToInt32(stringNum));
        Dictionary<CatalogItem, int> bundleHolder = new Dictionary<CatalogItem, int>(inventorySystem.GetBundleCount(GetRewardName));

        try
        {
            foreach (var bundle in bundleHolder)
            {
                foreach (var item2 in DataStoring.catalogItems)
                {
                    if (String.Equals(item2.ItemId, bundle.Key.ItemId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var item in DataStoring.playerInventory)
                        {
                            if (item2.ItemId == item.ItemId)
                            {
                                displayImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                itemInformation.text = item2.Description;
                                itemCount.text = item.RemainingUses.ToString();
                                itemName.text = item.DisplayName;
                            }
                            else // If player does not have the item just display zero count.
                            {
                                foreach (var currency in DataStoring.virtualCurrencyNames)
                                {
                                    if (item2.ItemClass.ToLower().Contains(currency.Value.Replace(" ", "").ToLower()))
                                    {
                                        int virutalCurrencyTotal = 0;
                                        if (item2.ItemId.Equals("astralcredit", StringComparison.OrdinalIgnoreCase) || item2.ItemId.Equals("freeastralcredit", StringComparison.OrdinalIgnoreCase))
                                        {
                                            virutalCurrencyTotal += DataStoring.VirtualCurrency["AC"];
                                            virutalCurrencyTotal += DataStoring.VirtualCurrency["FA"];
                                        }
                                        else
                                        {
                                            virutalCurrencyTotal = DataStoring.VirtualCurrency[DataStoring.inverseVirtualCurrencyNames[item2.ItemClass.ToLower()]];
                                        }

                                        displayImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                        itemInformation.text = item2.Description;
                                        itemCount.text = virutalCurrencyTotal.ToString();
                                        itemName.text = item2.DisplayName;
                                        break;
                                    }
                                    else
                                    {
                                        displayImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                        itemInformation.text = item2.Description;
                                        itemCount.text = "0";
                                        itemName.text = item2.DisplayName;
                                    }
                                }
                            }
                        }
                        if (DataStoring.playerInventory.Count == 0)
                        {
                            foreach (var currency in DataStoring.virtualCurrencyNames)
                            {
                                if (item2.ItemClass.ToLower().Contains(currency.Value.Replace(" ", "").ToLower()))
                                {
                                    int virutalCurrencyTotal = 0;

                                    //Get the right amount of currency no matter
                                    if (item2.ItemId.Equals("astralcredit", StringComparison.OrdinalIgnoreCase) || item2.ItemId.Equals("freeastralcredit", StringComparison.OrdinalIgnoreCase))
                                    {
                                        virutalCurrencyTotal += DataStoring.VirtualCurrency["AC"];
                                        virutalCurrencyTotal += DataStoring.VirtualCurrency["FA"];
                                    } 
                                    else
                                    {
                                        virutalCurrencyTotal = DataStoring.VirtualCurrency[DataStoring.inverseVirtualCurrencyNames[item2.ItemClass.ToLower()]];
                                    }

                                    displayImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                    itemInformation.text = item2.Description;
                                    itemCount.text = virutalCurrencyTotal.ToString();
                                    itemName.text = item2.DisplayName;
                                    break;
                                }
                                else
                                {
                                    displayImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                    itemInformation.text = item2.Description;
                                    itemCount.text = "0";
                                    itemName.text = item2.DisplayName;
                                }
                            }
                        }
                    }
                }
            }
            informationPopup.SetActive(true);
        } catch (Exception ex) { Debug.Log(ex); }
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
            PlayerPrefs.SetInt("HighestStreak", retrieveFromJson.HighestStreak);
            PlayerPrefs.SetString("FirstTimeCheckin", "false");
            AfterDate = Convert.ToDateTime(retrieveFromJson.LoginAfter);
            getReceivedButton.interactable = true;
            StartCoroutine(receivedItemScript.GetRewardPopup(retrieveFromJson.GrantedItem));
            inventorySystem.PlayerInventory();
            GetPlayerData();
        }, OnError);
    }

    //Get the reward information.
    private string GetCorrectRewardSwitch(int DailyRewards)
    {
        string jsonresults = "";
        if (TitleDataTrackerName.Contains("Consecutive"))
        {
            jsonresults = JsonConvert.DeserializeObject(DataStoring.ConsecutiveTitleDataJson).ToString();
            titleData = JsonConvert.DeserializeObject<List<GetTitleData>>(jsonresults);
        } 
        else if (TitleDataTrackerName.Contains("Event"))
        {
            jsonresults = JsonConvert.DeserializeObject(DataStoring.EventTitleDataJson).ToString();
            titleData = JsonConvert.DeserializeObject<List<GetTitleData>>(jsonresults);
        } 
        else if (TitleDataTrackerName.Contains("Monthly"))
        {
            jsonresults = JsonConvert.DeserializeObject(DataStoring.MonthlyTitleDataJson).ToString();
            titleData = JsonConvert.DeserializeObject<List<GetTitleData>>(jsonresults);
        }
        return titleData[DailyRewards].Reward;
    }

    private void DelayStampAfterRecievedItem()
    {

        int intOne = PlayerPrefs.GetInt("HighestStreak");
        for (int i = 0; i < getItemInfoButton.Count; i++)
        {
            if (intOne >= i)
            {
                audioManagement.PlaySoundClip(2);
                getItemInfoButton[i].transform.GetChild(3).gameObject.SetActive(true);
            }
            else if (intOne < i)
            {
                break;
            }
        }
    }

    private void OnError(PlayFabError error)
    {
        if (error.Error.ToString().Contains("/CloudScript/ExecuteFunction: Invocation of cloud script function"))
            getReceivedButton.interactable = true;
        Debug.Log(error);
    }
}

public class GetJson
{
    public int HighestStreak;
    public string LoginAfter;
    public string GrantedItem;
}