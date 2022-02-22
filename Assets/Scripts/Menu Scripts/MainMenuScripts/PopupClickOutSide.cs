using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupClickOutSide : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI outsideClose;
    [SerializeField] private GameObject outsideCloseGO, closeButton;
    [SerializeField] private Button outsideButton;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_64
        outsideClose.text = "Click outside to close.";
        closeButton.SetActive(false);
        outsideButton.interactable = true;
#endif
#if UNITY_IOS
        outsideClose.text = "Tap outside to close.";
        closeButton.SetActive(false);
        outsideButton.interactable = true;
#endif
#if UNITY_ANDRIOD
        outsideClose.text = "Tap outside to close";
        closeButton.SetActive(false);
        outsideButton.interactable = true;
#endif
#if UNITY_XBOXONE
        outsideCloseGO.SetActive(false);
        closeButton.SetActive(true);
        outsideButton.interactable = false;
#endif
#if UNITY_PS5
        outsideCloseGO.SetActive(false);
        closeButton.SetActive(true);
        outsideButton.interactable = false;
#endif
#if UNITY_SWITCH
        outsideCloseGO.SetActive(false);
        closeButton.SetActive(true);
        outsideButton.interactable = false;
#endif
    }
}
