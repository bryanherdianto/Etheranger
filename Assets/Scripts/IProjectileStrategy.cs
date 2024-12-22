using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileBehavior
{
    void OnLaunch(Rigidbody2D rb, Vector2 direction, float force);
    void OnCollision(Vector2 position);
}

public class NormalProjectileBehavior : IProjectileBehavior
{
    public void OnLaunch(Rigidbody2D rb, Vector2 direction, float force)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void OnCollision(Vector2 position)
    {
        // Normal projectile has no special collision behavior
    }
}

public class BombProjectileBehavior : IProjectileBehavior
{
    private readonly float blastForce;
    private readonly float blastRadius;
    private Animator animator;

    public BombProjectileBehavior(float blastForce, float blastRadius, Animator animator)
    {
        this.blastForce = blastForce;
        this.blastRadius = blastRadius;
        this.animator = animator;
    }

    public void OnLaunch(Rigidbody2D rb, Vector2 direction, float force)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void OnCollision(Vector2 position)
    {
        if (animator != null)
        {
            animator.SetBool("isBlasting", true); 
        }
        
        Collider2D[] inBlastRadius = Physics2D.OverlapCircleAll(position, blastRadius);

        foreach (Collider2D obj in inBlastRadius)
        {
            Rigidbody2D objectRb = obj.GetComponent<Rigidbody2D>();
            if (objectRb != null)
            {
                Vector2 distanceVector = (Vector2)obj.transform.position - position;
                if (distanceVector.magnitude > 0)
                {
                    float force = blastForce / distanceVector.magnitude;
                    objectRb.AddForce(distanceVector.normalized * force, ForceMode2D.Impulse);
                }
            }
        }
    }
}

public class ShurikenProjectileBehavior : IProjectileBehavior
{
    private readonly float pullForce;
    private readonly float pullRadius;
    private Animator animator;

    public ShurikenProjectileBehavior(float pullForce, float pullRadius, Animator animator)
    {
        this.pullForce = pullForce;
        this.pullRadius = pullRadius;
        this.animator = animator;
    }

    public void OnLaunch(Rigidbody2D rb, Vector2 direction, float force)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        if (animator != null)
        {
            animator.SetBool("isLaunched", true); 
        }
    }

    public void OnCollision(Vector2 position)
    {   
        Collider2D[] inBlastRadius = Physics2D.OverlapCircleAll(position, pullRadius);

        foreach (Collider2D obj in inBlastRadius)
        {
            Rigidbody2D objectRb = obj.GetComponent<Rigidbody2D>();
            if (objectRb != null)
            {
                Vector2 distanceVector = (Vector2)obj.transform.position - position;
                if (distanceVector.magnitude > 0)
                {
                    float force = -pullForce / distanceVector.magnitude;
                    objectRb.AddForce(distanceVector.normalized * force, ForceMode2D.Impulse);
                }
            }
        }
    }
}