using System.Collections;
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

    private float counter = 12f;

    private Color originalColor;
    bool isFlashing = false;
    Coroutine cr;

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
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            if (cr != null)
            {
                StopCoroutine(cr);
                spriteRenderer.color = originalColor;
            }
            Destroy(gameObject);
        }
        else if (counter <= 6 && !isFlashing)
        {
            isFlashing = true;
            cr = StartCoroutine(FlashTransparentRoutine());
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
            spriteRenderer.color = new Color(255f, 255f, 255f, 5f);
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
            if (cr != null)
            {
                StopCoroutine(cr);
                spriteRenderer.color = originalColor;
            }
            Destroy(gameObject);
        }
    }
}

