#if VERAFile_Experiment_Telemetry
using UnityEngine;
using System;

namespace VERA
{
	
	/// <summary>
	/// Static class for recording new entries to the Experiment_Telemetry CSV file.
	/// <br/><br/>This class has been generated based on the CSV file you have defined on the VERA portal.
	/// This class should be the only way you record new CSV entries to the Experiment_Telemetry file.
	/// <br/><br/>Notably, use the CreateCsvEntry() method to create new entries in the Experiment_Telemetry CSV log file.
	/// </summary>
	public static class VERAFile_Experiment_Telemetry
	{
		
		private const string fileName = "Experiment_Telemetry";
		
		/// <summary>
		/// Creates a new row entry in the Experiment_Telemetry CSV log file.
		/// This file is automatically populated and handled by VERA; researchers should NOT need to call this function directly.
		public static void CreateCsvEntry(bool headsetDetected, float headsetPosX, float headsetPosY, float headsetPosZ, string headsetRot, bool leftDetected, float leftControllerPosX, float leftControllerPosY, float leftControllerPosZ, string leftControllerRot, float leftTrigger, float leftGrip, int leftPrimaryButton, int leftSecondaryButton, int leftPrimary2DAxisClick, int leftThumbstickX, int leftThumbstickY, bool rightDetected, float rightControllerPosX, float rightControllerPosY, float rightControllerPosZ, string rightControllerRot, float rightTrigger, float rightGrip, int rightPrimaryButton, int rightSecondaryButton, int rightPrimary2DAxisClick, int rightThumbstickX, int rightThumbstickY)
		{
			VERASessionManager.CreateArbitraryCsvEntry(fileName, headsetDetected, headsetPosX, headsetPosY, headsetPosZ, headsetRot, leftDetected, leftControllerPosX, leftControllerPosY, leftControllerPosZ, leftControllerRot, leftTrigger, leftGrip, leftPrimaryButton, leftSecondaryButton, leftPrimary2DAxisClick, leftThumbstickX, leftThumbstickY, rightDetected, rightControllerPosX, rightControllerPosY, rightControllerPosZ, rightControllerRot, rightTrigger, rightGrip, rightPrimaryButton, rightSecondaryButton, rightPrimary2DAxisClick, rightThumbstickX, rightThumbstickY);
		}
		
	}
}
#endif
