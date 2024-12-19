using UnityEngine;

[ExecuteInEditMode]
public class Feet : MonoBehaviour
{
    [Header("Feet variáveis")]
    public bool isGrounded;
    public string floorTag = "";
    public Transform groundTransform;

    [TooltipAttribute("layer de colisão com o Ator (inimigo, player ou npc) para pular")]
    [SerializeField] private LayerMask layerMask;
    public float feetRadius = 1f;

    void Update()
    {
        CheckFloor();
    }

    /// <summary>
    ///  Checa se colidiu, depois se foi com um platformEffector2D, e depois se o angulo é´de 180 graus para trocar isGrounded para true senão é false
    /// </summary>
    private void CheckFloor()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, feetRadius, layerMask);
        if (!collider)
        {
            isGrounded = false;
            floorTag = "";
            groundTransform = null;
        }
        else
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Floor") && !isGrounded)
            {
                isGrounded = true;
                floorTag = collider.tag;

                if (collider.CompareTag("Ground")) groundTransform = collider.transform;
                else groundTransform = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, feetRadius);
    }
}
