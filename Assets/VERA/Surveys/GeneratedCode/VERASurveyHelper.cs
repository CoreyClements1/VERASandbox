using UnityEngine;
using System;
using System.Collections.Generic;

namespace VERA
{
	/// <summary>
	/// Static helper class for starting surveys in your VERA experiment.
	/// <br/><br/>This class has been automatically generated based on your surveys defined in the VERA portal for your currently selected experiment.
	/// Use the StartSurvey() method to begin a survey for the current participant.
	/// </summary>
	public static class VERASurveyHelper
	{

		/// <summary>
		/// Enum of available surveys in this experiment.
		/// <br/><br/>This enum has been generated based on your surveys defined in the VERA portal for your currently selected experiment.
		/// Each enum value is prefixed with S_ to avoid issues with names starting with numbers or other invalid enum names.
		/// </summary>
		public enum VERASurveyReference
		{
			/// <summary>Confidence Rating Questionnaire</summary>
			S_ConfidenceRatingQuestionnaire,
		}

		/// <summary>
		/// Mapping of survey IDs to VERASurveyReference enum values for easy lookup when only the survey ID is known (e.g. in trials)
		/// </summary>
		private static Dictionary<string, VERASurveyReference> surveyIdToReferenceMap = new Dictionary<string, VERASurveyReference>
		{
			{ "69b1a92be2dfe7bd23c62b16", VERASurveyReference.S_ConfidenceRatingQuestionnaire },
		};

		/// <summary>
		/// Gets the VERASurveyReference enum value corresponding to the given survey ID.
		/// <br/><br/>This method can be useful when utilizing trials or other features which reference surveys by their ID.
		/// </summary>
		/// <param name="surveyId">The ID of the survey.</param>
		/// <returns>The corresponding VERASurveyReference enum value, or null if not found.</returns>
		public static VERASurveyReference? GetSurveyReferenceById(string surveyId)
		{
			if (surveyIdToReferenceMap.TryGetValue(surveyId, out var reference))
			{
				return reference;
			}

			Debug.LogWarning($"[VERASurveyHelper] No survey reference found for survey ID: {surveyId}");
			return null;
		}

		/// <summary>
		/// Starts the specified survey for the current participant.
		/// </summary>
		/// <param name="surveyToStart">The survey to start, specified using the VERASurveyReference enum</param>
		/// <param name="transportToLobby">Whether to temporarily transport the participant to a survey lobby while the survey is active. Default is true.</param>
		/// <param name="dimEnvironment">Whether to dim the environment when transporting to the survey lobby. Default is true.</param>
		/// <param name="heightOffset">How far the survey will be offset vertically from the user's head position. Default is 0.</param>
		/// <param name="distanceOffset">How far the survey will be offset horizontally from the user's head position. Default is 3.</param>
		/// <param name="onSurveyComplete">An optional callback Action that will be invoked when the survey is completed by the participant.</param>
		public static void StartSurvey(VERASurveyReference surveyToStart, bool transportToLobby = true, bool dimEnvironment = true, float heightOffset = 0f, float distanceOffset = 3f, Action onSurveyComplete = null)
		{
			// Get the resource path for the selected survey
			string resourcePath = surveyToStart switch
			{
				VERASurveyReference.S_ConfidenceRatingQuestionnaire => "GeneratedSurveyInfos/Confidence Rating Questionnaire",
				_ => throw new ArgumentException($"Unknown survey reference: {surveyToStart}")
			};

			// Load the survey info from Resources
			VERASurveyInfo surveyInfo = Resources.Load<VERASurveyInfo>(resourcePath);

			if (surveyInfo == null)
			{
				Debug.LogError($"[VERASurveyHelper] Failed to load survey info at path: {resourcePath}");
				return;
			}

			// Start the survey using VERASessionManager
			VERASessionManager.StartSurvey(surveyInfo, transportToLobby, dimEnvironment, heightOffset, distanceOffset, onSurveyComplete);
		}
	}
}
