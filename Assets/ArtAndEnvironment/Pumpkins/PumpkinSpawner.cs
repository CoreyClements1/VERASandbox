using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PumpkinSpawner : MonoBehaviour
{

    // PumpkinSpawner is responsible for spawning new pumpkins

    #region VARIABLES

    public static PumpkinSpawner Instance { get; private set; }

    [SerializeField] private ExplodingPumpkin pumpkinPrefab;
    private ExplodingPumpkin spawnedPumpkin;

    [SerializeField] private Canvas timerCanvas;
    [SerializeField] private Image timerFillImage;
    private float canvasDefaultScale = .01f;

    #endregion

    #region SETUP

    // Setup singleton
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensure timer canvas starts hidden and at default scale
        if (timerCanvas != null)
        {
            timerCanvas.gameObject.SetActive(false);
            timerCanvas.transform.localScale = Vector3.zero;
        }
    }

    #endregion

    #region START / STOP SPAWNING

    // Starts a new round of pumpkin spawning, lasting a given duration.
    public void StartPumpkinRound(float roundDuration, Action onRoundEnd)
    {
        StartCoroutine(PumpkinRoundCoroutine(roundDuration, onRoundEnd));
    }

    private IEnumerator PumpkinRoundCoroutine(float roundDuration, Action onRoundEnd)
    {
        SpawnPumpkin();

        // Animate timer canvas growing in with quadratic ease-out
        if (timerCanvas != null)
        {
            timerCanvas.gameObject.SetActive(true);
            float growDuration = 0.3f;
            float elapsed = 0f;

            while (elapsed < growDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / growDuration;
                // Quadratic ease-out: 1 - (1 - t)^2
                float easeOut = 1f - (1f - t) * (1f - t);
                timerCanvas.transform.localScale = Vector3.one * easeOut * canvasDefaultScale;
                yield return null;
            }

            timerCanvas.transform.localScale = Vector3.one * canvasDefaultScale;
        }

        // Update timer fill during the round
        float timeRemaining = roundDuration;
        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = Mathf.Clamp01(timeRemaining / roundDuration);
            }
            yield return null;
        }

        // Round is over, stop spawning pumpkins
        StopSpawningPumpkins();

        // Make timer canvas disappear
        if (timerCanvas != null)
        {
            timerCanvas.gameObject.SetActive(false);
        }

        onRoundEnd?.Invoke();
    }

    // Stops spawning pumpkins and fades out the currently spawned pumpkin if it exists.
    public void StopSpawningPumpkins()
    {
        if (spawnedPumpkin != null)
        {
            spawnedPumpkin.DisableAndFadeAway();
        }
    }

    #endregion

    #region SPAWN

    // Spawns a new pumpkin at a random spawn point and makes it face the main camera
    public void SpawnPumpkin()
    {
        // Get random position between spawn points
        Vector3 randomPosition = EnvironmentManager.Instance.GetRandomSpawnPoint().position;

        // Instantiate new pumpkin with default rotation
        spawnedPumpkin = Instantiate(pumpkinPrefab, randomPosition, Quaternion.identity);

        // Make the pumpkin face the main camera
        if (Camera.main != null)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 lookDirection = cameraPosition - spawnedPumpkin.transform.position;
            lookDirection.y = 0f; // Keep only horizontal rotation
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                spawnedPumpkin.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        // Fade in the new pumpkin
        spawnedPumpkin.FadeIn();
        spawnedPumpkin.onExplode.AddListener(SpawnPumpkin);
    }

    #endregion

}
