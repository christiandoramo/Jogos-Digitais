using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TransparencyTrigger : MonoBehaviour
{
    public float fadeDuration = 1f; // Duração da transição em segundos
    public float transparentAlpha = 0.15f; // Alpha desejado ao entrar no trigger
    [SerializeField] Tilemap tilemap; // Referência ao Tilemap
    private Coroutine fadeCoroutine; // Referência para controlar corrotinas em andamento
    private float originalAlpha; // Alpha original do Tilemap

    private void Start()
    {

        if (tilemap != null)
        {
            originalAlpha = tilemap.color.a;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Iniciar a transição para quase transparente
        //if (fadeCoroutine != null)
        //    StopCoroutine(fadeCoroutine);]

        Debug.Log("Entrou");
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb.linearVelocity.x >= 0) return;
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeToAlpha(transparentAlpha));
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Iniciar a transição de volta ao alpha original
        Debug.Log("Saiu");

        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb.linearVelocity.x <= 0) return;
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToAlpha(originalAlpha));

        }
    }

    IEnumerator FadeToAlpha(float targetAlpha)
    {
        if (tilemap == null) yield break;

        // Cor inicial
        Color currentColor = tilemap.color;
        float startAlpha = currentColor.a;

        // Realizar a transição ao longo do tempo
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            tilemap.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        // Garantir o alpha final exato
        tilemap.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
}
