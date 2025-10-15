using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Building_Battery : Building
{
	private int ticksToExplode;

	private Sustainer wickSustainer;

	private static readonly Vector2 BarSize = new Vector2(1.3f, 0.4f);

	private const float MinEnergyToExplode = 500f;

	private const float EnergyToLoseWhenExplode = 400f;

	private const float ExplodeChancePerDamage = 0.05f;

	private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f));

	private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref ticksToExplode, "ticksToExplode", 0);
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.DrawAt(drawLoc, flip);
		CompPowerBattery comp = GetComp<CompPowerBattery>();
		GenDraw.FillableBarRequest fillableBarRequest = default(GenDraw.FillableBarRequest);
		fillableBarRequest.center = drawLoc + Vector3.up * 0.1f;
		fillableBarRequest.size = BarSize;
		fillableBarRequest.fillPercent = comp.StoredEnergy / comp.Props.storedEnergyMax;
		fillableBarRequest.filledMat = BatteryBarFilledMat;
		fillableBarRequest.unfilledMat = BatteryBarUnfilledMat;
		fillableBarRequest.margin = 0.15f;
		GenDraw.FillableBarRequest r = fillableBarRequest;
		Rot4 rotation = base.Rotation;
		rotation.Rotate(RotationDirection.Clockwise);
		r.rotation = rotation;
		GenDraw.DrawFillableBar(r);
		if (ticksToExplode > 0 && base.Spawned)
		{
			base.Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
		}
	}

	protected override void Tick()
	{
		base.Tick();
		if (ticksToExplode > 0)
		{
			if (wickSustainer == null)
			{
				StartWickSustainer();
			}
			else
			{
				wickSustainer.Maintain();
			}
			ticksToExplode--;
			if (ticksToExplode == 0)
			{
				GenExplosion.DoExplosion(base.Spawned ? this.OccupiedRect().RandomCell : base.PositionHeld, radius: Rand.Range(0.5f, 1f) * 3f, map: base.MapHeld, damType: DamageDefOf.Flame, instigator: null);
				GetComp<CompPowerBattery>().DrawPower(400f);
			}
		}
	}

	public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
	{
		base.PostApplyDamage(dinfo, totalDamageDealt);
		if (!base.Destroyed && ticksToExplode == 0 && dinfo.Def == DamageDefOf.Flame && Rand.Value < 0.05f && GetComp<CompPowerBattery>().StoredEnergy > 500f)
		{
			ticksToExplode = Rand.Range(70, 150);
			StartWickSustainer();
		}
	}

	private void StartWickSustainer()
	{
		SoundInfo info = SoundInfo.InMap(base.SpawnedParentOrMe, MaintenanceType.PerTick);
		wickSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
	}
}
