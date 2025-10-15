using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class BuildingsDamageSectionLayerUtility
{
	private static readonly Material[] DefaultScratchMats = (Material[])(object)new Material[3]
	{
		MaterialPool.MatFrom("Damage/Scratch1", ShaderDatabase.Transparent),
		MaterialPool.MatFrom("Damage/Scratch2", ShaderDatabase.Transparent),
		MaterialPool.MatFrom("Damage/Scratch3", ShaderDatabase.Transparent)
	};

	private static List<DamageOverlay> availableOverlays = new List<DamageOverlay>();

	private static List<DamageOverlay> overlaysWorkingList = new List<DamageOverlay>();

	private static List<DamageOverlay> overlays = new List<DamageOverlay>();

	public static void TryInsertIntoAtlas()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		for (int i = 0; i < DefaultScratchMats.Length; i++)
		{
			GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)DefaultScratchMats[i].mainTexture);
		}
	}

	public static void Notify_BuildingHitPointsChanged(Building b, int oldHitPoints)
	{
		if (b.Spawned && b.def.useHitPoints && b.HitPoints != oldHitPoints && b.def.drawDamagedOverlay && GetDamageOverlaysCount(b, b.HitPoints) != GetDamageOverlaysCount(b, oldHitPoints))
		{
			b.Map.mapDrawer.MapMeshDirty(b.Position, MapMeshFlagDefOf.BuildingsDamage);
		}
	}

	public static bool UsesLinkableCornersAndEdges(Building b)
	{
		if (b.def.size.x == 1 && b.def.size.z == 1)
		{
			return b.def.Fillage == FillCategory.Full;
		}
		return false;
	}

	public static IList<Material> GetScratchMats(Building b)
	{
		IList<Material> result = DefaultScratchMats;
		if (b.def.graphicData != null && b.def.graphicData.damageData != null && b.def.graphicData.damageData.scratchMats != null)
		{
			result = b.def.graphicData.damageData.scratchMats;
		}
		return result;
	}

	public static List<DamageOverlay> GetAvailableOverlays(Building b)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		availableOverlays.Clear();
		if (GetScratchMats(b).Any())
		{
			int num = 3;
			Rect damageRect = GetDamageRect(b);
			float num2 = ((Rect)(ref damageRect)).width * ((Rect)(ref damageRect)).height;
			if (num2 > 4f)
			{
				num += Mathf.RoundToInt((num2 - 4f) * 0.54f);
			}
			for (int i = 0; i < num; i++)
			{
				availableOverlays.Add(DamageOverlay.Scratch);
			}
		}
		if (UsesLinkableCornersAndEdges(b))
		{
			if (b.def.graphicData != null && b.def.graphicData.damageData != null)
			{
				IntVec3 position = b.Position;
				DamageGraphicData damageData = b.def.graphicData.damageData;
				if ((Object)(object)damageData.edgeTopMat != (Object)null && DifferentAt(b, position.x, position.z + 1) && SameAndDamagedAt(b, position.x + 1, position.z) && DifferentAt(b, position.x + 1, position.z + 1))
				{
					availableOverlays.Add(DamageOverlay.TopEdge);
				}
				if ((Object)(object)damageData.edgeRightMat != (Object)null && DifferentAt(b, position.x + 1, position.z) && SameAndDamagedAt(b, position.x, position.z + 1) && DifferentAt(b, position.x + 1, position.z + 1))
				{
					availableOverlays.Add(DamageOverlay.RightEdge);
				}
				if ((Object)(object)damageData.edgeBotMat != (Object)null && DifferentAt(b, position.x, position.z - 1) && SameAndDamagedAt(b, position.x + 1, position.z) && DifferentAt(b, position.x + 1, position.z - 1))
				{
					availableOverlays.Add(DamageOverlay.BotEdge);
				}
				if ((Object)(object)damageData.edgeLeftMat != (Object)null && DifferentAt(b, position.x - 1, position.z) && SameAndDamagedAt(b, position.x, position.z + 1) && DifferentAt(b, position.x - 1, position.z + 1))
				{
					availableOverlays.Add(DamageOverlay.LeftEdge);
				}
				if ((Object)(object)damageData.cornerTLMat != (Object)null && DifferentAt(b, position.x - 1, position.z) && DifferentAt(b, position.x, position.z + 1))
				{
					availableOverlays.Add(DamageOverlay.TopLeftCorner);
				}
				if ((Object)(object)damageData.cornerTRMat != (Object)null && DifferentAt(b, position.x + 1, position.z) && DifferentAt(b, position.x, position.z + 1))
				{
					availableOverlays.Add(DamageOverlay.TopRightCorner);
				}
				if ((Object)(object)damageData.cornerBRMat != (Object)null && DifferentAt(b, position.x + 1, position.z) && DifferentAt(b, position.x, position.z - 1))
				{
					availableOverlays.Add(DamageOverlay.BotRightCorner);
				}
				if ((Object)(object)damageData.cornerBLMat != (Object)null && DifferentAt(b, position.x - 1, position.z) && DifferentAt(b, position.x, position.z - 1))
				{
					availableOverlays.Add(DamageOverlay.BotLeftCorner);
				}
			}
		}
		else
		{
			GetCornerMats(out var topLeft, out var topRight, out var botRight, out var botLeft, b);
			if ((Object)(object)topLeft != (Object)null)
			{
				availableOverlays.Add(DamageOverlay.TopLeftCorner);
			}
			if ((Object)(object)topRight != (Object)null)
			{
				availableOverlays.Add(DamageOverlay.TopRightCorner);
			}
			if ((Object)(object)botLeft != (Object)null)
			{
				availableOverlays.Add(DamageOverlay.BotLeftCorner);
			}
			if ((Object)(object)botRight != (Object)null)
			{
				availableOverlays.Add(DamageOverlay.BotRightCorner);
			}
		}
		return availableOverlays;
	}

	public static void GetCornerMats(out Material topLeft, out Material topRight, out Material botRight, out Material botLeft, Building b)
	{
		if (b.def.graphicData == null || b.def.graphicData.damageData == null)
		{
			topLeft = null;
			topRight = null;
			botRight = null;
			botLeft = null;
			return;
		}
		DamageGraphicData damageData = b.def.graphicData.damageData;
		if (b.Rotation == Rot4.North)
		{
			topLeft = damageData.cornerTLMat;
			topRight = damageData.cornerTRMat;
			botRight = damageData.cornerBRMat;
			botLeft = damageData.cornerBLMat;
		}
		else if (b.Rotation == Rot4.East)
		{
			topLeft = damageData.cornerBLMat;
			topRight = damageData.cornerTLMat;
			botRight = damageData.cornerTRMat;
			botLeft = damageData.cornerBRMat;
		}
		else if (b.Rotation == Rot4.South)
		{
			topLeft = damageData.cornerBRMat;
			topRight = damageData.cornerBLMat;
			botRight = damageData.cornerTLMat;
			botLeft = damageData.cornerTRMat;
		}
		else
		{
			topLeft = damageData.cornerTRMat;
			topRight = damageData.cornerBRMat;
			botRight = damageData.cornerBLMat;
			botLeft = damageData.cornerTLMat;
		}
	}

	public static List<DamageOverlay> GetOverlays(Building b)
	{
		overlays.Clear();
		overlaysWorkingList.Clear();
		overlaysWorkingList.AddRange(GetAvailableOverlays(b));
		if (!overlaysWorkingList.Any())
		{
			return overlays;
		}
		Rand.PushState();
		Rand.Seed = Gen.HashCombineInt(b.thingIDNumber, 1958376471);
		int damageOverlaysCount = GetDamageOverlaysCount(b, b.HitPoints);
		for (int i = 0; i < damageOverlaysCount; i++)
		{
			if (!overlaysWorkingList.Any())
			{
				break;
			}
			DamageOverlay item = overlaysWorkingList.RandomElement();
			overlaysWorkingList.Remove(item);
			overlays.Add(item);
		}
		Rand.PopState();
		return overlays;
	}

	public static Rect GetDamageRect(Building b)
	{
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		DamageGraphicData damageGraphicData = null;
		if (b.def.graphicData != null)
		{
			damageGraphicData = b.def.graphicData.damageData;
		}
		CellRect cellRect = b.OccupiedRect();
		Rect result = default(Rect);
		((Rect)(ref result))._002Ector((float)cellRect.minX, (float)cellRect.minZ, (float)cellRect.Width, (float)cellRect.Height);
		if (damageGraphicData != null)
		{
			if (b.Rotation == Rot4.North && damageGraphicData.rectN != default(Rect))
			{
				((Rect)(ref result)).position = ((Rect)(ref result)).position + ((Rect)(ref damageGraphicData.rectN)).position;
				((Rect)(ref result)).size = ((Rect)(ref damageGraphicData.rectN)).size;
			}
			else if (b.Rotation == Rot4.East && damageGraphicData.rectE != default(Rect))
			{
				((Rect)(ref result)).position = ((Rect)(ref result)).position + ((Rect)(ref damageGraphicData.rectE)).position;
				((Rect)(ref result)).size = ((Rect)(ref damageGraphicData.rectE)).size;
			}
			else if (b.Rotation == Rot4.South && damageGraphicData.rectS != default(Rect))
			{
				((Rect)(ref result)).position = ((Rect)(ref result)).position + ((Rect)(ref damageGraphicData.rectS)).position;
				((Rect)(ref result)).size = ((Rect)(ref damageGraphicData.rectS)).size;
			}
			else if (b.Rotation == Rot4.West && damageGraphicData.rectW != default(Rect))
			{
				((Rect)(ref result)).position = ((Rect)(ref result)).position + ((Rect)(ref damageGraphicData.rectW)).position;
				((Rect)(ref result)).size = ((Rect)(ref damageGraphicData.rectW)).size;
			}
			else if (damageGraphicData.rect != default(Rect))
			{
				Rect rect = damageGraphicData.rect;
				if (b.Rotation == Rot4.North)
				{
					((Rect)(ref result)).x = ((Rect)(ref result)).x + ((Rect)(ref rect)).x;
					((Rect)(ref result)).y = ((Rect)(ref result)).y + ((Rect)(ref rect)).y;
					((Rect)(ref result)).width = ((Rect)(ref rect)).width;
					((Rect)(ref result)).height = ((Rect)(ref rect)).height;
				}
				else if (b.Rotation == Rot4.South)
				{
					((Rect)(ref result)).x = ((Rect)(ref result)).x + ((float)cellRect.Width - ((Rect)(ref rect)).x - ((Rect)(ref rect)).width);
					((Rect)(ref result)).y = ((Rect)(ref result)).y + ((float)cellRect.Height - ((Rect)(ref rect)).y - ((Rect)(ref rect)).height);
					((Rect)(ref result)).width = ((Rect)(ref rect)).width;
					((Rect)(ref result)).height = ((Rect)(ref rect)).height;
				}
				else if (b.Rotation == Rot4.West)
				{
					((Rect)(ref result)).x = ((Rect)(ref result)).x + ((float)cellRect.Width - ((Rect)(ref rect)).y - ((Rect)(ref rect)).height);
					((Rect)(ref result)).y = ((Rect)(ref result)).y + ((Rect)(ref rect)).x;
					((Rect)(ref result)).width = ((Rect)(ref rect)).height;
					((Rect)(ref result)).height = ((Rect)(ref rect)).width;
				}
				else if (b.Rotation == Rot4.East)
				{
					((Rect)(ref result)).x = ((Rect)(ref result)).x + ((Rect)(ref rect)).y;
					((Rect)(ref result)).y = ((Rect)(ref result)).y + ((float)cellRect.Height - ((Rect)(ref rect)).x - ((Rect)(ref rect)).width);
					((Rect)(ref result)).width = ((Rect)(ref rect)).height;
					((Rect)(ref result)).height = ((Rect)(ref rect)).width;
				}
			}
		}
		return result;
	}

	private static int GetDamageOverlaysCount(Building b, int hp)
	{
		float num = (float)hp / (float)b.MaxHitPoints;
		int count = GetAvailableOverlays(b).Count;
		return count - Mathf.FloorToInt((float)count * num);
	}

	private static bool DifferentAt(Building b, int x, int z)
	{
		IntVec3 c = new IntVec3(x, 0, z);
		if (!c.InBounds(b.Map))
		{
			return true;
		}
		List<Thing> thingList = c.GetThingList(b.Map);
		for (int i = 0; i < thingList.Count; i++)
		{
			if (thingList[i].def == b.def)
			{
				return false;
			}
		}
		return true;
	}

	private static bool SameAndDamagedAt(Building b, int x, int z)
	{
		IntVec3 c = new IntVec3(x, 0, z);
		if (!c.InBounds(b.Map))
		{
			return false;
		}
		List<Thing> thingList = c.GetThingList(b.Map);
		for (int i = 0; i < thingList.Count; i++)
		{
			if (thingList[i].def == b.def && thingList[i].HitPoints < thingList[i].MaxHitPoints)
			{
				return true;
			}
		}
		return false;
	}

	public static void DebugDraw()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (Prefs.DevMode && DebugViewSettings.drawDamageRects && Find.CurrentMap != null && Find.Selector.FirstSelectedObject is Building b)
		{
			Material val = DebugSolidColorMats.MaterialOf(Color.red);
			Rect damageRect = GetDamageRect(b);
			float num = 14.99f;
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(((Rect)(ref damageRect)).x + ((Rect)(ref damageRect)).width / 2f, num, ((Rect)(ref damageRect)).y + ((Rect)(ref damageRect)).height / 2f);
			Vector3 val3 = default(Vector3);
			((Vector3)(ref val3))._002Ector(((Rect)(ref damageRect)).width, 1f, ((Rect)(ref damageRect)).height);
			Graphics.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(val2, Quaternion.identity, val3), val, 0);
		}
	}
}
