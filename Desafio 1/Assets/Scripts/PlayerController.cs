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

    public Transform player;


    private struct AnimationStates
    {
        public const string WALKING = "Walking";
        public const string IDLING = "Idling";
        public const string JUMPING = "Jumping";
    }
    void Start()
    {
        player = player == null ? transform.GetChild(0) : player; 
        customAnimator = player.GetComponent<CustomAnimator>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        rb2 = player.GetComponent<Rigidbody2D>();
        customAnimator.ChangeState(AnimationStates.IDLING);
    }
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!isGrounded) Debug.Log("flutuando");
        bool isJumpPressed = Input.GetButtonDown("Jump");
        float horizontalInput = Input.GetAxis("Horizontal");
        Jump(isJumpPressed, isGrounded);
        Move(isGrounded, horizontalInput);

        Animate();
    }

    void Animate()
    {
        if (rb2.linearVelocity.x < 0 && spriteRenderer.flipX == false)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb2.linearVelocity.x > 0 && spriteRenderer.flipX == true)
        {
            spriteRenderer.flipX = false;
        }

        if (rb2.linearVelocity.x != 0 && isGrounded && customAnimator.currentState != AnimationStates.WALKING)
        {
            customAnimator.ChangeState(AnimationStates.WALKING);
        }
        else if (rb2.linearVelocity.x == 0 && isGrounded && customAnimator.currentState != AnimationStates.IDLING)
        {
            customAnimator.ChangeState(AnimationStates.IDLING);
        }

        if (!isGrounded && customAnimator.currentState != AnimationStates.JUMPING)
        {
            customAnimator.ChangeState(AnimationStates.JUMPING);

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
            Debug.Log("Pulou");
            rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
        }
    }

    private void Shoot()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
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
