using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    // EnvironmentManager is responsible for managing the overall environment, 
    // such as what visuals are displayed and where items should spawn

    #region VARIABLES

    public static EnvironmentManager Instance { get; private set; }

    [SerializeField] private GameObject desertEnvironment;
    [SerializeField] private Transform[] desertSpawnpoints;
    [SerializeField] private GameObject iceEnvironment;
    [SerializeField] private Transform[] iceSpawnpoints;
    private Transform lastUsedSpawnPoint;

    private string currentEnvironment;

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
    }

    #endregion

    #region ENVIRONMENT SWAPPING

    public string GetCurrentEnvironment()
    {
        return currentEnvironment;
    }

    // Sets the current environment and updates visuals and spawn points accordingly
    public void SetEnvironment(string environmentName)
    {
        switch (environmentName.ToLower())
        {
            case "desert":
                desertEnvironment.SetActive(true);
                iceEnvironment.SetActive(false);
                break;
            case "snow":
                desertEnvironment.SetActive(false);
                iceEnvironment.SetActive(true);
                break;
        }

        currentEnvironment = environmentName;
    }

    #endregion

    #region SPAWNPOINTS

    // Gets a random spawnpoint based on the current environment type
    public Transform GetRandomSpawnPoint()
    {
        Transform[] spawnpoints = null;

        switch (currentEnvironment.ToLower())
        {
            case "desert":
                spawnpoints = desertSpawnpoints;
                break;
            case "snow":
                spawnpoints = iceSpawnpoints;
                break;
        }

        if (spawnpoints != null && spawnpoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnpoints.Length);
            if (spawnpoints[randomIndex] == lastUsedSpawnPoint)
            {
                randomIndex = (randomIndex + 1) % spawnpoints.Length; // Ensure a different spawn point is used
            }
            lastUsedSpawnPoint = spawnpoints[randomIndex];
            return spawnpoints[randomIndex];
        }

        return null;
    }

    #endregion

}
