using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_NodeTreeWithFactionInfo : Dialog_NodeTree
{
	private Faction faction;

	private const float RelatedFactionInfoSize = 79f;

	public Dialog_NodeTreeWithFactionInfo(DiaNode nodeRoot, Faction faction, bool delayInteractivity = false, bool radioMode = false, string title = null)
		: base(nodeRoot, delayInteractivity, radioMode, title)
	{
		this.faction = faction;
		if (faction != null)
		{
			minOptionsAreaHeight = 60f;
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.DoWindowContents(inRect);
		if (faction != null && !faction.Hidden)
		{
			float curY = ((Rect)(ref inRect)).height - 79f;
			FactionUIUtility.DrawRelatedFactionInfo(inRect, faction, ref curY);
		}
	}
}
