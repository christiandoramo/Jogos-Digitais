using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI contadorText; // Refer�ncia ao texto do contador
    public GameObject victoryPanel; // Painel de vit�ria
    private static int totalTriangulos; // Total de tri�ngulos na cena
    private static GameManager instance; // Refer�ncia � inst�ncia do GameManager
    public RectTransform triangle;
    readonly float rotationSpeed = 360f; // graus por segundo
    float rotationAngle;

    void Awake()
    {
        // Garante que o GameManager � �nico na cena
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        // Inicializa contadores e elementos de UI
        GameObject[] triangulos = GameObject.FindGameObjectsWithTag("Enemy");
        totalTriangulos = triangulos.Length;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        UpdateCount();
    }

    void FixedUpdate()
    {
        if (totalTriangulos <= 0)
        {
            Win();
        }
    }
    private void Update()
    {
        rotationAngle = rotationSpeed * Time.deltaTime;
        triangle.Rotate(new Vector3(0, 0, 1), rotationAngle, Space.Self); // Rota��o ao redor do eixo Y

    }

    public static void OnTriangleDestroy()
    {
        if (instance == null) return;

        totalTriangulos--;
        instance.UpdateCount();
    }

    private void UpdateCount()
    {
        // Atualiza o texto do contador
        if (contadorText != null)
        {
            contadorText.text = $"{totalTriangulos}";
        }
    }

    private void Win()
    {
        // Pausa o jogo e exibe o painel de vit�ria
        Time.timeScale = 0;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Reinicia o jogo
        Time.timeScale = 1; // Volta o tempo ao normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
