using UnityEngine;

namespace Verse;

public static class Scribe_Values
{
	public static void Look<T>(ref T value, string label, T defaultValue = default(T), bool forceSave = false)
	{
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		if (Scribe.mode == LoadSaveMode.Saving)
		{
			if (typeof(T) == typeof(TargetInfo))
			{
				Log.Error("Saving a TargetInfo " + label + " with Scribe_Values. TargetInfos must be saved with Scribe_TargetInfo.");
			}
			else if (typeof(Thing).IsAssignableFrom(typeof(T)))
			{
				Log.Error("Using Scribe_Values with a Thing reference " + label + ". Use Scribe_References or Scribe_Deep instead.");
			}
			else if (typeof(IExposable).IsAssignableFrom(typeof(T)))
			{
				Log.Error("Using Scribe_Values with a IExposable reference " + label + ". Use Scribe_References or Scribe_Deep instead.");
			}
			else if (GenTypes.IsDef(typeof(T)))
			{
				Log.Error("Using Scribe_Values with a Def " + label + ". Use Scribe_Defs instead.");
			}
			else
			{
				if (!forceSave && (value != null || defaultValue == null) && (value == null || value.Equals(defaultValue)))
				{
					return;
				}
				if (value == null)
				{
					if (!Scribe.EnterNode(label))
					{
						return;
					}
					try
					{
						Scribe.saver.WriteAttribute("IsNull", "True");
						return;
					}
					finally
					{
						Scribe.ExitNode();
					}
				}
				T val = value;
				string value2;
				if (val is Vector2 val2)
				{
					value2 = ((Vector2)(ref val2)).ToString("G9");
				}
				else
				{
					val = value;
					if (val is Vector3 val3)
					{
						value2 = ((Vector3)(ref val3)).ToString("G9");
					}
					else
					{
						val = value;
						if (val is Quaternion val4)
						{
							value2 = ((Quaternion)(ref val4)).ToString("G9");
						}
						else
						{
							val = value;
							value2 = ((!(val is float num)) ? value.ToString() : num.ToString("G9"));
						}
					}
				}
				Scribe.saver.WriteElement(label, value2);
			}
		}
		else if (Scribe.mode == LoadSaveMode.LoadingVars)
		{
			value = ScribeExtractor.ValueFromNode(Scribe.loader.curXmlParent[label], defaultValue);
		}
	}
}
