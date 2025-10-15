using UnityEngine;
using Verse;

namespace RimWorld;

public class GeneGizmo_DeathrestCapacity : Gizmo
{
	protected Gene_Deathrest gene;

	private const float Padding = 6f;

	private const float Width = 140f;

	public override float GetWidth(float maxWidth)
	{
		return 140f;
	}

	public GeneGizmo_DeathrestCapacity(Gene_Deathrest gene)
	{
		this.gene = gene;
		Order = -100f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = GenUI.ContractedBy(rect, 6f);
		float num = ((Rect)(ref val)).height / 3f;
		Widgets.DrawWindowBackground(rect);
		GUI.BeginGroup(val);
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref val)).width, num), "Deathrest".Translate().CapitalizeFirst());
		if (gene.DeathrestNeed != null)
		{
			gene.DeathrestNeed.DrawOnGUI(new Rect(0f, num, ((Rect)(ref val)).width, num + 2f), int.MaxValue, 2f, drawArrows: false, doTooltip: true, (Rect?)new Rect(0f, 0f, ((Rect)(ref val)).width, num * 2f), drawLabel: false);
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, num * 2f, ((Rect)(ref val)).width, Text.LineHeight);
		Text.Anchor = (TextAnchor)1;
		Widgets.Label(rect2, string.Format("{0}: {1} / {2}", "Buildings".Translate().CapitalizeFirst(), gene.CurrentCapacity, gene.DeathrestCapacity));
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect2))
		{
			Widgets.DrawHighlight(rect2);
			TooltipHandler.TipRegion(rect2, "DeathrestCapacityDesc".Translate() + "\n\n" + "PawnIsConnectedToBuildings".Translate(gene.pawn.Named("PAWN"), gene.CurrentCapacity.Named("CURRENT"), gene.DeathrestCapacity.Named("MAX")));
		}
		GUI.EndGroup();
		return new GizmoResult(GizmoState.Clear);
	}
}
