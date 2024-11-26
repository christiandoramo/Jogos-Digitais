using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI contadorText; // Referência ao texto do contador
    public GameObject victoryPanel; // Painel de vitória
    private static int totalTriangulos; // Total de triângulos na cena
    private static GameManager instance; // Referência à instância do GameManager
    public RectTransform triangle;
    readonly float rotationSpeed = 360f; // graus por segundo
    float rotationAngle;

    void Awake()
    {
        // Garante que o GameManager é único na cena
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
        triangle.Rotate(new Vector3(0, 0, 1), rotationAngle, Space.Self); // Rotação ao redor do eixo Y

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
        // Pausa o jogo e exibe o painel de vitória
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
