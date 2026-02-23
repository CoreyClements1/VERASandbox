using System;
using System.Collections;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{

    // ExperimentManager is responsible for managing the overall experiment flow, 
    // including participant state and condition management.

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
        // Here, we might want to differentiate firing mode based on the participant's ID -
        // For example, maybe we want half the participants to use perfect aim mode and half to use poor aim mode.
        // Using VERA, we can assign participants to these conditions in a balanced way, and use that assignment here to set the firing mode.
        // You can use the VERASessionManager to get the participant's ID -
        // For example, you could do a modulo operation on VERASessionManager.participantId to assign conditions in a balanced way.
        //----------------------------------------------------//
        int participantId = 0;
        bool useBadAim = participantId % 2 == 0; // Example condition assignment based on participant ID
        SetUseBadAim(useBadAim);

        // Display instructions and tutorial round
        InstructionsHandler.Instance.ShowInstructions(
            "Welcome to the experiment!\n\n" +
            "In this experiment, pumpkins will appear around you. Your task is to shoot as many pumpkins as possible in a limited amount of time. Each round will last for " + pumpkinRoundDuration + " seconds, and there will be " + roundsPerEnvironment + " rounds in each environment before switching to the next one.\n\n" +
            "To shoot a pumpkin, simply aim at it with your controller and pull the trigger. Try to shoot as many pumpkins as you can before time runs out!\n\n" +
            "To begin the first round, shoot the pumpkin that just appeared in front of you.",
            onInstructionsComplete: () =>
            {
                // Start the first round of pumpkin shooting after instructions are complete
                StartCoroutine(ExperimentFlowCoroutine());
            }
        );
    }

    #endregion

    #region EXPERIMENT FLOW

    // Manages the flow of the experiment, including timing rounds and switching environments.
    private IEnumerator ExperimentFlowCoroutine()
    {
        // Calculate total environment blocks (each environment experienced numRepetitionsOfEachEnvironment times)
        int totalEnvironmentBlocks = 2 * numRepetitionsOfEachEnvironment;

        // Loop through each environment block
        // A single environment block consists of roundsPerEnvironment rounds in the current environment
        for (int blockIndex = 0; blockIndex < totalEnvironmentBlocks; blockIndex++)
        {
            // Do roundsPerEnvironment rounds in current environment
            for (int round = 0; round < roundsPerEnvironment; round++)
            {
                // Wait 2 seconds before starting a round
                yield return new WaitForSeconds(2f);

                // Show instructions before each round
                bool roundStarted = false;
                InstructionsHandler.Instance.ShowInstructions(
                    $"Round {round + 1} of {roundsPerEnvironment}\n\n" +
                    "Shoot the pumpkin to begin!",
                    onInstructionsComplete: () => { roundStarted = true; }
                );

                // Wait for the instructions pumpkin to be shot
                yield return new WaitUntil(() => roundStarted);

                // Start the pumpkin shooting round
                bool roundEnded = false;
                PumpkinSpawner.Instance.StartPumpkinRound(pumpkinRoundDuration, () => { roundEnded = true; });

                // Wait for the round to end
                yield return new WaitUntil(() => roundEnded);
            }

            // The block is completed
            yield return new WaitForSeconds(2f);

            // Display a survey between blocks asking participants to rate their proficiency in the current environment
            bool surveyCompleted = false;
            ShowSurvey(onSurveyComplete: () =>
            {
                surveyCompleted = true;
            });

            // Wait for the survey to be completed
            yield return new WaitUntil(() => surveyCompleted);

            // If this is not the last block, switch to the next environment
            if (blockIndex < totalEnvironmentBlocks - 1)
            {
                // Wait a bit before switching environments
                yield return new WaitForSeconds(2f);

                float fadeDuration = 1f;

                // Fade in the black canvas
                FadeCanvas.Instance.FadeIn(fadeDuration);
                yield return new WaitForSeconds(fadeDuration);

                // Switch to the other environment
                EnvironmentManager.EnvironmentType currentEnvironment = EnvironmentManager.Instance.GetCurrentEnvironment();
                EnvironmentManager.EnvironmentType nextEnvironment = (currentEnvironment == EnvironmentManager.EnvironmentType.Ice)
                    ? EnvironmentManager.EnvironmentType.Desert
                    : EnvironmentManager.EnvironmentType.Ice;
                SetEnvironment(nextEnvironment);

                // Fade out the black canvas
                FadeCanvas.Instance.FadeOut(fadeDuration);
                yield return new WaitForSeconds(fadeDuration);
            }
        }

        // All blocks are completed, end the experiment
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
        // For example, VERAIV_EnvironmentType.SetValue.
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
        // For example, VERAIV_FiringMode.SetValue.
        //----------------------------------------------------//
        BlasterController.Instance.UseBadAimMode = useBadAim;
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
        //----------------------------------------------------//
        Debug.Log("Showing survey..."); // For now, simply log that we would show a survey here.
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
        // For example, VERASessionManager.FinalizeSession();
        //----------------------------------------------------//
        Debug.Log("Experiment complete!"); // For now, simply log that the experiment is complete.
    }

    #endregion

}
