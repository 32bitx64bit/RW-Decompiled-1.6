using UnityEngine;

namespace Verse;

public struct SkyColorSet
{
	public Color sky;

	public Color shadow;

	public Color overlay;

	public float saturation;

	public SkyColorSet(Color sky, Color shadow, Color overlay, float saturation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.sky = sky;
		this.shadow = shadow;
		this.overlay = overlay;
		this.saturation = saturation;
	}

	public static SkyColorSet Lerp(SkyColorSet A, SkyColorSet B, float t)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		SkyColorSet result = default(SkyColorSet);
		result.sky = Color.Lerp(A.sky, B.sky, t);
		result.shadow = Color.Lerp(A.shadow, B.shadow, t);
		result.overlay = Color.Lerp(A.overlay, B.overlay, t);
		result.saturation = Mathf.Lerp(A.saturation, B.saturation, t);
		return result;
	}

	public static SkyColorSet LerpDarken(SkyColorSet A, SkyColorSet B, float t)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		SkyColorSet result = default(SkyColorSet);
		result.sky = Color.Lerp(A.sky, A.sky.Min(B.sky), t);
		result.shadow = Color.Lerp(A.shadow, A.shadow.Min(B.shadow), t);
		result.overlay = Color.Lerp(A.overlay, A.overlay.Min(B.overlay), t);
		result.saturation = Mathf.Lerp(A.saturation, Mathf.Min(A.saturation, B.saturation), t);
		return result;
	}

	public override string ToString()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[9] { "(sky=", null, null, null, null, null, null, null, null };
		Color val = sky;
		obj[1] = ((object)(Color)(ref val)).ToString();
		obj[2] = ", shadow=";
		val = shadow;
		obj[3] = ((object)(Color)(ref val)).ToString();
		obj[4] = ", overlay=";
		val = overlay;
		obj[5] = ((object)(Color)(ref val)).ToString();
		obj[6] = ", sat=";
		obj[7] = saturation.ToString();
		obj[8] = ")";
		return string.Concat(obj);
	}
}
