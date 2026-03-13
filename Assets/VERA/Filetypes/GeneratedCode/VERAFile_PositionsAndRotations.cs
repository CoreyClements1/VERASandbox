#if VERAFile_PositionsAndRotations
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the PositionsAndRotations CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the PositionsAndRotations file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the PositionsAndRotations CSV log file.
	/// </summary>
	public static class VERAFile_PositionsAndRotations
	{
		
		private const string fileName = "PositionsAndRotations";
		
		/// <summary>
		/// Creates a new row entry in the PositionsAndRotations CSV log file.
		/// This CSV entry will automatically have the following fields populated:
		/// <list type="bullet">
		/// <item><description>pID (Participant ID)</description></item>
		/// <item><description>TS (Timestamp in milliseconds since application start)</description></item>
		/// <item><description>Conditions (Experimental conditions the participant was under during this log, in JSON format)</description></item>
		/// </list>
		/// This function has been set up according to your configuration and preferences for this file type on the VERA portal.
		/// Included in your configuration are the following additional columns:
		/// <list type="bullet">
		/// <item>headsetGlobalPosition: Value for the 'headsetGlobalPosition' column, of type float.</item>
		/// <item>headsetLocalPosition: Value for the 'headsetLocalPosition' column, of type float.</item>
		/// <item>headsetGlobalRotationQuaternion: Value for the 'headsetGlobalRotationQuaternion' column, of type string.</item>
		/// <item>headsetGlobalRotationEuler: Value for the 'headsetGlobalRotationEuler' column, of type string.</item>
		/// <item>headsetLocalRotationQuaternion: Value for the 'headsetLocalRotationQuaternion' column, of type string.</item>
		/// <item>headsetLocalRotationEuler: Value for the 'headsetLocalRotationEuler' column, of type string.</item>
		/// <item>leftControllerGlobalPosition: Value for the 'leftControllerGlobalPosition' column, of type float.</item>
		/// <item>leftControllerLocalPosition: Value for the 'leftControllerLocalPosition' column, of type float.</item>
		/// <item>leftControllerGlobalRotationQuaternion: Value for the 'leftControllerGlobalRotationQuaternion' column, of type string.</item>
		/// <item>leftControllerGlobalRotationEuler: Value for the 'leftControllerGlobalRotationEuler' column, of type string.</item>
		/// <item>leftControllerLocalRotationQuaternion: Value for the 'leftControllerLocalRotationQuaternion' column, of type string.</item>
		/// <item>leftControllerLocalRotationEuler: Value for the 'leftControllerLocalRotationEuler' column, of type string.</item>
		/// <item>rightControllerGlobalPosition: Value for the 'rightControllerGlobalPosition' column, of type float.</item>
		/// <item>rightControllerLocalPosition: Value for the 'rightControllerLocalPosition' column, of type float.</item>
		/// <item>rightControllerGlobalRotationQuaternion: Value for the 'rightControllerGlobalRotationQuaternion' column, of type string.</item>
		/// <item>rightControllerGlobalRotationEuler: Value for the 'rightControllerGlobalRotationEuler' column, of type string.</item>
		/// <item>rightControllerLocalRotationQuaternion: Value for the 'rightControllerLocalRotationQuaternion' column, of type string.</item>
		/// <item>rightControllerLocalRotationEuler: Value for the 'rightControllerLocalRotationEuler' column, of type string.</item>
		/// </list>
		/// </summary>
		/// <param name="headsetGlobalPosition">headsetGlobalPosition: Value for the 'headsetGlobalPosition' column, of type float.</param>
		/// <param name="headsetLocalPosition">headsetLocalPosition: Value for the 'headsetLocalPosition' column, of type float.</param>
		/// <param name="headsetGlobalRotationQuaternion">headsetGlobalRotationQuaternion: Value for the 'headsetGlobalRotationQuaternion' column, of type string.</param>
		/// <param name="headsetGlobalRotationEuler">headsetGlobalRotationEuler: Value for the 'headsetGlobalRotationEuler' column, of type string.</param>
		/// <param name="headsetLocalRotationQuaternion">headsetLocalRotationQuaternion: Value for the 'headsetLocalRotationQuaternion' column, of type string.</param>
		/// <param name="headsetLocalRotationEuler">headsetLocalRotationEuler: Value for the 'headsetLocalRotationEuler' column, of type string.</param>
		/// <param name="leftControllerGlobalPosition">leftControllerGlobalPosition: Value for the 'leftControllerGlobalPosition' column, of type float.</param>
		/// <param name="leftControllerLocalPosition">leftControllerLocalPosition: Value for the 'leftControllerLocalPosition' column, of type float.</param>
		/// <param name="leftControllerGlobalRotationQuaternion">leftControllerGlobalRotationQuaternion: Value for the 'leftControllerGlobalRotationQuaternion' column, of type string.</param>
		/// <param name="leftControllerGlobalRotationEuler">leftControllerGlobalRotationEuler: Value for the 'leftControllerGlobalRotationEuler' column, of type string.</param>
		/// <param name="leftControllerLocalRotationQuaternion">leftControllerLocalRotationQuaternion: Value for the 'leftControllerLocalRotationQuaternion' column, of type string.</param>
		/// <param name="leftControllerLocalRotationEuler">leftControllerLocalRotationEuler: Value for the 'leftControllerLocalRotationEuler' column, of type string.</param>
		/// <param name="rightControllerGlobalPosition">rightControllerGlobalPosition: Value for the 'rightControllerGlobalPosition' column, of type float.</param>
		/// <param name="rightControllerLocalPosition">rightControllerLocalPosition: Value for the 'rightControllerLocalPosition' column, of type float.</param>
		/// <param name="rightControllerGlobalRotationQuaternion">rightControllerGlobalRotationQuaternion: Value for the 'rightControllerGlobalRotationQuaternion' column, of type string.</param>
		/// <param name="rightControllerGlobalRotationEuler">rightControllerGlobalRotationEuler: Value for the 'rightControllerGlobalRotationEuler' column, of type string.</param>
		/// <param name="rightControllerLocalRotationQuaternion">rightControllerLocalRotationQuaternion: Value for the 'rightControllerLocalRotationQuaternion' column, of type string.</param>
		/// <param name="rightControllerLocalRotationEuler">rightControllerLocalRotationEuler: Value for the 'rightControllerLocalRotationEuler' column, of type string.</param>
		public static void CreateCsvEntry(string headsetGlobalPosition, string headsetLocalPosition, string headsetGlobalRotationQuaternion, string headsetGlobalRotationEuler, string headsetLocalRotationQuaternion, string headsetLocalRotationEuler, string leftControllerGlobalPosition, string leftControllerLocalPosition, string leftControllerGlobalRotationQuaternion, string leftControllerGlobalRotationEuler, string leftControllerLocalRotationQuaternion, string leftControllerLocalRotationEuler, string rightControllerGlobalPosition, string rightControllerLocalPosition, string rightControllerGlobalRotationQuaternion, string rightControllerGlobalRotationEuler, string rightControllerLocalRotationQuaternion, string rightControllerLocalRotationEuler)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, headsetGlobalPosition, headsetLocalPosition, headsetGlobalRotationQuaternion, headsetGlobalRotationEuler, headsetLocalRotationQuaternion, headsetLocalRotationEuler, leftControllerGlobalPosition, leftControllerLocalPosition, leftControllerGlobalRotationQuaternion, leftControllerGlobalRotationEuler, leftControllerLocalRotationQuaternion, leftControllerLocalRotationEuler, rightControllerGlobalPosition, rightControllerLocalPosition, rightControllerGlobalRotationQuaternion, rightControllerGlobalRotationEuler, rightControllerLocalRotationQuaternion, rightControllerLocalRotationEuler);
		}
		
	}
}
#endif
