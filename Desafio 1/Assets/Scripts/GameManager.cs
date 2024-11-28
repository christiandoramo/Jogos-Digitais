using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI contadorText;
    public GameObject victoryPanel; 
    private static int totalTriangulos; 
    private static GameManager instance; 
    public RectTransform triangle;
    float rotationAngle;

    void Awake()
    {
        // GameManager é único na cena
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
        GameObject[] triangulos = GameObject.FindGameObjectsWithTag("Enemy"); // pegando e contandos triangulos azuis (tag enemy)
        totalTriangulos = triangulos.Length;

        if (victoryPanel != null)
            victoryPanel.SetActive(false); // desativa menu de parabens

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
        rotationAngle = 360f * Time.deltaTime;
        triangle.Rotate(new Vector3(0, 0, 1), rotationAngle, Space.Self); // rotação do triangulo na UI

    }

    public static void OnTriangleDestroy()
    {
        if (instance == null) return;

        totalTriangulos--;
        instance.UpdateCount();
    }

    private void UpdateCount()
    {
        if (contadorText != null)
        {
            contadorText.text = $"{totalTriangulos}";
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
