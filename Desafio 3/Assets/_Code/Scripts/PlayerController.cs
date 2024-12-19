using System;
using System.Collections;
using UnityEngine;
using static CollectableManager;

public class PlayerController : MonoBehaviour
{
    // status
    [Header("Lógica do jogo")]
    public int hp = 100;
    public int maxHp = 100;
    public float moveSpeed = 7f;
    public float jumpForce = 5f;
    public float bulletDmg = 25f;
    public float stamina = 0;
    public float maxStamina = 5;
    private float staminaHasBeenUsedCounter;
    public bool isStarBoostActivated;
    public bool superJump = false;
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

    [Header("Física e animações")]
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
    public GameObject shootSoundPrefab;
    public float shootCooldown = 0.3f;
    public float armedAnimationCooldown = 5f;

    private float shootCooldownTimer = 0f;
    private float armedAnimationCooldownTimer = 0f;

    private float targetSpeed;
    private float currentSpeed;

    private bool isArmed;
    [NonSerialized] public bool isGrounded;
    [NonSerialized] public string floorTag;

    private bool isWalking;
    private bool isRunning;
    // private bool isIdling;
    private bool isStarBoosting;


    private Material armsMaterial;
    private Material material;      // Material associado ao SpriteRenderer
    private Shader defaultShader;  // Shader padrão do material
    private Shader flashShader;
    private Coroutine flashRoutine;

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
        collectables = new Collectables(0, 0, 0, 0, 0);

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (GameManager.instance.isPaused)
                GameManager.instance.ResumeGame();
            else
                GameManager.instance.Pause();
        }
        HandleStamina();
        isGrounded = playerFeet.isGrounded;
        floorTag = playerFeet.floorTag;
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isJumpKeyDown = Input.GetButtonDown("Jump");
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isShootPressed = Input.GetButton("Fire1"); // down só quado clicka, sem down atira enquanto pressionar


        ItemUse();
        Move(horizontalInput, isShiftPressed);
        Jump(isJumpKeyDown);
        Shoot(isShootPressed);
        Animate(horizontalInput != 0);

    }

    void ItemUse()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.instance.collectableManager.UseCollectable(playerFeet.groundTransform, CollectableType.SEED);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) GameManager.instance.collectableManager.UseCollectable(playerFeet.groundTransform, CollectableType.HPREGEN);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.instance.collectableManager.UseCollectable(playerFeet.groundTransform, CollectableType.STAR);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) GameManager.instance.collectableManager.UseCollectable(playerFeet.groundTransform, CollectableType.JUMP);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) GameManager.instance.collectableManager.UseCollectable(playerFeet.groundTransform, CollectableType.DMG);
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
        if (horizontalInput != 0 && isShiftPressed && stamina > 0)
        {
            targetSpeed = runSpeed;
            isRunning = true;
            isWalking = false;
            stamina -= Time.deltaTime;
            staminaHasBeenUsedCounter = 2f; // 2 segundos sem usar para encher
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
            if (!superJump)
            {
                rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
            }
            else
            {
                superJump = false;
                rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce + 15f);
            }
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

            GameObject shotSound = Instantiate(shootSoundPrefab, player.transform.position, Quaternion.identity);
            AudioSource audioSource = shotSound.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                // Destruir o som após a reprodução
                Destroy(shotSound, audioSource.clip.length);
            }

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
        if (flashRoutine == null)
            flashRoutine = StartCoroutine(FlashRoutine());
    }
    private IEnumerator FlashRoutine()
    {
        int flashes = 3;          // Quantidade de piscadas
        float interval = 0.5f;    // Intervalo entre as piscadas (meio segundo)

        for (int i = 0; i < flashes; i++)
        {
            // Troca para o shader que "acende"
            material.color = Color.white;
            material.shader = flashShader;
            armsMaterial.shader = flashShader;
            yield return new WaitForSeconds(interval / 2);

            // Retorna ao shader padrão
            material.color = Color.white;
            armsMaterial.shader = defaultShader;
            material.shader = defaultShader;
            yield return new WaitForSeconds(interval / 2);
        }
        flashRoutine = null;
    }

    private IEnumerator StarBoostFlashRoutine()
    {
        //if (!isStarBoostActivated) yield break;
        float interval = 0.3f;

        material.shader = flashShader;
        armsMaterial.shader = flashShader;

        while (isStarBoostActivated)
        {
            material.color = Color.yellow;
            yield return new WaitForSeconds(interval);
            material.color = Color.blue;
            yield return new WaitForSeconds(interval);
        }
        armsMaterial.shader = defaultShader;
        material.shader = defaultShader;
        material.color = Color.white;
        armsMaterial.color = Color.white;
    }

    public void HandleStarBoost()
    {
        if (isStarBoostActivated && !isStarBoosting)
        {
            Debug.Log("Positivo 2");
            float interval = 10f;
            isStarBoosting = true;

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine); // parando rotina do flash se ainda estiver rodando
                armsMaterial.shader = defaultShader;
                material.shader = defaultShader;
                material.color = Color.white;
                armsMaterial.color = Color.white;
            }
            StartCoroutine(StarBoostFlashRoutine());
            StartCoroutine(HandleDectivateStarBoost(interval));
        }
        else
        {
            Debug.Log("Negativo 2");
        }
    }

    private IEnumerator HandleDectivateStarBoost(float interval)
    {
        Debug.Log("ATIVOU STAR BOOST");
        yield return new WaitForSeconds(interval);
        Debug.Log("DESATIVOU STAR BOOST");
        isStarBoostActivated = false;
        isStarBoosting = false;
    }

    private void HandleStamina()
    {
        Debug.Log("staminaHasBeenUsedCounter: " + staminaHasBeenUsedCounter);
        if (stamina < maxStamina && staminaHasBeenUsedCounter <= 0)
        {
            if (staminaHasBeenUsedCounter < 2f && stamina < maxStamina)
            {
                stamina += Time.deltaTime; // fica aumentando a cada frame da unity em 0.0000... float dando 1 a cada seg
                if (stamina >= maxStamina)
                {
                    stamina = maxStamina;
                }
            }
        }
        else if (staminaHasBeenUsedCounter >= 0 && staminaHasBeenUsedCounter <= 2f) // não foi usada em 2 segundos
        {
            staminaHasBeenUsedCounter -= Time.deltaTime;
            if (staminaHasBeenUsedCounter <= 0f)
                staminaHasBeenUsedCounter = 0f;
        }
    }
}
