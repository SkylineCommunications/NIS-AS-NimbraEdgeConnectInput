/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

27/02/2023	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

using Skyline.DataMiner.Automation;

/// <summary>
/// DataMiner Script Class.
/// </summary>
public class Script
{
	private const int InputNamesPid = 10002;
	private const int OutputNamesPid = 15002;
	private const int SetInputWritePid = 15059;

	/// <summary>
	/// The Script entry point.
	/// </summary>
	/// <param name="engine">Link with SLAutomation process.</param>
	public void Run(Engine engine)
	{
		var dummy = engine.GetDummy("dummy1");
		if (!dummy.IsActive)
		{
			engine.ExitFail($"{dummy.ElementName} element is not active!");
		}

		// Inputs
		var input = engine.GetScriptParam("Input").Value;
		var setInput = ValidateParam(engine, dummy, input, InputNamesPid);

		// Outputs
		var output = engine.GetScriptParam("Output").Value;
		var setOutput = ValidateParam(engine, dummy, output, OutputNamesPid);

		engine.GenerateInformation($"Connect input {setInput} to {setOutput} output");
		dummy.SetParameter(SetInputWritePid, setOutput, setInput);
	}

	/// <summary>
	/// The ValidateParam.
	/// </summary>
	/// <param name="engine">Link with SLAutomation process<see cref="Engine"/>.</param>
	/// <param name="dummy_element">Link to the Nimbra Edge element<see cref="ScriptDummy"/>.</param>
	/// <param name="paramValidation">Received name of the input or output<see cref="string"/>.</param>
	/// <param name="pid">Parameter ID of the column with the input or output names<see cref="int"/>.</param>
	/// <returns>The <see cref="string"/>The correct input or output name to be set</returns>
	private static string ValidateParam(Engine engine, ScriptDummy dummy_element, string paramValidation, int pid)
	{
		// Checking PIDs
		if (pid != InputNamesPid && pid != OutputNamesPid)
		{
			engine.ExitFail($"Invalid PID: {pid}. Please use either 10002 for inputs or 15002 for outputs.");
		}

		// Checking first characters
		var firstCharacters = "[\"";
		var setVal = (paramValidation.Substring(0, 2) == firstCharacters) ?
			paramValidation.Substring(2, paramValidation.Length - 4) :
			paramValidation;

		// Checking if it is a valid param in the table
		var tableValue = dummy_element.GetParameterDisplay(pid, setVal);
		if (tableValue != setVal)
		{
			var param = pid == InputNamesPid ? "Inputs" : "Outputs";
			engine.ExitFail($"{setVal} is not in the {param}' Table");
		}

		return setVal;
	}
}