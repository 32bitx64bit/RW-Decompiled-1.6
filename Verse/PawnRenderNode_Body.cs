using UnityEngine;

namespace Verse;

public class PawnRenderNode_Body : PawnRenderNode
{
	public PawnRenderNode_Body(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
		: base(pawn, props, tree)
	{
	}

	public override Graphic GraphicFor(Pawn pawn)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		Shader val = ShaderFor(pawn);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		if (pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated)
		{
			return GraphicDatabase.Get<Graphic_Multi>(pawn.story.bodyType.bodyDessicatedGraphicPath, val);
		}
		if (pawn.IsMutant && !pawn.mutant.Def.bodyTypeGraphicPaths.NullOrEmpty())
		{
			string bodyGraphicPath = pawn.mutant.Def.GetBodyGraphicPath(pawn);
			if (bodyGraphicPath != null)
			{
				return GraphicDatabase.Get<Graphic_Multi>(bodyGraphicPath, val, Vector2.one, ColorFor(pawn));
			}
		}
		if (ModsConfig.AnomalyActive && pawn.IsCreepJoiner && pawn.story.bodyType != null && !pawn.creepjoiner.form.bodyTypeGraphicPaths.NullOrEmpty())
		{
			return GraphicDatabase.Get<Graphic_Multi>(pawn.creepjoiner.form.GetBodyGraphicPath(pawn), val, Vector2.one, ColorFor(pawn));
		}
		if (pawn.story?.bodyType?.bodyNakedGraphicPath == null)
		{
			return null;
		}
		return GraphicDatabase.Get<Graphic_Multi>(pawn.story.bodyType.bodyNakedGraphicPath, val, Vector2.one, ColorFor(pawn));
	}
}
