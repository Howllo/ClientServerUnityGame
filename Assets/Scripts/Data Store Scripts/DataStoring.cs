using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

namespace DataStoringIDError
{
    public static class DataStoring
    {
        public static bool isResourceBarClicked = false;
        public static string GetAPI = "";
        public static List<CatalogItem> catalogItemsCombined = new List<CatalogItem>();
        public static Dictionary<ItemInstance, int> playerInventory = new Dictionary<ItemInstance, int>();
    }

    public class InventoryItem : MonoBehaviour
    {
        public string DisplayName;
        public string Description;
        public string IconImage;
        public int ItemCount;
    }
}