using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int hp = 100;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Transform player;
    // [SerializeField] float stopDistance = .05f; // distancia da colisão
    [SerializeField] Transform enemyBody;


    //private float targetSpeed;
    private float currentSpeed;
    private float acceleration = 3f;


    public Rigidbody2D rb2;
    public Animator animator;
    public CustomAnimator customAnimator;
    public SpriteRenderer spriteRenderer;

    [SerializeField] private LayerMask playerColliderMask;
    [SerializeField] private LayerMask hitMask;


    private bool isMoving = true;
    private bool isColliding = false;

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
        Debug.Log($"colliderBullet: {colliderBullet}");
        //float distance = Math.Abs(Vector2.Distance(enemyBody.transform.position, player.position));
        if (colliderPlayer != null)//(distance > stopDistance) 
        {
            StartCoroutine(HandleCollision());
        }
        else if (colliderBullet != null && colliderBullet.gameObject.layer == LayerMask.NameToLayer("Bullet")) // colisão com bala
        {
            int damage = 50; // Defina o dano da bala
            hp -= damage;

            // Verifica se o inimigo ainda está vivo
            if (hp <= 0)
            {
                Die();
            }
            // Destroi a bala para evitar múltiplas colisões
            Destroy(colliderBullet.gameObject);

        }
        else
        {
            Move();
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
        playerController.hp -= 34; // da 50 de dano ao colidir com o player
        isColliding = true;
        rb2.linearVelocity = Vector2.zero;
        isMoving = false;
        yield return new WaitForSeconds(1.5f);
        isColliding = false;
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

    void Die()
    {
        //BoxCollider2D bc = GetComponent<BoxCollider2D>();

        //rb2.linearVelocity = Vector2.zero;
        //rb2.bodyType = RigidbodyType2D.Kinematic;
        //bc.forceSendLayers = 0;
        //bc.forceReceiveLayers = 0;


        //// tem que igonorar apenas entre os corpos e não o layer
        //if (bc != null && playerCollider != null)
        //{
        //    Physics2D.IgnoreCollision(bc, playerCollider);
        //}

        customAnimator.ChangeState(AnimationStates.DYING);
        float duration = customAnimator.GetAnimationDuration(AnimationStates.DYING);
        Destroy(gameObject, duration);
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.waveManager.currentEnemies--;
        }
    }

    //void Die()
    //{
    //    rb2.linearVelocity = Vector2.zero;
    //    customAnimator.ChangeState(AnimationStates.DYING);
    //    StartCoroutine(DestroyAfterAnimation());
    //}

    //IEnumerator DestroyAfterAnimation()
    //{
    //    float duration = customAnimator.GetAnimationDuration(AnimationStates.DYING);
    //    Debug.Log(duration);
    //    yield return new WaitForSeconds(duration); // Supondo que CustomAnimator tenha esse m�todo
    //    Destroy(gameObject);
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(enemyBody.transform.position, hitBoxSize);
    }
}
