using UnityEngine;
using Verse;

namespace RimWorld;

public class CompFleckEmitterLongTerm : ThingComp
{
	public bool Enabled { get; set; }

	private CompProperties_FleckEmitterLongTerm Props => (CompProperties_FleckEmitterLongTerm)props;

	private bool ShouldEmit
	{
		get
		{
			if (!Enabled)
			{
				return Props.forceEnabled;
			}
			return true;
		}
	}

	private void EmissionTick(IFleckCreator fleckDestination)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!ShouldEmit || !(Rand.Value < Props.spawnChance))
		{
			return;
		}
		CompProperties_FleckEmitterLongTerm compProperties_FleckEmitterLongTerm = Props;
		Vector3 val = parent.TrueCenter() + compProperties_FleckEmitterLongTerm.spawnOffsetFromCenter + Rand.InsideUnitCircleVec3 * compProperties_FleckEmitterLongTerm.spawnRadius;
		if (val.ToIntVec3().ShouldSpawnMotesAt(parent.Map))
		{
			FleckCreationData dataStatic = FleckMaker.GetDataStatic(val, parent.Map, compProperties_FleckEmitterLongTerm.fleckDef, compProperties_FleckEmitterLongTerm.fleckScale);
			dataStatic.rotationRate = Rand.Range(compProperties_FleckEmitterLongTerm.minRotationRate, compProperties_FleckEmitterLongTerm.maxRotationRate);
			dataStatic.velocityAngle = Rand.Range(compProperties_FleckEmitterLongTerm.minVelocityAngle, compProperties_FleckEmitterLongTerm.maxVelocityAngle);
			dataStatic.velocitySpeed = Rand.Range(compProperties_FleckEmitterLongTerm.minVelocitySpeed, compProperties_FleckEmitterLongTerm.maxVelocitySpeed);
			if (fleckDestination is FleckSystem)
			{
				dataStatic.spawnPosition.y = dataStatic.def.altitudeLayer.AltitudeFor(dataStatic.def.altitudeLayerIncOffset);
			}
			fleckDestination.CreateFleck(dataStatic);
		}
	}

	public void Prewarm()
	{
		if (ShouldEmit && !(Props.prewarmCycles <= 0f))
		{
			FleckSystem system = parent.Map.flecks.CreateFleckSystemFor(Props.fleckDef);
			system.Prewarm(Props.fleckDef.Lifetime * Props.prewarmCycles, null, delegate
			{
				EmissionTick(system);
			});
			parent.Map.flecks.HandOverSystem(system);
		}
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		Prewarm();
	}

	public override void CompTick()
	{
		EmissionTick(parent.Map.flecks);
	}
}
