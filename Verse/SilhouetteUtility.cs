using System;
using System.Collections.Generic;
using RimWorld;
using Unity.Mathematics;
using UnityEngine;

namespace Verse;

public static class SilhouetteUtility
{
	private readonly struct SilhouetteCacheKey
	{
		private readonly ThingDef thingDef;

		private readonly LifeStageDef lifeStageDef;

		private readonly int graphicIndex;

		private readonly Gender gender;

		private readonly RotStage rotMode;

		public SilhouetteCacheKey(Pawn pawn)
		{
			thingDef = pawn.def;
			lifeStageDef = pawn.ageTracker.CurLifeStage;
			graphicIndex = pawn.GetGraphicIndex();
			gender = pawn.gender;
			rotMode = pawn.mutant?.rotStage ?? RotStage.Fresh;
		}

		public SilhouetteCacheKey(Thing thing)
		{
			thingDef = thing.def;
			lifeStageDef = null;
			graphicIndex = thing.OverrideGraphicIndex ?? (-1);
			gender = Gender.None;
			rotMode = RotStage.Fresh;
		}

		public override int GetHashCode()
		{
			int seed = thingDef.GetHashCode();
			if (lifeStageDef != null)
			{
				seed = Gen.HashCombineInt(seed, lifeStageDef.GetHashCode());
			}
			seed = Gen.HashCombineInt(seed, graphicIndex);
			seed = Gen.HashCombineInt(seed, gender.GetHashCode());
			return Gen.HashCombineInt(seed, rotMode.GetHashCode());
		}
	}

	private class SilhouetteCacheValue : IDisposable
	{
		public readonly Material east;

		public readonly Material west;

		public SilhouetteCacheValue(Material east, Material west)
		{
			this.east = east;
			this.west = west;
		}

		public void Dispose()
		{
			Object.Destroy((Object)(object)east);
			Object.Destroy((Object)(object)west);
		}
	}

	private static readonly Dictionary<SilhouetteCacheKey, SilhouetteCacheValue> materialCache = new Dictionary<SilhouetteCacheKey, SilhouetteCacheValue>(512);

	private static readonly Dictionary<float3, MaterialPropertyBlock> materialPropertyBlockCache = new Dictionary<float3, MaterialPropertyBlock>();

	private static int lastCachedAlphaFrame = -1;

	private static float lastCachedAlpha = 0f;

	private const float DotHighlightSizeRatio = 0.01f;

	private const float DotAlpha = 0.75f;

	private const float SilhouetteAlpha = 0.9f;

	private static readonly Color ThingColor = new Color(0.56f, 0.62f, 0.9f);

	private static readonly Color UncontrolledMechDotColor = new Color(0.8f, 0.55f, 0.17f, 0.75f);

	public const float DotHighlightStartRange = 0.9f;

	private const int MaximumMaterialCache = 512;

	public static void DrawGraphicSilhouette(Thing thing, Vector3 pos)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (ShouldDrawSilhouette(thing))
		{
			DrawSilhouette(thing, pos);
		}
	}

	public static void DrawGUISilhouette(Thing thing)
	{
		if (thing is Pawn thing2 && ShouldDrawPawnDotSilhouette(thing2))
		{
			DrawPawnDotSilhouette(thing2);
		}
	}

	private static SilhouetteCacheKey GetSilhouetteCacheKey(Thing thing)
	{
		if (thing is Pawn pawn)
		{
			return new SilhouetteCacheKey(pawn);
		}
		return new SilhouetteCacheKey(thing);
	}

	public static void NotifyGraphicDirty(Thing thing)
	{
		SilhouetteCacheKey silhouetteCacheKey = GetSilhouetteCacheKey(thing);
		if (materialCache.ContainsKey(silhouetteCacheKey))
		{
			materialCache.Remove(silhouetteCacheKey);
		}
	}

	private static void DrawPawnDotSilhouette(Thing thing)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Color color = GetColor(thing);
		color.a = GetAlpha();
		GUI.DrawTexture(GetAdjustedScreenspaceRect(thing), (Texture)(object)TexUI.DotHighlight, (ScaleMode)2, true, 0f, color, 0f, 0f);
	}

	public static void DrawSilhouetteJob(Thing thing, Matrix4x4 trs)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		(Mesh mesh, Material material) cachedSilhouetteData = GetCachedSilhouetteData(thing);
		Mesh item = cachedSilhouetteData.mesh;
		Material item2 = cachedSilhouetteData.material;
		MaterialPropertyBlock cachedMaterialPropertyBlock = GetCachedMaterialPropertyBlock(GetColor(thing));
		GenDraw.DrawMeshNowOrLater(item, trs, item2, drawNow: false, cachedMaterialPropertyBlock);
	}

	private static void DrawSilhouette(Thing thing, Vector3 pos)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		(Mesh mesh, Material material) cachedSilhouetteData = GetCachedSilhouetteData(thing);
		Mesh item = cachedSilhouetteData.mesh;
		Material item2 = cachedSilhouetteData.material;
		Graphic graphic = ((thing is Pawn pawn) ? pawn.Drawer.renderer.SilhouetteGraphic : thing.Graphic);
		MaterialPropertyBlock cachedMaterialPropertyBlock = GetCachedMaterialPropertyBlock(GetColor(thing));
		Vector3 val = (thing.def.rotatable ? new Vector3(graphic.drawSize.y, 0f, graphic.drawSize.x) : new Vector3(graphic.drawSize.x, 0f, graphic.drawSize.y));
		Vector3 inverseFovScale = Find.CameraDriver.InverseFovScale;
		if (val.x < 2.5f)
		{
			inverseFovScale.x *= val.x + AdjustScale(val.x);
		}
		else
		{
			inverseFovScale.x *= val.x;
		}
		if (val.z < 2.5f)
		{
			inverseFovScale.z *= val.z + AdjustScale(val.z);
		}
		else
		{
			inverseFovScale.z *= val.z;
		}
		Matrix4x4 matrix = Matrix4x4.TRS(pos.SetToAltitude(AltitudeLayer.Silhouettes), Quaternion.AngleAxis(0f, Vector3.up), inverseFovScale);
		GenDraw.DrawMeshNowOrLater(item, matrix, item2, drawNow: false, cachedMaterialPropertyBlock);
	}

	public static bool ShouldDrawSilhouette(Thing thing)
	{
		if (WorldComponent_GravshipController.GravshipRenderInProgess)
		{
			return false;
		}
		if (Find.ScreenshotModeHandler.Active)
		{
			return false;
		}
		if (Prefs.HighlightStyleMode != HighlightStyleMode.Silhouettes)
		{
			return false;
		}
		if (!thing.def.drawHighlight)
		{
			return false;
		}
		if (!ShouldHighlight(thing))
		{
			return false;
		}
		return true;
	}

	private static bool ShouldDrawPawnDotSilhouette(Thing thing)
	{
		if (Prefs.HighlightStyleMode == HighlightStyleMode.Dots && thing.def.drawHighlight)
		{
			return ShouldHighlight(thing);
		}
		return false;
	}

	public static bool CanHighlightAny()
	{
		if (Prefs.DotHighlightDisplayMode == DotHighlightDisplayMode.None)
		{
			return false;
		}
		CameraDriver cameraDriver = Find.CameraDriver;
		if (cameraDriver.ZoomRootSize < cameraDriver.config.sizeRange.max * 0.9f)
		{
			return false;
		}
		return true;
	}

	private static bool ShouldHighlight(Thing thing)
	{
		if (!CanHighlightAny())
		{
			return false;
		}
		if (!thing.Spawned)
		{
			return false;
		}
		if (thing.shouldHighlightCachedTick == GenTicks.TicksGame)
		{
			return thing.shouldHighlightCached;
		}
		thing.shouldHighlightCachedTick = GenTicks.TicksGame;
		return thing.shouldHighlightCached = ShouldHighlightInt(thing);
	}

	private static bool ShouldHighlightInt(Thing thing)
	{
		if (thing is Pawn pawn)
		{
			if (!pawn.IsPlayerControlled && pawn.Fogged())
			{
				return false;
			}
			if (Prefs.DotHighlightDisplayMode == DotHighlightDisplayMode.HighlightHostiles && !pawn.HostileTo(Faction.OfPlayer))
			{
				return false;
			}
			if (pawn.IsHiddenFromPlayer())
			{
				return false;
			}
			if (ModsConfig.AnomalyActive && pawn.Map.gameConditionManager.MapBrightness < 0.1f && thing.Map.glowGrid.GroundGlowAt(thing.Position) <= 0f && pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
		}
		else
		{
			if (thing.Fogged())
			{
				return false;
			}
			if (thing.def.drawHighlightOnlyForHostile && !thing.HostileTo(Faction.OfPlayer))
			{
				return false;
			}
		}
		return true;
	}

	private static MaterialPropertyBlock GetCachedMaterialPropertyBlock(Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		float3 key = default(float3);
		((float3)(ref key))._002Ector(color.r, color.g, color.b);
		color.a = GetAlpha();
		if (materialPropertyBlockCache.TryGetValue(key, out var value))
		{
			value.SetColor(ShaderPropertyIDs.Color, color);
		}
		else
		{
			value = new MaterialPropertyBlock();
			value.SetColor(ShaderPropertyIDs.Color, color);
			materialPropertyBlockCache[key] = value;
		}
		return value;
	}

	private static Color GetColor(Thing thing)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (thing.highlightColorCachedTick == GenTicks.TicksGame)
		{
			return thing.highlightColorCached;
		}
		thing.highlightColorCachedTick = GenTicks.TicksGame;
		return thing.highlightColorCached = GetColorInt(thing);
	}

	private static Color GetColorInt(Thing thing)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (thing.def.highlightColor.HasValue)
		{
			return thing.def.highlightColor.Value;
		}
		if (thing is Pawn pawn)
		{
			if (pawn.IsColonyMech && !pawn.IsColonyMechPlayerControlled)
			{
				return UncontrolledMechDotColor;
			}
			return PawnNameColorUtility.PawnNameColorOf(pawn);
		}
		if (thing.HostileTo(Faction.OfPlayer))
		{
			return PawnNameColorUtility.ColorBaseHostile;
		}
		return ThingColor;
	}

	private static float GetAlpha()
	{
		if (lastCachedAlphaFrame == RealTime.frameCount)
		{
			return lastCachedAlpha;
		}
		lastCachedAlphaFrame = RealTime.frameCount;
		bool num = Prefs.HighlightStyleMode == HighlightStyleMode.Silhouettes;
		CameraDriver cameraDriver = Find.CameraDriver;
		float num2 = Mathf.Clamp01(Mathf.InverseLerp(cameraDriver.config.sizeRange.max * 0.84999996f, cameraDriver.config.sizeRange.max, cameraDriver.ZoomRootSize));
		if (num)
		{
			return lastCachedAlpha = 0.9f * num2;
		}
		return lastCachedAlpha = 0.75f * num2;
	}

	private static (Mesh mesh, Material material) GetCachedSilhouetteData(Thing thing)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (materialCache.Count > 512)
		{
			materialCache.Clear();
		}
		SilhouetteCacheKey silhouetteCacheKey = GetSilhouetteCacheKey(thing);
		if (!materialCache.ContainsKey(silhouetteCacheKey))
		{
			Graphic coloredVersion = ((thing is Pawn pawn) ? pawn.Drawer.renderer.SilhouetteGraphic : thing.Graphic).GetColoredVersion(ShaderDatabase.Silhouette, Color.white, Color.white);
			materialCache[silhouetteCacheKey] = new SilhouetteCacheValue(coloredVersion.MatEast, coloredVersion.MatWest);
		}
		Material item;
		Mesh item2;
		if (thing.Rotation == Rot4.West)
		{
			item = materialCache[silhouetteCacheKey].west;
			item2 = MeshPool.GridPlaneFlip(Vector2.one);
		}
		else
		{
			item = materialCache[silhouetteCacheKey].east;
			item2 = MeshPool.GridPlane(Vector2.one);
		}
		return (mesh: item2, material: item);
	}

	public static float AdjustScale(float scale)
	{
		return Mathf.InverseLerp(2.5f, 0f, scale) * 0.75f;
	}

	public static Rect GetAdjustedScreenspaceRect(Thing thing, float screenSizeRatio = 0.01f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.op_Implicit(Find.Camera.WorldToScreenPoint(thing.DrawPos)) / Prefs.UIScale;
		val.y = (float)UI.screenHeight - val.y;
		float num = screenSizeRatio * (float)Mathf.Min(Screen.width, Screen.height) / Prefs.UIScale;
		return new Rect(val.x - num, val.y - num / 2f, num * 2f, num);
	}
}
