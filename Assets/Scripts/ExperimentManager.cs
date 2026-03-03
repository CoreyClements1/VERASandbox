using System;
using System.Collections;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{

    // ExperimentManager is responsible for managing the overall experiment flow, 
    // including participant state and condition management.

    // Various VERA sandbox-related notes are included throughout as comments
    // to guide you on how to use VERA's features to implement the experiment.

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
    }


    // On start, initialize the experiment
    void Start()
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE:
        // Here, we might want to wait for VERA to fully initialize before starting the experiment setup.
        // You can use the VERASessionManager to check if the session is ready before proceeding - 
        //     EXAMPLE: VERASessionManager.initialized indicates whether VERA has finished initializing.
        //     EXAMPLE: VERASessionManager.onInitialized is a UnityEvent which is triggered once VERA finishes initializing.
        //              Subscribe to this event via code like this: VERASessionManager.onInitialized.AddListener(InitializeExperiment);
        //----------------------------------------------------//
        InitializeExperiment();
    }


    // Initializes the experiment by determining participant conditions and displaying instructions.
    private void InitializeExperiment()
    {
        SetEnvironment(EnvironmentManager.EnvironmentType.Ice); // Start in the ice environment

        //----------------------------------------------------//
        // VERA SANDBOX NOTE:
        // Here, we might want to differentiate firing mode based on the participant's ID -
        // For example, maybe we want half the participants to use perfect good aim mode and half to use bad aim mode.
        // Using VERA, we can assign participants to these conditions in a balanced way, and use that assignment here to set the firing mode.
        // You can use the VERASessionManager to get the participant's ID -
        // For example, you could do a modulo operation on VERASessionManager.participantId to assign conditions in a balanced way.
        //----------------------------------------------------//
        int participantId = 0; // Placeholder ID - replace with actual participant ID from VERA session management
        bool useBadAim = participantId % 2 == 0; // Example condition assignment based on participant ID (50/50 split)
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
        /*
         *  General experiment flow:
         *  Each round, a participant will shoot pumpkins for pumpkinRoundDuration seconds.
         *  After each round, there is a short break; once participants are ready, they can proceed to the next round.
         *  A "block" represents a full set of roundsPerEnvironment rounds in a single environment.
         *  After each block concludes, participants will complete a short survey about how well they think they performed.
         *  After the survey, the environment will switch, and the next block will begin in the new environment.
         *  After all blocks are completed, the experiment will conclude.
         */

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

            // Display a survey between blocks asking participants to rate their proficiency in the current environment
            yield return new WaitForSeconds(2f);
            bool surveyCompleted = false;
            ShowSurvey(onSurveyComplete: () => { surveyCompleted = true; });

            // Wait for the survey to be completed
            yield return new WaitUntil(() => surveyCompleted);

            // If there are more blocks to go, switch environments before starting the next block
            if (currentEnvironmentBlock < totalEnvironmentBlocks - 1)
            {
                yield return new WaitForSeconds(2f);

                float fadeDuration = 1f;

                // Fade in a black canvas for smooth transition
                FadeCanvas.Instance.FadeIn(fadeDuration);
                yield return new WaitForSeconds(fadeDuration);

                // Switch environments (Ice -> Desert, or Desert -> Ice)
                EnvironmentManager.EnvironmentType currentEnvironment = EnvironmentManager.Instance.GetCurrentEnvironment();
                EnvironmentManager.EnvironmentType nextEnvironment = (currentEnvironment == EnvironmentManager.EnvironmentType.Ice)
                    ? EnvironmentManager.EnvironmentType.Desert
                    : EnvironmentManager.EnvironmentType.Ice;
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
    private void SetEnvironment(EnvironmentManager.EnvironmentType environmentType)
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE:
        // Here, we might want to use VERA's conditions management to sync experimental conditions across the entire experiment.
        // This way, all CSV data logged will be associated with the participant's current assigned condition,
        // and there is no ambiguity across different scripts about which condition the participant is in.
        // Use the auto-generated VERAIV class for your experiment to set and get conditions in a centralized way.
        //     EXAMPLE: VERAIV_Environment.SetValue([your environment condition value here]);
        //----------------------------------------------------//
        EnvironmentManager.Instance.SetEnvironment(environmentType);
    }


    // Sets the firing mode for the participant (e.g., perfect aim vs. poor aim)
    private void SetUseBadAim(bool useBadAim)
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE:
        // Similar to the environment, we can also manage firing mode conditions using VERA's conditions management system.
        // Use the auto-generated VERAIV class for your experiment to set and get conditions in a centralized way.
        //     EXAMPLE: VERAIV_FiringMode.SetValue([your firing mode condition value here]);
        //----------------------------------------------------//
        BlasterController[] blasters = FindObjectsByType<BlasterController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var blaster in blasters)
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
        // VERA SANDBOX NOTE:
        // To show surveys, you can use VERA's built-in survey system.
        // Once you've defined a survey in the VERA dashboard for your experiment,
        // use the VERASurveyHelper.StartSurvey function to display the survey to the participant.
        // The survey will automatically display in front of the participant and log all responses.
        //    EXAMPLE: VERASurveyHelper.StartSurvey(VERASurveyHelper.VERASurveyReference.[your survey name here], onSurveyComplete);
        //----------------------------------------------------//
        Debug.Log("Survey should be shown here - skipping for now."); // For now, simply log that we would show a survey here.
        onSurveyComplete?.Invoke(); // Invoke the callback immediately for now since we don't have an actual survey implemented.
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

        //----------------------------------------------------//
        // VERA SANDBOX NOTE:
        // For data logging, you can use VERA's built-in data logging system to log any relevant data points you want to track.
        // This will automatically associate the logged data with the participant's ID and current conditions, and save it in a CSV file for later analysis.
        // For every file type you have defined on the VERA web interface, VERA will auto-generate a single static class for logging data.
        // For example, if you have defined a file type called "LaserShots", VERA will generate a class called VERAFile_LaserShots with static methods for logging data to that file.
        //     EXAMPLE: If we have a file type called "LaserShots" with columns for block, round, handedness, origin, direction, and hitTarget, we could log an entry like this:
        //              VERAFile_LaserShots.CreateCsvEntry(0, currentEnvironmentBlock + 1, currentRound + 1, handedness.ToString(), origin, direction, hitTarget);
        //----------------------------------------------------//
        Debug.Log($"Laser shot logged: Block={currentEnvironmentBlock + 1}, Round={currentRound + 1}, Handedness={handedness}, Origin={origin}, Direction={direction}, HitTarget={hitTarget}");

        // Keep track of how many shots have been fired and hit in the current round
        shotsFiredInRound++;
        if (hitTarget)
            shotsHitInRound++;
    }


    // Logs summary data about a completed round of pumpkin shooting
    public void LogRoundData()
    {
        float accuracy = shotsFiredInRound > 0 ? (float)shotsHitInRound / shotsFiredInRound : 0f;
        //----------------------------------------------------//
        // VERA SANDBOX NOTE:
        // Similar to logging individual laser shots, here we can log summary data about each round using VERA's data logging system.
        //     EXAMPLE: If we have a file type called "RoundData" with columns for block, round, totalShots, pumpkinsHit, and accuracy, we could log an entry like this:
        //              VERAFile_RoundData.CreateCsvEntry(0, currentEnvironmentBlock + 1, currentRound + 1, shotsFiredInRound, shotsHitInRound, accuracy);
        //----------------------------------------------------//
        Debug.Log($"Round data logged: Block={currentEnvironmentBlock + 1}, Round={currentRound + 1}, TotalShots={shotsFiredInRound}, PumpkinsHit={shotsHitInRound}, Accuracy={accuracy:P2}");
    }


    #endregion


    #region CONCLUSION


    // Concludes the experiment and finalizes the session
    private void ConcludeExperiment()
    {
        //----------------------------------------------------//
        // VERA SANDBOX NOTE:
        // Here, you can use VERA's session management system to finalize the experiment.
        // This will automatically perform any necessary cleanup, mark the participant as complete,
        // and kick them out of the experiment safely. All data will be saved and associated with the participant's ID.
        //     EXAMPLE: VERASessionManager.FinalizeSession();
        //----------------------------------------------------//
        Debug.Log("Experiment complete!"); // For now, simply log that the experiment is complete.
    }


    #endregion

}
