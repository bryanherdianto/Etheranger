using UnityEngine;

public class FlyingPatrol : Patrol
{
    [SerializeField] private float verticalMoveRange = 1f;
    [SerializeField] private float verticalMoveInterval = 2f;
    [SerializeField] private float gravityIncreaseRate = 0.1f;

    private float verticalDirection = 0f;
    private float verticalMoveTimer = 0f;
    private bool isFalling = false;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        verticalDirection = Random.Range(-verticalMoveRange, verticalMoveRange);
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    protected override void Update()
    {
        base.Update();
        if (isFalling)
        {
            rb.gravityScale = Mathf.Min(rb.gravityScale + gravityIncreaseRate * Time.deltaTime, 1f);
        }
    }

    protected override void HandleMovingState()
    {
        if (isFalling) return;

        animator.SetBool("isRunning", true);
        Vector2 currentPos = transform.position;
        Vector2 targetPos = direction > 0 ? targetPosition : originPosition;

        float newX = Mathf.MoveTowards(currentPos.x, targetPos.x, moveSpeed * Time.deltaTime);

        verticalMoveTimer += Time.deltaTime;
        if (verticalMoveTimer >= verticalMoveInterval)
        {
            verticalMoveTimer = 0f;
            verticalDirection = Random.Range(-verticalMoveRange, verticalMoveRange);
        }
        float newY = Mathf.MoveTowards(currentPos.y, currentPos.y + verticalDirection, moveSpeed * Time.deltaTime);

        transform.position = new Vector2(newX, newY);

        if (Mathf.Abs(currentPos.x - targetPos.x) < 0.01f)
        {
            transform.position = new Vector2(targetPos.x, currentPos.y);
            isRunning = false;
        }

        spriteRenderer.flipX = direction < 0;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (!collision.gameObject.CompareTag("Ground"))
        {
            isFalling = true;
        }
    }

    protected override void DeathEffect()
    {
        Vector3 particlePos = transform.position;
        Instantiate(deathParticles, particlePos, Quaternion.identity);
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }
}