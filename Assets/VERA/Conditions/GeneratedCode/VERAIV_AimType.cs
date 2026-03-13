#if VERAIV_AimType
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

namespace VERA
{
	/// <summary>
	/// Static class for accessing and modifying the AimType independent variable and its possible values.
	/// <br/><br/>This class has been generated based on the condition values defined for this variable in the VERA portal.
	/// This class should be the only way you access or modify the AimType independent variable.
	/// <br/><br/>Notably, use the GetSelectedValue() method to get the currently selected value of this independent variable, and the SetSelectedValue() method to change it.
	/// </summary>
	public static class VERAIV_AimType
	{

		/// <summary>
		/// Enum of possible values for the AimType independent variable.
		/// <br/><br/>This enum has been generated based on the condition values defined for this variable in the VERA portal.
		/// Each enum value is prefixed with V_ to avoid issues with names starting with numbers or other invalid enum names.
		/// <br/><br/>For this particular independent variable (AimType), the possible values are:<list type="bullet">
		/// <item><description>GoodAim</description></item>
		/// <item><description>BadAim</description></item>
		/// </list>
		/// </summary>
		public enum IVValue
		{
			V_GoodAim,
			V_BadAim,
		}

		/// <summary>
		/// Gets the currently selected condition value of the AimType independent variable.
		/// (Note - This method uses a cached value stored locally on the client. As such, it may be out of date if you have manually changed the condition value on the server externally.)
		/// <br/><br/>This method has been generated based on the condition values defined for this variable in the VERA portal.
		/// </summary>
		/// <returns>The current selected value of the AimType independent variable</returns>
		public static IVValue GetSelectedValue()
		{
			// Get the value - will be a string, needs to be converted to enum
			string selectedValue = VERASessionManager.GetSelectedIVValue("AimType");

			if (string.IsNullOrEmpty(selectedValue))
			{
				Debug.LogError("[VERA IVGroup_AimType] Error while getting selected IV value, got empty or null string as response");
				throw new InvalidOperationException("Unknown selected condition value");
			}

			// Direct conversion from string value to enum
			return selectedValue switch
			{
				"GoodAim" => IVValue.V_GoodAim,
				"BadAim" => IVValue.V_BadAim,
				_ => throw new InvalidOperationException($"Unknown selected condition value: {selectedValue}")
			};
		}

		/// <summary>
		/// Sets the currently selected condition value of the AimType independent variable.
		/// <br/><br/>This method has been generated based on the condition values defined for this variable in the VERA portal.
		/// </summary>
		/// <param name="value">The new selected value for the AimType independent variable</param>
		public static void SetSelectedValue(IVValue value)
		{
			// Convert enum to server value
			string valueStr = value switch
			{
				IVValue.V_GoodAim => "GoodAim",
				IVValue.V_BadAim => "BadAim",
				_ => throw new InvalidOperationException($"Unknown enum value: {value}")
			};

			VERASessionManager.SetSelectedIVValue("AimType", valueStr);
		}

	}
}
#endif
