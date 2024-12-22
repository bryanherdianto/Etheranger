using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBlocks : MonoBehaviour
{
    [SerializeField] private float destructionThreshold = 10.0f; 
    [SerializeField] private AudioClip destructionClip; 
    [SerializeField] private GameObject destructionParticles; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = CalculateDamage(collision.relativeVelocity);
        if (damage > destructionThreshold)
        {
            DestroyBlock();
        }
    }

    private float CalculateDamage(Vector2 collisionVelocity)
    {
        return collisionVelocity.magnitude;
    }

    private void DestroyBlock()
    {
        PlayDestructionEffect();
        Destroy(gameObject);
    }

    private void PlayDestructionEffect()
    {
        if (destructionParticles != null)
        {
            Instantiate(destructionParticles, transform.position, Quaternion.identity);
        }
        if (destructionClip != null)
        {
            AudioSource.PlayClipAtPoint(destructionClip, transform.position);
        }
    }
}