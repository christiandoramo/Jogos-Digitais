using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Rigidbody2D rb2;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;

    public bool isArmed;
    public bool isGrounded;
    public bool isWalking;
    public bool isRunning;
    public bool isIdling;
    private struct AnimationStates
    {
        public const string WALKING = "Walking";
        public const string IDLING = "Idling";
        public const string JUMPING = "Jumping";
    }
    void Start()
    {
        customAnimator = GetComponent<CustomAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2 = GetComponent<Rigidbody2D>();
        customAnimator.ChangeState(AnimationStates.IDLING);
    }
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool isJumpPressed = Input.GetButtonDown("Jump");
        float horizontalInput = Input.GetAxis("Horizontal");

        Move(isGrounded, horizontalInput);
        Jump(isJumpPressed, isGrounded);
    }

    void Animate()
    {
        if (rb2.linearVelocity.x < 0) spriteRenderer.flipX = true;
        else if (rb2.linearVelocity.x > 0) spriteRenderer.flipX = false;

        if (rb2.linearVelocity.x != 0 && isGrounded)
        {
            customAnimator.ChangeState(AnimationStates.WALKING);
        }
        else if (rb2.linearVelocity.x == 0 && isGrounded)
        {
            customAnimator.ChangeState(AnimationStates.IDLING);
        }
    }
    void Move(bool isGrounded, float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            rb2.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb2.linearVelocity.y);
            //else customAnimator.ChangeState("WalkingArmed");
        }
    }
    void Jump(bool isJumpPressed, bool isGrounded)
    {
        if (isJumpPressed && isGrounded)
        {
            rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
            //customAnimator.ChangeState("Jumping");
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
