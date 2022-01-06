using System.Collections.Generic;
using PlayFab.ClientModels;

namespace DataStoringIDError
{
    public static class DataStoring
    {
        public static bool hasAlreadyRanInventorySystem = false;
        public static bool isResourceBarClicked = false;
        public static bool hasRanTitleData = false;
        public static bool hasRanCatalog = false;
        public static string GetAPI = "";
        public static string ConsecutiveTitleDataJson = "";
        public static string EventTitleDataJson = "";
        public static string MonthlyTitleDataJson = "";
        public static string AchievementTitleDataJson = "";
        public static string CharacterTitleDataJson = "";
        public static List<CatalogItem> catalogItems = new List<CatalogItem>();
        public static Dictionary<ItemInstance, int> playerInventory = new Dictionary<ItemInstance, int>();
        public static Dictionary<string, string> virtualCurrency = new Dictionary<string, string>();
    }
}