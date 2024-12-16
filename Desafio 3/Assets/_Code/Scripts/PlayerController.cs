using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // status
    public int hp = 100;
    public float moveSpeed = 8f;
    public float jumpForce = 8f;
    public float bulletDmg = 25f;

    public struct Collectables
    {
        public int seeds;
        public int hpregens;
        public int stars;
        public int jumps;
        public int dmgs;
        public Collectables(int seeds, int hpregens, int stars, int jumps, int dmgs)
        {
            this.seeds = seeds;
            this.hpregens = hpregens;
            this.stars = stars;
            this.jumps = jumps;
            this.dmgs = dmgs;
        }
    }
    public Collectables collectables;

    // fisica
    public float fakeGravityThreshold = 2f; // simulando gravidade mais forte ao cair (final do pulo)

    public Rigidbody2D rb2;
    public Transform groundCheck;
    public float groundCheckRadius = 1f;
    public LayerMask groundLayer;
    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer armsSr;
    public Feet playerFeet;

    public Transform player;
    public Transform arms;

    //public TrailRenderer trailRenderer;

    public float runSpeed;
    public float acceleration = 3f;

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
    // private bool isIdling;


    private Material armsMaterial;
    private Material material;      // Material associado ao SpriteRenderer
    private Shader defaultShader;  // Shader padrão do material
    private Shader flashShader;

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
        collectables = new(0, 0, 0, 0, 0);

        player = player == null ? this.transform.GetChild(0) : player;
        arms = arms == null ? player.GetChild(0) : arms;
        armsSr = arms.GetComponent<SpriteRenderer>();
        armsSr.enabled = false;

        customAnimator = player.GetComponent<CustomAnimator>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();

        armsMaterial = armsSr.material;
        material = spriteRenderer.material; // Obtém o material do SpriteRenderer
        defaultShader = Shader.Find("Sprites/Default"); // Shader padrão do Unity
        flashShader = Shader.Find("GUI/Text Shader");  // Shader para "acender"

        rb2 = player.GetComponent<Rigidbody2D>();
        customAnimator.ChangeState(AnimationStates.IDLING);

        //trailRenderer = player.GetComponent<TrailRenderer>();
        //trailRenderer.enabled = false;

        targetSpeed = moveSpeed;
        runSpeed = moveSpeed * 2f;

        currentSpeed = rb2.linearVelocity.x;
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
            if (structMouseDirectionAndAngle.direction.x < 0 && spriteRenderer.flipX == false) // flipa em relação a arma - quando a arma aponta para esquerda deve flipar
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

        if (isMoving && Math.Abs(rb2.linearVelocity.x) > 0.1f * moveSpeed && isGrounded)
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
        else if (!isMoving && Math.Abs(rb2.linearVelocity.x) <= 0.1f && isGrounded)
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
            targetSpeed = runSpeed;
            isRunning = true;
            isWalking = false;
        }
        else if (horizontalInput != 0)
        {
            targetSpeed = moveSpeed;
            isWalking = true;
            isRunning = false;
        }

        currentSpeed = rb2.linearVelocity.x;

        float newSpeed = Mathf.Lerp(currentSpeed, horizontalInput * targetSpeed, Time.deltaTime * acceleration);

        rb2.linearVelocity = new Vector2(newSpeed, rb2.linearVelocity.y);
    }
    void Jump(bool isJumpPressed)
    {
        if (isJumpPressed && isGrounded)
        {
            rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
        }
    }

    private void Shoot(bool isShootPressed)
    {
        shootCooldownTimer = shootCooldownTimer > 0 ? shootCooldownTimer - Time.deltaTime : shootCooldownTimer; // manejando cooldown
        armedAnimationCooldownTimer = armedAnimationCooldownTimer > 0 ? armedAnimationCooldownTimer - Time.deltaTime : armedAnimationCooldownTimer;

        if (shootCooldownTimer <= 0 && isShootPressed)
        {
            //Vector3 pos = player.transform.position;
            //if (!spriteRenderer.flipX) pos.x *= 1.02f;
            //else pos.x *= 0.98f;

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

    public void PlayerFlash()
    {
        StartCoroutine(FlashRoutine());
    }
    private IEnumerator FlashRoutine()
    {
        int flashes = 3;          // Quantidade de piscadas
        float interval = 0.5f;    // Intervalo entre as piscadas (meio segundo)

        for (int i = 0; i < flashes; i++)
        {
            // Troca para o shader que "acende"
            material.shader = flashShader;
            armsMaterial.shader = flashShader;
            yield return new WaitForSeconds(interval / 2);

            // Retorna ao shader padrão
            armsMaterial.shader = defaultShader;
            material.shader = defaultShader;
            yield return new WaitForSeconds(interval / 2);
        }
    }
}
