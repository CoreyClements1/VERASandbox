#if VERAFile_RoundSummaries
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the RoundSummaries CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the RoundSummaries file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the RoundSummaries CSV log file.
	/// </summary>
	public static class VERAFile_RoundSummaries
	{
		
		private const string fileName = "RoundSummaries";
		
		/// <summary>
		/// Creates a new row entry in the RoundSummaries CSV log file.
		/// This CSV entry will automatically have the following fields populated:
		/// <list type="bullet">
		/// <item><description>pID (Participant ID)</description></item>
		/// <item><description>TS (Timestamp in milliseconds since application start)</description></item>
		/// <item><description>Conditions (Experimental conditions the participant was under during this log, in JSON format)</description></item>
		/// </list>
		/// This function has been set up according to your configuration and preferences for this file type on the VERA portal.
		/// Included in your configuration are the following additional columns:
		/// <list type="bullet">
		/// <item>block: Value for the 'block' column, of type int.</item>
		/// <item>round: Value for the 'round' column, of type int.</item>
		/// <item>totalShotsFired: Value for the 'totalShotsFired' column, of type int.</item>
		/// <item>totalShotsHit: Value for the 'totalShotsHit' column, of type int.</item>
		/// <item>accuracy: Value for the 'accuracy' column, of type float.</item>
		/// </list>
		/// </summary>
		/// <param name="block">block: Value for the 'block' column, of type int.</param>
		/// <param name="round">round: Value for the 'round' column, of type int.</param>
		/// <param name="totalShotsFired">totalShotsFired: Value for the 'totalShotsFired' column, of type int.</param>
		/// <param name="totalShotsHit">totalShotsHit: Value for the 'totalShotsHit' column, of type int.</param>
		/// <param name="accuracy">accuracy: Value for the 'accuracy' column, of type float.</param>
		public static void CreateCsvEntry(int block, int round, int totalShotsFired, int totalShotsHit, float accuracy)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, block, round, totalShotsFired, totalShotsHit, accuracy);
		}
		
	}
}
#endif
