using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using DataStoringIDError;

public class ReceivedItemScript : MonoBehaviour
{
    [SerializeField] private AudioManagement audioManagement;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameObject receivedRewardPopup;
    [SerializeField] private GameObject clickableReward;
    [SerializeField] private GameObject parentObject;
    private GameObject[] GetClickableButtonObjects = new GameObject[8];
    public float WaitSeconds = 0.5f;

    /// <summary>
    /// Display all reward that the player recieved. Max of 8 rewards are allowed and bundles only.
    /// </summary>
    /// <param name="IncomingItem"></param>
    public IEnumerator GetRewardPopup(string IncomingItem)
    {
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
            GetClickableButtonObjects[i] = Instantiate(clickableReward, transform.position, Quaternion.identity);
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

    private void OnDisable()
    {
        for(int i =0; i < GetClickableButtonObjects.Length; i++)
        {
            Destroy(GetClickableButtonObjects[i]);
        }
    }
}
