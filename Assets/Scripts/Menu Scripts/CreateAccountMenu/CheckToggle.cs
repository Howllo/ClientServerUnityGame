using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckToggle : MonoBehaviour
{
    //Public Variables
    public bool isToggle = false;

    //Private Variables
    [SerializeField] private Toggle termsToggle;
    [SerializeField] private Toggle olderThan13Toggle;
    [SerializeField] private Toggle acceptAll;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button buttonClose;
    private bool isAcceptAllToggled = false;
    private bool runOnceBoolAcceptToggleTrue = false;
    private bool runOnceFalseTerms = false;

    void Start()
    {
        confirmButton.onClick.AddListener(TaskOnClick);
        buttonClose.onClick.AddListener(TaskOnClick);
    }

    void Update()
    {
        if(confirmButton != null) {
            //Good Luck. 
            //Prevents the updater from constnatly setting one of the statements to true or false.
            //This allows untoggling when player clicks or taps on of the toggles.
            if (acceptAll.isOn && !isAcceptAllToggled)
            {
                termsToggle.isOn = true;
                olderThan13Toggle.isOn = true;
                isAcceptAllToggled = true;
            }
            else if (!acceptAll.isOn && isAcceptAllToggled && runOnceBoolAcceptToggleTrue)
            {
                termsToggle.isOn = false;
                olderThan13Toggle.isOn = false;
                isAcceptAllToggled = false;
                runOnceBoolAcceptToggleTrue = false;
            }

            if (!termsToggle.isOn && !runOnceFalseTerms || !olderThan13Toggle.isOn && !runOnceFalseTerms)
            {
                acceptAll.isOn = false;
                runOnceBoolAcceptToggleTrue = false;
                runOnceFalseTerms = true;
            }
            else if (termsToggle.isOn && olderThan13Toggle.isOn)
            {
                runOnceFalseTerms = false;
                if (!runOnceBoolAcceptToggleTrue)
                {
                    acceptAll.isOn = true;
                    runOnceBoolAcceptToggleTrue = true;
                }
                isToggle = true;
            }
            else
                isToggle = false;
        }
    }

    void TaskOnClick()
    {
        termsToggle.isOn = false;
        olderThan13Toggle.isOn = false;
        acceptAll.isOn = false;
        isAcceptAllToggled = false;
        runOnceBoolAcceptToggleTrue = false;
        runOnceFalseTerms = false;
    }
}
