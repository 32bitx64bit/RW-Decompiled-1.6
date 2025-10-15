using System;
using System.Collections.Generic;
using System.Text;
using LudeonTK;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompPowerPlantWind : CompPowerPlant
{
	public int updateWeatherEveryXTicks = 250;

	private int ticksSinceWeatherUpdate;

	private float cachedPowerOutput;

	private List<IntVec3> windPathCells = new List<IntVec3>();

	private List<Thing> windPathBlockedByThings = new List<Thing>();

	private List<IntVec3> windPathBlockedCells = new List<IntVec3>();

	private bool blockedDueToInOrbit;

	private float spinPosition;

	private Sustainer sustainer;

	private const float MaxUsableWindIntensity = 1.5f;

	[TweakValue("Graphics", 0f, 0.1f)]
	private static float SpinRateFactor = 0.035f;

	[TweakValue("Graphics", -1f, 1f)]
	private static float HorizontalBladeOffset = -0.02f;

	[TweakValue("Graphics", 0f, 1f)]
	private static float VerticalBladeOffset = 0.7f;

	[TweakValue("Graphics", 4f, 8f)]
	private static float BladeWidth = 6.6f;

	private const float PowerReductionPercentPerObstacle = 0.2f;

	private const string TranslateWindPathIsBlockedBy = "WindTurbine_WindPathIsBlockedBy";

	private const string TranslateWindPathIsBlockedByRoof = "WindTurbine_WindPathIsBlockedByRoof";

	private static Vector2 BarSize;

	private static readonly Material WindTurbineBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));

	private static readonly Material WindTurbineBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));

	private static readonly Material WindTurbineBladesMat = MaterialPool.MatFrom("Things/Building/Power/WindTurbine/WindTurbineBlades");

	protected override float DesiredPowerOutput => cachedPowerOutput;

	private float PowerPercent => base.PowerOutput / ((0f - base.Props.PowerConsumption) * 1.5f);

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.PostSpawnSetup(respawningAfterLoad);
		BarSize = new Vector2((float)parent.def.size.z - 0.95f, 0.14f);
		RecalculateBlockages();
		spinPosition = Rand.Range(0f, 15f);
	}

	public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
	{
		base.PostDeSpawn(map, mode);
		windPathCells.Clear();
		Sustainer sustainer = this.sustainer;
		if (sustainer != null && !sustainer.Ended)
		{
			this.sustainer.End();
		}
	}

	public override void PostSwapMap()
	{
		RecalculateBlockages();
	}

	public override void PostExposeData()
	{
		base.PostExposeData();
		Scribe_Values.Look(ref ticksSinceWeatherUpdate, "updateCounter", 0);
		Scribe_Values.Look(ref cachedPowerOutput, "cachedPowerOutput", 0f);
	}

	public override void CompTick()
	{
		base.CompTick();
		if (!base.PowerOn)
		{
			cachedPowerOutput = 0f;
			return;
		}
		ticksSinceWeatherUpdate++;
		if (ticksSinceWeatherUpdate >= updateWeatherEveryXTicks)
		{
			float num = Mathf.Min(parent.Map.windManager.WindSpeed, 1.5f);
			ticksSinceWeatherUpdate = 0;
			cachedPowerOutput = 0f - base.Props.PowerConsumption * num;
			RecalculateBlockages();
			if (blockedDueToInOrbit)
			{
				cachedPowerOutput = 0f;
			}
			else if (windPathBlockedCells.Count > 0)
			{
				float num2 = 0f;
				for (int i = 0; i < windPathBlockedCells.Count; i++)
				{
					num2 += cachedPowerOutput * 0.2f;
				}
				cachedPowerOutput -= num2;
				if (cachedPowerOutput < 0f)
				{
					cachedPowerOutput = 0f;
				}
			}
		}
		if (cachedPowerOutput > 0.01f)
		{
			spinPosition += PowerPercent * SpinRateFactor;
		}
		if (sustainer == null || sustainer.Ended)
		{
			sustainer = SoundDefOf.WindTurbine_Ambience.TrySpawnSustainer(SoundInfo.InMap(parent));
		}
		sustainer.Maintain();
		sustainer.externalParams["PowerOutput"] = PowerPercent;
	}

	public override void PostDraw()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		base.PostDraw();
		GenDraw.FillableBarRequest fillableBarRequest = default(GenDraw.FillableBarRequest);
		fillableBarRequest.center = parent.DrawPos + Vector3.up * 0.1f;
		fillableBarRequest.size = BarSize;
		fillableBarRequest.fillPercent = PowerPercent;
		fillableBarRequest.filledMat = WindTurbineBarFilledMat;
		fillableBarRequest.unfilledMat = WindTurbineBarUnfilledMat;
		fillableBarRequest.margin = 0.15f;
		GenDraw.FillableBarRequest r = fillableBarRequest;
		Rot4 rotation = parent.Rotation;
		rotation.Rotate(RotationDirection.Clockwise);
		r.rotation = rotation;
		GenDraw.DrawFillableBar(r);
		Vector3 val = parent.TrueCenter();
		val += parent.Rotation.FacingCell.ToVector3() * VerticalBladeOffset;
		val += parent.Rotation.RighthandCell.ToVector3() * HorizontalBladeOffset;
		val.y += 0.03658537f;
		float num = BladeWidth * Mathf.Sin(spinPosition);
		if (num < 0f)
		{
			num *= -1f;
		}
		bool num2 = spinPosition % MathF.PI * 2f < MathF.PI;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(num, 1f);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(val2.x, 1f, val2.y);
		Matrix4x4 val4 = default(Matrix4x4);
		((Matrix4x4)(ref val4)).SetTRS(val, parent.Rotation.AsQuat, val3);
		Graphics.DrawMesh(num2 ? MeshPool.plane10 : MeshPool.plane10Flip, val4, WindTurbineBladesMat, 0);
		val.y -= 0.07317074f;
		((Matrix4x4)(ref val4)).SetTRS(val, parent.Rotation.AsQuat, val3);
		Graphics.DrawMesh(num2 ? MeshPool.plane10Flip : MeshPool.plane10, val4, WindTurbineBladesMat, 0);
	}

	public override string CompInspectStringExtra()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(base.CompInspectStringExtra());
		if (windPathBlockedCells.Count > 0)
		{
			stringBuilder.AppendLine();
			Thing thing = null;
			if (windPathBlockedByThings != null)
			{
				thing = windPathBlockedByThings[0];
			}
			if (thing != null)
			{
				stringBuilder.Append("WindTurbine_WindPathIsBlockedBy".Translate() + " " + thing.Label);
			}
			else
			{
				stringBuilder.Append("WindTurbine_WindPathIsBlockedByRoof".Translate());
			}
		}
		if (blockedDueToInOrbit)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append("CannotFunctionOnLayer".Translate(parent.Map.Tile.LayerDef.label).CapitalizeFirst().Colorize(ColoredText.WarningColor));
		}
		return stringBuilder.ToString();
	}

	private void RecalculateBlockages()
	{
		if (windPathCells.Count == 0)
		{
			IEnumerable<IntVec3> collection = WindTurbineUtility.CalculateWindCells(parent.Position, parent.Rotation, parent.def.size);
			windPathCells.AddRange(collection);
		}
		windPathBlockedCells.Clear();
		windPathBlockedByThings.Clear();
		blockedDueToInOrbit = parent.Map.Biome.inVacuum;
		for (int i = 0; i < windPathCells.Count; i++)
		{
			IntVec3 intVec = windPathCells[i];
			if (parent.Map.roofGrid.Roofed(intVec))
			{
				windPathBlockedByThings.Add(null);
				windPathBlockedCells.Add(intVec);
				continue;
			}
			List<Thing> list = parent.Map.thingGrid.ThingsListAt(intVec);
			for (int j = 0; j < list.Count; j++)
			{
				Thing thing = list[j];
				if (thing.def.blockWind)
				{
					windPathBlockedByThings.Add(thing);
					windPathBlockedCells.Add(intVec);
					break;
				}
			}
		}
	}
}
