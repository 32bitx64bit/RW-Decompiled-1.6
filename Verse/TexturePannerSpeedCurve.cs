using LudeonTK;
using UnityEngine;

namespace Verse;

public class TexturePannerSpeedCurve : TexturePanner
{
	public ComplexCurve timescaleCurve;

	public TexturePannerSpeedCurve(Material material, Vector2 direction, ComplexCurve timescaleCurve, float speed)
		: base(material, Shader.PropertyToID("_MainTex"), direction, speed)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this.timescaleCurve = timescaleCurve;
	}

	public TexturePannerSpeedCurve(Material material, string property, ComplexCurve timescaleCurve, Vector2 direction, float speed)
		: base(material, Shader.PropertyToID(property), direction, speed)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		this.timescaleCurve = timescaleCurve;
	}

	public TexturePannerSpeedCurve(Material material, int propertyID, ComplexCurve timescaleCurve, Vector2 direction, float speed)
		: base(material, propertyID, direction, speed)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		this.timescaleCurve = timescaleCurve;
	}

	public override void Tick()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		pan -= direction * speed * material.GetTextureScale(propertyID).x * timescaleCurve.Evaluate(Find.TickManager.TickRateMultiplier);
		material.SetTextureOffset(propertyID, pan);
	}
}
