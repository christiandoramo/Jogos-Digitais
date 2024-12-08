using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public WaveManager waveManager;

    public TextMeshProUGUI enemiesCounterUI;
    public TextMeshProUGUI wavesCounterUI;
    public TextMeshProUGUI dayTimerCounterUI;

    public GameObject victoryPanel;

    public static GameManager instance;
    public RectTransform enemyImage;
    float rotationAngle;

    private bool nightHasStarted;

    private float dayTimer = 0f;
    public float sunnyTime = 10f;

    void Awake()
    {
        // GameManager é único na cena
        if (instance == null)
        {
            instance = this; // definindo a instancia do objeto como estática para ser acessado como um
            // singleton em outros arquivos
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        waveManager = Instantiate(waveManager);
        waveManager.Initialize();

        if (victoryPanel != null)
            victoryPanel.SetActive(false); // desativa menu de parabens
    }

    private void TimeManage()
    {
        float second = Time.deltaTime;
        dayTimer += second;
        if (dayTimer <= sunnyTime && nightHasStarted)
        {
            nightHasStarted = false;
        }
        else if (waveManager.currentWave > waveManager.waveAmount && waveManager.currentEnemies <= 0)
        {
            waveManager.currentWave--; // corrigi o waveManager
            Win(); // ganha quando nao tem inimigos e finalizou waves
        }
        else if (nightHasStarted && waveManager.currentWave <= waveManager.waveAmount) // todos inimigos eliminados
        {
            if (waveManager.currentEnemies <= 0)
            {
                dayTimer = 0;
                nightHasStarted = false;  // termina noite ao acabar wave
            }
        }
        else if (dayTimer > sunnyTime && waveManager.currentWave <= waveManager.waveAmount && nightHasStarted ==false)
        {
            waveManager.GenerateNewWave();
            nightHasStarted = true;
        }
    }
    private void Update()
    {
        TimeManage();
        UpdateWaveCount();
        rotationAngle = 360f * Time.deltaTime; //metade de 1 volta em 1 segundo
        enemyImage.Rotate(new Vector3(0, 1, 0), rotationAngle, Space.Self); // rotação do inimigo na UI
    }

    private void UpdateWaveCount()
    {
        if (enemiesCounterUI != null && wavesCounterUI != null && dayTimerCounterUI != null)
        {
            enemiesCounterUI.text = $"{waveManager.currentEnemies}/{waveManager.currentEnemiesWaveAmount}";
            wavesCounterUI.text = $"{waveManager.currentWave}/{waveManager.waveAmount}";
            dayTimerCounterUI.text = $"{(int) dayTimer}";

        }
    }

    private void Win()
    {
        Time.timeScale = 0; // pausa jogo

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // recarrega cena - "restart"
    }
}
