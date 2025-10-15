using LudeonTK;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_SandHard : SkyOverlay
{
	private Color color;

	private static readonly Material CloudLayer = MatLoader.LoadMat("Weather/Sandstorm/CloudLayer");

	private static readonly Material ParticleLayer = MatLoader.LoadMat("Weather/Sandstorm/ParticleLayer");

	private static readonly ComplexCurve speedCurve = new ComplexCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f), new Keyframe(2f, 1.075f), new Keyframe(3f, 1.15f));

	private TexturePannerSpeedCurve panner0 = new TexturePannerSpeedCurve(CloudLayer, "_MainTex", speedCurve, new Vector2(1f, -0.3f), 0.04f);

	private TexturePannerSpeedCurve panner1 = new TexturePannerSpeedCurve(CloudLayer, "_MainTex2", speedCurve, new Vector2(1f, -0.28f), 0.08f);

	private TexturePannerSpeedCurve panner2 = new TexturePannerSpeedCurve(ParticleLayer, "_MainTex", speedCurve, new Vector2(1f, -0.3f), 0.03f);

	private TexturePannerSpeedCurve panner3 = new TexturePannerSpeedCurve(ParticleLayer, "_MainTex2", speedCurve, new Vector2(1f, -0.28f), 0.06f);

	public override void DrawOverlay(Map map)
	{
		SkyOverlay.DrawWorldOverlay(map, ParticleLayer);
		SkyOverlay.DrawWorldOverlay(map, CloudLayer);
	}

	public override void SetOverlayColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.color = color;
	}

	public override void TickOverlay(Map map, float lerpFactor)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.5f + 0.5f * GenCelestial.CurCelestialSunGlow(map);
		Color val = default(Color);
		((Color)(ref val))._002Ector(color.r * num * num, color.g * num * num, color.b * num, color.a);
		CloudLayer.color = val;
		ParticleLayer.color = val;
		panner0.Tick();
		panner1.Tick();
		panner2.Tick();
		panner3.Tick();
	}
}
