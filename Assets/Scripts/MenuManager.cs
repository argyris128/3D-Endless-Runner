using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button ContinueButton;

    void Awake()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            if(PlayerPrefs.HasKey("SaveExists"))
                ContinueButton.interactable = true;
            else
                ContinueButton.interactable = false;
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
