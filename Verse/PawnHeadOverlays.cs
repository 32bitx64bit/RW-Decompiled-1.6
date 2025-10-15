using RimWorld;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class PawnHeadOverlays
{
	private Pawn pawn;

	private const float AngerBlinkPeriod = 1.2f;

	private const float AngerBlinkLength = 0.4f;

	private static readonly Material UnhappyMat = MaterialPool.MatFrom("Things/Pawn/Effects/Unhappy");

	private static readonly Material MentalStateImminentMat = MaterialPool.MatFrom("Things/Pawn/Effects/MentalStateImminent");

	public PawnHeadOverlays(Pawn pawn)
	{
		this.pawn = pawn;
	}

	public void RenderStatusOverlays(Vector3 offset, Quaternion quat, Mesh headMesh)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pawn.DrawPos + offset + new Vector3(0f, 0f, 0.32f);
		if (pawn.needs?.mood == null || pawn.Downed)
		{
			return;
		}
		if (pawn.HitPoints > 0 && pawn.IsColonistPlayerControlled)
		{
			if (pawn.mindState.mentalBreaker.BreakExtremeIsImminent)
			{
				if (Time.time % 1.2f < 0.4f)
				{
					DrawHeadGlow(val, MentalStateImminentMat);
				}
			}
			else if (pawn.mindState.mentalBreaker.BreakExtremeIsApproaching && Time.time % 1.2f < 0.4f)
			{
				DrawHeadGlow(val, UnhappyMat);
			}
		}
		MentalStateDef mentalStateDef = pawn.mindState?.mentalStateHandler?.CurStateDef;
		if ((ModsConfig.OdysseyActive && mentalStateDef == MentalStateDefOf.Terror) || (ModsConfig.BiotechActive && mentalStateDef == MentalStateDefOf.PanicFleeFire))
		{
			ThingDefOf.Mote_TerrorHalo.graphicData.Graphic.DrawWorker(val, pawn.Rotation, null, null, 0f);
		}
	}

	private void DrawHeadGlow(Vector3 headLoc, Material mat)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Graphics.DrawMesh(MeshPool.plane20, headLoc, Quaternion.identity, mat, 0);
	}
}
