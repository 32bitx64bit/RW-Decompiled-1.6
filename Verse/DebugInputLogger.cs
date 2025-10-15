using UnityEngine;

namespace Verse;

public static class DebugInputLogger
{
	public static void InputLogOnGUI()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		if (DebugViewSettings.logInput && ((int)Event.current.type == 0 || (int)Event.current.type == 1 || (int)Event.current.type == 4 || (int)Event.current.type == 5 || (int)Event.current.type == 6))
		{
			Log.Message("Frame " + Time.frameCount + ": " + Event.current.ToStringFull());
		}
	}

	public static string ToStringFull(this Event ev)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		string[] array = new string[41];
		array[0] = "(EVENT\ntype=";
		EventType val = ev.type;
		array[1] = ((object)(EventType)(ref val)).ToString();
		array[2] = "\nbutton=";
		array[3] = ev.button.ToString();
		array[4] = "\nkeyCode=";
		KeyCode keyCode = ev.keyCode;
		array[5] = ((object)(KeyCode)(ref keyCode)).ToString();
		array[6] = "\ndelta=";
		Vector2 val2 = ev.delta;
		array[7] = ((object)(Vector2)(ref val2)).ToString();
		array[8] = "\nalt=";
		array[9] = ev.alt.ToString();
		array[10] = "\ncapsLock=";
		array[11] = ev.capsLock.ToString();
		array[12] = "\ncharacter=";
		array[13] = ((ev.character != 0) ? ev.character : ' ').ToString();
		array[14] = "\nclickCount=";
		array[15] = ev.clickCount.ToString();
		array[16] = "\ncommand=";
		array[17] = ev.command.ToString();
		array[18] = "\ncommandName=";
		array[19] = ev.commandName;
		array[20] = "\ncontrol=";
		array[21] = ev.control.ToString();
		array[22] = "\nfunctionKey=";
		array[23] = ev.functionKey.ToString();
		array[24] = "\nisKey=";
		array[25] = ev.isKey.ToString();
		array[26] = "\nisMouse=";
		array[27] = ev.isMouse.ToString();
		array[28] = "\nmodifiers=";
		EventModifiers modifiers = ev.modifiers;
		array[29] = ((object)(EventModifiers)(ref modifiers)).ToString();
		array[30] = "\nmousePosition=";
		val2 = ev.mousePosition;
		array[31] = ((object)(Vector2)(ref val2)).ToString();
		array[32] = "\nnumeric=";
		array[33] = ev.numeric.ToString();
		array[34] = "\npressure=";
		array[35] = ev.pressure.ToString();
		array[36] = "\nrawType=";
		val = ev.rawType;
		array[37] = ((object)(EventType)(ref val)).ToString();
		array[38] = "\nshift=";
		array[39] = ev.shift.ToString();
		array[40] = "\n)";
		return string.Concat(array);
	}
}
