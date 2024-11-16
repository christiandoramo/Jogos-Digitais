using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Rigidbody2D rb2;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool isJumpPressed = Input.GetButtonDown("Jump");
        float horizontalInput = Input.GetAxis("Horizontal");
        Move(horizontalInput);
        Jump(isJumpPressed, isGrounded);


    }
    void Move(float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            rb2.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb2.linearVelocity.y);
        }
    }
    void Jump(bool isJumpPressed, bool isGrounded)
    {
        if (isJumpPressed && isGrounded)
        {
            rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
        }
    }

    private void OnDrawGizmos() // função mostra a área do overlap da mascara de colisão no pé do player
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
