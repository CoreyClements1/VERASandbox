#if VERAFile_LaserShots
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the LaserShots CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the LaserShots file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the LaserShots CSV log file.
	/// </summary>
	public static class VERAFile_LaserShots
	{
		
		private const string fileName = "LaserShots";
		
		/// <summary>
		/// Creates a new row entry in the LaserShots CSV log file.
		/// This CSV entry will automatically have the following fields populated:
		/// <list type="bullet">
		/// <item><description>pID (Participant ID)</description></item>
		/// <item><description>TS (Timestamp in milliseconds since application start)</description></item>
		/// <item><description>Conditions (Experimental conditions the participant was under during this log, in JSON format)</description></item>
		/// </list>
		/// This function has been set up according to your configuration and preferences for this file type on the VERA portal.
		/// Included in your configuration are the following additional columns:
		/// <list type="bullet">
		/// <item>LaserOrigin: Value for the 'LaserOrigin' column, of type string.</item>
		/// <item>HitOrMiss: Value for the 'HitOrMiss' column, of type bool.</item>
		/// </list>
		/// </summary>
		/// <param name="LaserOrigin">LaserOrigin: Value for the 'LaserOrigin' column, of type string.</param>
		/// <param name="HitOrMiss">HitOrMiss: Value for the 'HitOrMiss' column, of type bool.</param>
		public static void CreateCsvEntry(string LaserOrigin, bool HitOrMiss)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, LaserOrigin, HitOrMiss);
		}
		
	}
}
#endif
