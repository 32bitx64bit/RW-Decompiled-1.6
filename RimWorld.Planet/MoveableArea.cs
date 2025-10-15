using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class MoveableArea : IExposable
{
	protected List<IntVec3> relativeCells = new List<IntVec3>();

	protected string label;

	protected string renamableLabel;

	protected Color color;

	protected int id;

	private Gravship gravship;

	public IEnumerable<IntVec3> RelativeCells
	{
		get
		{
			foreach (IntVec3 relativeCell in relativeCells)
			{
				yield return PrefabUtility.GetAdjustedLocalPosition(relativeCell, gravship.Rotation);
			}
		}
	}

	public MoveableArea()
	{
	}

	public MoveableArea(Gravship gravship, string label, string renamableLabel, Color color, int id)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		this.gravship = gravship;
		this.label = label;
		this.renamableLabel = renamableLabel;
		this.color = color;
		this.id = id;
	}

	public virtual void ExposeData()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Scribe_Collections.Look(ref relativeCells, "relativeCells", LookMode.Value);
		Scribe_Values.Look(ref label, "label");
		Scribe_Values.Look(ref renamableLabel, "renamableLabel");
		Scribe_Values.Look(ref color, "color");
		Scribe_Values.Look(ref id, "id", 0);
		Scribe_References.Look(ref gravship, "gravship");
	}

	public void Add(IntVec3 relativeCell)
	{
		relativeCells.Add(relativeCell);
	}
}
