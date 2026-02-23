using System;
using TMPro;
using UnityEngine;

public class InstructionsHandler : MonoBehaviour
{

    // InstructionsHandler is responsible for displaying instructions to the participant at the start of the experiment and before each environment switch.

    #region VARIABLES

    public static InstructionsHandler Instance { get; private set; }

    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] private TMP_Text instructionsText;
    [SerializeField] private ExplodingPumpkin pumpkinPrefab;
    [SerializeField] private Transform instructionsPumpkinSpawnPoint;
    private ExplodingPumpkin instructionsPumpkinInstance;

    #endregion

    #region SETUP

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        HideInstructionsImmediate();
    }

    #endregion

    #region INSTRUCTIONS DISPLAY

    // Shows the given instructions text on the canvas and fades it in
    public void ShowInstructions(string instructions, Action onInstructionsComplete)
    {
        if (instructionsText != null)
        {
            instructionsText.text = instructions;
        }

        if (mainCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(mainCanvasGroup, mainCanvasGroup.alpha, 1f, 0.5f));
        }

        // Spawn the pumpkin in front of the participant as a visual cue to shoot it to start
        if (pumpkinPrefab != null && instructionsPumpkinSpawnPoint != null)
        {
            if (instructionsPumpkinInstance != null)
            {
                Destroy(instructionsPumpkinInstance.gameObject);
            }
            instructionsPumpkinInstance = Instantiate(pumpkinPrefab, instructionsPumpkinSpawnPoint.position, Quaternion.identity);
            instructionsPumpkinInstance.onExplode.AddListener(() =>
            {
                onInstructionsComplete?.Invoke();
                HideInstructions();
            });
        }
    }

    // Hides the instructions canvas with fade out
    public void HideInstructions()
    {
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.interactable = false;
            mainCanvasGroup.blocksRaycasts = false;
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(mainCanvasGroup, mainCanvasGroup.alpha, 0f, 0.5f));
        }
    }

    // Coroutine to fade CanvasGroup alpha
    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        cg.alpha = startAlpha;
        cg.interactable = endAlpha > 0.9f;
        cg.blocksRaycasts = endAlpha > 0.9f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = endAlpha;
        cg.interactable = endAlpha > 0.9f;
        cg.blocksRaycasts = endAlpha > 0.9f;
    }

    // Hides the instructions canvas immediately without fade
    public void HideInstructionsImmediate()
    {
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.alpha = 0f;
            mainCanvasGroup.interactable = false;
            mainCanvasGroup.blocksRaycasts = false;
        }
    }

    #endregion

}
