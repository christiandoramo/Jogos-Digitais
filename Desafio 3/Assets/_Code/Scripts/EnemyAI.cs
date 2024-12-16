using Pathfinding;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
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
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = .8f;
    public float jumpModifier = .3f;
    public float jumpCheckOffset = .1f;
    public Feet thisFeet;

    public Vector3 hitBoxSize = new(0, 0, 0);
    [SerializeField] LayerMask playerColliderMask;
    [SerializeField] LayerMask bulletColliderMask;


    [Header("Lógica do jogo")]
    public int hp = 100;
    //private bool isMoving = true;
    private bool isColliding = false;
    private bool isHitting = false;

    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;

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
        if (hp <= 0)
        {
            Die();
            return;
        }
        if (target == null || thisBody == null) return;
        Collider2D colliderPlayer = Physics2D.OverlapBox(thisBody.transform.position, new Vector2(hitBoxSize.x, hitBoxSize.y), 0f, playerColliderMask);
        Collider2D colliderBullet = Physics2D.OverlapBox(thisBody.transform.position, new Vector2(hitBoxSize.x, hitBoxSize.y), 0f, bulletColliderMask);
        if (colliderPlayer != null && !isColliding)
        {
            followEnabled = false;
            StartCoroutine(HandleCollision());// colisão com player
            target.gameObject.GetComponent<PlayerController>().PlayerFlash(); // flash pisca pisca do player
        }

        if (TargetInDistance() && followEnabled && !isColliding)
        {
            PathFollow();
        }

        if (colliderBullet != null)
        {
            StartCoroutine(FlashRoutine()); // pisca pisca do inimigo
            StartCoroutine(HandleBulletHit(colliderBullet)); // colisão com bala
        }


    }

    void Update()
    {
        Animate();
    }

    void Animate()
    {
        if (hp <= 0) return;
        if (rb != null && spriteRenderer != null)
        {
            if (customAnimator.currentState != AnimationStates.RUNNING && Mathf.Abs(rb.linearVelocity.x) > 0.05f)
            {
                customAnimator.ChangeState(AnimationStates.RUNNING);
            }
            else if (customAnimator.currentState != AnimationStates.IDLING && Mathf.Abs(rb.linearVelocity.x) < 0.05f)
            {
                customAnimator.ChangeState(AnimationStates.IDLING);
            }

            if (rb.linearVelocity.x < -0.05f && !spriteRenderer.flipX)
                spriteRenderer.flipX = true;
            else if (rb.linearVelocity.x > 0.05f && spriteRenderer.flipX)
                spriteRenderer.flipX = false;
        }
    }

    // Lógica do jogo

    private void Die()
    {
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


    IEnumerator HandleCollision()
    {
        PlayerController playerController = target.GetComponentInParent<PlayerController>();
        playerController.hp -= 20; // da 50 de dano ao colidir com o player
        isColliding = true;
        //rb.linearVelocity = Vector2.zero;
        //isMoving = false;
        yield return new WaitForSeconds(1.5f);
        isColliding = false;
        followEnabled = true;
    }

    IEnumerator HandleBulletHit(Collider2D colliderBullet)
    {
        int damage = (int) pc.bulletDmg; // dano da bala atual
        hp -= damage;

        Destroy(colliderBullet.gameObject);
        if (hp <= 0)
        {
            Die();
        }

        isHitting = true;
        //rb.linearVelocity = Vector2.zero;
        //isMoving = false;
        yield return new WaitForSeconds(.5f);
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
        Vector2 force = direction * speed * Time.deltaTime;

        // JUMP
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }
        // Movement
        rb.AddForce(force);
        //Debug.Log("rb.linearX: " + rb.linearVelocity.x);

        // proximo wayPoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Gizmo do path na tela
        //if (directionLookEnabled)
        //{
        //    if (rb.linearVelocity.x > 0.05f)
        //        thisBody.transform.localScale = new Vector3(-1f * Mathf.Abs(thisBody.transform.localScale.x), transform.localScale.y, thisBody.transform.localScale.z);
        //    else if (rb.linearVelocity.x < -0.05f)
        //        thisBody.transform.localScale = new Vector3(Mathf.Abs(thisBody.transform.localScale.x), thisBody.transform.localScale.y, thisBody.transform.localScale.z);

        //}
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
