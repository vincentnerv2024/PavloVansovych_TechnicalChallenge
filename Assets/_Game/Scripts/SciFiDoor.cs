using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class SciFiDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorTransform; // The door object to move
    [SerializeField] private Vector3 openPosition; // Target position when open (relative to start position)
    [SerializeField] private float duration = 1f; // Animation duration
    [SerializeField] private Ease easeType = Ease.OutCubic; // Animation ease
    [SerializeField] private float autoCloseDelay = 2f; // Delay before auto-closing the door

    private Vector3 closedPosition; // Original position of the door
    private bool isOpen = false; // State of the door
    private Tween currentTween; // Reference to the active tween

    public MMF_Player doorOpenPlayer;
    public MMF_Player doorClosePlayer;

    private void Start()
    {
        // Save the initial position of the door as the closed position
        if (doorTransform == null)
        {
            Debug.LogError("Door Transform not assigned.");
            return;
        }
        closedPosition = doorTransform.localPosition;
    }

    /// <summary>
    /// Opens the door and optionally closes it after a delay.
    /// </summary>
    public void OpenDoor()
    {
        if (isOpen || doorTransform == null) return;

        isOpen = true;

        // Stop any running tween and move the door to the open position
        currentTween?.Kill();
        currentTween = doorTransform.DOLocalMove(closedPosition + openPosition, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                if (autoCloseDelay > 0)
                {
                    Invoke(nameof(CloseDoor), autoCloseDelay);
                }
            });
    }

    /// <summary>
    /// Closes the door.
    /// </summary>
    public void CloseDoor()
    {
        if (!isOpen || doorTransform == null) return;

        isOpen = false;
        doorClosePlayer.PlayFeedbacks();

        // Stop any running tween and move the door to the closed position
        currentTween?.Kill();
        currentTween = doorTransform.DOLocalMove(closedPosition, duration)
            .SetEase(easeType);
    }

    /// <summary>
    /// Toggles the door's state between open and closed.
    /// </summary>
    public void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    // Optional: Debug testing in the Editor
    private void OnDrawGizmosSelected()
    {
        if (doorTransform == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(doorTransform.position + openPosition, doorTransform.localScale);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(closedPosition, doorTransform.localScale);
    }
}
