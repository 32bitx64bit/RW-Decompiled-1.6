using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class MainTabWindow_Work : MainTabWindow_PawnTable
{
	private const int SpaceBetweenPriorityArrowsAndWorkLabels = 40;

	protected override PawnTableDef PawnTableDef => PawnTableDefOf.Work;

	protected override float ExtraTopSpace => 40f;

	protected override IEnumerable<Pawn> Pawns => base.Pawns.Where((Pawn pawn) => !pawn.DevelopmentalStage.Baby());

	public override void DoWindowContents(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		base.DoWindowContents(rect);
		if ((int)Event.current.type != 8)
		{
			DoManualPrioritiesCheckbox();
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Text.Anchor = (TextAnchor)1;
			Text.Font = GameFont.Tiny;
			Widgets.Label(new Rect(370f, ((Rect)(ref rect)).y + 5f, 160f, 30f), "<= " + "HigherPriority".Translate());
			Widgets.Label(new Rect(630f, ((Rect)(ref rect)).y + 5f, 160f, 30f), "LowerPriority".Translate() + " =>");
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)0;
		}
	}

	private void DoManualPrioritiesCheckbox()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(5f, 5f, 140f, 30f);
		bool useWorkPriorities = Current.Game.playSettings.useWorkPriorities;
		Widgets.CheckboxLabeled(rect, "ManualPriorities".Translate(), ref Current.Game.playSettings.useWorkPriorities);
		if (useWorkPriorities != Current.Game.playSettings.useWorkPriorities)
		{
			foreach (Pawn item in PawnsFinder.AllMapsWorldAndTemporary_Alive)
			{
				if (item.Faction == Faction.OfPlayer && item.workSettings != null)
				{
					item.workSettings.Notify_UseWorkPrioritiesChanged();
				}
			}
		}
		if (Current.Game.playSettings.useWorkPriorities)
		{
			using (new TextBlock(new Color(1f, 1f, 1f, 0.5f)))
			{
				Widgets.Label(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMax - 6f, ((Rect)(ref rect)).width, 60f), "PriorityOneDoneFirst".Translate());
			}
		}
		if (!Current.Game.playSettings.useWorkPriorities)
		{
			UIHighlighter.HighlightOpportunity(rect, "ManualPriorities-Off");
		}
	}
}
