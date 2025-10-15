using UnityEngine;
using Verse;

namespace RimWorld;

public class Precept_Xenotype : Precept
{
	public XenotypeDef xenotype;

	public CustomXenotype customXenotype;

	private string XenotypeName
	{
		get
		{
			if (xenotype == null)
			{
				return customXenotype?.name;
			}
			return xenotype.label;
		}
	}

	public override string UIInfoFirstLine => XenotypeName.CapitalizeFirst();

	public override string TipLabel => def.issue.LabelCap + ": " + XenotypeName.CapitalizeFirst();

	public override string GenerateNameRaw()
	{
		return name;
	}

	public override void DrawIcon(Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (xenotype != null)
		{
			GUI.DrawTexture(rect, (Texture)(object)xenotype.Icon);
		}
		else if (customXenotype != null)
		{
			GUI.DrawTexture(rect, (Texture)(object)customXenotype.IconDef.Icon);
		}
	}

	public override void CopyTo(Precept other)
	{
		base.CopyTo(other);
		Precept_Xenotype obj = (Precept_Xenotype)other;
		obj.xenotype = xenotype;
		obj.customXenotype = customXenotype;
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Defs.Look(ref xenotype, "xenotype");
		Scribe_Deep.Look(ref customXenotype, "customXenotype");
	}
}
