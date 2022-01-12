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
        public static bool isUpdateInventory = false;
        public static string GetAPI = "";
        public static string ConsecutiveTitleDataJson = "";
        public static string EventTitleDataJson = "";
        public static string MonthlyTitleDataJson = "";
        public static string AchievementTitleDataJson = "";
        public static string CharacterTitleDataJson = "";

        public static List<ItemInstance> basicItems = new List<ItemInstance>();
        public static List<ItemInstance> growthItem = new List<ItemInstance>();
        public static List<ItemInstance> consumablesItems = new List<ItemInstance>();
        public static List<CatalogItem> basicItemCata = new List<CatalogItem>();
        public static List<CatalogItem> grwothItemsCata = new List<CatalogItem>();
        public static List<CatalogItem> consumablesItemsCata = new List<CatalogItem>();


        /// <summary>
        /// Catalog contain all the items in "Items" catalog.
        /// </summary>
        public static List<CatalogItem> catalogItems = new List<CatalogItem>();

        /// <summary>
        /// Virtual currency contain all the information and remaing currency from players currency.
        /// </summary>
        public static Dictionary<string,int> VirtualCurrency = new Dictionary<string,int>();

        /// <summary>
        /// Player inventory contains all the items in the players inventory.
        /// </summary>
        public static List<ItemInstance> playerInventory = new List<ItemInstance>();

        /// <summary>
        /// Virtual Currency Name key is "EC" with value of "Ethercredits". This allow access to names.
        /// </summary>
        public static Dictionary<string, string> virtualCurrencyNames = new Dictionary<string, string>();

        /// <summary>
        /// Inverse VC name is the opposite of Virtual Currency names. This contain keys of "astralcredit" to value of "AC".
        /// </summary>
        public static Dictionary<string, string> inverseVirtualCurrencyNames = new Dictionary<string, string>();
    }
}