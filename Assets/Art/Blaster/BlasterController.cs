using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BlasterController : MonoBehaviour
{

    // BlasterController handles the blaster rifle's firing and reloading mechanics.


    #region VARIABLES


    [Header("Input")]
    [SerializeField] private InputActionReference triggerAction;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Material readyToFireMaterial;
    [SerializeField] private GameObject blasterLaserPrefab;
    [SerializeField] private Transform laserOrigin;

    [Header("Settings")]
    [SerializeField] private string reloadAnimationName = "Reload";
    [SerializeField] private float laserMaxDistance = 100f;
    [SerializeField] private float laserFadeDuration = 0.5f;
    [SerializeField] private float manualReloadDuration = 1.5f; // Fallback if animation duration can't be found

    private bool isReloading = false;
    private float reloadStartTime;
    private float reloadDuration;


    #endregion


    #region EVENT SUBSCRIPTIONS


    // OnEnable, subscribe to the trigger action
    private void OnEnable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.Enable();
            triggerAction.action.performed += OnTriggerPulled;
        }
    }

    // OnDisable, unsubscribe from the trigger action
    private void OnDisable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed -= OnTriggerPulled;
            triggerAction.action.Disable();
        }
    }


    #endregion


    #region START AND SETUP


    // On Start, cache the reload animation duration and set initial material color
    private void Start()
    {
        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Cache the reload animation duration
        bool foundAnimation = false;
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == reloadAnimationName)
                {
                    reloadDuration = clip.length;
                    foundAnimation = true;
                    Debug.Log($"Found reload animation '{reloadAnimationName}' with duration: {reloadDuration}s");
                    break;
                }
            }

            if (!foundAnimation)
            {
                Debug.LogWarning($"Could not find animation '{reloadAnimationName}' in animator. Using manual reload duration: {manualReloadDuration}s");
                reloadDuration = manualReloadDuration;
            }
        }
        else
        {
            Debug.LogWarning($"Animator not found or has no controller. Using manual reload duration: {manualReloadDuration}s");
            reloadDuration = manualReloadDuration;
        }

        // Set initial material color to green (ready to fire)
        UpdateMaterialColor();
    }


    #endregion


    #region UPDATE


    // On Update, check if reload is complete and update material color accordingly
    private void Update()
    {
        // Check if reload is complete
        if (isReloading && Time.time >= reloadStartTime + reloadDuration)
        {
            isReloading = false;
            Debug.Log($"Reload complete - Ready to fire! (Reloaded for {reloadDuration}s)");
            UpdateMaterialColor();
        }
    }


    #endregion


    #region FIRING


    // When the trigger is pulled, attempt to fire the blaster
    private void OnTriggerPulled(InputAction.CallbackContext context)
    {
        // Only fire if not currently reloading
        if (!isReloading)
        {
            Fire();
        }
        else
        {
            float timeRemaining = (reloadStartTime + reloadDuration) - Time.time;
            Debug.Log($"Cannot fire - still reloading! {timeRemaining:F2}s remaining");
        }
    }

    // Handle the firing logic: play muzzle flash and start reload sequence
    private void Fire()
    {
        Debug.Log("Blaster fired!");

        // Play muzzle flash effect
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Fire laser raycast
        FireLaser();

        // Start reload sequence
        StartReload();
    }

    // Fire a laser raycast and spawn visual laser line
    private void FireLaser()
    {
        if (blasterLaserPrefab == null || laserOrigin == null)
        {
            Debug.LogWarning("BlasterLaser prefab or laser origin not assigned!");
            return;
        }

        // Perform raycast from laser origin
        Vector3 startPoint = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;
        Vector3 endPoint;

        RaycastHit hit;
        if (Physics.Raycast(startPoint, direction, out hit, laserMaxDistance))
        {
            // Hit something
            endPoint = hit.point;
            Debug.Log($"Laser hit: {hit.collider.gameObject.name}");
        }
        else
        {
            // Didn't hit anything, use max distance
            endPoint = startPoint + direction * laserMaxDistance;
        }

        // Spawn laser line
        GameObject laserObject = Instantiate(blasterLaserPrefab, startPoint, Quaternion.identity);
        LineRenderer lineRenderer = laserObject.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            // Set line positions
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);

            // Start fade coroutine
            StartCoroutine(FadeLaser(laserObject, lineRenderer));
        }
        else
        {
            Debug.LogWarning("BlasterLaser prefab does not have a LineRenderer component!");
            Destroy(laserObject);
        }
    }


    #endregion


    #region RELOADING


    // Start the reload process: set reloading state, update material color, and play reload animation
    private void StartReload()
    {
        isReloading = true;
        reloadStartTime = Time.time;

        // Update material color to red (not ready to fire)
        UpdateMaterialColor();

        // Play reload animation from the beginning
        if (animator != null)
        {
            // Play(stateName, layer, normalizedTime) - normalizedTime 0 means start from beginning
            animator.Play(reloadAnimationName, 0, 0f);
            Debug.Log($"Starting reload animation '{reloadAnimationName}' for {reloadDuration}s...");
        }
        else
        {
            Debug.LogWarning("Animator not found - reload will still work but no animation will play");
        }
    }

    // Update the material color based on the current state (red for reloading, green for ready to fire)
    private void UpdateMaterialColor()
    {
        if (readyToFireMaterial != null)
        {
            if (isReloading)
            {
                // Red when reloading
                readyToFireMaterial.color = Color.red;
            }
            else
            {
                // Green when ready to fire
                readyToFireMaterial.color = Color.green;
            }
        }
    }


    #endregion


    #region LASER EFFECTS


    // Fade the laser line over time by reducing its width, then destroy it
    private IEnumerator FadeLaser(GameObject laserObject, LineRenderer lineRenderer)
    {
        float elapsed = 0f;
        float initialWidth = lineRenderer.startWidth;

        while (elapsed < laserFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / laserFadeDuration;

            // Fade width from initial to 0
            float currentWidth = Mathf.Lerp(initialWidth, 0f, t);
            lineRenderer.startWidth = currentWidth;
            lineRenderer.endWidth = currentWidth;

            yield return null;
        }

        // Ensure fully faded
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;

        // Destroy the laser object
        Destroy(laserObject);
    }


    #endregion


}
