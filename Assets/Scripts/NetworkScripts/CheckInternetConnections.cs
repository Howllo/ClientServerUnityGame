using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class CheckInternetConnections : MonoBehaviour
{
    [Header("Display Information")]
    [SerializeField] private string URL = "https://iderrortdfunctions.azurewebsites.net/api/CheckPlayerConnection?code=nUDvXfn0svS7XkKwyoDRVmVR1kN3TpTRYrTP3PA6UKiirnQ1rsa5xA==";

    //Public
    [Header("Monitoring Checks")]
    public int howManyPingTest = 0;
    public bool isConnected = true;
    public float floatInformation = 0.0f;
    public bool hasRan = true;

    [Header("Game Objects")]
    public TextMeshProUGUI tryReconnectText;
    public GameObject errorNetworkPopup;
    [SerializeField] private LevelLoader levelLoader;

    public Button retryButton;
    public Button cancelButton;

    private void Start()
    {
        retryButton.onClick.AddListener(RetryConnection);
        cancelButton.onClick.AddListener(GoBackToMainMenu);
        StartCoroutine(CheckConnection());
    }

    /// <summary>
    /// Check for a internet connection through Azure Function Call.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckConnection()
    {
        while (isConnected)
        {
            if (isConnected)
            {
                for (float i = 10f; i > 0f; i -= Time.deltaTime)
                    yield return null;

                GetConnection getConnection = new GetConnection();
                getConnection.Ping = "Ping";
                string jsonData = JsonUtility.ToJson(getConnection);

                var request = new UnityWebRequest(URL, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    isConnected = false;
                    errorNetworkPopup.SetActive(true);
                    tryReconnectText.gameObject.SetActive(false);
                }
                else if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResults = request.downloadHandler.text;
                    GetConnection JsonObject = JsonUtility.FromJson<GetConnection>(jsonResults);
                    if (JsonObject.Ping == "Pong")
                    {
                        isConnected = true;
                        errorNetworkPopup.SetActive(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Retry a conenctions every 5 seconds.
    /// </summary>
    /// <returns></returns>
    public IEnumerator QuickCheckConnection()
    {
        tryReconnectText.gameObject.SetActive(true);
        while (!isConnected)
        {
            for (float i = 5f; i > 0f; i -= Time.deltaTime)
            {
                tryReconnectText.text = "Retrying connection in: " + i.ToString("n0") + " second(s).";
                if (!isConnected)
                    floatInformation += Time.deltaTime;
                if (floatInformation >= 90)
                    levelLoader.LoadLevel(0);
                yield return null;
            }

            GetConnection getConnection = new GetConnection();
            getConnection.Ping = "Ping";
            string jsonData = JsonUtility.ToJson(getConnection);
            var request = new UnityWebRequest(URL, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                isConnected = false;
                errorNetworkPopup.SetActive(true);
                tryReconnectText.gameObject.SetActive(false);
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResults = request.downloadHandler.text;
                GetConnection JsonObject = JsonUtility.FromJson<GetConnection>(jsonResults);
                if (JsonObject.Ping == "Pong")
                {
                    isConnected = true;
                    errorNetworkPopup.SetActive(false);
                }
            }
            howManyPingTest++;
        } 
        if(isConnected)
            StartCoroutine(CheckConnection());
    }

    private void RetryConnection()
    {
        StartCoroutine(QuickCheckConnection());
    }

    private void GoBackToMainMenu()
    {
        levelLoader.LoadLevel(0);
    }
}

[Serializable]
public class GetConnection
{
    public string Ping;
}