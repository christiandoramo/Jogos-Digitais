using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class Feet : MonoBehaviour
{


    [Header("Feet variáveis")]
    public bool isGrounded;
    private bool isColliding;
    [TooltipAttribute("layer de colisão com o Ator (inimigo, player ou npc) para pular")] 
    [SerializeField] private LayerMask layerMask;
    public float feetRadius = 1f;

    //void Start()
    //{
    //    Debug.Log("PlayerFeet.cs: debug liberado no edit mode");
    //}
    void Update()
    {
        CheckFloor();
    }

    /// <summary>
    ///  Checa se colidiu, depois se foi com um platformEffector2D, e depois se o angulo é´de 180 graus para trocar isGrounded para true senão é false
    /// </summary>
    private void CheckFloor()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position,feetRadius, layerMask);
        if (!collider)
        {
            isGrounded = false;
        }
        else
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Floor") && !isGrounded)
            {
                isGrounded = true;
            }
         
            //PlatformEffector2D platformEffector = collider.GetComponent<PlatformEffector2D>();
            //if (platformEffector != null)
            //{
            //    Vector2 normal = collider.transform.up; // Direção da superfície
            //    Vector2 directionToPlayer = (transform.position - collider.transform.position).normalized;

            //    float angle = Vector2.Angle(normal, directionToPlayer);

            //    // Verifica se está dentro do arco do Platform Effector (típico: 0°-180° no topo)
            //    if (angle <= platformEffector.surfaceArc / 2f && collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
            //    {
            //        isGrounded = true;
            //    }
            //}
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, feetRadius); 
    }
}
