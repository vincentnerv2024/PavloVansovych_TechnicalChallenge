using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public bool IsDead;

    public Image healthFillImage; 

    public MMF_Player onHitFeedback;
    public MMF_Player gameOverFeedback;

    private void Start()
    {
        currentHealth = maxHealth;
        IsDead = false;

        UpdateHealthUI(); 
    }

    public void TakeDamage(int damage)
    {
        onHitFeedback.PlayFeedbacks();
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        UpdateHealthUI();
        // Lock the cursor to the center of the screen
        
        if (currentHealth <= 0 && !IsDead)
        {

            Die();
            DOVirtual.DelayedCall(3f, ()=>SceneManager.LoadScene(0));
        }
    }

    private void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)currentHealth / maxHealth; 
        }
    }

    private void Die()
    {
        gameOverFeedback.PlayFeedbacks();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; // Make the cursor visible
        IsDead = true;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); 
        UpdateHealthUI();
    }
}
