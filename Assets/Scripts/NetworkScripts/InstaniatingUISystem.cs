using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using DataStoringIDError;

public class InstaniatingUISystem : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private AudioManagement audioManagement;
    [SerializeField] private InventorySystem inventorySystem;

    [Header("Important Information")]
    [SerializeField] private GameObject receivedRewardPopup;
    [SerializeField] private GameObject userInterfaceDisplay;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private GameObject characterPortrait;
    public float WaitSeconds = 0.5f;
    private bool runOnce = true;

    [Header("Arrays")]
    private GameObject[] GetClickableButtonObjects = new GameObject[8];
    private GameObject[] getAgentCharacterPortriat = new GameObject[18];
    private GameObject[] getOperatorCharacterPortiat = new GameObject[4];

    private void Update()
    {
        if (!runOnce)
        {
            for (int i = 0; i < GetClickableButtonObjects.Length; i++)
            {
                Destroy(GetClickableButtonObjects[i]);
            }
            runOnce = true;
        }
    }

    /// <summary>
    /// Display all reward that the player recieved. Max of 8 rewards are allowed and bundles only.
    /// </summary>
    /// <param name="IncomingItem"></param>
    public IEnumerator GetRewardPopup(string IncomingItem)
    {
        runOnce = false;
        Dictionary<CatalogItem, int> BundlePairs = new Dictionary<CatalogItem, int>(inventorySystem.GetBundleCount(IncomingItem));
        TextMeshProUGUI child;
        CatalogItem tempItem;
        int i = 0;
        bool setActiveOnce = false;

        foreach (var item in DataStoring.catalogItems)
        {
            if (item.ItemId == IncomingItem)
            {
                tempItem = item;
            }
        }

        foreach (var items in BundlePairs)
        {
            if (i > 8)
                break;

            yield return new WaitForSeconds(WaitSeconds);
            if (!setActiveOnce)
            {
                receivedRewardPopup.SetActive(true);
                setActiveOnce = true;
            }
            GetClickableButtonObjects[i] = Instantiate(userInterfaceDisplay, transform.position, Quaternion.identity);
            GetClickableButtonObjects[i].name = $"{i}+";
            GetClickableButtonObjects[i].transform.SetParent(parentObject.gameObject.transform, false);
            child = GetClickableButtonObjects[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            child.text = items.Value.ToString();
            if (items.Key.ItemImageUrl != null)
            {
                GetClickableButtonObjects[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(items.Key.ItemImageUrl);
            }   
            audioManagement.PlaySoundClip(1);
            i++;
        }
    }

    /// <summary>
    /// Instantiate character UI through squadrons or veiwing the operators. They will be the same thing.
    /// </summary>
    /// <param name="ItemKey"></param>
    /// <param name="sortingSystem"></param>
    /// <param name="parentObject"></param>
    /// <param name="agentOrOperator"></param>
    public void InstantiateCharacterUI(string ItemKey, string sortingSystem, GameObject parentGameObject, string agentOrOperator)
    {
        int i = 0;
        TextMeshProUGUI child;

        if (agentOrOperator.Equals("Agent"))
        {
            foreach (var item in DataStoring.agencyCharacters)
            {
                if (item.DisplayName == ItemKey)
                {
                    getAgentCharacterPortriat[i] = Instantiate(userInterfaceDisplay, transform.position, Quaternion.identity);
                    getAgentCharacterPortriat[i].name = item.DisplayName;
                    getAgentCharacterPortriat[i].transform.SetParent(parentGameObject.transform.transform, false);
                    //getAgentCharacterPortriat[i].transform.GetChild(2).GetComponent<>
                    child = getAgentCharacterPortriat[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    child.text = item.CustomData["Level"].ToString();
                    if(item.CustomData["CharacterPicture"] != null)
                    {
                        GetClickableButtonObjects[i].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(item.CustomData["CharacterPicture"]);
                    }
                }
                i++;
            }
        } 
        else if (agentOrOperator.Equals("Operator"))
        {
            foreach (var item in DataStoring.operatorCharacter)
            {
                if (item.DisplayName == ItemKey)
                {
                    getAgentCharacterPortriat[i] = Instantiate(userInterfaceDisplay, transform.position, Quaternion.identity);
                    getAgentCharacterPortriat[i].name = item.DisplayName;
                    getAgentCharacterPortriat[i].transform.SetParent(parentGameObject.transform.transform, false);
                    child = getAgentCharacterPortriat[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    child.text = item.CustomData["Level"].ToString();
                    if (item.CustomData["CharacterPicture"] != null)
                    {
                        GetClickableButtonObjects[i].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(item.CustomData["CharacterPicture"]);
                    }
                }
                i++;
            }
            i++;
        }
        i = 0;
    }

    private void OnDisable()
    {
        for(int i =0; i < GetClickableButtonObjects.Length; i++)
        {
            Destroy(GetClickableButtonObjects[i]);
        }
    }
}