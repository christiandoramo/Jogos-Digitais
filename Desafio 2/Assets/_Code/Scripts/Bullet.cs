using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 5f;
    private TrailRenderer trailRenderer; // rastro
    private Transform playerTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = true;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // criando velocidade inicial baseado na posição do mouse e distancia para o player
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - playerTransform.position).normalized;
        rb.linearVelocity = direction * bulletSpeed;

        AlignToVelocity();
        Destroy(gameObject, bulletLifetime);
    }


    private void Update()
    {
        AlignToVelocity();
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!collision.CompareTag("Player"))
    //    {
    //       // Destroy(collision.gameObject);
    //        Destroy(gameObject);
    //        //GameManager.OnTriangleDestroy();
    //    }
    //}

    private void AlignToVelocity()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player; // setter pra pegar player
    }
}
