using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class KeyBindingDef : Def
{
	public KeyBindingCategoryDef category;

	public KeyCode defaultKeyCodeA;

	public KeyCode defaultKeyCodeB;

	public bool devModeOnly;

	public List<KeyBindingDef> ignoreConflictsWith;

	[NoTranslate]
	public List<string> extraConflictTags;

	public KeyCode MainKey
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if ((int)value.keyBindingA != 0)
				{
					return value.keyBindingA;
				}
				if ((int)value.keyBindingB != 0)
				{
					return value.keyBindingB;
				}
			}
			return (KeyCode)0;
		}
	}

	public string MainKeyLabel => MainKey.ToStringReadable();

	public bool KeyDownEvent
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Invalid comparison between Unknown and I4
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Invalid comparison between Unknown and I4
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Invalid comparison between Unknown and I4
			if ((int)Event.current.type == 4 && (int)Event.current.keyCode != 0 && KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if ((int)value.keyBindingA != 310 && (int)value.keyBindingA != 309 && (int)value.keyBindingB != 310 && (int)value.keyBindingB != 309 && Event.current.command)
				{
					return false;
				}
				if (Find.WindowStack.AnySearchWidgetFocused)
				{
					return false;
				}
				if (Event.current.keyCode != value.keyBindingA)
				{
					return Event.current.keyCode == value.keyBindingB;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsDownEvent
	{
		get
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Invalid comparison between Unknown and I4
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Invalid comparison between Unknown and I4
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Invalid comparison between Unknown and I4
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Invalid comparison between Unknown and I4
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Invalid comparison between Unknown and I4
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Invalid comparison between Unknown and I4
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Invalid comparison between Unknown and I4
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Invalid comparison between Unknown and I4
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Invalid comparison between Unknown and I4
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Invalid comparison between Unknown and I4
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Invalid comparison between Unknown and I4
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Invalid comparison between Unknown and I4
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Invalid comparison between Unknown and I4
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Invalid comparison between Unknown and I4
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Invalid comparison between Unknown and I4
			if (Event.current == null)
			{
				return false;
			}
			if (!KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				return false;
			}
			if (Find.WindowStack.AnySearchWidgetFocused)
			{
				return false;
			}
			if (KeyDownEvent)
			{
				return true;
			}
			if (Event.current.shift && ((int)value.keyBindingA == 304 || (int)value.keyBindingA == 303 || (int)value.keyBindingB == 304 || (int)value.keyBindingB == 303))
			{
				return true;
			}
			if (Event.current.control && ((int)value.keyBindingA == 306 || (int)value.keyBindingA == 305 || (int)value.keyBindingB == 306 || (int)value.keyBindingB == 305))
			{
				return true;
			}
			if (Event.current.alt && ((int)value.keyBindingA == 308 || (int)value.keyBindingA == 307 || (int)value.keyBindingB == 308 || (int)value.keyBindingB == 307))
			{
				return true;
			}
			if (Event.current.command && ((int)value.keyBindingA == 310 || (int)value.keyBindingA == 309 || (int)value.keyBindingB == 310 || (int)value.keyBindingB == 309))
			{
				return true;
			}
			return IsDown;
		}
	}

	public bool JustPressed
	{
		get
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if (Find.WindowStack.AnySearchWidgetFocused)
				{
					return false;
				}
				if (!Input.GetKeyDown(value.keyBindingA))
				{
					return Input.GetKeyDown(value.keyBindingB);
				}
				return true;
			}
			return false;
		}
	}

	public bool IsDown
	{
		get
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if (Find.WindowStack.AnySearchWidgetFocused)
				{
					return false;
				}
				if (!Input.GetKey(value.keyBindingA))
				{
					return Input.GetKey(value.keyBindingB);
				}
				return true;
			}
			return false;
		}
	}

	public KeyCode GetDefaultKeyCode(KeyPrefs.BindingSlot slot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return (KeyCode)(slot switch
		{
			KeyPrefs.BindingSlot.A => defaultKeyCodeA, 
			KeyPrefs.BindingSlot.B => defaultKeyCodeB, 
			_ => throw new InvalidOperationException(), 
		});
	}

	public static KeyBindingDef Named(string name)
	{
		return DefDatabase<KeyBindingDef>.GetNamedSilentFail(name);
	}
}
