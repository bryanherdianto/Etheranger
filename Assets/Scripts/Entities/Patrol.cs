using Unity.VisualScripting;
using UnityEngine;

public class Patrol : Enemy
{
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] private float idleTime = 2f;
    
    protected Vector2 originPosition;
    protected Vector2 targetPosition;
    protected float direction = 1f;
    protected bool isRunning = false;
    private float idleTimer = 0f;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    private Vector2 lastPosition;
    private Vector2 selfMovementVelocity;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originPosition = transform.position;
        targetPosition = originPosition + Vector2.right * patrolDistance;
        lastPosition = transform.position;
    }

    protected virtual void Update()
    {
        selfMovementVelocity = ((Vector2)transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        if (!isRunning)
        {
            HandleIdleState();
        }
        else
        {
            HandleMovingState();
        }
    }

    private void HandleIdleState()
    {
        animator.SetBool("isRunning", false);
        idleTimer += Time.deltaTime;
        
        if (idleTimer >= idleTime)
        {
            isRunning = true;
            idleTimer = 0f;
            direction *= -1f;
            spriteRenderer.flipX = direction < 0;
        }
    }

    protected virtual void HandleMovingState()
    {
        animator.SetBool("isRunning", true);
        Vector2 currentPos = transform.position;
        Vector2 targetPos = direction > 0 ? targetPosition : originPosition;
        
        float newX = Mathf.MoveTowards(currentPos.x, targetPos.x, moveSpeed * Time.deltaTime);
        transform.position = new Vector2(newX, currentPos.y);

        if (Mathf.Abs(currentPos.x - targetPos.x) < 0.01f)
        {
            transform.position = new Vector2(targetPos.x, currentPos.y);
            isRunning = false;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            base.OnCollisionEnter2D(collision);
        }
    }

    protected override float CalculateDamage(Vector2 collisionVelocity)
    {
        Vector2 effectiveVelocity = collisionVelocity - selfMovementVelocity;
        return effectiveVelocity.magnitude;
    }

    protected override void DeathEffect()
    {
        Vector3 particlePos = transform.position + new Vector3(0, 2.7f, 0);
        Instantiate(deathParticles, particlePos, Quaternion.identity);
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }
}