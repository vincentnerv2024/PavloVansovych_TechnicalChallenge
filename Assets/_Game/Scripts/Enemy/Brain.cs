using UnityEngine;

[CreateAssetMenu(fileName = "NewBrain", menuName = "AI/Brain")]
public class Brain : ScriptableObject
{
    [Header("Reaction Settings")]
    public float reactionTime = 0.5f; // Delay before actions
    public float aimAccuracy = 0.8f; // Precision in aiming (0-1)

    [Header("Animation Settings")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;

    [Header("Special Parameters")]
    public string botTypeDescription; // Description of the bot type for debugging or lore purposes
}
