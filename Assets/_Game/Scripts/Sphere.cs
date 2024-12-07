using UnityEngine;

public class Sphere : MonoBehaviour
{
    [Header("Sphere Settings")]
    public int damage = 50;                   // Damage dealt to the bot
    public GameObject explosionFX;            // Explosion effect prefab
    public float grabHoldDistance = 0.5f;     // Distance at which the sphere stays held
    public float explosionCooldown = 1f;      // Cooldown between explosions

    private Rigidbody rb;                     // Rigidbody for physics
    private bool isHeld = false;              // Whether the sphere is currently held
    private Transform holdingHand;            // Reference to the hand holding the sphere
    private bool canExplode = true;           // Ensures cooldown between explosions

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing from Sphere!");
        }
    }

    void Update()
    {
        if (isHeld && holdingHand != null)
        {
            // Move the sphere to the holding position
            transform.position = holdingHand.position + holdingHand.forward * grabHoldDistance;
            rb.linearVelocity = Vector3.zero; // Disable physics while held
        }
    }

    public void Grab(Transform hand)
    {
        isHeld = true;
        holdingHand = hand;
        rb.isKinematic = true; // Disable physics while held
    }

    public void Release(Vector3 throwForce)
    {
        isHeld = false;
        holdingHand = null;
        rb.isKinematic = false; // Enable physics when released
        rb.AddForce(throwForce, ForceMode.Impulse); // Apply throw force
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canExplode && collision.collider.CompareTag("Bot"))
        {
            Debug.Log("Collided with bot...");
            // Damage the bot
            AIBotController botHealth = collision.collider.GetComponent<AIBotController>();
            if (botHealth != null)
            {
                botHealth.TakeDamage(damage);
            }

            // Spawn explosion effect
            SpawnExplosion();

            // Start cooldown
            StartCoroutine(ExplosionCooldown());
        }
    }

    private void SpawnExplosion()
    {
        if (explosionFX != null)
        {
            Instantiate(explosionFX, transform.position, Quaternion.identity);
        }
    }

    private System.Collections.IEnumerator ExplosionCooldown()
    {
        canExplode = false;
        yield return new WaitForSeconds(explosionCooldown);
        canExplode = true;
    }
}
