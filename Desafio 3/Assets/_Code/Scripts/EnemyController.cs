using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int hp = 100;
    [SerializeField] int hitDamage = 5;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Transform player;
    // [SerializeField] float stopDistance = .05f; // distancia da colisão
    [SerializeField][Range(1, 100)] int dropProb = 40;

    [SerializeField] Transform enemyBody;

    private bool hasSpawnedDrop = false; // 


    //private float targetSpeed;
    private float currentSpeed;
    private float acceleration = 3f;


    public Rigidbody2D rb2;
    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;

    [SerializeField] private LayerMask playerColliderMask;
    [SerializeField] private LayerMask hitMask;

    private Material material;      // Material associado ao SpriteRenderer
    private Shader defaultShader;  // Shader padrão do material
    private Shader flashShader;

    private bool isMoving = true;
    private bool isColliding = false;
    private bool isHitting = false;


    public Vector3 hitBoxSize = new(0, 0, 0);


    private struct AnimationStates
    {
        public const string IDLING = "ZombieIdling";
        public const string RUNNING = "ZombieRunning";
        public const string DYING = "ZombieDying";
    }

    private void Start()
    {
        player = player == null ? GameObject.FindGameObjectWithTag("Player").transform : player; // pega o player já na cena
        enemyBody = enemyBody == null ? this.transform.GetChild(0) : enemyBody;
        customAnimator = enemyBody.GetComponent<CustomAnimator>();
        spriteRenderer = enemyBody.GetComponent<SpriteRenderer>();
        rb2 = enemyBody.GetComponent<Rigidbody2D>();
        customAnimator.ChangeState(AnimationStates.IDLING);

        material = spriteRenderer.material; // Obtém o material do SpriteRenderer
        defaultShader = Shader.Find("Sprites/Default"); // Shader padrão do Unity
        flashShader = Shader.Find("GUI/Text Shader");  // Shader para "acender"


    }

    private void Update()
    {
        if (hp <= 0)
        {
            Die();
            return;
        }
        FollowPlayer();
        Animate();
    }

    void FollowPlayer()
    {
        if (player == null || hp <= 0 || isColliding || enemyBody == null) return;
        Collider2D colliderPlayer = Physics2D.OverlapBox(enemyBody.transform.position, new Vector2(hitBoxSize.x, hitBoxSize.y), 0f, playerColliderMask);
        Collider2D colliderBullet = Physics2D.OverlapBox(enemyBody.transform.position, new Vector2(hitBoxSize.x, hitBoxSize.y), 0f, hitMask);
        //Debug.Log($"colliderBullet: {colliderBullet}");
        //float distance = Math.Abs(Vector2.Distance(enemyBody.transform.position, player.position));
        if (colliderPlayer != null && !isColliding) //(distance > stopDistance) 
        {
            StartCoroutine(HandleCollision());
            player.GetComponentInParent<PlayerController>().PlayerFlash();
        }
        else
        {
            Move();
        }

        if (colliderBullet != null && !isHitting)
        {

            StartCoroutine(FlashRoutine()); // pisca pisca do inimigo
            StartCoroutine(HandleBulletHit(colliderBullet)); // colisão com bala

        }

    }

    private void Move()
    {
        Vector2 direction = (player.position - enemyBody.transform.position).normalized;
        rb2.linearVelocity = new Vector2(direction.x * moveSpeed, rb2.linearVelocity.y);
        isMoving = true;

        currentSpeed = rb2.linearVelocity.x;

        float newSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * acceleration);

        rb2.linearVelocity = new Vector2(newSpeed, rb2.linearVelocity.y);
    }

    IEnumerator HandleCollision()
    {
        PlayerController playerController = player.GetComponentInParent<PlayerController>();

        if (playerController.isStarBoostActivated)
        {
            hp = 0;
            Die();
            yield break;
        }
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
        int damage = (int)player.GetComponent<PlayerController>().bulletDmg; // dano da bala atual
        hp -= damage;

        Destroy(colliderBullet.gameObject);
        if (hp <= 0)
        {
            Die();
        }
        yield return new WaitForSeconds(.3f);
        isHitting = false;
    }

    void Animate()
    {
        if (hp <= 0) return;

        if (isMoving && rb2 != null && spriteRenderer != null)
        {
            if (customAnimator.currentState != AnimationStates.RUNNING) customAnimator.ChangeState(AnimationStates.RUNNING);

            if (rb2.linearVelocity.x < -0.05 && !spriteRenderer.flipX)
                spriteRenderer.flipX = true;
            else if (rb2.linearVelocity.x > 0.05 && spriteRenderer.flipX)
                spriteRenderer.flipX = false;
        }
        else if (customAnimator.currentState != AnimationStates.IDLING)
        {
            customAnimator.ChangeState(AnimationStates.IDLING);
        }
    }

    private void Die()
    {
        if (!hasSpawnedDrop)
        {
            hasSpawnedDrop = true;
            GameManager.instance.collectableManager.SpawnDrop(dropProb, enemyBody.transform);
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(enemyBody.transform.position, hitBoxSize);
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
