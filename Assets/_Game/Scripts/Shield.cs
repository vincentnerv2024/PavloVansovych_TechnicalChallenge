using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [Header("Shield Settings")]
    public int maxShieldHealth = 100;   // Maximum shield health
    private int currentShieldHealth;   // Current shield health

    [Header("UI References")]
    public Image shieldHealthBar;      // UI Image to represent shield health (fillAmount)

    [Header("Effects")]
    public GameObject shieldDepletedFX; // Effect when shield is destroyed

    void Start()
    {
        // Initialize shield health
        currentShieldHealth = maxShieldHealth;

        // Set the initial fill amount
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentShieldHealth -= damage;

        // Clamp health to avoid negative values
        currentShieldHealth = Mathf.Clamp(currentShieldHealth, 0, maxShieldHealth);

        // Update the health bar
        UpdateHealthBar();

        // Check if the shield is depleted
        if (currentShieldHealth <= 0)
        {
            ShieldDepleted();
        }
    }

    private void UpdateHealthBar()
    {
        if (shieldHealthBar != null)
        {
            shieldHealthBar.fillAmount = (float)currentShieldHealth / maxShieldHealth;
        }
    }

    private void ShieldDepleted()
    {
        if (shieldDepletedFX != null)
        {
            Instantiate(shieldDepletedFX, transform.position, Quaternion.identity);
        }

        // Optionally disable or destroy the shield
        Destroy(gameObject);
    }

    public void RepairShield(int repairAmount)
    {
        currentShieldHealth += repairAmount;

        // Clamp health to avoid exceeding the max value
        currentShieldHealth = Mathf.Clamp(currentShieldHealth, 0, maxShieldHealth);

        // Update the health bar
        UpdateHealthBar();
    }
}
