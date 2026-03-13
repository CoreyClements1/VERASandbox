#if VERAFile_AcuityResult
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the AcuityResult CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the AcuityResult file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the AcuityResult CSV log file.
	/// </summary>
	public static class VERAFile_AcuityResult
	{
		
		private const string fileName = "AcuityResult";
		
		/// <summary>
		/// Creates a new row entry in the AcuityResult CSV log file.
		/// This CSV entry will automatically have the following fields populated:
		/// <list type="bullet">
		/// <item><description>pID (Participant ID)</description></item>
		/// <item><description>TS (Timestamp in milliseconds since application start)</description></item>
		/// <item><description>Conditions (Experimental conditions the participant was under during this log, in JSON format)</description></item>
		/// </list>
		/// This function has been set up according to your configuration and preferences for this file type on the VERA portal.
		/// Included in your configuration are the following additional columns:
		/// <list type="bullet">
		/// <item>Acuity: Value for the 'Acuity' column, of type string.</item>
		/// <item>ChartCharacter: Value for the 'ChartCharacter' column, of type string.</item>
		/// <item>GuessCharacter: Value for the 'GuessCharacter' column, of type string.</item>
		/// <item>MatchedCorrectly: Value for the 'MatchedCorrectly' column, of type bool.</item>
		/// </list>
		/// </summary>
		/// <param name="Acuity">Acuity: Value for the 'Acuity' column, of type string.</param>
		/// <param name="ChartCharacter">ChartCharacter: Value for the 'ChartCharacter' column, of type string.</param>
		/// <param name="GuessCharacter">GuessCharacter: Value for the 'GuessCharacter' column, of type string.</param>
		/// <param name="MatchedCorrectly">MatchedCorrectly: Value for the 'MatchedCorrectly' column, of type bool.</param>
		public static void CreateCsvEntry(string Acuity, string ChartCharacter, string GuessCharacter, bool MatchedCorrectly)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, Acuity, ChartCharacter, GuessCharacter, MatchedCorrectly);
		}
		
	}
}
#endif
