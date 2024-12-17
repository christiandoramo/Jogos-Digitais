using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] string githubUrl = "https://www.github.com/christiandoramo";

    public void NewGame()
    {
        if (Time.timeScale == 0) Time.timeScale = 1;
        SceneManager.LoadScene("Fase1"); // recarrega cena - "restart"
    }
    public void GoToCredits()
    {
        menuPanel?.SetActive(false);
        creditsPanel?.SetActive(true);
    }

    public void BackToMenu()
    {
        creditsPanel?.SetActive(false);
        menuPanel?.SetActive(true);
    }

    public void OpenWebsite()
    {
        Application.OpenURL(githubUrl);
    }
}
