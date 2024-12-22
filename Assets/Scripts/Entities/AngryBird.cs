using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryBird : MonoBehaviour
{
    [SerializeField] private AudioClip hitClip;
    public int projectileType;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private AudioSource audioSource;
    public bool hasBeenLaunched;

    private IProjectileBehavior projectileBehavior;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        rb.isKinematic = true;
        circleCollider.enabled = false;
    }

    public void SetProjectileBehavior(IProjectileBehavior behavior)
    {
        projectileBehavior = behavior;
    }

    public void LaunchBird(Vector2 direction, float force)
    {
        circleCollider.enabled = true;
        hasBeenLaunched = true;

        projectileBehavior?.OnLaunch(rb, direction, force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenLaunched)
        {
            projectileBehavior?.OnCollision(transform.position);
            SoundManager.instance.PlayClip(hitClip, audioSource);

            if(projectileBehavior is BombProjectileBehavior || projectileBehavior is ShurikenProjectileBehavior)
            {
                StartCoroutine(HandleExplosionAndDestroy());
            }
        }
    }

    private IEnumerator HandleExplosionAndDestroy()
    {
        float animationDuration = 0.3f; 
        yield return new WaitForSeconds(animationDuration);

        Destroy(gameObject);
    }
}

