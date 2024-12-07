using System.Collections;
using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour
{
    [Header("NPC Settings")]
    public Animator animator; // Animator for Idle, Cough, and Interaction animations
    public TMP_Text worldUIText; // TextMeshPro component for World UI
    public float typingSpeed = 0.05f; // Speed of the typing effect

    [Header("Tips")]
    public string tip1 = "Hello, traveler! Take this advice.";
    public string tip2 = "Remember to save your progress often.";

    [Header("Cough Settings")]
    public float coughInterval = 10f; // Interval between coughs in seconds

    private bool hasShownTip1 = false;

    void Start()
    {
        if (worldUIText == null)
        {
            Debug.LogError("World UI TextMeshPro is not assigned!");
        }

        ResetWorldUIText();

        // Start the coughing routine
        StartCoroutine(CoughRoutine());
    }

    // Method to trigger interaction
    public void Interact()
    {
        animator.SetTrigger("Interact");

        if (!hasShownTip1)
        {
            StartCoroutine(DisplayTip(tip1));
            hasShownTip1 = true;
        }
        else
        {
            StartCoroutine(DisplayTip(tip2));
        }
    }

    // Typing effect for displaying text
    private IEnumerator DisplayTip(string tip)
    {
        ResetWorldUIText();
        foreach (char letter in tip)
        {
            worldUIText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Trigger idle animation after text is displayed
        animator.SetTrigger("Idle");
    }

    // Reset the UI text
    private void ResetWorldUIText()
    {
        worldUIText.text = string.Empty;
    }

    // Coroutine to trigger coughing at set intervals
    private IEnumerator CoughRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(coughInterval); // Wait for the specified interval
            TriggerCough();
        }
    }

    // Method to manually trigger cough animation
    public void TriggerCough()
    {
        animator.SetTrigger("Cough");
    }
}
