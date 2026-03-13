#if VERAFile_RoundData
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the RoundData CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the RoundData file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the RoundData CSV log file.
	/// </summary>
	public static class VERAFile_RoundData
	{
		
		private const string fileName = "RoundData";
		
		/// <summary>
		/// Creates a new row entry in the RoundData CSV log file.
		/// This CSV entry will automatically have the following fields populated:
		/// <list type="bullet">
		/// <item><description>pID (Participant ID)</description></item>
		/// <item><description>TS (Timestamp in milliseconds since application start)</description></item>
		/// <item><description>Conditions (Experimental conditions the participant was under during this log, in JSON format)</description></item>
		/// </list>
		/// This function has been set up according to your configuration and preferences for this file type on the VERA portal.
		/// Included in your configuration are the following additional columns:
		/// <list type="bullet">
		/// <item>eventId: An identifier for this log entry, of type int. Mandatory for each user-generated file type, but may be arbitrarily assigned according to your preferences.</item>
		/// <item>block: Value for the 'block' column, of type int.</item>
		/// <item>round: Value for the 'round' column, of type int.</item>
		/// <item>totalShots: Value for the 'totalShots' column, of type int.</item>
		/// <item>pumpkinsHit: Value for the 'pumpkinsHit' column, of type int.</item>
		/// <item>accuracy: Value for the 'accuracy' column, of type string.</item>
		/// </list>
		/// </summary>
		/// <param name="eventId">eventId: An identifier for this log entry, of type int. Mandatory for each user-generated file type, but may be arbitrarily assigned according to your preferences.</param>
		/// <param name="block">block: Value for the 'block' column, of type int.</param>
		/// <param name="round">round: Value for the 'round' column, of type int.</param>
		/// <param name="totalShots">totalShots: Value for the 'totalShots' column, of type int.</param>
		/// <param name="pumpkinsHit">pumpkinsHit: Value for the 'pumpkinsHit' column, of type int.</param>
		/// <param name="accuracy">accuracy: Value for the 'accuracy' column, of type string.</param>
		public static void CreateCsvEntry(int eventId, int block, int round, int totalShots, int pumpkinsHit, string accuracy)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, eventId, block, round, totalShots, pumpkinsHit, accuracy	);
		}
		
	}
}
#endif
