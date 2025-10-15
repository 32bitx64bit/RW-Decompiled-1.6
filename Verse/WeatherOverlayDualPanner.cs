using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class WeatherOverlayDualPanner : SkyOverlay
{
	private Vector2 worldPan1 = Vector2.zero;

	private Vector2 worldPan2 = Vector2.zero;

	public Material worldOverlayMat;

	public Material screenOverlayMat;

	protected float worldOverlayPanSpeed1;

	protected float worldOverlayPanSpeed2;

	protected Vector2 worldPanDir1;

	protected Vector2 worldPanDir2;

	private static readonly int RenderLayer = LayerMask.NameToLayer("GravshipExclude");

	private static readonly int MainTex2 = Shader.PropertyToID("_MainTex2");

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	public WeatherOverlayDualPanner()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			SetOverlayColor(Color.clear);
		});
	}

	public override void TickOverlay(Map map, float lerpFactor)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)worldOverlayMat == (Object)null))
		{
			worldPan1 -= worldPanDir1 * (worldOverlayPanSpeed1 * worldOverlayMat.GetTextureScale(MainTex).x * Find.TickManager.TickRateMultiplier);
			worldOverlayMat.SetTextureOffset(MainTex, worldPan1);
			if (worldOverlayMat.HasProperty(MainTex2))
			{
				worldPan2 -= worldPanDir2 * (worldOverlayPanSpeed2 * worldOverlayMat.GetTextureScale(MainTex2).x * Find.TickManager.TickRateMultiplier);
				worldOverlayMat.SetTextureOffset(MainTex2, worldPan2);
			}
		}
	}

	public override void DrawOverlay(Map map)
	{
		if ((Object)(object)worldOverlayMat != (Object)null)
		{
			SkyOverlay.DrawWorldOverlay(map, worldOverlayMat, GetRenderLayer());
		}
		if ((Object)(object)screenOverlayMat != (Object)null)
		{
			SkyOverlay.DrawScreenOverlay(screenOverlayMat, GetRenderLayer());
		}
	}

	public override void SetOverlayColor(Color color)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)worldOverlayMat != (Object)null)
		{
			worldOverlayMat.color = color;
		}
		if ((Object)(object)screenOverlayMat != (Object)null)
		{
			screenOverlayMat.color = color;
		}
	}

	protected virtual int GetRenderLayer()
	{
		return RenderLayer;
	}

	public override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		worldPan1 = Vector2.zero;
		worldPan2 = Vector2.zero;
		if ((Object)(object)worldOverlayMat != (Object)null)
		{
			worldOverlayMat.SetTextureOffset(MainTex, worldPan1);
			if (worldOverlayMat.HasProperty(MainTex2))
			{
				worldOverlayMat.SetTextureOffset(MainTex2, worldPan2);
			}
		}
	}

	public override string ToString()
	{
		if ((Object)(object)worldOverlayMat != (Object)null)
		{
			return ((Object)worldOverlayMat).name;
		}
		if ((Object)(object)screenOverlayMat != (Object)null)
		{
			return ((Object)screenOverlayMat).name;
		}
		return base.ToString();
	}
}
