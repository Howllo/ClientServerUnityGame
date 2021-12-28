using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class TabbingMenu : MonoBehaviour
{
    public GameObject currentObject = null;
    public TextMeshProUGUI getFirstInputText;
    public TextMeshProUGUI getSecondInputText;
    public Button button;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(currentObject);
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            GameObject c = EventSystem.current.currentSelectedGameObject;
            if (c == null) return;

            Selectable s = c.GetComponent<Selectable>();
            if (s == null) return;

            Selectable jump = Keyboard.current.shiftKey.isPressed ? s.FindSelectableOnUp() : s.FindSelectableOnDown();

            // try similar direction
            if (!jump)
            {
                jump = Keyboard.current.shiftKey.isPressed ? s.FindSelectableOnLeft() : s.FindSelectableOnRight();
                if (!jump) return;
            }
            jump.Select();
        }

        if(getFirstInputText != null && getSecondInputText != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                button.onClick.Invoke();
            }
        }
    }
}