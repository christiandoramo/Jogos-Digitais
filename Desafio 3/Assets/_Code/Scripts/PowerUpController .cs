using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using static CollectableManager;


public class PowerUpController : MonoBehaviour
{
    [NonSerialized] public PowerUpType powerUpType;
    public CollectableManager collectableManager;

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
            collectableManager.PowerUpCollect(this.powerUpType);
            Destroy(gameObject);
        }
    }
}

