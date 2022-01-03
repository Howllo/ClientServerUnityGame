using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using DataStoringIDError;

//Only use this on the main menu.
public class InventorySystem : MonoBehaviour
{
    [SerializeField] private Check_InSystem checkInSystem;

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
                checkInSystem.checkinPopup.SetActive(true);
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
            foreach (var item in results.Inventory)
            {
                DataStoring.playerInventory.Add(item, item.RemainingUses.Value);
            }
        }, OnError);
    }

    /// <summary>
    /// Get all Catagory items and store in a cache in DataStore. Requires "using DataStoringIDError".
    /// </summary>
    public void GetCatagoryItems()
    {
        List<CatalogItem> armorCatalogItems = new List<CatalogItem>();
        List<CatalogItem> characterCatalogItems = new List<CatalogItem>();
        List<CatalogItem> itemsCatalogItems = new List<CatalogItem>();
        List<CatalogItem> resourcesCatalogItems = new List<CatalogItem>();
        List<CatalogItem> charSkinsCatalogItems = new List<CatalogItem>();

        int tempCountHolder = 0, oldCount = 0;
        string itemNameID = "", lastItemName = "", processedString = "", p;

        for (int i = 0; i < checkInSystem.Catagory.Length; i++)
        {
            PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
            {
                CatalogVersion = checkInSystem.Catagory[i],
            }, results =>
            {
                if (i == 0)
                    armorCatalogItems = new List<CatalogItem>(results.Catalog);
                else if (i == 1)
                    characterCatalogItems = new List<CatalogItem>(results.Catalog);
                else if (i == 2)
                    itemsCatalogItems = new List<CatalogItem>(results.Catalog);
                else if (i == 3)
                    resourcesCatalogItems = new List<CatalogItem>(results.Catalog);
                else if (i == 4)
                    charSkinsCatalogItems = new List<CatalogItem>(results.Catalog);
            }, OnError);
        }
        DataStoring.catalogItemsCombined = new List<CatalogItem>(armorCatalogItems.Concat(characterCatalogItems).Concat(itemsCatalogItems).Concat(
                                                    resourcesCatalogItems).Concat(charSkinsCatalogItems));
        armorCatalogItems = null;
        characterCatalogItems = null;
        itemsCatalogItems = null;
        resourcesCatalogItems = null;
        charSkinsCatalogItems = null;
    }

    private void 

    private void OnError(PlayFabError error)
    {
        Debug.Log(error);
    }
}