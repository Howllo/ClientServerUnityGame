using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWaitForSeconds : MonoBehaviour
{
    [SerializeField] private float WaitForSecondsCount = 3f;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        MakeButtonInteractable(WaitForSecondsCount);
    }

    private IEnumerator MakeButtonInteractable(float waitAmount)
    {
        button.interactable = false;
        yield return new WaitForSeconds(waitAmount);
        button.interactable = true;
    }
}
