#if VERAFile_RoundSummary
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the RoundSummary CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the RoundSummary file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the RoundSummary CSV log file.
	/// </summary>
	public static class VERAFile_RoundSummary
	{
		
		private const string fileName = "RoundSummary";
		
		/// <summary>
		/// Creates a new row entry in the RoundSummary CSV log file.
		/// This CSV entry will automatically have the following fields populated:
		/// <list type="bullet">
		/// <item><description>pID (Participant ID)</description></item>
		/// <item><description>TS (Timestamp in milliseconds since application start)</description></item>
		/// <item><description>Conditions (Experimental conditions the participant was under during this log, in JSON format)</description></item>
		/// </list>
		/// This function has been set up according to your configuration and preferences for this file type on the VERA portal.
		/// Included in your configuration are the following additional columns:
		/// <list type="bullet">
		/// <item>Round: Value for the 'Round' column, of type int.</item>
		/// <item>Accuracy: Value for the 'Accuracy' column, of type float.</item>
		/// </list>
		/// </summary>
		/// <param name="Round">Round: Value for the 'Round' column, of type int.</param>
		/// <param name="Accuracy">Accuracy: Value for the 'Accuracy' column, of type float.</param>
		public static void CreateCsvEntry(int Round, float Accuracy)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, Round, Accuracy);
		}
		
	}
}
#endif
