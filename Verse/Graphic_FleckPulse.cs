using UnityEngine;

namespace Verse;

public class Graphic_FleckPulse : Graphic_Fleck
{
	protected override bool AllowInstancing => false;

	public override void DrawFleck(FleckDrawData drawData, DrawBatch batch)
	{
		drawData.propertyBlock = drawData.propertyBlock ?? batch.GetPropertyBlock();
		drawData.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecs, drawData.ageSecs);
		drawData.propertyBlock.SetFloat(ShaderPropertyIDs.RandomPerObject, drawData.id);
		base.DrawFleck(drawData, batch);
	}

	public override string ToString()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[7]
		{
			"Graphic_FleckPulse(path=",
			path,
			", shader=",
			((object)base.Shader)?.ToString(),
			", color=",
			null,
			null
		};
		Color val = color;
		obj[5] = ((object)(Color)(ref val)).ToString();
		obj[6] = ", colorTwo=unsupported)";
		return string.Concat(obj);
	}
}
