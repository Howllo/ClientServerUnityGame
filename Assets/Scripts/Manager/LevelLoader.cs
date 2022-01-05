using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animation getAnimation;
    [SerializeField] private GetAccountInfoScript getAccountInfoScript;
    private string getCurrentSceneName = "";

    private void Awake()
    {
        getCurrentSceneName = SceneManager.GetActiveScene().name;
        if(getCurrentSceneName != "AuthenticationMenu")
            loadingScreen.SetActive(true);
    }

    private void OnEnable()
    {
        if (getCurrentSceneName != "MainMenu")
            StartCoroutine(DelayUnlockedOtherThanMainMenu());
        else
            StartCoroutine(DelayUnlockForBool());
    }

    private IEnumerator DelayUnlockedOtherThanMainMenu()
    {
        yield return new WaitForSeconds(0.65f);
        if (getCurrentSceneName != "MainMenu")
            loadingScreen.SetActive(false);
    }

    private IEnumerator DelayUnlockForBool()
    {
        yield return new WaitUntil(() => getAccountInfoScript.isServerStillLoading == false);
        if (getAccountInfoScript != null && !getAccountInfoScript.isServerStillLoading)
            loadingScreen.SetActive(false);
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
        return;
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        getAnimation.Play();

        while (!operation.isDone)
            yield return null;
    }
}
