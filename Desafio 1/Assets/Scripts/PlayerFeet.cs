using UnityEngine;
using UnityEngine.UIElements;
using static PlayerController;

public class PlayerFeet : MonoBehaviour
{
    public bool isGrounded;
    public LayerMask layerMask;
    public float feedRadius = 1f;

    void Start()
    {
    }
    void Update()
    {
        CheckFloor();
    }

    private void CheckFloor()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position,feedRadius, layerMask);
        if (!collider)
        {
            isGrounded = false;
        }
        else
        {
            PlatformEffector2D platformEffector = collider.GetComponent<PlatformEffector2D>();
            if (platformEffector != null)
            {
                Vector2 normal = collider.transform.up; // Direção da superfície
                Vector2 directionToPlayer = (transform.position - collider.transform.position).normalized;

                float angle = Vector2.Angle(normal, directionToPlayer);

                // Verifica se está dentro do arco do Platform Effector (típico: 0°-180° no topo)
                if (angle <= platformEffector.surfaceArc / 2f && collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    isGrounded = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, feedRadius); 
    }
}
