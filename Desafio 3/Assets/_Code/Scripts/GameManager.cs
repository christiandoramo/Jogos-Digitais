using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public WaveManager waveManager;
    public CollectableManager collectableManager;
    public BarrierManager barrierManager;


    [SerializeField] Light2D globalIlumination;

    [SerializeField] TextMeshProUGUI enemiesCounterUI;
    [SerializeField] TextMeshProUGUI wavesCounterUI;
    [SerializeField] TextMeshProUGUI dayTimerCounterUI;
    [SerializeField] Slider hpBarUI;
    [SerializeField] TextMeshProUGUI hpCounterUI;
    [SerializeField] Slider stBarUI;

    [SerializeField] TextMeshProUGUI starCounterUI;
    [SerializeField] TextMeshProUGUI seedCounterUI;
    [SerializeField] TextMeshProUGUI hpRegenCounterUI;
    [SerializeField] TextMeshProUGUI jumpCounterUI;
    [SerializeField] TextMeshProUGUI dmgCounterUI;


    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [NonSerialized] public bool isPaused = false;

    public static GameManager instance;

    public RectTransform enemyImage;
    float rotationAngle;

    private bool nightHasStarted;

    public float dayTimer = 0f;
    public float sunnyTime = 10f;
    private float colorTimeReference = 0f;
    private Color bgNightColor;
    private float gradientSpeed = 1.0f;
    private List<Portal> portalComponents;

    public GameObject player;
    public PlayerController playerController;

    void Start()
    {

        if (instance == null)
        {
            instance = this; // definindo a instancia do objeto como estática para ser acessado como um
                             // singleton em outros arquivos
        }
        else
        {
            Destroy(gameObject);
        }
        playerController = player.GetComponent<PlayerController>();

        hpBarUI.maxValue = playerController.maxHp;
        hpBarUI.minValue = 0;
        hpBarUI.value = playerController.hp;

        stBarUI.maxValue = playerController.maxStamina;
        stBarUI.minValue = 0;
        stBarUI.value = playerController.stamina;

        waveManager = Instantiate(waveManager);
        waveManager.playerTransform = player.transform.GetChild(0).transform;

        waveManager.Initialize();

        portalComponents = waveManager.portals.Select((portal) => portal.GetComponent<Portal>()).ToList<Portal>();

        collectableManager = Instantiate(collectableManager);

        collectableManager.Initialize();

        if (ColorUtility.TryParseHtmlString("#FF5F00", out bgNightColor))

            if (gameOverPanel != null)
                gameOverPanel.SetActive(false); // desativa menu de parabens
        if (victoryPanel != null)
            victoryPanel.SetActive(false); // desativa menu de parabens
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void TimeManage()
    {
        float second = Time.deltaTime;
        dayTimer += second;

        if (waveManager.currentWave + 1 > waveManager.waveAmount && waveManager.currentEnemies <= 0)
        {
            waveManager.currentWave--; // corrigi o waveManager
            WinGame(); // ganha quando nao tem inimigos e finalizou waves
        }
        else if (dayTimer <= sunnyTime && nightHasStarted)
        {
            nightHasStarted = false;
        }
        else if (nightHasStarted && waveManager.currentWave <= waveManager.waveAmount) // todos inimigos eliminados
        {
            if (waveManager.currentEnemies <= 0) // quando acaba wave - o que acontece
            {
                portalComponents.ForEach((portal) => portal.isActivated = false);
                dayTimer = 0;
                nightHasStarted = false;  // termina noite ao acabar wave
                globalIlumination.color = Color.white;
                globalIlumination.intensity = 1f;
                portalComponents = waveManager.portals.Select((portal) => portal.GetComponent<Portal>()).ToList<Portal>();
            }
            else // o que roda enquanto os inimigos vivos,
            {
                portalComponents.ForEach((portal) => portal.isActivated = true);
                colorTimeReference += Time.deltaTime * gradientSpeed;
                globalIlumination.color = Color.Lerp(Color.white, bgNightColor, Mathf.PingPong(colorTimeReference, 1f));
                globalIlumination.intensity = Mathf.Lerp(globalIlumination.intensity, 0.5f, Time.deltaTime * gradientSpeed);
            }
        }
        else if (dayTimer > sunnyTime && waveManager.currentWave <= waveManager.waveAmount && nightHasStarted == false)
        {
            waveManager.GenerateNewWave();
            barrierManager.waveHasChanged = true;
            nightHasStarted = true;
        }
    }
    private void Update()
    {
        if (playerController.hp <= 0) GameOver();
        TimeManage();
        //UpdateUI();
        rotationAngle = 360f * Time.deltaTime; //metade de 1 volta em 1 segundo
        enemyImage.Rotate(new Vector3(0, 1, 0), rotationAngle, Space.Self); // rotação do inimigo na UI
    }

    private void FixedUpdate()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (enemiesCounterUI != null && wavesCounterUI != null && dayTimerCounterUI != null && hpBarUI != null && starCounterUI != null && hpCounterUI != null && stBarUI != null)
        {
            enemiesCounterUI.text = $"{waveManager.currentEnemies}/{waveManager.currentEnemiesWaveAmount}";
            wavesCounterUI.text = $"{waveManager.currentWave}/{waveManager.waveAmount}";
            dayTimerCounterUI.text = $"{(int)dayTimer}";

            hpBarUI.value = playerController.hp;
            hpBarUI.maxValue = playerController.maxHp;

            stBarUI.value = playerController.stamina;


            hpCounterUI.text = $"{playerController.hp}/{playerController.maxHp}";

            // itens coletados
            starCounterUI.text = $"{playerController.collectables.stars}";
            seedCounterUI.text = $"{playerController.collectables.seeds}";
            hpRegenCounterUI.text = $"{playerController.collectables.hpregens}";
            jumpCounterUI.text = $"{playerController.collectables.jumps}";
            dmgCounterUI.text = $"{playerController.collectables.dmgs}";

            if (playerController.hp < playerController.maxHp * 0.5f)
            {
                hpBarUI.fillRect.GetComponent<Image>().color = Color.red; // Barra vermelha para HP crítico
            }
            else if (playerController.hp < playerController.maxHp)
            {
                hpBarUI.fillRect.GetComponent<Image>().color = Color.yellow; // Barra verde para HP normal
            }
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0; // pausa jogo
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void WinGame()
    {
        Time.timeScale = 0; // pausa jogo
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Destroy(instance);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // recarrega cena - "restart"
    }

    public void GoToMenu()
    {
        Destroy(instance);
        SceneManager.LoadScene("Menu"); // recarrega cena - "restart"
    }

    public void Pause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
        Time.timeScale = 0; // Pausa o tempo
        isPaused = true;
    }

    // Retoma o jogo
    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1; // Retorna o tempo ao normal
        isPaused = false;
        // Esconde o menu de pausa
        // (implemente aqui a lógica para desativar a UI de pausa)
    }
}
