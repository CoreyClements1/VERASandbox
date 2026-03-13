using System;
using System.Collections;
using UnityEngine;

// VERA SANDBOX NOTE: If you are using VERA, make sure to include the VERA namespace at the top of your script to access VERA's features.
//using VERA;

public class ExperimentManager : MonoBehaviour
{

    /*
     * This ExperimentManager script serves as the central hub for managing the flow of the sandbox experiment.
     * It handles the initialization, progression, and termination of the experiment.
     *
     * Various comments throughout the script provide guidance on how to utilize VERA's features for:
     *     - Session management (e.g., initializing, finalizing sessions)
     *     - Condition management (e.g., assigning participants to conditions, syncing conditions across scripts)
     *     - Data logging (e.g., logging relevant data points for later analysis)
     *     - Survey integration (e.g., displaying surveys and logging responses)
     * These comments are tagged with "VERA SANDBOX NOTE" for easy identification.
     *
     * NOTE - If you are using VERA, it is assumed you have set up a corresponding experiment on the VERA web portal
     * with the necessary conditions, files for data logging, and surveys as referenced in the code.
     *
     * For more information, refer to the VERA Sandbox documentation at vera-xr.io/documentation/sandbox-demo
     */


    #region VARIABLES


    public static ExperimentManager Instance { get; private set; } // Singleton instance for easy access across scripts

    [Tooltip("How long each round of pumpkin shooting will last in seconds.")]
    [SerializeField] private float pumpkinRoundDuration = 20f;
    [Tooltip("How many pumpkin shooting rounds will occur in each environment before switching to the next one.")]
    [SerializeField] private int roundsPerEnvironment = 3;
    [Tooltip("How many times a single participant will experience each environment before the experiment ends.")]
    [SerializeField] private int numRepetitionsOfEachEnvironment = 1;

    private int currentEnvironmentBlock = 0; // Tracks the current block of the experiment (a block consists of roundsPerEnvironment rounds in a single environment)
    private int currentRound = 0; // Tracks the current round number within the current environment block
    private int shotsFiredInRound = 0; // Tracks the number of shots fired by the participant in the current round
    private int shotsHitInRound = 0; // Tracks the number of shots that hit a target in the current round
    private bool inShootingRound = false; // Tracks whether the participant is currently in an active shooting round
    private BlasterController[] blasterControllers; // References to the participant's blaster controllers for managing firing modes


    #endregion


    #region SETUP


    // On awake, set up singleton instance
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance of ExperimentManager exists
            return;
        }

        Instance = this;

        // Get a reference to the blaster controllers in the scene
        blasterControllers = FindObjectsByType<BlasterController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }


    // On start, initialize the experiment
    void Start()
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE 1: INITIALIZATION
        // Before we initialize our experiment (e.g., setting conditions, displaying instructions), 
        // we need to ensure that VERA has finished its own initialization process.
        // You can use the VERASessionManager to check if the session is ready before proceeding:
        //     * VERASessionManager.initialized - a boolean that indicates whether VERA has finished initializing.
        //     * VERASessionManager.onInitialized - a UnityEvent that is triggered once VERA finishes initializing.

        // WITH VERA, we'd initialize the experiment only after VERA is ready:
        //VERASessionManager.onInitialized.AddListener(InitializeExperiment);

        // WITHOUT VERA, we can simply call InitializeExperiment directly:
        InitializeExperiment();

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//
    }


    // Initializes the experiment by determining participant conditions and displaying instructions.
    private void InitializeExperiment()
    {
        SetEnvironment("snow"); // Start in the snow environment

        //----------------------------------------------------//
        // VERA SANDBOX NOTE 2: PARTICIPANT IDS
        // You can use VERA to get the current participant's ID:
        //     * VERASessionManager.participantID - an integer representing the participant's unique ID for the session.
        // For this demo, we are using the participant ID to assign participants to different firing mode conditions 
        // (good aim vs. bad aim) in a balanced way - i.e., even ID's get one condition, odd ID's get the other

        // WITH VERA, we can assign conditions based on participant ID:
        //bool useBadAim = VERASessionManager.participantID % 2 == 0;

        // WITHOUT VERA, we can simply randomize condition assignment:
        bool useBadAim = UnityEngine.Random.value > 0.5f;

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//

        SetUseBadAim(useBadAim);

        // Display instructions and tutorial round
        InstructionsHandler.Instance.ShowInstructions(
            "In this experiment, pumpkins will appear around you.\n\n" +
            "Your task is to shoot as many pumpkins as you can within the time limit each round. " +
            "Point your controller at a pumpkin and pull the trigger to shoot.\n\n" +
            "Shoot the pumpkin in front of you to begin.",
            onInstructionsComplete: () =>
            {
                // Start the general experiment flow after instructions are complete
                StartCoroutine(ExperimentFlowCoroutine());
            }
        );
    }


    #endregion


    #region EXPERIMENT FLOW


    // Manages the flow of the experiment, including timing rounds and switching environments.
    private IEnumerator ExperimentFlowCoroutine()
    {
        // Calculate total "blocks" of the experiment - a single block consists of roundsPerEnvironment rounds in a single environment.
        int totalEnvironmentBlocks = 2 * numRepetitionsOfEachEnvironment;

        // Loop through each environment block
        for (currentEnvironmentBlock = 0; currentEnvironmentBlock < totalEnvironmentBlocks; currentEnvironmentBlock++)
        {
            // Do roundsPerEnvironment rounds in current environment
            for (currentRound = 0; currentRound < roundsPerEnvironment; currentRound++)
            {
                yield return new WaitForSeconds(2f);

                // Show instructions before each round
                bool roundStarted = false;
                InstructionsHandler.Instance.ShowInstructions(
                    $"Environment {currentEnvironmentBlock + 1} of {totalEnvironmentBlocks}\n" +
                    $"Round {currentRound + 1} of {roundsPerEnvironment}\n\n" +
                    "Shoot the pumpkin to begin!",
                    onInstructionsComplete: () => { roundStarted = true; }
                );

                // Wait for the participant to start the round
                yield return new WaitUntil(() => roundStarted);

                // Start the pumpkin shooting round
                bool roundEnded = false;
                shotsFiredInRound = 0;
                shotsHitInRound = 0;
                PumpkinSpawner.Instance.StartPumpkinRound(pumpkinRoundDuration, () => { roundEnded = true; });
                inShootingRound = true;

                // Wait for the round to end
                yield return new WaitUntil(() => roundEnded);
                inShootingRound = false;

                // Log summary data about the completed round
                LogRoundData();
            }

            // The block is completed - roundsPerEnvironment rounds have been completed in the current environment.
            // Disable firing while survey is shown
            foreach (var blaster in blasterControllers)
            {
                blaster.SetCanFire(false);
            }

            // Display a survey between blocks asking participants to rate their proficiency in the current environment
            yield return new WaitForSeconds(2f);
            bool surveyCompleted = false;
            ShowSurvey(onSurveyComplete: () => { surveyCompleted = true; });

            // Wait for the survey to be completed
            yield return new WaitUntil(() => surveyCompleted);

            // Re-enable firing after survey completion
            foreach (var blaster in blasterControllers)
            {
                blaster.SetCanFire(true);
            }

            // If there are more blocks to go, switch environments before starting the next block
            if (currentEnvironmentBlock < totalEnvironmentBlocks - 1)
            {
                yield return new WaitForSeconds(2f);

                float fadeDuration = 1f;

                // Fade in a black canvas for smooth transition
                FadeCanvas.Instance.FadeIn(fadeDuration);
                yield return new WaitForSeconds(fadeDuration);

                // Switch environments (Snow -> Desert, or Desert -> Snow)
                string currentEnvironment = EnvironmentManager.Instance.GetCurrentEnvironment();
                string nextEnvironment = (currentEnvironment == "snow") ? "desert" : "snow";
                SetEnvironment(nextEnvironment);
                yield return new WaitForSeconds(1f);

                // Fade out the black canvas for smooth transition
                FadeCanvas.Instance.FadeOut(fadeDuration);
                yield return new WaitForSeconds(fadeDuration);
            }
        }

        // All blocks are completed, conclude the experiment
        ConcludeExperiment();
    }


    #endregion


    #region CONDITION MANAGEMENT


    // Sets the environment surroundings
    private void SetEnvironment(string environmentName)
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE 3: CONDITION MANAGEMENT
        // VERA provides a condition management system that allows you to define independent variables (IVs) 
        // for your experiment and assign values to those IVs for each participant.
        // For each IV you define on the VERA web interface, VERA will auto-generate a single static class.
        // This class is named VERAIV_[YourIVName] and contains static methods for setting and getting the value of that IV:
        //     * VERAIV_[YourIVName].SetValue([your IV value here]) - sets the value of the IV for the current participant.
        //     * VERAIV_[YourIVName].GetValue() - gets the current value of the IV for the participant.
        // The possible values you can set for each IV are also named according to what you set in the web interface.
        // For example, if you have an IV called "Environment" with a possible value of "Desert",
        // you can set the environment by using: VERAIV_Environment.SetValue(VERAIV_Environment.IVValue.V_Desert).

        // WITH VERA, we can manage our environment conditions using VERA's condition management system:
        //if (environmentName.Equals("desert"))
        //    VERAIV_Environment.SetSelectedValue(VERAIV_Environment.IVValue.V_Desert);
        //else if (environmentName.Equals("snow"))
        //    VERAIV_Environment.SetSelectedValue(VERAIV_Environment.IVValue.V_Snow);

        // WITHOUT VERA, we don't use a condition management system; no code is necessary.

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//

        EnvironmentManager.Instance.SetEnvironment(environmentName); // Sets the visuals of the environment
    }


    // Sets the firing mode for the participant (e.g., perfect aim vs. poor aim)
    private void SetUseBadAim(bool useBadAim)
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE 4: CONDITION MANAGEMENT (CONTINUED)
        // Similar to the environment, we can use VERA's condition management system for the aim type.

        // WITH VERA, we can manage our aim type conditions using VERA's condition management system:
        //if (useBadAim)
        //    VERAIV_AimType.SetSelectedValue(VERAIV_AimType.IVValue.V_BadAim);
        //else
        //    VERAIV_AimType.SetSelectedValue(VERAIV_AimType.IVValue.V_GoodAim);

        // WITHOUT VERA, we don't use a condition management system; no code is necessary.

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//

        // Set the aim mode on the blaster controllers based on the assigned condition
        foreach (var blaster in blasterControllers)
        {
            blaster.UseBadAimMode = useBadAim;
        }
    }


    #endregion


    #region SURVEYS


    // Displays a survey to the participant
    private void ShowSurvey(Action onSurveyComplete)
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE 5: SURVEYS
        // VERA provides a built-in survey system for displaying surveys and collecting results.
        // For each survey you define on the VERA web interface, VERA will add an entry to the VERASurveyHelper static class.
        // You can use this class to start surveys:
        //     * VERASurveyHelper.StartSurvey(VERASurveyHelper.VERASurveyReference.[your survey name here])
        // This will automatically display the survey and log all responses to the VERA portal.
        // You can also provide a variety of optional parameters to customize the experience; most notably:
        //     * onSurveyComplete: A callback which will be triggered once the participant completes the survey.

        // WITH VERA, we can display a survey using VERA's built-in survey system, and call onSurveyComplete on completion:
        //VERASurveyHelper.StartSurvey(VERASurveyHelper.VERASurveyReference.S_ConfidenceRatingQuestionnaire, onSurveyComplete: onSurveyComplete);

        // WITHOUT VERA, we'd have to make our own survey system; we'll skip the survey step and call onSurveyComplete:
        onSurveyComplete?.Invoke();

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//

    }


    #endregion


    #region DATA LOGGING


    // Logs data about a laser shot fired by the participant
    // Automatically called by BlasterController script when a shot is fired
    public void LogLaserShot(BlasterController.BlasterHandedness handedness, Vector3 origin, Vector3 direction, bool hitTarget)
    {
        // Do not log if we are not actively in a shooting round
        if (!inShootingRound)
            return;

        // Keep track of how many shots have been fired and hit in the current round
        shotsFiredInRound++;
        if (hitTarget)
            shotsHitInRound++;

        //----------------------------------------------------//
        // VERA SANDBOX NOTE 6: DATA LOGGING
        // VERA provides a data logging system that allows you to easily log entries to files for later analysis.
        // For each CSV file you define on the VERA web interface, VERA will auto-generate a static class.
        // This file will be named VERAFile_[YourFileName] and contains static methods for creating entries in that file:
        //     * VERAFile_[YourFileName].CreateCsvEntry([your parameters here]) - creates a new entry in the CSV file with the specified parameters.
        // The parameters you can include in each entry are strictly typed according to the values you set on the web interface.
        // For example, if you defined an integer column, the function would accept an integer parameter.

        // WITH VERA, we can log laser shot data using VERA's data logging system:
        //VERAFile_LaserLogs.CreateCsvEntry(currentEnvironmentBlock, currentRound, hitTarget);

        // WITHOUT VERA, we'd have to make our own data logging system; for now, simply log to the console.
        Debug.Log($"Laser shot logged: block={currentEnvironmentBlock}, round={currentRound}, hitTarget={hitTarget}");

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//

    }


    // Logs summary data about a completed round of pumpkin shooting
    public void LogRoundData()
    {
        float accuracy = shotsFiredInRound > 0 ? (float)shotsHitInRound / shotsFiredInRound : 0f;
        //----------------------------------------------------//
        // VERA SANDBOX NOTE 7: DATA LOGGING (CONTINUED)
        // Similar to logging laser shots, we can log summary data about the round using VERA's data logging.

        // WITH VERA, we can log round summary data using VERA's data logging system:
        //VERAFile_RoundSummaries.CreateCsvEntry(currentEnvironmentBlock, currentRound, shotsFiredInRound, shotsHitInRound, accuracy);

        // WITHOUT VERA, we'd have to make our own data logging system; for now, simply log to the console.
        Debug.Log($"Round data logged: block={currentEnvironmentBlock}, round={currentRound}, totalShots={shotsFiredInRound}, totalHits={shotsHitInRound}, accuracy={accuracy}");

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//
    }


    #endregion


    #region CONCLUSION


    // Concludes the experiment and finalizes the session
    private void ConcludeExperiment()
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE 8: SESSION FINALIZATION
        // VERA provides session management tools to help you manage the lifecycle of your experiment sessions.
        // Once the experiment is complete, you should call VERASessionManager.FinalizeSession() to properly end the session.
        // This will ensure the participant is marked as "COMPLETE", and all data is saved and uploaded to the VERA portal.

        // WITH VERA, we finalize the session using VERA's session management tools:
        //VERASessionManager.FinalizeSession();

        // WITHOUT VERA, we don't have session management; for now, simply log that the experiment is complete.
        Debug.Log("Experiment complete!");

        // Comment / uncomment the above lines depending on whether you are using VERA or not.
        // Before using any VERA features, make sure to import the VERA namespace at the top of this script: "using VERA;"
        //----------------------------------------------------//

    }


    #endregion

}
