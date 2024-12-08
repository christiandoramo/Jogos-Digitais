using UnityEngine;

public class Portal : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; 
    public bool isActivated = false;
    public float gradientSpeed = 1.0f; 

    private Color colorRed = Color.red; 
    private Color colorBlack = Color.black; 
    private float t = 0f;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.color = colorBlack;
    }

    void Update()
    {
        if (isActivated)
        {
            t += Time.deltaTime * gradientSpeed;
            spriteRenderer.color = Color.Lerp(colorBlack, colorRed, Mathf.PingPong(t, 1f));
        }
        else if (spriteRenderer.color != colorBlack)
        {
            spriteRenderer.color = colorBlack;
        }
    }

    public void ToggleActivation()
    {
        isActivated = !isActivated;

        if (!isActivated)
        {
            // Reseta o gradiente ao voltar ao modo normal
            spriteRenderer.color = colorBlack;
            t = 0f;
        }
    }
}
