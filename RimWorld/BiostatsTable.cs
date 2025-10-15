using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class BiostatsTable
{
	private struct BiostatData
	{
		public string labelKey;

		public string descKey;

		public Texture2D icon;

		public bool displayMaxGCXInfo;

		public BiostatData(string labelKey, string descKey, Texture2D icon, bool displayMaxGCXInfo)
		{
			this.labelKey = labelKey;
			this.descKey = descKey;
			this.icon = icon;
			this.displayMaxGCXInfo = displayMaxGCXInfo;
		}
	}

	private static float cachedWidth;

	private const float NumberWidth = 90f;

	private const float IconSize = 22f;

	private static readonly BiostatData[] Biostats = new BiostatData[3]
	{
		new BiostatData("Complexity", "ComplexityDesc", GeneUtility.GCXTex.Texture, displayMaxGCXInfo: true),
		new BiostatData("Metabolism", "MetabolismDesc", GeneUtility.METTex.Texture, displayMaxGCXInfo: false),
		new BiostatData("ArchitesRequired", "ArchitesRequiredDesc", GeneUtility.ARCTex.Texture, displayMaxGCXInfo: false)
	};

	private static Dictionary<string, string> truncateCache = new Dictionary<string, string>();

	private static float MaxLabelWidth(int arc)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		int num2 = ((arc > 0) ? Biostats.Length : (Biostats.Length - 1));
		for (int i = 0; i < num2; i++)
		{
			num = Mathf.Max(num, Text.CalcSize(Biostats[i].labelKey.Translate().CapitalizeFirst()).x);
		}
		return num;
	}

	public static float HeightForBiostats(int arc)
	{
		float num = Text.LineHeight * 3f;
		if (arc > 0)
		{
			num += Text.LineHeight * 1.5f;
		}
		return num;
	}

	public static void Draw(Rect rect, int gcx, int met, int arc, bool drawMax, bool ignoreLimits, int maxGCX = -1)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		int num = ((arc > 0) ? Biostats.Length : (Biostats.Length - 1));
		float num2 = MaxLabelWidth(arc);
		float num3 = ((Rect)(ref rect)).height / (float)num;
		GUI.BeginGroup(rect);
		Rect val = default(Rect);
		Rect rect2 = default(Rect);
		Rect rect3 = default(Rect);
		for (int i = 0; i < num; i++)
		{
			((Rect)(ref val))._002Ector(0f, (float)i * num3 + (num3 - 22f) / 2f, 22f, 22f);
			((Rect)(ref rect2))._002Ector(((Rect)(ref val)).xMax + 4f, (float)i * num3, num2, num3);
			((Rect)(ref rect3))._002Ector(0f, ((Rect)(ref rect2)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect2)).height);
			if (i % 2 == 1)
			{
				Widgets.DrawLightHighlight(rect3);
			}
			Widgets.DrawHighlightIfMouseover(rect3);
			((Rect)(ref rect3)).xMax = ((Rect)(ref rect2)).xMax + 4f + 90f;
			TaggedString taggedString = Biostats[i].descKey.Translate();
			if (maxGCX >= 0 && Biostats[i].displayMaxGCXInfo)
			{
				taggedString += "\n\n" + "MaxComplexityDesc".Translate();
			}
			TooltipHandler.TipRegion(rect3, taggedString);
			GUI.DrawTexture(val, (Texture)(object)Biostats[i].icon);
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect2, Biostats[i].labelKey.Translate().CapitalizeFirst());
			Text.Anchor = (TextAnchor)0;
		}
		float num4 = num2 + 4f + 22f + 4f;
		string text = gcx.ToString();
		string text2 = met.ToStringWithSign();
		if (drawMax && !ignoreLimits)
		{
			if (maxGCX >= 0)
			{
				if (gcx > maxGCX)
				{
					text = text.Colorize(ColorLibrary.RedReadable);
				}
				text = text + " / " + maxGCX;
			}
			if (met < GeneTuning.BiostatRange.TrueMin)
			{
				text2 = string.Concat(text2, " (" + "min".Translate() + " ", GeneTuning.BiostatRange.TrueMin.ToString(), ")");
				text2 = text2.Colorize(ColorLibrary.RedReadable);
			}
		}
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(new Rect(num4, 0f, 90f, num3), text);
		Widgets.Label(new Rect(num4, num3, 90f, num3), text2);
		if (arc > 0)
		{
			Widgets.Label(new Rect(num4, num3 * 2f, 90f, num3), arc.ToString());
		}
		Text.Anchor = (TextAnchor)3;
		float num5 = ((Rect)(ref rect)).width - num2 - 90f - 22f - 4f;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(num4 + 90f + 4f, num3, num5, num3);
		if (((Rect)(ref rect4)).width != cachedWidth)
		{
			cachedWidth = ((Rect)(ref rect4)).width;
			truncateCache.Clear();
		}
		string text3 = MetabolismDescAt(met);
		Widgets.Label(rect4, text3.Truncate(((Rect)(ref rect4)).width, truncateCache));
		if (Mouse.IsOver(rect4) && !text3.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect4, text3);
		}
		Text.Anchor = (TextAnchor)0;
		GUI.EndGroup();
	}

	private static string MetabolismDescAt(int met)
	{
		if (met == 0)
		{
			return string.Empty;
		}
		return "HungerRate".Translate() + " x" + GeneTuning.MetabolismToFoodConsumptionFactorCurve.Evaluate(met).ToStringPercent();
	}
}
