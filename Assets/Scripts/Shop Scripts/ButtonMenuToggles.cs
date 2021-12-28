using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataStoringIDError;

public class ButtonMenuToggles : MonoBehaviour
{
    [SerializeField] private Button packageButton;
    [SerializeField] private Button resourceButton;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button agentCostumeButton;
    [SerializeField] private Button operatorCostumeButton;
    [SerializeField] private Button mileageButton;
    [SerializeField] private Button agentGrowthButton;
    [SerializeField] private Button AddAgencyResourcesButton;
    [SerializeField] private Button AddEthercreditButton;
    [SerializeField] private Button AddAstralCreditButton;
    private Button lastButton;

    [SerializeField] private RectTransform focusHighlight;
    [SerializeField] private RectTransform packageRect;
    [SerializeField] private RectTransform resourceRect;
    [SerializeField] private RectTransform equipmentRect;
    [SerializeField] private RectTransform agentCostumeRect;
    [SerializeField] private RectTransform operatorCostumeRect;
    [SerializeField] private RectTransform mileageRect;
    [SerializeField] private RectTransform agentGrowthRect;

    //Shops
    private GameObject lastGO;
    [SerializeField] private GameObject packageShop;
    [SerializeField] private GameObject resourceShop;
    [SerializeField] private GameObject equipmentShop;
    [SerializeField] private GameObject agentCostumeShop;
    [SerializeField] private GameObject operatorCostumeShop;
    [SerializeField] private GameObject mileageShop;
    [SerializeField] private GameObject agencyShop;
    [SerializeField] private GameObject shopGO;
    [SerializeField] private GameObject focusHighlightGO;

    //Holder
    private Vector3 holderVector;

    private void Start()
    {
        //Set LastButton
        lastButton = packageButton;
        lastGO = packageShop;
        holderVector = focusHighlight.localScale;
        OnClickPackageShop();

        packageButton.onClick.AddListener(OnClickPackageShop);
        resourceButton.onClick.AddListener(OnClickResourceShop);
        equipmentButton.onClick.AddListener(OnClickEquipmentShop);
        agentCostumeButton.onClick.AddListener(OnClickAgentCostumeShop);
        operatorCostumeButton.onClick.AddListener(OnClickOperatorCostumeShop);
        mileageButton.onClick.AddListener(OnClickMileageShop);
        agentGrowthButton.onClick.AddListener(OnClickAgencyGrowthShop);
        AddAgencyResourcesButton.onClick.AddListener(OnClickResourceShop);
        AddEthercreditButton.onClick.AddListener(OnClickResourceShop);
        AddAstralCreditButton.onClick.AddListener(OnClickResourceShop);

        if (DataStoring.isResourceBarClicked)
        {
            OnClickResourceShop();
            DataStoring.isResourceBarClicked = false;
        }
    }

    #region Primary Buttons
    private void OnClickPackageShop()
    {
        focusHighlightGO.SetActive(true);
        focusHighlight.localScale = holderVector;
        focusHighlight.anchoredPosition = packageRect.anchoredPosition;
        lastButton.interactable = true;
        lastGO.SetActive(false);
        packageShop.SetActive(true);
        lastGO = packageShop;
        lastButton = packageButton;
        packageButton.interactable = false;

        //Setting Color
        var CBButton = lastButton.colors;
        CBButton.disabledColor = Color.white;
        lastButton.colors = CBButton;
    }

    private void OnClickResourceShop()
    {
        focusHighlightGO.SetActive(true);
        focusHighlight.localScale = holderVector;
        focusHighlight.anchoredPosition = resourceRect.anchoredPosition;
        lastButton.interactable = true;
        lastGO.SetActive(false);
        resourceShop.SetActive(true);
        lastGO = resourceShop;
        lastButton = resourceButton;
        resourceButton.interactable = false;

        //Setting Color
        var CBButton = lastButton.colors;
        CBButton.disabledColor = Color.white;
        lastButton.colors = CBButton;
    }

    private void OnClickEquipmentShop()
    {
        focusHighlightGO.SetActive(true);
        focusHighlight.localScale = new Vector3(2.58252144f, focusHighlight.localScale.y, focusHighlight.localScale.z);
        focusHighlight.anchoredPosition = equipmentRect.anchoredPosition;
        lastButton.interactable = true;
        lastGO.SetActive(false);
        equipmentShop.SetActive(true);
        lastGO = equipmentShop;
        lastButton = equipmentButton;
        equipmentButton.interactable = false;

        //Setting Color
        var CBButton = lastButton.colors;
        CBButton.disabledColor = Color.white;
        lastButton.colors = CBButton;
    }

    private void OnClickAgentCostumeShop()
    {
        focusHighlightGO.SetActive(true);
        focusHighlight.localScale = new Vector3(3.74320531f, focusHighlight.localScale.y, focusHighlight.localScale.z);
        focusHighlight.anchoredPosition = agentCostumeRect.anchoredPosition;
        lastButton.interactable = true;
        lastGO.SetActive(false);
        agentCostumeShop.SetActive(true);
        lastGO = agentCostumeShop;
        lastButton = agentCostumeButton;
        agentCostumeButton.interactable = false;

        //Setting Color
        var CBButton = lastButton.colors;
        CBButton.disabledColor = Color.white;
        lastButton.colors = CBButton;
    }

    private void OnClickOperatorCostumeShop()
    {
        focusHighlightGO.SetActive(true);
        focusHighlight.localScale = new Vector3(4.40388727f, focusHighlight.localScale.y, focusHighlight.localScale.z);
        focusHighlight.anchoredPosition = operatorCostumeRect.anchoredPosition;
        lastButton.interactable = true;
        lastGO.SetActive(false);
        operatorCostumeShop.SetActive(true);
        lastGO = operatorCostumeShop;
        lastButton = operatorCostumeButton;
        operatorCostumeButton.interactable = false;

        //Setting Color
        var CBButton = lastButton.colors;
        CBButton.disabledColor = Color.white;
        lastButton.colors = CBButton;
    }

    private void OnClickMileageShop()
    {
        focusHighlightGO.SetActive(true);
        focusHighlight.localScale = holderVector;
        focusHighlight.anchoredPosition = mileageRect.anchoredPosition;
        lastButton.interactable = true;
        lastGO.SetActive(false);
        mileageShop.SetActive(true);
        lastGO = mileageShop;
        lastButton = mileageButton;
        mileageButton.interactable = false;

        //Setting Color
        var CBButton = lastButton.colors;
        CBButton.disabledColor = Color.white;
        lastButton.colors = CBButton;
    }

    private void OnClickAgencyGrowthShop()
    {
        focusHighlightGO.SetActive(true);
        focusHighlight.anchoredPosition = agentGrowthRect.anchoredPosition;
        focusHighlight.localScale = new Vector3(3.507759f, focusHighlight.localScale.y, focusHighlight.localScale.z);
        lastButton.interactable = true;
        lastGO.SetActive(false);
        agencyShop.SetActive(true);
        lastGO = agencyShop;
        lastButton = agentGrowthButton;
        agentGrowthButton.interactable = false;

        //Setting Color
        var CBButton = lastButton.colors;
        CBButton.disabledColor = Color.white;
        lastButton.colors = CBButton;
    }
    #endregion

    public void ClickOnMenuButton()
    {
        packageButton.interactable = true;
        resourceButton.interactable = true;
        equipmentButton.interactable = true;   
        agentCostumeButton.interactable = true;
        operatorCostumeButton.interactable = true;
        mileageButton.interactable = true;
        agentCostumeButton.interactable = true;
        focusHighlightGO.SetActive(false);
    }
}
