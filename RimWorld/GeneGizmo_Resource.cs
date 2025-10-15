using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class GeneGizmo_Resource : Gizmo_Slider
{
	protected Gene_Resource gene;

	protected List<IGeneResourceDrain> drainGenes;

	protected override Color BarColor { get; }

	protected override Color BarHighlightColor { get; }

	protected override bool IsDraggable
	{
		get
		{
			if (!gene.pawn.IsColonistPlayerControlled)
			{
				return gene.pawn.IsPrisonerOfColony;
			}
			return true;
		}
	}

	protected override string BarLabel => $"{gene.ValueForDisplay} / {gene.MaxForDisplay}";

	protected override int Increments => gene.MaxForDisplay / 10;

	protected override float ValuePercent => gene.ValuePercent;

	protected override FloatRange DragRange => new FloatRange(0f, gene.Max);

	protected override float Target
	{
		get
		{
			return gene.targetValue / gene.Max;
		}
		set
		{
			gene.SetTargetValuePct(value);
		}
	}

	protected override string Title
	{
		get
		{
			string text = gene.ResourceLabel.CapitalizeFirst();
			if (Find.Selector.SelectedPawns.Count != 1)
			{
				text = text + " (" + gene.pawn.LabelShort + ")";
			}
			return text;
		}
	}

	public GeneGizmo_Resource(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barHighlightColor)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this.gene = gene;
		this.drainGenes = drainGenes;
		BarColor = barColor;
		BarHighlightColor = barHighlightColor;
	}

	protected override IEnumerable<float> GetBarThresholds()
	{
		for (int i = 0; i < gene.def.resourceGizmoThresholds.Count; i++)
		{
			yield return gene.def.resourceGizmoThresholds[i];
		}
	}
}
