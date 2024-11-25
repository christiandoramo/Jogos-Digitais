using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpForce = 10f;
    public Rigidbody2D rb2;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;

    public Transform player;
    public TrailRenderer trailRenderer;


    public float runSpeed;
    public float acceleration;

    private float targetSpeed;
    private float currentSpeed;

    private bool isArmed;
    private bool isGrounded;
    private bool isWalking;
    private bool isRunning;
    private bool isIdling;



    private struct AnimationStates
    {
        public const string WALKING = "Walking";
        public const string RUNNING = "Running";
        public const string IDLING = "Idling";
        public const string JUMPING = "Jumping";
    }
    void Start()
    {
        player = player == null ? this.transform.GetChild(0) : player; 
        customAnimator = player.GetComponent<CustomAnimator>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        rb2 = player.GetComponent<Rigidbody2D>();
        customAnimator.ChangeState(AnimationStates.IDLING);
        trailRenderer = player.GetComponent<TrailRenderer>();

        targetSpeed = moveSpeed; // Velocidade padr�o
        runSpeed = moveSpeed * 2f; // Velocidade m�xima ao correr
        currentSpeed = rb2.linearVelocity.x;
        acceleration = 3f;
    }
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isJumpKeyDown = Input.GetButtonDown("Jump");
        float horizontalInput = Input.GetAxis("Horizontal");

        Move(isGrounded, horizontalInput, isShiftPressed);
        Jump(isJumpKeyDown, isGrounded);
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

        if (rb2.linearVelocity.x != 0 && isGrounded)
        {
            if (isRunning && customAnimator.currentState != AnimationStates.RUNNING)
            {
                customAnimator.ChangeState(AnimationStates.RUNNING);
            }
            else if (isWalking && customAnimator.currentState != AnimationStates.WALKING)
            {
                customAnimator.ChangeState(AnimationStates.WALKING);
            }
        }
        else if (rb2.linearVelocity.x == 0 && isGrounded && customAnimator.currentState != AnimationStates.IDLING )
        {
            customAnimator.ChangeState(AnimationStates.IDLING);
        }

        if (!isGrounded && customAnimator.currentState != AnimationStates.JUMPING)
        {
            customAnimator.ChangeState(AnimationStates.JUMPING);

        }
    }
    void Move(bool isGrounded, float horizontalInput, bool isShiftPressed)
    {
        if (horizontalInput != 0 && isShiftPressed)
        {
            Debug.Log("Running");
            targetSpeed = runSpeed; // Correr
            isRunning = true;
            isWalking = false;
        }
        else if (horizontalInput != 0)
        {
            Debug.Log("Walking");
            targetSpeed = moveSpeed; // Caminhar
            isWalking = true;
            isRunning = false;
        }

        currentSpeed = rb2.linearVelocity.x;
        // Suavizar transi��o entre velocidades
        // float newSpeed = Mathf.Lerp(currentSpeed, horizontalInput * targetSpeed, Time.deltaTime * acceleration);
        float newSpeed = Mathf.Lerp(currentSpeed, horizontalInput * targetSpeed, Time.deltaTime * acceleration);
        //Debug.Log($"horizontalInput: {horizontalInput} acceleration: {acceleration}");
        //Debug.Log($" Time.deltaTime: {Time.deltaTime} targetSpeed: {targetSpeed}");
        //Debug.Log($"currentSpeed: {currentSpeed} newSpeed: {newSpeed}");

        rb2.linearVelocity = new Vector2(newSpeed, rb2.linearVelocity.y);
    }
    void Jump(bool isJumpPressed, bool isGrounded)
    {
        if (isJumpPressed && isGrounded)
        {
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

    private void OnDrawGizmos() // fun��o mostra a �rea do overlap da mascara de colis�o no p� do player
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
