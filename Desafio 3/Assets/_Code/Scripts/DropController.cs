using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using static CollectableManager;


public class DropController : MonoBehaviour
{
    [SerializeField] float floatSpeed = 1f; // Velocidade da flutuação
    [SerializeField] float floatAmplitude = 0.5f; // Amplitude da flutuação


    private Vector3 startPosition;
    public CollectableManager collectableManager;
    public CollectableType collectableType;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private int counter = 20;

    private Color originalColor;
    bool isFlashing = false;
    void Start()
    {
        collectableManager = GameManager.instance.collectableManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
        startPosition = transform.position;

    }
    void Update()
    {
        counter += (int)Time.deltaTime;
        if (counter <= 0)
        {
            collectableManager.powerUpInstances = collectableManager.powerUpInstances
            .Where(a => a.objInstance.transform.position.x != transform.position.x)
            .ToList();
            Destroy(gameObject);
        }
        else if (counter <= 10 && !isFlashing)
        {
            isFlashing = true;
            StartCoroutine(FlashTransparentRoutine());
        }
    }




    void FixedUpdate()
    {
        // Calcula a nova posição usando uma função senoidal para flutuação
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
    private IEnumerator FlashTransparentRoutine()
    {
        int flashes = 10;          // Quantidade de piscadas
        float interval = 1f;    // Intervalo entre as piscadas (meio segundo)

        for (int i = 0; i < flashes; i++)
        {
            spriteRenderer.color = new Color(255f, 255f, 255f, 24f);
            yield return new WaitForSeconds(interval / 2);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(interval / 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider == null) return;
        if (collider.CompareTag("Player"))
        {
            collectableManager.Collect(collectableType);
            Destroy(gameObject);
        }
    }
}

