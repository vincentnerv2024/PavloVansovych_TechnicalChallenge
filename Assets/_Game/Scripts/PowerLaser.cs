using UnityEngine;

public class PowerLaser : MonoBehaviour
{
    [Header("Laser Settings")]
    public int damagePerSecond = 10;  // Damage dealt to the player per second
    public float damageInterval = 1f; // Time interval for applying damage
    public bool isLaserActive = true; // Whether the laser is active or not

    private float damageTimer;        // Timer to track intervals

    private void Update()
    {
        damageTimer += Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("LaserWorks");
        }

        if (other.CompareTag("Player"))
        {
            

            if (damageTimer >= damageInterval)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damagePerSecond);
                    Debug.Log("Laser dealt damage to the player!");
                }

                damageTimer = 0f; // Reset the timer
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isLaserActive) return;

        // Check if the player is inside the laser's trigger
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damagePerSecond);
                    Debug.Log("Laser dealt damage to the player!");
                }

                damageTimer = 0f; // Reset the timer
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer = 0f; // Reset the timer when the player leaves the laser
        }
    }
}
