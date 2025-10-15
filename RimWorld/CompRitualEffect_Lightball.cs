using UnityEngine;
using Verse;

namespace RimWorld;

public class CompRitualEffect_Lightball : CompRitualEffect_IntervalSpawn
{
	public new CompProperties_RitualEffectLightball Props => (CompProperties_RitualEffectLightball)props;

	protected override Vector3 SpawnPos(LordJob_Ritual ritual)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	public override void SpawnFleck(LordJob_Ritual ritual, Vector3? forcedPos = null, float? exactRotation = null)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		CompPowerTrader compPowerTrader = ritual.selectedTarget.Thing.TryGetComp<CompPowerTrader>();
		if (compPowerTrader != null && compPowerTrader.PowerOn)
		{
			float num = Rand.Range(0f, 360f);
			float num2 = num + 180f;
			float num3 = (num + num2) / 2f + (float)Rand.Range(-55, 55);
			Vector3 val = parent.ritual.selectedTarget.Cell.ToVector3Shifted();
			Vector3 val2 = Quaternion.AngleAxis(num, Vector3.up) * Vector3.forward * Props.radius;
			Vector3 val3 = Quaternion.AngleAxis(num2, Vector3.up) * Vector3.forward * Props.radius;
			Vector3 val4 = Quaternion.AngleAxis(num3, Vector3.up) * Vector3.forward * Props.radius;
			base.SpawnFleck(parent.ritual, val + val2, num - 45f);
			base.SpawnFleck(parent.ritual, val + val3, num2 - 45f);
			base.SpawnFleck(parent.ritual, val + val4, num3 - 45f);
			lastSpawnTick = GenTicks.TicksGame;
		}
	}
}
