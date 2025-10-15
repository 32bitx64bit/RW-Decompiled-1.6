using Verse;

namespace RimWorld;

public class CompDrawAdditionalGraphics : ThingComp
{
	private CompProperties_DrawAdditionalGraphics Props => (CompProperties_DrawAdditionalGraphics)props;

	public override void PostDraw()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		foreach (GraphicData graphic in Props.graphics)
		{
			graphic.Graphic.Draw(parent.DrawPos, parent.Rotation, parent);
		}
	}
}
