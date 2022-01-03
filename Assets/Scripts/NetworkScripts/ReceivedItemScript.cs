using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using DataStoringIDError;

public class ReceivedItemScript : MonoBehaviour
{
    [SerializeField] private AudioManagement audioManagement;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameObject receivedRewardPopup;
    [SerializeField] private GameObject clickableReward;
    [SerializeField] private GameObject parentObject;
    private TextMeshProUGUI[] getAllText;
    private GameObject[] GetGameObject;

    private void Start()
    {
        GetGameObject[0] = clickableReward;
    }

    /// <summary>
    /// Dispaly all reward that the player recieved. Max of 8 rewards are allowed.
    /// </summary>
    /// <param name="WhatReward"></param>
    /// <param name="AmountItems"></param>
    public void GetRewardPopup(ItemInstance[] WhatReward, int[] AmountItems)
    {
        var newItem = Instantiate(clickableReward, transform.position, Quaternion.identity);
        Image imageChild;
        TextMeshProUGUI child;

        for (float i = AmountItems.Length; i > 0; i -= Time.deltaTime)
        {
            foreach(var getItemID in DataStoring.catalogItemsCombined)
            {
                if(getItemID.ItemId == WhatReward[(int)i].ItemId)
                {
                    imageChild = Resources.Load<Image>(getItemID.ItemImageUrl);
                    break;
                }
            }
            newItem.transform.SetParent(parentObject.gameObject.transform, false);
            GetGameObject[(int)i] = newItem;
            child = GetGameObject[(int)i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            child.text = AmountItems.ToString();

            audioManagement.PlaySoundClip(1);
            if (Input.anyKey)
            {
                for(int k = (int)i; k < AmountItems.Length; k++)
                {
                    foreach (var getItemID in DataStoring.catalogItemsCombined)
                    {
                        if (getItemID.ItemId == WhatReward[(int)i].ItemId)
                        {
                            imageChild = Resources.Load<Image>(getItemID.ItemImageUrl);
                            break;
                        }
                    }
                    newItem.transform.SetParent(parentObject.gameObject.transform, false);
                    GetGameObject[k] = newItem;
                    child = GetGameObject[k].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                    child.text = AmountItems.ToString();
                    audioManagement.PlaySoundClip(1);
                }
                break;
            }
        }
        receivedRewardPopup.SetActive(true);
    }

    private void OnDisable()
    {
        for(int i =0; i < GetGameObject.Length; i++)
        {
            Destroy(GetGameObject[i]);
        }
    }
}
