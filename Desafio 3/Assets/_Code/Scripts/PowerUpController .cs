using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using static CollectableManager;


public class PowerUpController : MonoBehaviour
{
    public PowerUpType powerUpType;
    public CollectableManager collectableManager;
    public SpriteRenderer spriteRenderer;
    private float counter = 12f;

    [SerializeField] float floatSpeed = 1f; // Velocidade da flutuação
    [SerializeField] float floatAmplitude = 0.5f; // Amplitude da flutuação
    Coroutine cr;

    private Vector3 startPosition;


    private Color originalColor;
    bool isFlashing = false;
    void Start()
    {
        collectableManager = GameManager.instance.collectableManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        startPosition = transform.position;
    }
    void Update()
    {
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            collectableManager.powerUpInstances = collectableManager.powerUpInstances
            .Where(a => a.objInstance.transform.position.x != transform.position.x)
            .ToList();
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
            collectableManager.Collect(collectableManager.ConvertPowerUpTypeToCollectableType(powerUpType));
            if (cr != null)
            {
                StopCoroutine(cr);
                spriteRenderer.color = originalColor;
            }
            Destroy(gameObject);
        }
    }
}

