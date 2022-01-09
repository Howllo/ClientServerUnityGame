using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataStoringIDError;
using PlayFab.ClientModels;

public class InventoryUISystem : MonoBehaviour
{
    [Header("Clicked Item")]
    [SerializeField] private GameObject item_Information;
    [SerializeField] private TextMeshProUGUI itemCount, itemInfo, itemName;
    [SerializeField] private Image itemImage;

    [Header("Other Stuff")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Button All_ItemsButton, BasicItems_Button, GrowthItems_Button, Consumable_Button;
    [SerializeField] private GameObject All_ItemsGO, BasicItemsGO, GrowthItemsGO, ConsumableGO;
    [SerializeField] private GameObject clickableReward, CloseButton;
    [SerializeField] private TextMeshProUGUI ClickOutside;
    [SerializeField] private List<GameObject> parentObject = new List<GameObject>();
    private List<GameObject> Inventory = new List<GameObject>();
    private List<GameObject> BasicItemList = new List<GameObject>();
    private List<GameObject> GrowthList = new List<GameObject>();
    private List<GameObject> ConsumablesList = new List<GameObject>();
    private GameObject LastGO;
    private Button lastButton;
    private TextMeshProUGUI child;
    int i = 0;

    private void Start()
    {
        LastGO = All_ItemsGO;
        lastButton = All_ItemsButton;

#if UNITY_EDITOR
        ClickOutside.text = "Click outside to Close.";
#endif
#if UNITY_IOS
        ClickOutside.text = "Tap outside to close.";
#endif
#if UNITY_ANDRIOD
        ClickOutside.text = "Tap outside to close.";
#endif
#if UNITY_64
        ClickOutside.text = "Click outside to Close.";
#endif
#if UNITY_XBOXONE
        CloseButton.SetActive(true);
        ClickOutside.gameObject.SetActive(false);
#endif
#if UNITY_PS5
        CloseButton.SetActive(true);
        ClickOutside.gameObject.SetActive(false);
#endif
#if UNITY_SWITCH
        CloseButton.SetActive(true);
        ClickOutside.gameObject.SetActive(false);
#endif
    }

    private void TabMenuPages()
    {
        string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        if (buttonName.Equals("All_Items"))
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = All_ItemsButton;
            All_ItemsButton.interactable = false;

            //Setting Color
            var CBButton = lastButton.colors;
            CBButton.disabledColor = Color.gray;
            lastButton.colors = CBButton;
        } 
        else if (buttonName.Equals("BasicItems")) 
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = BasicItems_Button;
            BasicItems_Button.interactable = false;

            //Setting Color
            var CBButton = lastButton.colors;
            CBButton.disabledColor = Color.gray;
            lastButton.colors = CBButton;
        }
        else if (buttonName.Equals("GrowthItems"))
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = GrowthItems_Button;
            GrowthItems_Button.interactable = false;

            //Setting Color
            var CBButton = lastButton.colors;
            CBButton.disabledColor = Color.gray;
            lastButton.colors = CBButton;
        }
        else if (buttonName.Equals("Consumables"))
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = Consumable_Button;
            Consumable_Button.interactable = false;

            //Setting Color
            var CBButton = lastButton.colors;
            CBButton.disabledColor = Color.gray;
            lastButton.colors = CBButton;
        }
    }
    
    private void GetAllInventory()
    {
        if(Inventory.Count == 0 || DataStoring.isUpdateInventory)
        {
            foreach (var item in DataStoring.playerInventory)
            {
                foreach (var item2 in DataStoring.catalogItems)
                {
                    if (item.Key.ItemClass == "basicItem" || item.Key.ItemClass == "consumable" || item.Key.ItemClass == "growthItem")
                    {
                        Inventory[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                        Inventory[i].gameObject.name = $"{item.Key.ItemId}";
                        Inventory[i].transform.SetParent(parentObject[0].gameObject.transform, false);
                        child = Inventory[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        child.text = item.Value.ToString();
                        Inventory[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                        Inventory[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                    }
                }
                i++;
            }
        }
        All_ItemsGO.SetActive(true);
        DataStoring.isUpdateInventory = false;
    }

    public void GetBasicItems()
    {
        if (BasicItemList.Count == 0 || DataStoring.isUpdateInventory)
        {
            foreach (var item in DataStoring.playerInventory)
            {
                foreach (var item2 in DataStoring.catalogItems)
                {
                    if (item.Key.ItemClass == "basicItem")
                    {
                        BasicItemList[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                        BasicItemList[i].gameObject.name = $"{item.Key.ItemId}";
                        BasicItemList[i].transform.SetParent(parentObject[1].gameObject.transform, false);
                        child = BasicItemList[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        child.text = item.Value.ToString();
                        BasicItemList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                        BasicItemList[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                    }
                }
                i++;
            }
        }
        BasicItemsGO.SetActive(true);
        DataStoring.isUpdateInventory = true;
    }

    public void GetGrowthItems()
    {
        if(GrowthList.Count == 0 || DataStoring.isUpdateInventory)
        {
            foreach (var item in DataStoring.playerInventory)
            {
                foreach (var item2 in DataStoring.catalogItems)
                {
                    if (item.Key.ItemClass == "growthItem")
                    {
                        GrowthList[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                        GrowthList[i].gameObject.name = $"{item.Key.ItemId}";
                        GrowthList[i].transform.SetParent(parentObject[2].gameObject.transform, false);
                        child = GrowthList[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        child.text = item.Value.ToString();
                        GrowthList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                        GrowthList[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                    }
                }
                i++;
            }
        }
        GrowthItemsGO.SetActive(true);
        DataStoring.isUpdateInventory = false;
    }

    public void GetConsumableInventory()
    {
        if(ConsumablesList.Count == 0 || DataStoring.isUpdateInventory)
        {
            foreach (var item in DataStoring.playerInventory)
            {
                foreach (var item2 in DataStoring.catalogItems)
                {
                    if (item.Key.ItemClass == "consumable")
                    {
                        ConsumablesList[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                        ConsumablesList[i].gameObject.name = $"{item.Key.ItemId}";
                        ConsumablesList[i].transform.SetParent(parentObject[3].gameObject.transform, false);
                        child = ConsumablesList[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        child.text = item.Value.ToString();
                        ConsumablesList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                        ConsumablesList[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                    }
                }
                i++;
            }
        }
        ConsumableGO.SetActive(true);
        DataStoring.isUpdateInventory = false;
    }

    private void ClickedItem()
    {
        string itemClicked = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        Dictionary<CatalogItem, int> bundleHolder = new Dictionary<CatalogItem, int>(inventorySystem.GetBundleCount(itemClicked));

        try
        {
            foreach (var bundle in bundleHolder)
            {
                foreach (var item2 in DataStoring.catalogItems)
                {
                    if (item2.ItemId == bundle.Key.ItemId)
                    {
                        foreach (var item in DataStoring.playerInventory)
                        {
                            if (item2.ItemId == item.Key.ItemId)
                            {
                                itemImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                itemInfo.text = item2.Description;
                                itemCount.text = item.Key.RemainingUses.ToString();
                                itemName.text = item.Key.DisplayName;
                            }
                            else // If player does not have the item just display zero count.
                            {
                                foreach (var currency in DataStoring.virtualCurrencyNames)
                                {
                                    if (item2.ItemClass.ToLower().Contains(currency.Value.Replace(" ", "").ToLower()))
                                    {
                                        int CombinedAstralCredits = 0;
                                        if (DataStoring.VirtualCurrency[DataStoring.inverseVirtualCurrencyNames[item2.ItemClass.ToLower()]].ToString() == "AC" ||
                                            DataStoring.VirtualCurrency[DataStoring.inverseVirtualCurrencyNames[item2.ItemClass.ToLower()]].ToString() == "FA")
                                        {
                                            CombinedAstralCredits += DataStoring.VirtualCurrency["AC"];
                                            CombinedAstralCredits += DataStoring.VirtualCurrency["FA"];
                                        }
                                        itemImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                        itemInfo.text = item2.Description;
                                        itemCount.text = DataStoring.VirtualCurrency[DataStoring.inverseVirtualCurrencyNames[item2.ItemClass.ToLower()]].ToString();
                                        itemName.text = item2.DisplayName;
                                        break;
                                    }
                                    else
                                    {
                                        itemImage.sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                                        itemInfo.text = item2.Description;
                                        itemCount.text = "0";
                                        itemName.text = item2.DisplayName;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            item_Information.SetActive(true);
        }
        catch (Exception ex) { Debug.Log(ex); }
    }
}