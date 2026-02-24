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


    [Tooltip("How long each round of pumpkin shooting will last in seconds.")]
    [SerializeField] private float pumpkinRoundDuration = 20f;
    [Tooltip("How many pumpkin shooting rounds will occur in each environment before switching to the next one.")]
    [SerializeField] private int roundsPerEnvironment = 3;
    [Tooltip("How many times a single participant will experience each environment before the experiment ends.")]
    [SerializeField] private int numRepetitionsOfEachEnvironment = 1;


    #endregion


    #region SETUP


    void Start()
    {
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
        for (int blockIndex = 0; blockIndex < totalEnvironmentBlocks; blockIndex++)
        {
            // Do roundsPerEnvironment rounds in current environment
            for (int round = 0; round < roundsPerEnvironment; round++)
            {
                yield return new WaitForSeconds(2f);

                // Show instructions before each round
                bool roundStarted = false;
                InstructionsHandler.Instance.ShowInstructions(
                    $"Environment {blockIndex + 1} of {totalEnvironmentBlocks}\n" +
                    $"Round {round + 1} of {roundsPerEnvironment}\n\n" +
                    "Shoot the pumpkin to begin!",
                    onInstructionsComplete: () => { roundStarted = true; }
                );

                // Wait for the participant to start the round
                yield return new WaitUntil(() => roundStarted);

                // Start the pumpkin shooting round
                bool roundEnded = false;
                PumpkinSpawner.Instance.StartPumpkinRound(pumpkinRoundDuration, () => { roundEnded = true; });

                // Wait for the round to end
                yield return new WaitUntil(() => roundEnded);
            }

            // The block is completed - roundsPerEnvironment rounds have been completed in the current environment.

            // Display a survey between blocks asking participants to rate their proficiency in the current environment
            yield return new WaitForSeconds(2f);
            bool surveyCompleted = false;
            ShowSurvey(onSurveyComplete: () => { surveyCompleted = true; });

            // Wait for the survey to be completed
            yield return new WaitUntil(() => surveyCompleted);

            // If there are more blocks to go, switch environments before starting the next block
            if (blockIndex < totalEnvironmentBlocks - 1)
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
