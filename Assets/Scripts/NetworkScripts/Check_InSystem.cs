using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;

public class Check_InSystem : MonoBehaviour
{
    [Header("Get Popup Info")]
    public string PlayerTrackerName = "";
    public string TitleDataTrackerName = "";
    public string WeeklyOrMonthly = "";
    private string GetTitleJsonData = "";
    private uint highestStreak = 0;
    private DateTime AfterDate;
    private uint TotalAmount = 0;

    [Header("Popup Info")]
    [SerializeField] private GameObject checkinPopup, informationPopup;
    [SerializeField] private Button getReceivedButton;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private TextMeshProUGUI itemInformation, itemCount, itemName;
    [SerializeField] private List<Button> GetAllButtons = new List<Button>();
    [SerializeField] private List<TextMeshProUGUI> textInfo = new List<TextMeshProUGUI>();
    private List<string> CatagoryList = new List<string>(); 
    
    private void Start()
    {
        if (TitleDataTrackerName.Contains("Event") || TitleDataTrackerName.Contains("Consecutive"))
            TotalAmount = 7;
        else
            TotalAmount = (uint)DateTime.Now.Month;

        GetItemInformation((int)TotalAmount);

        GetItemsInCheckin();
        StartCoroutine(DelayStart());
        getReceivedButton.onClick.AddListener(GetPlayerWeekly);
        if (PlayerPrefs.HasKey("LoginAfter"))
            AfterDate = Convert.ToDateTime(PlayerPrefs.GetString("LoginAfter"));
    }

    private IEnumerator DelayStart()
    {
        for (int i = 0; i < GetAllButtons.Count; i++)
        {
            GetAllButtons[i].onClick.AddListener(OnClickInfo);
        }
            
        yield return new WaitForSeconds(2f);
        checkinPopup.SetActive(true);
    }

    private void OnClickInfo()
    {
        
    }


    private void GetItemsInCheckin()
    {
        List<string> list = new List<string>();
        list.Add(TitleDataTrackerName);

        var request = new GetTitleDataRequest()
        {
            Keys = list
        };
        PlayFabClientAPI.GetTitleData(request, GetItemsCheckinResutls, OnError);
    }

    private void GetItemsCheckinResutls(GetTitleDataResult results)
    {
        GetTitleJsonData = results.ToString();
    }

    private void GetItemInformation(int i)
    {
        string catalog = GetCorrectCatalogSwitch(i);
        for(int k = 0; k < i; k++)
        {
            var request = new GetCatalogItemsRequest()
            {
                CatalogVersion = catalog,
            };
            PlayFabClientAPI.GetCatalogItems(request, GetItemResults, OnError);
        }
    }

    private void GetItemResults(GetCatalogItemsResult results)
    {
        List<string> holderStr = new List<string>();
        holderStr.Concat(results.Catalog);

        informationPopup.SetActive(true);
    }

    //Cloud script | Grants items if after x amount of time.
    private void GetPlayerWeekly()
    {
        //Return if Now is less than AfterDate
        if(AfterDate != null)
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
            FunctionParameter = new { TitleDataTrackerName, PlayerTrackerName, WeeklyOrMonthly },
            GeneratePlayStreamEvent = true
        }, GetPlayerWeeklyResults, OnError);
    }
    private void GetPlayerWeeklyResults(ExecuteFunctionResult results)
    {
        GetJson retrieveFromJson = JsonUtility.FromJson<GetJson>(results.FunctionResult.ToString());
        highestStreak = retrieveFromJson.HighestStreak;
        AfterDate = retrieveFromJson.LoginAfter;
        PlayerPrefs.SetString("LoginAfter", retrieveFromJson.LoginAfter.ToString());
        //Set buttons that inactive that have already been used.
        for (int i = 0; i < highestStreak; i++)
            GetAllButtons[i].interactable = false;
    }

    //Get the reward information.
    private string GetCorrectRewardSwitch(int DailyRewards)
    {
        string reward = "";
        TitleInformation titleData = JsonUtility.FromJson<TitleInformation>(GetTitleJsonData);

        switch (DailyRewards)
        {
            case 0: reward = titleData.days[1].Catalog; break;
            case 1: reward = titleData.days[2].Catalog; break;
            case 2: reward = titleData.days[3].Catalog; break;
            case 3: reward = titleData.days[4].Catalog; break;
            case 4: reward = titleData.days[5].Catalog; break;
            case 5: reward = titleData.days[6].Catalog; break;
            case 6: reward = titleData.days[7].Catalog; break;
            case 7: reward = titleData.days[8].Catalog; break;
            case 8: reward = titleData.days[9].Catalog; break;
            case 9: reward = titleData.days[10].Catalog; break;
            case 10: reward = titleData.days[11].Catalog; break;
            case 11: reward = titleData.days[12].Catalog; break;
            case 12: reward = titleData.days[13].Catalog; break;
            case 13: reward = titleData.days[14].Catalog; break;
            case 14: reward = titleData.days[15].Catalog; break;
            case 15: reward = titleData.days[16].Catalog; break;
            case 16: reward = titleData.days[17].Catalog; break;
            case 17: reward = titleData.days[18].Catalog; break;
            case 18: reward = titleData.days[19].Catalog; break;
            case 19: reward = titleData.days[20].Catalog; break;
            case 20: reward = titleData.days[21].Catalog; break;
            case 21: reward = titleData.days[22].Catalog; break;
            case 22: reward = titleData.days[23].Catalog; break;
            case 23: reward = titleData.days[24].Catalog; break;
            case 24: reward = titleData.days[25].Catalog; break;
            case 25: reward = titleData.days[26].Catalog; break;
            case 26: reward = titleData.days[27].Catalog; break;
            case 27: reward = titleData.days[28].Catalog; break;
            case 28: reward = titleData.days[29].Catalog; break;
            case 29: reward = titleData.days[30].Catalog; break;
            case 30: reward = titleData.days[31].Catalog; break;
        }
        return reward;
    }

    //Get correct catalog to get the bundles information.
    private string GetCorrectCatalogSwitch(int DailyRewards)
    {
        string catalog = "";
        TitleInformation titleData = JsonUtility.FromJson<TitleInformation>(GetTitleJsonData);

        switch (DailyRewards)
        {
            case 0: catalog = titleData.days[1].Catalog; break;
            case 1: catalog = titleData.days[2].Catalog; break;
            case 2: catalog = titleData.days[3].Catalog; break;
            case 3: catalog = titleData.days[4].Catalog; break;
            case 4: catalog = titleData.days[5].Catalog; break;
            case 5: catalog = titleData.days[6].Catalog; break;
            case 6: catalog = titleData.days[7].Catalog; break;
            case 7: catalog = titleData.days[8].Catalog; break;
            case 8: catalog = titleData.days[9].Catalog; break;
            case 9: catalog = titleData.days[10].Catalog; break;
            case 10: catalog = titleData.days[11].Catalog; break;
            case 11: catalog = titleData.days[12].Catalog; break;
            case 12: catalog = titleData.days[13].Catalog; break;
            case 13: catalog = titleData.days[14].Catalog; break;
            case 14: catalog = titleData.days[15].Catalog; break;
            case 15: catalog = titleData.days[16].Catalog; break;
            case 16: catalog = titleData.days[17].Catalog; break;
            case 17: catalog = titleData.days[18].Catalog; break;
            case 18: catalog = titleData.days[19].Catalog; break;
            case 19: catalog = titleData.days[20].Catalog; break;
            case 20: catalog = titleData.days[21].Catalog; break;
            case 21: catalog = titleData.days[22].Catalog; break;
            case 22: catalog = titleData.days[23].Catalog; break;
            case 23: catalog = titleData.days[24].Catalog; break;
            case 24: catalog = titleData.days[25].Catalog; break;
            case 25: catalog = titleData.days[26].Catalog; break;
            case 26: catalog = titleData.days[27].Catalog; break;
            case 27: catalog = titleData.days[28].Catalog; break;
            case 28: catalog = titleData.days[29].Catalog; break;
            case 29: catalog = titleData.days[30].Catalog; break;
            case 30: catalog = titleData.days[31].Catalog; break;
        }
        return catalog;
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
    public DateTime LoginAfter;
}

[Serializable]
public class TitleInformation
{
    public List<GetDays> days;
}

public class GetDays
{
    public string Day;
    public int MinStreak;
    public int Reward;
    public string Catalog;
}
