using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;

public class RememberMeEncryption : MonoBehaviour
{
    [SerializeField] private AccountSystem accountSystem;
    private string EncryptionOrDecryption = "", SingleOrDouble = "", GetID = "", getCurrentScene = "";
    private bool WaitForPlayerTitleID = false;

    private void Start()
    {
        getCurrentScene = SceneManager.GetActiveScene().name;
        if (getCurrentScene != "AuthenticationMenu")
            GetPlayerTitleID();
    }

    /// <summary>
    /// This is for accounts that are already online. 
    /// </summary>
    /// <returns></returns>
    public void GetPlayerTitleID()
    {
        PlayFabClientAPI.GetPlayerCombinedInfo(new GetPlayerCombinedInfoRequest
        {
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true
            }
        }, results =>
        {
            GetID = results.InfoResultPayload.AccountInfo.TitleInfo.TitlePlayerAccount.Id.ToString();
            WaitForPlayerTitleID = true;
        }, OnError);
    }

    /// <summary>
    /// Encrypts both e-mail and password. REMEMBER: This requires account to be login before it can be executed.
    /// </summary>
    /// <param name="GetPlayerAccountEmail"></param>
    /// <param name="GetPlayerAccountPass"></param>
    /// <returns></returns>
    public IEnumerator EncrptAccount(string GetPlayerAccountEmail, string GetPlayerAccountPass)
    {
        yield return new WaitUntil(() => WaitForPlayerTitleID);
        EncryptionOrDecryption = "Encryption";
        SingleOrDouble = "Double";
        Debug.Log("Get Public Key" + GetID);

        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "EncryptAccount",
            FunctionParameter = new 
            { 
                GetPlayerAccountEmail, 
                GetPlayerAccountPass, 
                EncryptionOrDecryption, 
                SingleOrDouble, 
                GetID,
            }, GeneratePlayStreamEvent = true
        }, results =>
        {
            RetrieveFromJson JsonObject = JsonUtility.FromJson<RetrieveFromJson>(results.FunctionResult.ToString());
            PlayerPrefs.SetString("PlayerEmail", JsonObject.GetPlayerAccountEmail);
            PlayerPrefs.SetString("PlayerPass", JsonObject.GetPlayerAccountPass);
            PlayerPrefs.SetString("GetPublic", JsonObject.GetPublic);
            PlayerPrefs.Save();
        }, OnError);
    }

    /// <summary>
    /// Encrypts any type of single piece information. This will return a string. For online use only.
    /// </summary>
    /// <param name="GetIncomingInfo"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator EncrptSingleInfo(string GetIncomingInfo, Action<string> callback)
    {
        EncryptionOrDecryption = "Encryption";
        SingleOrDouble = "Single";
        yield return new WaitUntil(() => WaitForPlayerTitleID);
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "EncryptAccount",
            FunctionParameter = new
            {
                GetIncomingInfo,
                EncryptionOrDecryption,
                SingleOrDouble,
                GetID
            },
            GeneratePlayStreamEvent = true
        }, results =>
        {
            RetrieveFromJson JsonObject = JsonUtility.FromJson<RetrieveFromJson>(results.FunctionResult.ToString());
            callback(JsonObject.GetIncomingInfo);
        }, OnError);
    }

    /// <summary>
    /// Encrypts any type of single piece information. This will return a string. For online use only.
    /// This function also returns the public key in a encrypted form.
    /// </summary>
    /// <param name="GetIncomingInfo"></param>
    /// <param name="callback"></param>
    /// <param name="GetPublic"></param>
    /// <returns></returns>
    public IEnumerator EncrptSingleInfo(string GetIncomingInfo, Action<string> callback, bool GetPublic)
    {
        EncryptionOrDecryption = "Encryption";
        SingleOrDouble = "Single";
        yield return new WaitUntil(() => WaitForPlayerTitleID);
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "EncryptAccount",
            FunctionParameter = new
            {
                GetIncomingInfo,
                EncryptionOrDecryption,
                SingleOrDouble,
                GetID,
                GetPublic
            },
            GeneratePlayStreamEvent = true
        }, results =>
        {
            RetrieveFromJson JsonObject = JsonUtility.FromJson<RetrieveFromJson>(results.FunctionResult.ToString());
            PlayerPrefs.SetString("GetPublic", JsonObject.GetPublic);
            callback(JsonObject.GetIncomingInfo);
        }, OnError);
    }

    /// <summary>
    /// Decrypts account information from the players PlayerPrefs. This requires AccountSystem.
    /// </summary>
    /// <param name="GetPlayerAccountEmail"></param>
    /// <param name="GetPlayerAccountPass"></param>
    public IEnumerator DecrptionOfAccount(string GetPlayerAccountEmail, string GetPlayerAccountPass)
    {
        EncryptionOrDecryption = "Decryption";
        SingleOrDouble = "Double";
        yield return new WaitUntil(() => WaitForPlayerTitleID);

        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "EncryptAccount",
            FunctionParameter = new 
            {   
                GetPlayerAccountEmail,
                GetPlayerAccountPass, 
                GetID, 
                EncryptionOrDecryption,
                SingleOrDouble,
            }, GeneratePlayStreamEvent = true
        }, results =>
        {
            RetrieveFromJson JsonObject = JsonUtility.FromJson<RetrieveFromJson>(results.FunctionResult.ToString());
            accountSystem.LoginThroughRememberMe(JsonObject.GetPlayerAccountPass, JsonObject.GetPlayerAccountPass);
        }, OnError);
    }

    /// <summary>
    /// Decrypts single piece of information. This will return a result.
    /// </summary>
    /// <param name="GetIncomingInfo"></param>
    /// <returns></returns>
    public IEnumerator DecrptionOfSingle(string GetIncomingInfo, Action<string> callback)
    {
        EncryptionOrDecryption = "Decryption";
        SingleOrDouble = "Single";
        yield return new WaitUntil(() => WaitForPlayerTitleID);

        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "EncryptAccount",
            FunctionParameter = new
            {
                GetIncomingInfo,
                GetID,
                EncryptionOrDecryption,
                SingleOrDouble,
            },
            GeneratePlayStreamEvent = true

        }, results =>
        {
            RetrieveFromJson JsonObject = JsonUtility.FromJson<RetrieveFromJson>(results.FunctionResult.ToString());
            callback(JsonObject.GetIncomingInfo);
        }, OnError);
    }

    /// <summary>
    /// If game is switching scenes and GetPlayerTitleID does not load fast enough, use SingleDecrpytionOffline.
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator SingleDecrpytionOffline(string GetIncomingInfo, Action<string> callback)
    {
        OfflineToJson offlineJson = new OfflineToJson();
        if (PlayerPrefs.GetString("WhatLoginWasUsed") == "guessLogin")
        {
            offlineJson.GetIncomingInfo = GetIncomingInfo;
            offlineJson.SingleOrDouble = "Single";
            offlineJson.GetPublic = PlayerPrefs.GetString("GetPublic");
            offlineJson.WhatLogin = PlayerPrefs.GetString("WhatLoginWasUsed");
        } else
        {
            offlineJson.GetIncomingInfo = GetIncomingInfo;
            offlineJson.SingleOrDouble = "Single";
            offlineJson.GetPublic = PlayerPrefs.GetString("GetPublic");
            offlineJson.WhatLogin = PlayerPrefs.GetString("WhatLoginWasUsed");
        }
        string jsonData = JsonUtility.ToJson(offlineJson);

        var request = new UnityWebRequest("https://iderrortdfunctions.azurewebsites.net/api/GetOfflineEncryption?code=dMdcjBHWLWtxviNPLuhCYefYnTZW9WX7PC8AvZVLMjZ456cHDiRnMQ==", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.Log(request.error);
        else if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResults = request.downloadHandler.text;
            OfflineToJson JsonObject = JsonUtility.FromJson<OfflineToJson>(jsonResults);
            callback(JsonObject.GetIncomingInfo);
        }
    }

    /// <summary>
    /// Login to the game without playfab being required to login with a custom Azure Function call.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetRememberLogin()
    {
        OfflineToJson offlineJson = new OfflineToJson();
        offlineJson.GetPlayerAccountEmail = PlayerPrefs.GetString("PlayerEmail");
        offlineJson.GetPlayerAccountPass = PlayerPrefs.GetString("PlayerPass");
        offlineJson.SingleOrDouble = "Double";
        offlineJson.GetPublic = PlayerPrefs.GetString("GetPublic");
        string jsonData = JsonUtility.ToJson(offlineJson);

        var request = new UnityWebRequest("https://iderrortdfunctions.azurewebsites.net/api/GetOfflineEncryption?code=dMdcjBHWLWtxviNPLuhCYefYnTZW9WX7PC8AvZVLMjZ456cHDiRnMQ==", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else if (request.result == UnityWebRequest.Result.Success)
            GetOfflinePlayfabSignin(request.downloadHandler.text);
    }

    private void GetOfflinePlayfabSignin(string jsonString)
    {
        RetrieveFromJson JsonObject = JsonUtility.FromJson<RetrieveFromJson>(jsonString);
        accountSystem.LoginThroughRememberMe(JsonObject.GetPlayerAccountEmail, JsonObject.GetPlayerAccountPass);
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error);
    }
}

[Serializable]
public class RetrieveFromJson
{
    public string GetPlayerAccountEmail;
    public string GetPlayerAccountPass;
    public string GetPublic;
    public string GetIncomingInfo;
}

[Serializable]
public class OfflineToJson
{
    public string SingleOrDouble;
    public string GetPlayerAccountEmail;
    public string GetPlayerAccountPass;
    public string GetPublic;
    public string GetIncomingInfo;
    public string Offline;
    public string WhatLogin;
}