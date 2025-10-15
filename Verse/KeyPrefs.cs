using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Verse;

public class KeyPrefs
{
	public enum BindingSlot : byte
	{
		A,
		B
	}

	private static KeyPrefsData data;

	private static Dictionary<string, KeyBindingData> unresolvedBindings;

	public static KeyPrefsData KeyPrefsData
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}

	public static void Init()
	{
		bool flag = !new FileInfo(GenFilePaths.KeyPrefsFilePath).Exists;
		Dictionary<string, KeyBindingData> dictionary = DirectXmlLoader.ItemFromXmlFile<Dictionary<string, KeyBindingData>>(GenFilePaths.KeyPrefsFilePath);
		data = new KeyPrefsData();
		unresolvedBindings = new Dictionary<string, KeyBindingData>();
		foreach (KeyValuePair<string, KeyBindingData> item in dictionary)
		{
			KeyBindingDef namedSilentFail = DefDatabase<KeyBindingDef>.GetNamedSilentFail(item.Key);
			if (namedSilentFail != null)
			{
				data.keyPrefs[namedSilentFail] = item.Value;
			}
			else
			{
				unresolvedBindings[item.Key] = item.Value;
			}
		}
		if (flag)
		{
			data.ResetToDefaults();
		}
		data.AddMissingDefaultBindings();
		data.ErrorCheck();
		if (flag)
		{
			Save();
		}
	}

	public static void Save()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Dictionary<string, KeyBindingData> dictionary = new Dictionary<string, KeyBindingData>();
			foreach (KeyValuePair<KeyBindingDef, KeyBindingData> keyPref in data.keyPrefs)
			{
				dictionary[keyPref.Key.defName] = keyPref.Value;
			}
			foreach (KeyValuePair<string, KeyBindingData> unresolvedBinding in unresolvedBindings)
			{
				try
				{
					dictionary.Add(unresolvedBinding.Key, unresolvedBinding.Value);
				}
				catch (ArgumentException)
				{
				}
			}
			XDocument val = new XDocument();
			XElement val2 = DirectXmlSaver.XElementFromObject(dictionary, typeof(KeyPrefsData));
			((XContainer)val).Add((object)val2);
			val.Save(GenFilePaths.KeyPrefsFilePath);
		}
		catch (Exception ex2)
		{
			GenUI.ErrorDialog("ProblemSavingFile".Translate(GenFilePaths.KeyPrefsFilePath, ex2.ToString()));
			Log.Error("Exception saving keyprefs: " + ex2);
		}
	}
}
