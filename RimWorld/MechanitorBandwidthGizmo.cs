using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MechanitorBandwidthGizmo : Gizmo
{
	public const int InRectPadding = 6;

	private const int CellPadding = 2;

	private const float Width = 136f;

	private const int StartingBandwidthRows = 2;

	private static readonly Color EmptyBlockColor = new Color(0.3f, 0.3f, 0.3f, 1f);

	private static readonly Color FilledBlockColor = ColorLibrary.Yellow;

	private static readonly Color ExcessBlockColor = ColorLibrary.Red;

	private const int HeaderHeight = 20;

	private Pawn_MechanitorTracker tracker;

	public override bool Visible => Find.Selector.SelectedPawns.Count == 1;

	public MechanitorBandwidthGizmo(Pawn_MechanitorTracker tracker)
	{
		this.tracker = tracker;
		Order = -90f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckBiotech("Mechanitor bandwidth gizmo"))
		{
			return new GizmoResult(GizmoState.Clear);
		}
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = GenUI.ContractedBy(rect, 6f);
		Widgets.DrawWindowBackground(rect);
		int totalBandwidth = tracker.TotalBandwidth;
		int usedBandwidth = tracker.UsedBandwidth;
		string text = usedBandwidth.ToString("F0") + " / " + totalBandwidth.ToString("F0");
		TaggedString taggedString = "Bandwidth".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "BandwidthGizmoTip".Translate();
		int usedBandwidthFromSubjects = tracker.UsedBandwidthFromSubjects;
		if (usedBandwidthFromSubjects > 0)
		{
			taggedString += string.Concat("\n\n" + ("BandwidthUsage".Translate() + ": "), usedBandwidthFromSubjects.ToString());
			IEnumerable<string> entries = from p in tracker.OverseenPawns
				where !p.IsGestating()
				group p by p.kindDef into p
				select string.Concat(p.Key.LabelCap + " x", p.Count().ToString(), " (+", p.Sum((Pawn mech) => mech.GetStatValue(StatDefOf.BandwidthCost)).ToString(), ")");
			taggedString += "\n\n" + entries.ToLineList(" - ");
		}
		int usedBandwidthFromGestation = tracker.UsedBandwidthFromGestation;
		if (usedBandwidthFromGestation > 0)
		{
			taggedString += string.Concat("\n\n" + "MechGestationBandwidthUsed".Translate() + ": ", usedBandwidthFromGestation.ToString());
			IEnumerable<string> entries2 = from p in tracker.OverseenPawns
				where p.IsGestating()
				group p by p.kindDef into p
				select string.Concat(p.Key.LabelCap + " x", p.Count().ToString(), " (+", p.Sum((Pawn mech) => mech.GetStatValue(StatDefOf.BandwidthCost)).ToString(), ")");
			taggedString += "\n\n" + entries2.ToLineList(" - ");
		}
		TooltipHandler.TipRegion(rect, taggedString);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, 20f);
		Widgets.Label(rect2, "Bandwidth".Translate());
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)2;
		Widgets.Label(rect2, text);
		Text.Anchor = (TextAnchor)0;
		int num = Mathf.Max(usedBandwidth, totalBandwidth);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref rect2)).yMax + 6f, ((Rect)(ref val)).width, ((Rect)(ref val)).height - ((Rect)(ref rect2)).height - 6f);
		int num2 = 2;
		int num3 = Mathf.FloorToInt(((Rect)(ref val2)).height / (float)num2);
		int num4 = Mathf.FloorToInt(((Rect)(ref val2)).width / (float)num3);
		int num5 = 0;
		while (num2 * num4 < num)
		{
			num2++;
			num3 = Mathf.FloorToInt(((Rect)(ref val2)).height / (float)num2);
			num4 = Mathf.FloorToInt(((Rect)(ref val2)).width / (float)num3);
			num5++;
			if (num5 >= 1000)
			{
				Log.Error("Failed to fit bandwidth cells into gizmo rect.");
				return new GizmoResult(GizmoState.Clear);
			}
		}
		int num6 = Mathf.FloorToInt(((Rect)(ref val2)).width / (float)num3);
		int num7 = num2;
		float num8 = (((Rect)(ref val2)).width - (float)(num6 * num3)) / 2f;
		int num9 = 0;
		int usedBandwidthFromGestation2 = tracker.UsedBandwidthFromGestation;
		int num10 = ((num7 <= 2) ? 4 : 2);
		for (int i = 0; i < num7; i++)
		{
			for (int j = 0; j < num6; j++)
			{
				num9++;
				Rect val3 = GenUI.ContractedBy(new Rect(((Rect)(ref val2)).x + (float)(j * num3) + num8, ((Rect)(ref val2)).y + (float)(i * num3), (float)num3, (float)num3), 2f);
				if (num9 <= num)
				{
					if (num9 <= usedBandwidthFromGestation2)
					{
						Widgets.DrawRectFast(val3, EmptyBlockColor);
						Widgets.DrawRectFast(val3.ContractedBy(num10), FilledBlockColor);
					}
					else if (num9 <= usedBandwidth)
					{
						Widgets.DrawRectFast(val3, (num9 <= totalBandwidth) ? FilledBlockColor : ExcessBlockColor);
					}
					else
					{
						Widgets.DrawRectFast(val3, EmptyBlockColor);
					}
				}
			}
		}
		return new GizmoResult(GizmoState.Clear);
	}

	public override float GetWidth(float maxWidth)
	{
		return 136f;
	}
}
