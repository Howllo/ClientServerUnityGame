using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DataStoringIDError;
using PlayFab.ClientModels;

public class InventoryUISystem : MonoBehaviour
{
    [Header("Clicked Item")]
    [SerializeField] private GameObject item_Information;
    [SerializeField] private TextMeshProUGUI itemCount, itemInformation, itemName;
    [SerializeField] private Image displayImage;

    [Header("Other Stuff")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Button All_ItemsButton, BasicItems_Button, GrowthItems_Button, Consumable_Button;
    [SerializeField] private GameObject All_Items_Page_GO, BasicItems_Page_GO, GrowthItems_Page_GO, Consumable_Page_GO;
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
        LastGO = All_Items_Page_GO;
        lastButton = All_ItemsButton;
        Startup();

        All_ItemsButton.onClick.AddListener(TabMenuPages);
        BasicItems_Button.onClick.AddListener(TabMenuPages);
        GrowthItems_Button.onClick.AddListener(TabMenuPages);
        Consumable_Button.onClick.AddListener(TabMenuPages);


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

    private void Startup()
    {
        lastButton.interactable = true;
        LastGO.SetActive(false);
        lastButton.interactable = true;
        lastButton = All_ItemsButton;
        All_ItemsButton.interactable = false;

        foreach (var item in DataStoring.playerInventory)
        {
            foreach (var item2 in DataStoring.catalogItems)
            {
                if (item.ItemClass.Equals("basicItem", StringComparison.OrdinalIgnoreCase)
                    || item.ItemClass.Equals("consumable", StringComparison.OrdinalIgnoreCase) 
                    || item.ItemClass.Equals("growthItem", StringComparison.OrdinalIgnoreCase))
                {
                    Inventory[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                    Inventory[i].gameObject.name = $"{item.ItemId}";
                    Inventory[i].transform.SetParent(parentObject[0].gameObject.transform, false);
                    child = Inventory[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                    child.text = item.RemainingUses.ToString();
                    Inventory[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                    Inventory[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                }
            }
            i++;
        }
        All_Items_Page_GO.SetActive(true);
    }

    private void TabMenuPages()
    {
        var buttonName = EventSystem.current.currentSelectedGameObject.name;

        if (buttonName.Equals("All_Items", StringComparison.OrdinalIgnoreCase))
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = All_ItemsButton;
            All_ItemsButton.interactable = false;
            GetAllInventory();
            All_Items_Page_GO.SetActive(true);
        } 
        else if (buttonName.Equals("BasicItems", StringComparison.OrdinalIgnoreCase)) 
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = BasicItems_Button;
            BasicItems_Button.interactable = false;
            GetBasicItems();
            BasicItems_Page_GO.SetActive(true);
        }
        else if (buttonName.Equals("GrowthItems", StringComparison.OrdinalIgnoreCase))
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = GrowthItems_Button;
            GrowthItems_Button.interactable = false;
            GetGrowthItems();
            GrowthItems_Page_GO.SetActive(true);
        }
        else if (buttonName.Equals("Consumables", StringComparison.OrdinalIgnoreCase))
        {
            LastGO.SetActive(false);
            lastButton.interactable = true;
            lastButton = Consumable_Button;
            Consumable_Button.interactable = false;
            GetConsumableItems();
            Consumable_Page_GO.SetActive(true);
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
                    if (item.ItemClass.Equals("basicItem", StringComparison.OrdinalIgnoreCase)
                        || item.ItemClass.Equals("consumable", StringComparison.OrdinalIgnoreCase)
                        || item.ItemClass.Equals("growthItem", StringComparison.OrdinalIgnoreCase))
                    {
                        if (item.ItemId.Equals(item2.ItemId, StringComparison.OrdinalIgnoreCase))
                        {
                            Inventory[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                            Inventory[i].gameObject.name = $"{item.ItemId}";
                            Inventory[i].transform.SetParent(parentObject[0].gameObject.transform, false);
                            child = Inventory[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                            child.text = item.RemainingUses.ToString();
                            Inventory[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                            Inventory[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                        }
                    }
                }
                i++;
            }
        }
        DataStoring.isUpdateInventory = false;
    }

    public void GetBasicItems()
    {
        if (BasicItemList.Count == 0 || DataStoring.isUpdateInventory)
        {
            foreach (var item in DataStoring.basicItems)
            {
                foreach (var item2 in DataStoring.basicItemCata)
                {
                    if(item.ItemId.Equals(item2.ItemId, StringComparison.OrdinalIgnoreCase))
                    {
                        if (item.ItemClass.Equals("basicItem", StringComparison.OrdinalIgnoreCase))
                        {
                            BasicItemList[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                            BasicItemList[i].gameObject.name = $"{item.ItemId}";
                            BasicItemList[i].transform.SetParent(parentObject[1].gameObject.transform, false);
                            child = BasicItemList[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                            child.text = item.RemainingUses.ToString();
                            BasicItemList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                            BasicItemList[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                        }
                    }
                }
                i++;
            }
        }
        DataStoring.isUpdateInventory = true;
    }

    public void GetGrowthItems()
    {
        if(GrowthList.Count == 0 || DataStoring.isUpdateInventory)
        {
            foreach (var item in DataStoring.growthItem)
            {
                foreach (var item2 in DataStoring.grwothItemsCata)
                {
                    if (item.ItemClass.Equals("growthItem", StringComparison.OrdinalIgnoreCase))
                    {
                        GrowthList[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                        GrowthList[i].gameObject.name = $"{item.ItemId}";
                        GrowthList[i].transform.SetParent(parentObject[2].gameObject.transform, false);
                        child = GrowthList[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        child.text = item.RemainingUses.ToString();
                        GrowthList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                        GrowthList[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                    }
                }
                i++;
            }
        }
        DataStoring.isUpdateInventory = false;
    }

    public void GetConsumableItems()
    {
        if(ConsumablesList.Count == 0 || DataStoring.isUpdateInventory)
        {
            foreach (var item in DataStoring.consumablesItems)
            {
                foreach (var item2 in DataStoring.consumablesItemsCata)
                {
                    if (item.ItemClass.Equals("consumable", StringComparison.OrdinalIgnoreCase))
                    {
                        ConsumablesList[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
                        ConsumablesList[i].gameObject.name = $"{item.ItemId}";
                        ConsumablesList[i].transform.SetParent(parentObject[3].gameObject.transform, false);
                        child = ConsumablesList[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        child.text = item.RemainingUses.ToString();
                        ConsumablesList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(item2.ItemImageUrl);
                        ConsumablesList[i].GetComponent<Button>().onClick.AddListener(ClickedItem);
                    }
                }
                i++;
            }
        }
        DataStoring.isUpdateInventory = false;
    }

    private void ClickedItem()
    {
        string itemClicked = EventSystem.current.currentSelectedGameObject.name;
        Dictionary<CatalogItem, int> bundleHolder = new Dictionary<CatalogItem, int>(inventorySystem.GetBundleCount(itemClicked));

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
                            Debug.Log($"{item2.ItemId} to {item.ItemId}.");
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
        }
        catch (Exception ex) { Debug.Log(ex); }
    }
}