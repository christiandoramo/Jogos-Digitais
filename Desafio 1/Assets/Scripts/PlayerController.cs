using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpForce = 10f; // Força do pulo
    public float fallMultiplier = 5f; // Gravidade extra na descida
    public float lowJumpMultiplier = 2f; // Gravidade extra ao cancelar o pulo
    public Rigidbody2D rb2;
    public Transform groundCheck;
    public float groundCheckRadius = 1f;
    public LayerMask groundLayer;
    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer armsSr;
    public PlayerFeet playerFeet;

    public Transform player;
    public Transform arms;

    //public TrailRenderer trailRenderer;

    public float runSpeed;
    public float acceleration;

    public GameObject bulletPrefab;
    public float shootCooldown = 0.5f;
    public float armedAnimationCooldown = 5f;

    private float shootCooldownTimer = 0f;
    private float armedAnimationCooldownTimer = 0f;

    private float targetSpeed;
    private float currentSpeed;

    private bool isArmed;
    private bool isGrounded;
    private bool isWalking;
    private bool isRunning;
    private bool isIdling;


    private struct AnimationStates
    {
        public const string IDLING = "Idling";
        public const string WALKING = "Walking";
        public const string RUNNING = "Running";
        public const string JUMPING = "Jumping";
        public const string IDLING_ARMED = "Idling Armed";
        public const string WALKING_ARMED = "Walking Armed";
        public const string RUNNING_ARMED = "Running Armed";
        public const string JUMPING_ARMED = "Jumping Armed";

    }
    public struct StructMouseDirectionAndAngle
    {
        public float angle;
        public Vector2 direction;
    }

    void Start()
    {
        player = player == null ? this.transform.GetChild(0) : player;
        arms = arms == null ? player.GetChild(0) : arms;
        armsSr = arms.GetComponent<SpriteRenderer>();
        armsSr.enabled = false;

        customAnimator = player.GetComponent<CustomAnimator>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        rb2 = player.GetComponent<Rigidbody2D>();
        customAnimator.ChangeState(AnimationStates.IDLING);
        //trailRenderer = player.GetComponent<TrailRenderer>();
        //trailRenderer.enabled = false;

        targetSpeed = moveSpeed; // Velocidade padr�o
        runSpeed = moveSpeed * 3f; // Velocidade m�xima ao correr
        currentSpeed = rb2.linearVelocity.x;
        acceleration = 3f;
    }
    void Update()
    {
        isGrounded = playerFeet.isGrounded;
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isJumpKeyDown = Input.GetButtonDown("Jump");
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isShootPressed = Input.GetButton("Fire1"); // down só quado clicka, sem down atira enquanto pressionar

        Move(horizontalInput, isShiftPressed);
        Jump(isJumpKeyDown);
        Shoot(isShootPressed);
        Animate(horizontalInput != 0);

    }
    void Animate(bool isMoving)
    {
        //if (isRunning && trailRenderer.enabled == false) trailRenderer.enabled = true;
        //else if (!isRunning && trailRenderer.enabled == true) trailRenderer.enabled = false;

        if (isArmed && !armsSr.enabled) armsSr.enabled = true; // se armado e com sprite de armed desabilitado então habilite
        else if (!isArmed && !!armsSr.enabled) armsSr.enabled = false;

        if (isArmed)
        {
            StructMouseDirectionAndAngle structMouseDirectionAndAngle = GetStructMouseDirectionAndAngle(player.transform);
            if (structMouseDirectionAndAngle.direction.x < 0 && spriteRenderer.flipX == false) // Flipando em relação a arma - quando a arma aponta para esquerda deve flipar
            {
                spriteRenderer.flipX = true;

            }
            else if (-1 * structMouseDirectionAndAngle.direction.x < 0 && spriteRenderer.flipX == true)
            {
                spriteRenderer.flipX = false;
            }
        }
        else if (!isArmed)
        {
            if (isMoving && rb2.linearVelocity.x < -0.05 && spriteRenderer.flipX == false)
            {
                spriteRenderer.flipX = true;
            }
            else if (isMoving && rb2.linearVelocity.x > 0.05 && spriteRenderer.flipX == true)
            {
                spriteRenderer.flipX = false;
            }
        }

        if (isMoving && Math.Abs(rb2.linearVelocity.x) > 0.05 && isGrounded)
        {
            if (isRunning)
            {
                if (!isArmed) customAnimator.ChangeState(AnimationStates.RUNNING); // se desarmado
                else customAnimator.ChangeState(AnimationStates.RUNNING_ARMED); // se armado
            }
            else if (isWalking)
            {
                if (!isArmed) customAnimator.ChangeState(AnimationStates.WALKING); // se desarmado
                else customAnimator.ChangeState(AnimationStates.WALKING_ARMED); // se armado
            }
        }
        else if (!isMoving && rb2.linearVelocity.x == 0 && isGrounded)
        {
            if (!isArmed) customAnimator.ChangeState(AnimationStates.IDLING); // se desarmado
            else customAnimator.ChangeState(AnimationStates.IDLING_ARMED); // se armado
        }

        if (!isGrounded)
        {
            if (!isArmed) customAnimator.ChangeState(AnimationStates.JUMPING); // se desarmado
            else customAnimator.ChangeState(AnimationStates.JUMPING_ARMED); // se armado
        }

    }
    void Move(float horizontalInput, bool isShiftPressed)
    {
        if (horizontalInput != 0 && isShiftPressed)
        {
            targetSpeed = runSpeed; // Correr
            isRunning = true;
            isWalking = false;
        }
        else if (horizontalInput != 0 && !isShiftPressed)
        {
            targetSpeed = moveSpeed; // Caminhar
            isWalking = true;
            isRunning = false;
        }

        currentSpeed = rb2.linearVelocity.x;

        float newSpeed = Mathf.Lerp(currentSpeed, horizontalInput * targetSpeed, Time.deltaTime * acceleration);

        rb2.linearVelocity = new Vector2(newSpeed, rb2.linearVelocity.y);
    }
    void Jump(bool isJumpPressed)
    {
        // Verifica se o jogador está pressionando o botão de pulo e está no chão
        if (isJumpPressed && isGrounded)
        {
            rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce); // Aplica o pulo
        }

        if (rb2.linearVelocity.y < 0)
        {
            rb2.linearVelocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
    }

    private void Shoot(bool isShootPressed)
    {
        shootCooldownTimer = shootCooldownTimer > 0 ? shootCooldownTimer - Time.deltaTime : shootCooldownTimer; // manejando cooldown
        armedAnimationCooldownTimer = armedAnimationCooldownTimer > 0 ? armedAnimationCooldownTimer - Time.deltaTime : armedAnimationCooldownTimer;

        if (shootCooldownTimer <= 0 && isShootPressed)
        {
            GameObject bullet = Instantiate(bulletPrefab, player.transform.position, Quaternion.identity);
            if (bullet != null) bullet.GetComponent<Bullet>().SetPlayerTransform(player);
            shootCooldownTimer = shootCooldown;
            armedAnimationCooldownTimer = armedAnimationCooldown;
            isArmed = true;
        }
        else if (armedAnimationCooldownTimer <= 0f && !!isArmed) // se o tempo de desarme = armedAnimationCooldownTimer acabou desativar isArmed e mudar animações
        {
            isArmed = false;
        }
    }



    public static StructMouseDirectionAndAngle GetStructMouseDirectionAndAngle(Transform transf)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - transf.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        StructMouseDirectionAndAngle mouseDirectionAndAngle;
        mouseDirectionAndAngle.direction = direction;
        mouseDirectionAndAngle.angle = angle;
        return mouseDirectionAndAngle;
    }
}
