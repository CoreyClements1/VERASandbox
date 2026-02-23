using UnityEngine;

internal class UiFollowGaze : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The player's head/camera transform. If null, will use Camera.main")]
    [SerializeField] private Transform playerHead;

    [Header("Position Settings")]
    [Tooltip("Horizontal distance to maintain from the player")]
    [SerializeField] private float horizontalDistance = 3f;
    [Tooltip("Vertical offset from the gaze height")]
    [SerializeField] private float verticalOffset = 0f;
    [Tooltip("Speed at which the canvas moves to target position")]
    [SerializeField] private float positionSmoothSpeed = 3f;
    [Tooltip("Minimum distance the gaze must move before updating target position")]
    [SerializeField] private float updateDistanceThreshold = 1f;

    [Header("Rotation Settings")]
    [Tooltip("Speed at which the canvas rotates to face the player")]
    [SerializeField] private float rotationSmoothSpeed = 3f;

    private Vector3 currentTargetPosition;

    private void Start()
    {
        // If no player head is assigned, try to find the main camera
        if (playerHead == null)
        {
            ResetPlayerHeadRef();
        }

        // Initialize current target position
        if (playerHead != null)
        {
            currentTargetPosition = CalculateTargetPosition();
            transform.position = currentTargetPosition;
        }
    }

    private void ResetPlayerHeadRef()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            playerHead = mainCamera.transform;
        }
    }

    private void LateUpdate()
    {
        if (playerHead == null) return;

        // Calculate potential new target position
        Vector3 newTargetPosition = CalculateTargetPosition();

        // Only update the target position if it's far enough from the current target
        float distanceToNewTarget = Vector3.Distance(currentTargetPosition, newTargetPosition);
        if (distanceToNewTarget >= updateDistanceThreshold)
        {
            currentTargetPosition = newTargetPosition;
        }

        // Smoothly move towards the current target position
        transform.position = Vector3.Lerp(transform.position, currentTargetPosition, positionSmoothSpeed * Time.deltaTime);

        // Calculate and apply target rotation (facing the player)
        Quaternion targetRotation = CalculateTargetRotation();
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
    }

    private Vector3 CalculateTargetPosition()
    {
        // Get player's forward direction projected onto horizontal plane (XZ)
        Vector3 playerForward = playerHead.forward;
        Vector3 horizontalForward = new Vector3(playerForward.x, 0, playerForward.z).normalized;

        // Handle case where player is looking straight up or down
        if (horizontalForward == Vector3.zero)
        {
            horizontalForward = new Vector3(playerHead.transform.forward.x, 0, playerHead.transform.forward.z);
            if (horizontalForward == Vector3.zero)
            {
                horizontalForward = Vector3.forward; // Fallback
            }
            horizontalForward.Normalize();
        }

        // Calculate position: player position + (horizontal forward * distance)
        Vector3 playerHorizontalPosition = new Vector3(playerHead.position.x, 0, playerHead.position.z);
        Vector3 targetHorizontalPosition = playerHorizontalPosition + horizontalForward * horizontalDistance;

        // Set Y position to match player's head height with vertical offset
        return new Vector3(targetHorizontalPosition.x, playerHead.position.y + verticalOffset, targetHorizontalPosition.z);
    }

    private Quaternion CalculateTargetRotation()
    {
        // Calculate direction from canvas to player on horizontal plane
        Vector3 canvasHorizontalPosition = new Vector3(transform.position.x, playerHead.position.y, transform.position.z);
        Vector3 directionToPlayer = (playerHead.position - canvasHorizontalPosition).normalized;

        // Project direction onto horizontal plane to ignore vertical component
        directionToPlayer = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;

        // Create rotation that looks at the player horizontally, then flip 180 degrees
        if (directionToPlayer != Vector3.zero)
        {
            // Flip 180 degrees around the Y axis
            return Quaternion.LookRotation(-directionToPlayer, Vector3.up);
        }

        return transform.rotation;
    }

    public void ResetPositionImmediate()
    {
        if (playerHead == null)
        {
            ResetPlayerHeadRef();
            if (playerHead == null) return;
        }

        // Calculate and apply target position
        Vector3 targetPosition = CalculateTargetPosition();
        currentTargetPosition = targetPosition;
        transform.position = targetPosition;

        // Calculate and apply target rotation (facing the player)
        Quaternion targetRotation = CalculateTargetRotation();
        transform.rotation = targetRotation;
    }

    public void SetParameters(float heightOffset, float distance)
    {
        verticalOffset = heightOffset;
        horizontalDistance = distance;
    }
}