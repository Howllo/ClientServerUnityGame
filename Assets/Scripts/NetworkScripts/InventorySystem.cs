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

    public void Awake()
    {
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

        if (!DataStoring.hasAlreadyRanInventorySystem)
        {
            GetCatagoryItems();
            DataStoring.hasAlreadyRanInventorySystem = true;
        }
    }

    /// <summary>
    /// Inventory system that allows updating and getting of the player inventory. Requires "using DataStoringIDError".
    /// </summary>
    public void PlayerInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), results =>
        {
            DataStoring.playerInventory.Clear();
            DataStoring.virtualCurrency.Clear();

            foreach (var item in results.Inventory)
            {
                DataStoring.playerInventory.Add(item, item.RemainingUses.Value);
            }

            foreach(var currency in results.VirtualCurrency)
            {
                if (currency.Key == "AC")
                {
                    DataStoring.virtualCurrency.Add(currency.Key, "Astral Credits");
                }
                else if (currency.Key == "AR")
                {
                    DataStoring.virtualCurrency.Add(currency.Key, "Agency Resource");
                }
                else if (currency.Key == "AR")
                {
                    DataStoring.virtualCurrency.Add(currency.Key, "Agency Resource");
                }
                else if (currency.Key == "BL")
                {
                    DataStoring.virtualCurrency.Add(currency.Key, "Bad Luck Token");
                }
                else if (currency.Key == "EC")
                {
                    DataStoring.virtualCurrency.Add(currency.Key, "Ethercredit");
                }
                else if (currency.Key == "FA")
                {
                    DataStoring.virtualCurrency.Add(currency.Key, "Free Astral Credits");
                }
                else if (currency.Key == "XP")
                {
                    DataStoring.virtualCurrency.Add(currency.Key, "Account Experience");
                }

            }

        }, OnError);
    }

    /// <summary>
    /// Get all Catagory items and cache it to DataStoring.
    /// </summary>
    public void GetCatagoryItems()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
        {
            CatalogVersion = "Items"
        }, results =>
        {
            DataStoring.catalogItems = new List<CatalogItem>(results.Catalog);
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
        foreach (var item in DataStoring.catalogItems)
        {
            if (item.ItemId == tempItemStr)
            {
                IncomingItem = item;
            }
        }

        if (IncomingItem.Bundle == null)
        {
            return Bundle;
        }

        if(IncomingItem.Bundle.BundledItems.Count >= 1)
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
                if (bundleItems[i] != bundleItems[x] || i == bundleItems.Count-1)
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
                foreach (var testCurrency in DataStoring.virtualCurrency)
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
                if(items.ItemId == BundleNames[i])
                {
                    Bundle.Add(items, BundleCount[i]);
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