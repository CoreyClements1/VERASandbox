#if VERAFile_EventTimestamps
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the EventTimestamps CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the EventTimestamps file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the EventTimestamps CSV log file.
	/// </summary>
	public static class VERAFile_EventTimestamps
	{
		
		private const string fileName = "EventTimestamps";
		
		/// <summary>
		/// Creates a new row entry in the EventTimestamps CSV log file.
		/// This CSV entry will automatically have the following fields populated:
		/// <list type="bullet">
		/// <item><description>pID (Participant ID)</description></item>
		/// <item><description>TS (Timestamp in milliseconds since application start)</description></item>
		/// <item><description>Conditions (Experimental conditions the participant was under during this log, in JSON format)</description></item>
		/// </list>
		/// This function has been set up according to your configuration and preferences for this file type on the VERA portal.
		/// Included in your configuration are the following additional columns:
		/// <list type="bullet">
		/// <item>DateTime: Value for the 'DateTime' column, of type string.</item>
		/// <item>EventName: Value for the 'EventName' column, of type string.</item>
		/// <item>InterpupillaryDistance: Value for the 'InterpupillaryDistance' column, of type float.</item>
		/// </list>
		/// </summary>
		/// <param name="DateTime">DateTime: Value for the 'DateTime' column, of type string.</param>
		/// <param name="EventName">EventName: Value for the 'EventName' column, of type string.</param>
		/// <param name="InterpupillaryDistance">InterpupillaryDistance: Value for the 'InterpupillaryDistance' column, of type float.</param>
		public static void CreateCsvEntry(string DateTime, string EventName, float InterpupillaryDistance)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, DateTime, EventName, InterpupillaryDistance);
		}
		
	}
}
#endif
