using UnityEngine;

namespace Verse;

public static class HumanlikeMeshPoolUtility
{
	public static float HumanlikeBodyWidthForPawn(Pawn pawn)
	{
		if (ModsConfig.BiotechActive && pawn.ageTracker.CurLifeStage.bodyWidth.HasValue)
		{
			return pawn.ageTracker.CurLifeStage.bodyWidth.Value;
		}
		return 1.5f;
	}

	public static float HumanlikeHeadWidthForPawn(Pawn pawn)
	{
		float num = 1.5f;
		if (ModsConfig.BiotechActive && pawn.ageTracker.CurLifeStage.headSizeFactor.HasValue)
		{
			num *= pawn.ageTracker.CurLifeStage.headSizeFactor.Value;
		}
		return num;
	}

	public static GraphicMeshSet GetHumanlikeBodySetForPawn(Pawn pawn, float wFactor = 1f, float hFactor = 1f)
	{
		float num = HumanlikeBodyWidthForPawn(pawn);
		return MeshPool.GetMeshSetForSize(num * wFactor, num * hFactor);
	}

	public static GraphicMeshSet GetHumanlikeHeadSetForPawn(Pawn pawn, float wFactor = 1f, float hFactor = 1f)
	{
		float num = HumanlikeHeadWidthForPawn(pawn);
		return MeshPool.GetMeshSetForSize(num * wFactor, num * hFactor);
	}

	public static GraphicMeshSet GetHumanlikeHairSetForPawn(Pawn pawn, float wFactor = 1f, float hFactor = 1f)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = pawn.story.headType.hairMeshSize * new Vector2(wFactor, hFactor);
		if (ModsConfig.BiotechActive && pawn.ageTracker.CurLifeStage.headSizeFactor.HasValue)
		{
			val *= pawn.ageTracker.CurLifeStage.headSizeFactor.Value;
		}
		return MeshPool.GetMeshSetForSize(val.x, val.y);
	}

	public static GraphicMeshSet GetHumanlikeBeardSetForPawn(Pawn pawn, float wFactor = 1f, float hFactor = 1f)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = pawn.story.headType.beardMeshSize * new Vector2(wFactor, hFactor);
		if (ModsConfig.BiotechActive && pawn.ageTracker.CurLifeStage.headSizeFactor.HasValue)
		{
			val *= pawn.ageTracker.CurLifeStage.headSizeFactor.Value;
		}
		return MeshPool.GetMeshSetForSize(val.x, val.y);
	}
}
