namespace Verse;

public class PawnRenderNode_Tattoo_Head : PawnRenderNode_Tattoo
{
	public PawnRenderNode_Tattoo_Head(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
		: base(pawn, props, tree)
	{
	}

	public override GraphicMeshSet MeshSetFor(Pawn pawn)
	{
		if (pawn.style?.FaceTattoo == null || pawn.style.FaceTattoo.noGraphic)
		{
			return null;
		}
		return HumanlikeMeshPoolUtility.GetHumanlikeHairSetForPawn(pawn);
	}

	public override Graphic GraphicFor(Pawn pawn)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckIdeology("Head tattoo"))
		{
			return null;
		}
		if (pawn.style?.FaceTattoo == null || pawn.style.FaceTattoo.noGraphic)
		{
			return null;
		}
		return pawn.style.FaceTattoo.GraphicFor(pawn, ColorFor(pawn));
	}
}
