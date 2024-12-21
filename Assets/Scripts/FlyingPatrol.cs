using UnityEngine;

public class FlyingPatrol : Patrol
{
    [SerializeField] private float verticalMoveRange = 1f;
    [SerializeField] private float verticalMoveInterval = 2f;

    private float verticalDirection = 0f;
    private float verticalMoveTimer = 0f;

    protected override void Start()
    {
        base.Start();
        verticalDirection = Random.Range(-verticalMoveRange, verticalMoveRange);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleMovingState()
    {
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

    protected override void DeathEffect()
    {
        Vector3 particlePos = transform.position;
        Instantiate(deathParticles, particlePos, Quaternion.identity);
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }
}