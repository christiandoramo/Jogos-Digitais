using Pathfinding;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Lógica do jogo")]
    public int hp = 100;
    [SerializeField][Range(0.05f, 1000f)] float continuousForceSpeedMeu = 10f;
    [SerializeField] float moveSpeedMeu = 10f;
    [SerializeField] float jumpForceMeu = 10f;
    [SerializeField] int hitDamage = 5;
    //private bool isMoving = true;
    private bool isColliding = false;
    private bool isHitting = false;
    private bool hasSpawnedDrop = false;
    [SerializeField][Range(1, 100)] int dropProb = 40;

    [Header("PathFinder")]
    private Path path;
    public Transform target;
    public PlayerController pc;

    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Config do Comportamento de IA")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;
    [SerializeField] GameObject thisBody;

    [Header("Física")]
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = .8f;
    public float jumpModifier = .3f;
    public float jumpCheckOffset = .1f;
    public Feet thisFeet;
    public Vector3 hitBoxSize = new(0, 0, 0);
    [SerializeField] LayerMask playerColliderMask;
    [SerializeField] LayerMask bulletColliderMask;

    [Header("Efeitos extra")]
    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;
    public GameObject zombieSoundPrefab;

    private Material material;      // Material associado ao SpriteRenderer
    private Shader defaultShader;  // Shader padrão do material
    private Shader flashShader;

    private struct AnimationStates
    {
        public const string IDLING = "ZombieIdling";
        public const string RUNNING = "ZombieRunning";
        public const string DYING = "ZombieDying";
    }

    public void Start()
    {
        thisFeet = GetComponentInChildren<Feet>();
        seeker = GetComponent<Seeker>();
        thisBody = thisBody == null ? transform.GetChild(0).gameObject : thisBody;
        rb = thisBody.GetComponent<Rigidbody2D>();
        if (target == null) target = GameObject.FindWithTag("Player").transform;
        pc = target.gameObject.GetComponentInParent<PlayerController>();


        customAnimator = thisBody.GetComponent<CustomAnimator>();
        spriteRenderer = thisBody.GetComponent<SpriteRenderer>();
        material = spriteRenderer.material; // Obtém o material do SpriteRenderer
        defaultShader = Shader.Find("Sprites/Default"); // Shader padrão do Unity
        flashShader = Shader.Find("GUI/Text Shader");  // Shader para "acender"


        customAnimator.ChangeState(AnimationStates.IDLING);

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (target == null || thisBody == null || hp <= 0) return;
        Collider2D colliderPlayer = Physics2D.OverlapBox(thisBody.transform.position, new Vector2(hitBoxSize.x, hitBoxSize.y), 0f, playerColliderMask);
        Collider2D colliderBullet = Physics2D.OverlapBox(thisBody.transform.position, new Vector2(hitBoxSize.x, hitBoxSize.y), 0f, bulletColliderMask);
        if (colliderPlayer != null && !isColliding)
        {
            StartCoroutine(HandleCollision());// colisão com player
            target.GetComponentInParent<PlayerController>().PlayerFlash(); // flash pisca pisca do player
        }
        if (colliderBullet != null)
        {
            if (!isHitting)
            {
                StartCoroutine(FlashRoutine()); // pisca pisca do inimigo
                StartCoroutine(HandleBulletHit(colliderBullet)); // colisão com bala
            }
        }

        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    void Update()
    {
        if (hp <= 0)
        {
            Die();
            return;
        }
        Animate();
    }

    void Animate()
    {
        if (hp <= 0) return;
        if (rb != null && spriteRenderer != null)
        {
            if (customAnimator.currentState != AnimationStates.RUNNING && Mathf.Abs(rb.linearVelocity.x) > 0.3f)
            {
                customAnimator.ChangeState(AnimationStates.RUNNING);
            }
            else if (customAnimator.currentState != AnimationStates.IDLING && Mathf.Abs(rb.linearVelocity.x) < 0.3f)
            {
                customAnimator.ChangeState(AnimationStates.IDLING);
            }

            if (rb.linearVelocity.x < -0.3f && !spriteRenderer.flipX)
                spriteRenderer.flipX = true;
            else if (rb.linearVelocity.x > 0.3f && spriteRenderer.flipX)
                spriteRenderer.flipX = false;
        }
    }

    private void Die()
    {
        if (!hasSpawnedDrop)
        {
            hasSpawnedDrop = true;
            GameManager.instance.collectableManager.SpawnDrop(dropProb, thisBody.transform);
            GameObject zombieSound = Instantiate(zombieSoundPrefab, target.transform.position, Quaternion.identity);
            AudioSource audioSource = zombieSoundPrefab.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                // Destruir o som após a reprodução
                Destroy(zombieSound, audioSource.clip.length);
            }
        }
        customAnimator.ChangeState(AnimationStates.DYING);
        float duration = customAnimator.GetAnimationDuration(AnimationStates.DYING);



        Destroy(gameObject, duration + 1f);
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.waveManager.currentEnemies--;
        }
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (pc.isStarBoostActivated)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                hp = 0;
                Die();
            }
        }
    }

    IEnumerator HandleCollision()
    {
        if (pc.isStarBoostActivated)
        {
            hp = 0;
            Die();
            yield break;
        }

        PlayerController playerController = target.GetComponentInParent<PlayerController>();
        playerController.hp -= hitDamage; // da 50 de dano ao colidir com o player
        isColliding = true;
        //rb.linearVelocity = Vector2.zero;
        //isMoving = false;
        yield return new WaitForSeconds(1.5f);
        isColliding = false;
    }

    IEnumerator HandleBulletHit(Collider2D colliderBullet)
    {
        isHitting = true;
        int damage = (int)pc.bulletDmg; // dano da bala atual
        hp -= damage;

        Destroy(colliderBullet.gameObject);
        if (hp <= 0)
        {
            Die();
        }
        yield return new WaitForSeconds(.3f);
        isHitting = false;
    }

    // AI código A*

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;
        isGrounded = thisFeet.isGrounded;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

        // JUMP
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                //rb.AddForce(Vector2.up * speed * jumpModifier);
                rb.linearVelocityX = 0; // testando
                rb.linearVelocityX = 0;
                rb.linearVelocity *= (Vector2.right);
                Debug.Log("Velocidade atual = " + rb.linearVelocity);
                rb.AddForce(Vector2.up * jumpForceMeu * Time.deltaTime, ForceMode2D.Impulse); // coloquei minha versão

            }
        }
        // Movement
        // rb.AddForce(force); // trocar aki velocidade para ajustar
        rb.linearVelocityX = direction.x * moveSpeedMeu * Time.deltaTime; // coloquei minha versão
        rb.AddForce(Vector2.right * direction.x * moveSpeedMeu * continuousForceSpeedMeu * Time.deltaTime, ForceMode2D.Force);

        // proximo wayPoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(thisBody.transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(thisBody.transform.position, hitBoxSize);
    }

    private IEnumerator FlashRoutine()
    {
        int flashes = 3;          // Quantidade de piscadas
        float interval = 0.5f;    // Intervalo entre as piscadas (meio segundo)

        for (int i = 0; i < flashes; i++)
        {
            // Troca para o shader que "acende"
            material.shader = flashShader;
            yield return new WaitForSeconds(interval / 2);

            // Retorna ao shader padrão
            material.shader = defaultShader;
            yield return new WaitForSeconds(interval / 2);
        }
    }
}
