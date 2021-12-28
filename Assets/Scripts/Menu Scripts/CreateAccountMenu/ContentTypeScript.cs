using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContentTypeScript : MonoBehaviour
{
    [SerializeField] Toggle currentToggle;
    [SerializeField] TMP_InputField getInputField;

    private void Start()
    {
        currentToggle = this.GetComponent<Toggle>();
        getInputField = getInputField.GetComponent<TMP_InputField>();
    }

    private void Update()
    {
        if (currentToggle.isOn)
        {
            getInputField.contentType = TMP_InputField.ContentType.Standard;
            getInputField.ForceLabelUpdate();
        }
        else if (!currentToggle.isOn)
        {
            getInputField.contentType = TMP_InputField.ContentType.Password;
            getInputField.ForceLabelUpdate();
        }
    }
}
