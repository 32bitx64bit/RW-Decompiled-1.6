using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class KeyPrefsData
{
	public Dictionary<KeyBindingDef, KeyBindingData> keyPrefs = new Dictionary<KeyBindingDef, KeyBindingData>();

	public void ResetToDefaults()
	{
		keyPrefs.Clear();
		AddMissingDefaultBindings();
	}

	public void AddMissingDefaultBindings()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyBindingDef allDef in DefDatabase<KeyBindingDef>.AllDefs)
		{
			if (!keyPrefs.ContainsKey(allDef))
			{
				keyPrefs.Add(allDef, new KeyBindingData(allDef.defaultKeyCodeA, allDef.defaultKeyCodeB));
			}
		}
	}

	public bool SetBinding(KeyBindingDef keyDef, KeyPrefs.BindingSlot slot, KeyCode keyCode)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (keyPrefs.TryGetValue(keyDef, out var value))
		{
			switch (slot)
			{
			case KeyPrefs.BindingSlot.A:
				value.keyBindingA = keyCode;
				break;
			case KeyPrefs.BindingSlot.B:
				value.keyBindingB = keyCode;
				break;
			default:
				Log.Error("Tried to set a key binding for \"" + keyDef.LabelCap + "\" on a nonexistent slot: " + slot.ToString());
				return false;
			}
			return true;
		}
		Log.Error("Key not found in keyprefs: \"" + keyDef.LabelCap + "\"");
		return false;
	}

	public KeyCode GetBoundKeyCode(KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!keyPrefs.TryGetValue(keyDef, out var value))
		{
			Log.Error("Key not found in keyprefs: \"" + keyDef.LabelCap + "\"");
			return (KeyCode)0;
		}
		return (KeyCode)(slot switch
		{
			KeyPrefs.BindingSlot.A => value.keyBindingA, 
			KeyPrefs.BindingSlot.B => value.keyBindingB, 
			_ => throw new InvalidOperationException(), 
		});
	}

	private IEnumerable<KeyBindingDef> ConflictingBindings(KeyBindingDef keyDef, KeyCode code)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyBindingDef def in DefDatabase<KeyBindingDef>.AllDefs)
		{
			if (def != keyDef && (keyDef.ignoreConflictsWith == null || !keyDef.ignoreConflictsWith.Contains(def)) && ((def.category == keyDef.category && def.category.selfConflicting) || keyDef.category.checkForConflicts.Contains(def.category) || (keyDef.extraConflictTags != null && def.extraConflictTags != null && keyDef.extraConflictTags.Any((string tag) => def.extraConflictTags.Contains(tag)))) && keyPrefs.TryGetValue(def, out var value) && (value.keyBindingA == code || value.keyBindingB == code))
			{
				yield return def;
			}
		}
	}

	public void EraseConflictingBindingsForKeyCode(KeyBindingDef keyDef, KeyCode keyCode, Action<KeyBindingDef> callBackOnErase = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyBindingDef item in ConflictingBindings(keyDef, keyCode))
		{
			KeyBindingData keyBindingData = keyPrefs[item];
			if (keyBindingData.keyBindingA == keyCode)
			{
				keyBindingData.keyBindingA = (KeyCode)0;
			}
			if (keyBindingData.keyBindingB == keyCode)
			{
				keyBindingData.keyBindingB = (KeyCode)0;
			}
			callBackOnErase?.Invoke(item);
		}
	}

	public void CheckConflictsFor(KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		KeyCode boundKeyCode = GetBoundKeyCode(keyDef, slot);
		if ((int)boundKeyCode != 0)
		{
			EraseConflictingBindingsForKeyCode(keyDef, boundKeyCode);
			SetBinding(keyDef, slot, boundKeyCode);
		}
	}

	public KeyPrefsData Clone()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		KeyPrefsData keyPrefsData = new KeyPrefsData();
		foreach (KeyValuePair<KeyBindingDef, KeyBindingData> keyPref in keyPrefs)
		{
			keyPrefsData.keyPrefs[keyPref.Key] = new KeyBindingData(keyPref.Value.keyBindingA, keyPref.Value.keyBindingB);
		}
		return keyPrefsData;
	}

	public void ErrorCheck()
	{
		foreach (KeyBindingDef allDef in DefDatabase<KeyBindingDef>.AllDefs)
		{
			ErrorCheckOn(allDef, KeyPrefs.BindingSlot.A);
			ErrorCheckOn(allDef, KeyPrefs.BindingSlot.B);
		}
	}

	private void ErrorCheckOn(KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		KeyCode boundKeyCode = GetBoundKeyCode(keyDef, slot);
		if ((int)boundKeyCode == 0)
		{
			return;
		}
		foreach (KeyBindingDef item in ConflictingBindings(keyDef, boundKeyCode))
		{
			bool flag = boundKeyCode != keyDef.GetDefaultKeyCode(slot);
			Log.Warning("Key binding conflict: " + item?.ToString() + " and " + keyDef?.ToString() + " are both bound to " + ((object)(KeyCode)(ref boundKeyCode)).ToString() + "." + (flag ? " Fixed automatically." : ""));
			if (flag)
			{
				if (slot == KeyPrefs.BindingSlot.A)
				{
					keyPrefs[keyDef].keyBindingA = keyDef.defaultKeyCodeA;
				}
				else
				{
					keyPrefs[keyDef].keyBindingB = keyDef.defaultKeyCodeB;
				}
				KeyPrefs.Save();
			}
		}
	}
}
