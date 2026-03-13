#if VERAFile_LaserLogs
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the LaserLogs CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the LaserLogs file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the LaserLogs CSV log file.
	/// </summary>
	public static class VERAFile_LaserLogs
	{
		
		private const string fileName = "LaserLogs";
		
		/// <summary>
		/// Creates a new row entry in the LaserLogs CSV log file.
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
		/// <item>hitTarget: Value for the 'hitTarget' column, of type bool.</item>
		/// </list>
		/// </summary>
		/// <param name="block">block: Value for the 'block' column, of type int.</param>
		/// <param name="round">round: Value for the 'round' column, of type int.</param>
		/// <param name="hitTarget">hitTarget: Value for the 'hitTarget' column, of type bool.</param>
		public static void CreateCsvEntry(int block, int round, bool hitTarget)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, block, round, hitTarget);
		}
		
	}
}
#endif
