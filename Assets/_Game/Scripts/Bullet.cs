using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;             // Speed of the bullet
    public int damage = 10;              // Damage dealt to the player or shield
    public float maxDistance = 100f;     // Maximum distance the bullet can travel
    public float lifeTime = 5f;          // Lifetime of the bullet
    public GameObject explosionFX;       // Explosion effect prefab

    private Vector3 targetDirection;     // Direction the bullet will travel
    private Vector3 startPosition;       // Initial position of the bullet

    public void Initialize(Vector3 targetPosition)
    {
        startPosition = transform.position;
        targetDirection = (targetPosition - transform.position).normalized; // Direction toward target
        MoveInDirection();
        Destroy(gameObject, lifeTime); // Destroy the bullet after its lifetime
    }

    private void MoveInDirection()
    {
        Vector3 destination = startPosition + targetDirection * maxDistance;

        // Move the bullet toward the calculated destination using DOTween
        transform.DOMove(destination, maxDistance / speed)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shield")) // If the bullet hits the shield
        {
            Shield shield = other.GetComponent<Shield>();
            if (shield != null)
            {
                shield.TakeDamage(damage); // Apply damage to the shield
            }
            SpawnExplosion();
            
        }
        else if (other.CompareTag("Player")) // If the bullet hits the player
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Apply damage to the player
            }
            SpawnExplosion();

            
         
        }
    }

    private void SpawnExplosion()
    {
        if (explosionFX != null)
        {
            Instantiate(explosionFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); // Destroy the bullet on impact
    }
}
