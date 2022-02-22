using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataStoringIDError;

public class PlayerCurrencyBarScript : MonoBehaviour
{
    [SerializeField] private Button agencyResourcesButtons;
    [SerializeField] private Button ethercreditButtons;
    [SerializeField] private Button astralcreditsButtons;
    [SerializeField] private LevelLoader levelLoader;


    // Start is called before the first frame update
    void Start()
    {
        agencyResourcesButtons.onClick.AddListener(ClickedButton);
        ethercreditButtons.onClick.AddListener(ClickedButton);
        astralcreditsButtons.onClick.AddListener(ClickedButton);
    }

    private void ClickedButton()
    {
        DataStoring.isResourceBarClicked = true;
        levelLoader.LoadLevel(2);
    }
}
