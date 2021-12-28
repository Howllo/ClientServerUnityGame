using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPreviewScript : MonoBehaviour
{
    [SerializeField] private ButtonMenuToggles menuToggles;
    [SerializeField] private Button getCurrentButton;

    // Start is called before the first frame update
    void Start()
    {
        getCurrentButton = this.GetComponent<Button>();
        getCurrentButton.onClick.AddListener(menuToggles.ClickOnMenuButton);
    }
}
