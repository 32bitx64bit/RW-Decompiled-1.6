using System.Collections.Generic;
using LudeonTK;
using TMPro;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldFeatures : IExposable
{
	public List<WorldFeature> features = new List<WorldFeature>();

	public bool textsCreated;

	private static List<WorldFeatureTextMesh> texts = new List<WorldFeatureTextMesh>();

	private const float BaseAlpha = 0.3f;

	private const float AlphaChangeSpeed = 5f;

	[TweakValue("Interface", 0f, 300f)]
	private static float TextWrapThreshold = 150f;

	[TweakValue("Interface.World", 0f, 100f)]
	protected static bool ForceLegacyText = false;

	[TweakValue("Interface.World", 1f, 150f)]
	protected static float AlphaScale = 30f;

	[TweakValue("Interface.World", 0f, 1f)]
	protected static float VisibleMinimumSize = 0.04f;

	[TweakValue("Interface.World", 0f, 5f)]
	protected static float VisibleMaximumSize = 1f;

	private static void TextWrapThreshold_Changed()
	{
		Find.WorldFeatures.textsCreated = false;
	}

	protected static void ForceLegacyText_Changed()
	{
		Find.WorldFeatures.textsCreated = false;
	}

	public void ExposeData()
	{
		Scribe_Collections.Look(ref features, "features", LookMode.Deep);
		if (Scribe.mode != LoadSaveMode.PostLoadInit)
		{
			return;
		}
		WorldGrid grid = Find.WorldGrid;
		if (grid.Surface.tileFeature != null && grid.Surface.tileFeature.Length != 0)
		{
			DataSerializeUtility.LoadUshort(grid.Surface.tileFeature, grid.TilesCount, delegate(int i, ushort data)
			{
				grid[i].feature = ((data == ushort.MaxValue) ? null : GetFeatureWithID(data));
			});
		}
		textsCreated = false;
	}

	public void UpdateFeatures()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!textsCreated)
		{
			textsCreated = true;
			CreateTextsAndSetPosition();
		}
		bool flag = Find.PlaySettings.showWorldFeatures && !WorldRendererUtility.WorldBackgroundNow;
		for (int i = 0; i < features.Count; i++)
		{
			Vector3 position = texts[i].Position;
			bool flag2 = flag && !WorldRendererUtility.HiddenBehindTerrainNow(position);
			if (flag2 != texts[i].Active)
			{
				texts[i].SetActive(flag2);
				texts[i].WrapAroundPlanetSurface(features[i].layer);
			}
			if (flag2)
			{
				UpdateAlpha(texts[i], features[i]);
			}
		}
	}

	public WorldFeature GetFeatureWithID(int uniqueID)
	{
		for (int i = 0; i < features.Count; i++)
		{
			if (features[i].uniqueID == uniqueID)
			{
				return features[i];
			}
		}
		return null;
	}

	private void UpdateAlpha(WorldFeatureTextMesh text, WorldFeature feature)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.3f * feature.alpha;
		if (!Mathf.Approximately(text.Color.a, num))
		{
			text.Color = new Color(1f, 1f, 1f, num);
			text.WrapAroundPlanetSurface(feature.layer);
		}
		float num2 = Time.deltaTime * 5f;
		if (GoodCameraAltitudeFor(feature))
		{
			feature.alpha += num2;
		}
		else
		{
			feature.alpha -= num2;
		}
		feature.alpha = Mathf.Clamp01(feature.alpha);
	}

	private bool GoodCameraAltitudeFor(WorldFeature feature)
	{
		float effectiveDrawSize = feature.EffectiveDrawSize;
		float altitude = Find.WorldCameraDriver.altitude;
		float num = 1f / (altitude / AlphaScale * (altitude / AlphaScale));
		effectiveDrawSize *= num;
		if (!feature.layer.IsSelected)
		{
			return false;
		}
		if ((int)Find.WorldCameraDriver.CurrentZoom <= 0 && effectiveDrawSize >= 0.56f)
		{
			return false;
		}
		if (effectiveDrawSize < VisibleMinimumSize)
		{
			return Find.WorldCameraDriver.AltitudePercent <= 0.07f;
		}
		if (effectiveDrawSize > VisibleMaximumSize)
		{
			return Find.WorldCameraDriver.AltitudePercent >= 0.35f;
		}
		return true;
	}

	private void CreateTextsAndSetPosition()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		CreateOrDestroyTexts();
		float averageTileSize = Find.WorldGrid.AverageTileSize;
		for (int i = 0; i < features.Count; i++)
		{
			texts[i].Text = features[i].name.WordWrapAt(TextWrapThreshold);
			texts[i].Size = features[i].EffectiveDrawSize * averageTileSize;
			Vector3 normalized = ((Vector3)(ref features[i].drawCenter)).normalized;
			Quaternion val = Quaternion.LookRotation(Vector3.Cross(normalized, Vector3.up), normalized);
			val *= Quaternion.Euler(Vector3.right * 90f);
			val *= Quaternion.Euler(Vector3.forward * (90f - features[i].drawAngle));
			texts[i].Rotation = val;
			texts[i].LocalPosition = features[i].drawCenter;
			texts[i].WrapAroundPlanetSurface(features[i].layer);
			texts[i].SetActive(active: false);
		}
	}

	private void CreateOrDestroyTexts()
	{
		for (int i = 0; i < texts.Count; i++)
		{
			texts[i].Destroy();
		}
		texts.Clear();
		bool flag = LanguageDatabase.activeLanguage == LanguageDatabase.defaultLanguage;
		for (int j = 0; j < features.Count; j++)
		{
			WorldFeatureTextMesh worldFeatureTextMesh = ((!ForceLegacyText && (flag || !HasCharactersUnsupportedByTextMeshPro(features[j].name))) ? ((WorldFeatureTextMesh)new WorldFeatureTextMesh_TextMeshPro()) : ((WorldFeatureTextMesh)new WorldFeatureTextMesh_Legacy()));
			worldFeatureTextMesh.Init();
			texts.Add(worldFeatureTextMesh);
		}
	}

	private bool HasCharactersUnsupportedByTextMeshPro(string str)
	{
		TMP_FontAsset font = ((TMP_Text)WorldFeatureTextMesh_TextMeshPro.WorldTextPrefab.GetComponent<TextMeshPro>()).font;
		for (int i = 0; i < str.Length; i++)
		{
			if (!HasCharacter(font, str[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool HasCharacter(TMP_FontAsset font, char character)
	{
		if (TMP_FontAsset.GetCharacters(font).IndexOf(character) >= 0)
		{
			return true;
		}
		List<TMP_FontAsset> fallbackFontAssetTable = font.fallbackFontAssetTable;
		for (int i = 0; i < fallbackFontAssetTable.Count; i++)
		{
			if (TMP_FontAsset.GetCharacters(fallbackFontAssetTable[i]).IndexOf(character) >= 0)
			{
				return true;
			}
		}
		return false;
	}
}
