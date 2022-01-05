using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

namespace DataStoringIDError
{
    public static class DataStoring
    {
        public static bool hasAlreadyRanInventorySystem = false;
        public static bool isResourceBarClicked = false;
        public static string GetAPI = "";
        public static List<CatalogItem> catalogItems = new List<CatalogItem>();
        public static Dictionary<ItemInstance, int> playerInventory = new Dictionary<ItemInstance, int>();
        public static Dictionary<string, string> virtualCurrency = new Dictionary<string, string>();
    }
}