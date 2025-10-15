using UnityEngine;
using Verse;

namespace RimWorld;

public class CompAncientHeatVent : CompAncientVent
{
	private float targetGlow = 1f;

	private float intensity;

	private MaterialPropertyBlock glowProps;

	private MaterialPropertyBlock heatShimmerProps;

	private static int shaderPropertyIDIntensity;

	protected override bool AppliesEffectsToPawns => true;

	public new CompProperties_AncientHeatVent Props => (CompProperties_AncientHeatVent)props;

	protected override void ToggleIndividualVent(bool on)
	{
	}

	private void DrawFleckQuad(FleckDef def, MaterialPropertyBlock props)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		GraphicData graphicData = def.graphicData;
		Vector3 val = parent.Position.ToVector3Shifted() + graphicData.drawOffset;
		val.y = AltitudeLayer.MoteOverhead.AltitudeFor();
		Matrix4x4 val2 = Matrix4x4.TRS(val, Quaternion.identity, (parent.DrawSize * graphicData.drawSize).ToVector3());
		Graphics.DrawMesh(MeshPool.plane10, val2, graphicData.Graphic.MatSingle, 0, (Camera)null, 0, props);
	}

	private void DrawGlow()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		GraphicData graphicData = FleckDefOf.AncientVentHeatGlow.graphicData;
		glowProps = new MaterialPropertyBlock();
		glowProps.SetColor(ShaderPropertyIDs.Color, graphicData.color.WithAlpha(intensity * targetGlow * graphicData.color.a));
		DrawFleckQuad(FleckDefOf.AncientVentHeatGlow, glowProps);
	}

	private void DrawHeatShimmer()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (heatShimmerProps == null)
		{
			heatShimmerProps = new MaterialPropertyBlock();
		}
		heatShimmerProps.SetFloat(shaderPropertyIDIntensity, intensity);
		DrawFleckQuad(FleckDefOf.AncientVentHeatShimmer, glowProps);
	}

	public override void PostDraw()
	{
		if (base.VentOn)
		{
			DrawGlow();
			DrawHeatShimmer();
		}
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		base.PostSpawnSetup(respawningAfterLoad);
		intensity = (base.VentOn ? 1f : 0f);
		shaderPropertyIDIntensity = Shader.PropertyToID("_Intensity");
	}

	public override void CompTickInterval(int delta)
	{
		if (base.VentOn && intensity < 1f)
		{
			intensity += (float)delta * (1f / 60f) / Props.rampUpTime;
			if (intensity > 1f)
			{
				intensity = 1f;
			}
		}
		else
		{
			if (!(intensity > 0f))
			{
				return;
			}
			targetGlow = Mathf.Lerp(Props.minGlowBrightness, Props.maxGlowBrightness, 0.5f + 0.5f * Mathf.Sin((float)GenTicks.TicksGame * (1f / 60f) * Props.pulseSpeed));
			if (!base.VentOn)
			{
				intensity -= (float)delta * (1f / 60f) / Props.rampDownTime;
				if (intensity < 0f)
				{
					intensity = 0f;
				}
			}
		}
	}
}
