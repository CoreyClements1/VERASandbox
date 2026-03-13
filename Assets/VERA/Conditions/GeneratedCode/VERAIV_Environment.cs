#if VERAIV_Environment
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

namespace VERA
{
	/// <summary>
	/// Static class for accessing and modifying the Environment independent variable and its possible values.
	/// <br/><br/>This class has been generated based on the condition values defined for this variable in the VERA portal.
	/// This class should be the only way you access or modify the Environment independent variable.
	/// <br/><br/>Notably, use the GetSelectedValue() method to get the currently selected value of this independent variable, and the SetSelectedValue() method to change it.
	/// </summary>
	public static class VERAIV_Environment
	{

		/// <summary>
		/// Enum of possible values for the Environment independent variable.
		/// <br/><br/>This enum has been generated based on the condition values defined for this variable in the VERA portal.
		/// Each enum value is prefixed with V_ to avoid issues with names starting with numbers or other invalid enum names.
		/// <br/><br/>For this particular independent variable (Environment), the possible values are:<list type="bullet">
		/// <item><description>Desert</description></item>
		/// <item><description>Snow</description></item>
		/// </list>
		/// </summary>
		public enum IVValue
		{
			V_Desert,
			V_Snow,
		}

		/// <summary>
		/// Gets the currently selected condition value of the Environment independent variable.
		/// (Note - This method uses a cached value stored locally on the client. As such, it may be out of date if you have manually changed the condition value on the server externally.)
		/// <br/><br/>This method has been generated based on the condition values defined for this variable in the VERA portal.
		/// </summary>
		/// <returns>The current selected value of the Environment independent variable</returns>
		public static IVValue GetSelectedValue()
		{
			// Get the value - will be a string, needs to be converted to enum
			string selectedValue = VERASessionManager.GetSelectedIVValue("Environment");

			if (string.IsNullOrEmpty(selectedValue))
			{
				Debug.LogError("[VERA IVGroup_Environment] Error while getting selected IV value, got empty or null string as response");
				throw new InvalidOperationException("Unknown selected condition value");
			}

			// Direct conversion from string value to enum
			return selectedValue switch
			{
				"Desert" => IVValue.V_Desert,
				"Snow" => IVValue.V_Snow,
				_ => throw new InvalidOperationException($"Unknown selected condition value: {selectedValue}")
			};
		}

		/// <summary>
		/// Sets the currently selected condition value of the Environment independent variable.
		/// <br/><br/>This method has been generated based on the condition values defined for this variable in the VERA portal.
		/// </summary>
		/// <param name="value">The new selected value for the Environment independent variable</param>
		public static void SetSelectedValue(IVValue value)
		{
			// Convert enum to server value
			string valueStr = value switch
			{
				IVValue.V_Desert => "Desert",
				IVValue.V_Snow => "Snow",
				_ => throw new InvalidOperationException($"Unknown enum value: {value}")
			};

			VERASessionManager.SetSelectedIVValue("Environment", valueStr);
		}

	}
}
#endif
