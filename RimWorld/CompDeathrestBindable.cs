using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class CompDeathrestBindable : ThingComp
{
	private Pawn boundPawn;

	public int presenceTicks;

	[Unsaved(false)]
	private CompPowerTrader cachedPowerComp;

	[Unsaved(false)]
	private CompRefuelable cachedRefuelableComp;

	[Unsaved(false)]
	private Gene_Deathrest cachedDeathrestGene;

	[Unsaved(false)]
	private Material cachedHoseMat;

	[Unsaved(false)]
	private Sustainer sustainer;

	public CompProperties_DeathrestBindable Props => (CompProperties_DeathrestBindable)props;

	public Pawn BoundPawn => boundPawn;

	public bool CanIncreasePresence
	{
		get
		{
			if (PowerTraderComp != null && !PowerTraderComp.PowerOn)
			{
				return false;
			}
			if (RefuelableComp != null && !RefuelableComp.HasFuel)
			{
				return false;
			}
			if (boundPawn.InBed())
			{
				Building_Bed building_Bed = boundPawn.CurrentBed();
				if (building_Bed == parent)
				{
					return true;
				}
				CompDeathrestBindable compDeathrestBindable = building_Bed.TryGetComp<CompDeathrestBindable>();
				if (compDeathrestBindable == null || compDeathrestBindable.BoundPawn != boundPawn)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}

	private CompPowerTrader PowerTraderComp
	{
		get
		{
			if (cachedPowerComp == null)
			{
				cachedPowerComp = parent.TryGetComp<CompPowerTrader>();
			}
			return cachedPowerComp;
		}
	}

	private CompRefuelable RefuelableComp
	{
		get
		{
			if (cachedRefuelableComp == null)
			{
				cachedRefuelableComp = parent.TryGetComp<CompRefuelable>();
			}
			return cachedRefuelableComp;
		}
	}

	private Gene_Deathrest DeathrestGene
	{
		get
		{
			if (cachedDeathrestGene == null)
			{
				cachedDeathrestGene = boundPawn?.genes?.GetFirstGeneOfType<Gene_Deathrest>();
			}
			return cachedDeathrestGene;
		}
	}

	private Material HoseMat
	{
		get
		{
			if ((Object)(object)cachedHoseMat == (Object)null)
			{
				cachedHoseMat = MaterialPool.MatFrom("Other/DeathrestBuildingConnection");
			}
			return cachedHoseMat;
		}
	}

	public override void PostPostMake()
	{
		if (!ModLister.CheckBiotech("Deathrest binding"))
		{
			parent.Destroy();
		}
		else
		{
			base.PostPostMake();
		}
	}

	public bool CanBindTo(Pawn pawn)
	{
		if (Props.mustBeLayingInToBind && pawn.CurrentBed() != parent)
		{
			return false;
		}
		if (boundPawn != null)
		{
			return boundPawn == pawn;
		}
		return true;
	}

	public void BindTo(Pawn pawn)
	{
		boundPawn = pawn;
	}

	public void Notify_DeathrestGeneRemoved()
	{
		cachedDeathrestGene = null;
		boundPawn = null;
		presenceTicks = 0;
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		if (respawningAfterLoad || parent.BeingTransportedOnGravship)
		{
			return;
		}
		presenceTicks = 0;
		foreach (Pawn item in parent.Map.mapPawns.PawnsInFaction(parent.Faction))
		{
			if (item.Deathresting)
			{
				Gene_Deathrest gene_Deathrest = item.genes?.GetFirstGeneOfType<Gene_Deathrest>();
				if (gene_Deathrest != null && gene_Deathrest.CanBindToBindable(this))
				{
					gene_Deathrest.BindTo(this);
				}
			}
		}
	}

	public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
	{
		if (!parent.BeingTransportedOnGravship)
		{
			presenceTicks = 0;
			if (mode != DestroyMode.WillReplace && boundPawn != null)
			{
				boundPawn.genes?.GetFirstGeneOfType<Gene_Deathrest>()?.Notify_BoundBuildingDeSpawned(parent);
			}
			if (sustainer != null && !sustainer.Ended)
			{
				sustainer.End();
			}
			sustainer = null;
		}
	}

	public override void CompTick()
	{
		if (parent.IsHashIntervalTick(250))
		{
			CompTickRare();
		}
		if (presenceTicks <= 0)
		{
			return;
		}
		if (!Props.soundWorking.NullOrUndefined())
		{
			if (sustainer == null || sustainer.Ended)
			{
				sustainer = Props.soundWorking.TrySpawnSustainer(SoundInfo.InMap(parent));
			}
			sustainer.Maintain();
		}
		RefuelableComp?.Notify_UsedThisTick();
	}

	public override void CompTickRare()
	{
		if (PowerTraderComp != null)
		{
			PowerTraderComp.PowerOutput = ((presenceTicks > 0) ? (0f - PowerTraderComp.Props.PowerConsumption) : (0f - PowerTraderComp.Props.idlePowerDraw));
		}
	}

	public override void PostDraw()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (!Props.mustBeLayingInToBind && boundPawn != null && boundPawn.Map == parent.Map && boundPawn.Deathresting && DeathrestGene != null && DeathrestGene.BoundComps.Contains(this) && CanIncreasePresence)
		{
			Vector3 val = boundPawn.CurrentBed().TrueCenter();
			Vector3 val2 = parent.TrueCenter();
			val.y = (val2.y = AltitudeLayer.SmallWire.AltitudeFor());
			Matrix4x4 identity = Matrix4x4.identity;
			((Matrix4x4)(ref identity)).SetTRS((val + val2) / 2f, Quaternion.Euler(0f, val.AngleToFlat(val2) + 90f, 0f), new Vector3(1f, 1f, (val - val2).MagnitudeHorizontal()));
			Graphics.DrawMesh(MeshPool.plane10, identity, HoseMat, 0);
		}
	}

	public void Apply()
	{
		if (boundPawn != null)
		{
			if (Props.hediffToApply != null)
			{
				boundPawn.health.AddHediff(Props.hediffToApply);
			}
			if (Props.hemogenLimitOffset > 0f)
			{
				Gene_Hemogen gene_Hemogen = boundPawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				gene_Hemogen?.SetMax(gene_Hemogen.Max + Props.hemogenLimitOffset);
			}
		}
	}

	public void TryIncreasePresence(int delta)
	{
		if (!CanIncreasePresence)
		{
			return;
		}
		if (presenceTicks <= 0)
		{
			SoundInfo info = SoundInfo.InMap(parent);
			if (!Props.soundWorking.NullOrUndefined() && (sustainer == null || sustainer.Ended))
			{
				sustainer = Props.soundWorking.TrySpawnSustainer(info);
			}
			if (!Props.soundStart.NullOrUndefined())
			{
				Props.soundStart.PlayOneShot(info);
			}
		}
		presenceTicks += delta;
	}

	public void Notify_DeathrestEnded()
	{
		presenceTicks = 0;
		if (parent.Spawned)
		{
			if (sustainer != null && !sustainer.Ended)
			{
				sustainer.End();
			}
			if (!Props.soundEnd.NullOrUndefined())
			{
				Props.soundEnd.PlayOneShot(SoundInfo.InMap(parent));
			}
		}
	}

	public override string CompInspectStringExtra()
	{
		string text = null;
		if (boundPawn != null && DeathrestGene != null)
		{
			text = text + ("BoundTo".Translate() + ": " + boundPawn.NameShortColored).Resolve() + string.Format(" ({0}/{1} {2})", DeathrestGene.CurrentCapacity, DeathrestGene.DeathrestCapacity, "DeathrestCapacity".Translate());
			if (Props.displayTimeActive && presenceTicks > 0 && DeathrestGene.deathrestTicks > 0)
			{
				float f = Mathf.Clamp01((float)presenceTicks / (float)DeathrestGene.deathrestTicks);
				text += string.Format("\n{0}: {1} / {2} ({3})\n{4}", "TimeActiveThisDeathrest".Translate(), presenceTicks.ToStringTicksToPeriod(allowSeconds: true, shortForm: true), DeathrestGene.deathrestTicks.ToStringTicksToPeriod(allowSeconds: true, shortForm: true), f.ToStringPercent(), "MinimumNeededToApply".Translate(0.75f.ToStringPercent()));
			}
		}
		else
		{
			text += "WillBindOnFirstUse".Translate();
		}
		return text;
	}

	public override void PostExposeData()
	{
		Scribe_References.Look(ref boundPawn, "boundPawn");
		Scribe_Values.Look(ref presenceTicks, "presenceTicks", 0);
	}
}
