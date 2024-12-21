using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageThreshold = 0.2f;
    [SerializeField] protected GameObject deathParticles;
    [SerializeField] protected AudioClip deathClip;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = CalculateDamage(collision.relativeVelocity);
        if (damage > damageThreshold)
        {
            DamagePig(damage);
        }
    }

    protected virtual float CalculateDamage(Vector2 collisionVelocity)
    {
        return collisionVelocity.magnitude;
    }

    public void DamagePig(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.RemovePig(this);
        DeathEffect();
        Destroy(gameObject);
    }

    protected virtual void DeathEffect()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }
}
