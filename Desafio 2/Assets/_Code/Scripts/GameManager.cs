using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public WaveManager waveManager;
    [SerializeField] Light2D globalIlumination;

    public TextMeshProUGUI enemiesCounterUI;
    public TextMeshProUGUI wavesCounterUI;
    public TextMeshProUGUI dayTimerCounterUI;
    public Slider hpCounterUI;

    public GameObject victoryPanel;

    public static GameManager instance;
    public RectTransform enemyImage;
    float rotationAngle;

    private bool nightHasStarted;

    private float dayTimer = 0f;
    public float sunnyTime = 10f;
    private float colorTimeReference = 0f;
    private Color bgNightColor;
    private float gradientSpeed = 1.0f;
    private List<Portal> portalComponents;

    public GameObject player;
    public PlayerController playerController;



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
        playerController = player.GetComponent<PlayerController>();

        hpCounterUI.maxValue = playerController.hp;
        hpCounterUI.minValue = 0;
        hpCounterUI.value = playerController.hp;

        waveManager = Instantiate(waveManager);
        waveManager.Initialize();

        portalComponents = waveManager.portals.Select((portal) => portal.GetComponent<Portal>()).ToList<Portal>();


        if (ColorUtility.TryParseHtmlString("#FF5F5F", out bgNightColor))

            if (victoryPanel != null)
                victoryPanel.SetActive(false); // desativa menu de parabens
    }

    private void TimeManage()
    {
        float second = Time.deltaTime;
        dayTimer += second;

        if (waveManager.currentWave > waveManager.waveAmount && waveManager.currentEnemies <= 0)
        {
            waveManager.currentWave--; // corrigi o waveManager
            EndGame(); // ganha quando nao tem inimigos e finalizou waves
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
            nightHasStarted = true;
        }
    }
    private void Update()
    {
        if (playerController.hp <= 0) EndGame();
        TimeManage();
        UpdateUI();
        rotationAngle = 360f * Time.deltaTime; //metade de 1 volta em 1 segundo
        enemyImage.Rotate(new Vector3(0, 1, 0), rotationAngle, Space.Self); // rotação do inimigo na UI
    }

    private void UpdateUI()
    {
        if (enemiesCounterUI != null && wavesCounterUI != null && dayTimerCounterUI != null && hpCounterUI != null)
        {
            enemiesCounterUI.text = $"{waveManager.currentEnemies}/{waveManager.currentEnemiesWaveAmount}";
            wavesCounterUI.text = $"{waveManager.currentWave}/{waveManager.waveAmount}";
            dayTimerCounterUI.text = $"{(int)dayTimer}";

            hpCounterUI.value = playerController.hp;

            if (playerController.hp < 100 * 0.5f)
            {
                hpCounterUI.fillRect.GetComponent<Image>().color = Color.red; // Barra vermelha para HP crítico
            }
            else if(playerController.hp < 100)
            {
                hpCounterUI.fillRect.GetComponent<Image>().color = Color.green; // Barra verde para HP normal
            }


        }
    }

    private void EndGame()
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
