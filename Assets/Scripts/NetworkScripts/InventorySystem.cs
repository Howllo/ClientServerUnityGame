using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using DataStoringIDError;

//Only use this on the main menu.
public class InventorySystem : MonoBehaviour
{
    [SerializeField] private Check_InSystem checkInSystem;
    private CatalogItem IncomingItem;
    public bool isInventoryLoadingFinished = false;

    public void Awake()
    {
        PlayerInventory();
        if (!DataStoring.hasRanCatalog)
        {
            GetCatagoryItems();
        }

        if(checkInSystem != null)
        {
            LoadJson();
            StartCoroutine(WaitForJson());
        }
    }

    private void LoadJson()
    {
        if (!DataStoring.hasRanTitleData)
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest()
            {
                Keys = null
            }, results =>
            {
                foreach (var item in results.Data)
                {
                    if (item.Key.Contains("Monthly"))
                    {
                        DataStoring.MonthlyTitleDataJson  = item.Value;
                    } 
                    else if (item.Key.Contains("Consecutive"))
                    {
                        DataStoring.ConsecutiveTitleDataJson = item.Value;
                    } 
                    else if (item.Key.Contains("Event"))
                    {
                        DataStoring.EventTitleDataJson = item.Value;
                    } 
                    else if (item.Key.Contains("AchievementTitleDataJson"))
                    {
                        DataStoring.AchievementTitleDataJson = item.Value;
                    } 
                    else if (item.Key.Contains("Character"))
                    {
                        DataStoring.CharacterTitleDataJson = item.Value;
                    }  
                }
                DataStoring.hasRanTitleData = true;
            }, OnError); ;
        }
    }

    //Wait until Json is loaded into cache.
    private System.Collections.IEnumerator WaitForJson()
    {
        yield return new WaitUntil(() => DataStoring.hasRanTitleData && DataStoring.hasRanCatalog);

        if (!PlayerPrefs.HasKey("LoginAfter"))
        {
            checkInSystem.checkinPopup.SetActive(true);
        }
        else
        {
            int CheckTime = DateTime.Compare(DateTime.Now, Convert.ToDateTime(PlayerPrefs.GetString("LoginAfter")));
            if (CheckTime >= 0)
            {
                checkInSystem.checkinPopup.SetActive(true);
            }
        }
    }


    /// <summary>
    /// Inventory system that allows updating and getting of the player inventory. Requires "using DataStoringIDError".
    /// </summary>
    public void PlayerInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), results =>
        {
            DataStoring.playerInventory = new Dictionary<ItemInstance, int>();
            DataStoring.virtualCurrencyNames = new Dictionary<string, string>();
            DataStoring.VirtualCurrency = new Dictionary<string, int>(results.VirtualCurrency);
            foreach (var item in results.Inventory)
            {
                DataStoring.playerInventory.Add(item, item.RemainingUses.Value);
            }
            foreach(var currency in results.VirtualCurrency)
            {
                if (currency.Key == "AC")
                {
                    if (!DataStoring.virtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.virtualCurrencyNames.Add(currency.Key, "Astral Credit");
                    if(!DataStoring.inverseVirtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.inverseVirtualCurrencyNames.Add("astralcredit", currency.Key);
                }
                else if (currency.Key == "AR")
                {
                    if (!DataStoring.virtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.virtualCurrencyNames.Add(currency.Key, "Agency Resource");
                    if (!DataStoring.inverseVirtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.inverseVirtualCurrencyNames.Add("agencyresource", currency.Key);
                }
                else if (currency.Key == "BL")
                {
                    if (!DataStoring.virtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.virtualCurrencyNames.Add(currency.Key, "Bad Luck Token");
                    if (!DataStoring.inverseVirtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.inverseVirtualCurrencyNames.Add("badlucktoken", currency.Key);
                }
                else if (currency.Key == "EC")
                {
                    if (!DataStoring.virtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.virtualCurrencyNames.Add(currency.Key, "Ethercredit");
                    if (!DataStoring.inverseVirtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.inverseVirtualCurrencyNames.Add("ethercredit", currency.Key);
                }
                else if (currency.Key == "FA")
                {
                    if (!DataStoring.virtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.virtualCurrencyNames.Add(currency.Key, "Free Astral Credit");
                    if (!DataStoring.inverseVirtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.inverseVirtualCurrencyNames.Add("freeastralcredit", currency.Key);
                }
                else if (currency.Key == "XP")
                {
                    if (!DataStoring.virtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.virtualCurrencyNames.Add(currency.Key, "Account Experience");
                    if (!DataStoring.inverseVirtualCurrencyNames.ContainsKey(currency.Key))
                        DataStoring.inverseVirtualCurrencyNames.Add("accountexperience", currency.Key);
                }
            }
            isInventoryLoadingFinished = true;
        }, OnError);
    }

    /// <summary>
    /// Get all Catagory items and cache it to DataStoring.
    /// </summary>
    public void GetCatagoryItems()
    {
        DataStoring.catalogItems = null;
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
        {
            CatalogVersion = "Items"
        }, results =>
        {
            DataStoring.catalogItems = new List<CatalogItem>(results.Catalog);
            foreach(var items in results.Catalog)
            {
                if(items.ItemClass == "basicItem")
                {
                    DataStoring.basicItems.Add(items);
                }
                if (items.ItemClass == "consumable")
                {
                    DataStoring.consumablesItems.Add(items);  
                } 
                if(items.ItemClass == "growthItem")
                {
                    DataStoring.characterGrowth.Add(items);
                }
            }
            DataStoring.hasRanCatalog = true;
        }, OnError);
    }

    /// <summary>
    /// Gets all bundle items and total amount for each bundle. Returns catalog and int.
    /// </summary>
    /// <param name="IncomingItem"></param>
    /// <returns></returns>
    public Dictionary<CatalogItem, int> GetBundleCount(string tempItemStr)
    {
        Dictionary<CatalogItem, int> Bundle = new Dictionary<CatalogItem, int>();
        List<string> BundleNames = new List<string>();
        List<int> BundleCount = new List<int>();
        int x = 0;
        int counter = 0;
        string str = "";
        bool isChest = false;

        foreach (var item in DataStoring.catalogItems)
        {
            if (item.ItemId == tempItemStr)
            {
                IncomingItem = item;
            }
        }

        if(IncomingItem.ItemId.Contains("Chest") || IncomingItem.ItemId.Contains("chest"))
        {
            isChest = true;
            Bundle.Add(IncomingItem, 1);
        }

        if (!isChest)
        {
            if (IncomingItem.Bundle.BundledItems.Count >= 1 && IncomingItem != null)
            {
                List<string> bundleItems = new List<string>(IncomingItem.Bundle.BundledItems);
                for (int i = 0; i < IncomingItem.Bundle.BundledItems.Count; i++)
                {
                    x++;
                    counter++;
                    if (x >= bundleItems.Count)
                        x = bundleItems.Count - 1;
                    if (!BundleNames.Contains(bundleItems[i]))
                    {
                        BundleNames.Add(bundleItems[i]);
                    }
                    if (bundleItems[i] != bundleItems[x] || i == bundleItems.Count - 1)
                    {
                        BundleCount.Add(counter);
                        counter = 0;
                    }
                }
            }
            if (IncomingItem.Bundle.BundledVirtualCurrencies != null)
            {
                foreach (var currency in IncomingItem.Bundle.BundledVirtualCurrencies)
                {
                    foreach (var testCurrency in DataStoring.virtualCurrencyNames)
                    {
                        if (currency.Key == testCurrency.Key)
                        {
                            str = testCurrency.Value.Replace(" ", "").ToLower();
                            foreach (var item in DataStoring.catalogItems)           //Item List needs to be sorted, and this process need to be sped up.
                            {
                                if (item.ItemId.ToLower() == str)
                                {
                                    Bundle.Add(item, (int)currency.Value);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var items in DataStoring.catalogItems)
            {
                for (int i = 0; i < BundleNames.Count; i++)
                {
                    if (items.ItemId == BundleNames[i])
                    {
                        Bundle.Add(items, BundleCount[i]);
                    }
                }
            }
        } 
        return Bundle;
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error);
    }
}

public class InventoryJson
{
    public string ConsecutiveCheckin { get; set; }
    public string EventCheckin { get; set; } 
    public string MonthlyCheckin { get; set; }
}