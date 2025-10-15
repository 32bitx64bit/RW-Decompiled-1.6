using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI;

namespace Verse;

public static class DebugToolsMisc
{
	private static List<WeaponTraitDef> tmpTraitDefs = new List<WeaponTraitDef>();

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
	private static void AttachFire()
	{
		using List<Thing>.Enumerator enumerator = Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList().GetEnumerator();
		if (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (current.CanEverAttachFire())
			{
				current.TryAttachFire(1f, null);
			}
			else
			{
				FireUtility.TryStartFireIn(UI.MouseCell(), Find.CurrentMap, 1.75f, null);
			}
		}
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
	private static DebugActionNode SetStuff()
	{
		DebugActionNode debugActionNode = new DebugActionNode();
		foreach (ThingDef stuff in DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.IsStuff))
		{
			ThingDef stuffInner = stuff;
			debugActionNode.AddChild(new DebugActionNode(stuff.LabelCap, DebugActionType.ToolMap, delegate
			{
				foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
				{
					if (item.Stuff != null)
					{
						float num = (float)item.HitPoints / (float)item.MaxHitPoints;
						item.SetStuffDirect(stuffInner);
						StatDefOf.MaxHitPoints.Worker.ClearCacheForThing(item);
						item.HitPoints = Mathf.CeilToInt((float)item.MaxHitPoints * num);
						item.Notify_ColorChanged();
						item.DirtyMapMesh(item.Map);
						if (item is UnfinishedThing unfinishedThing)
						{
							int num2 = 0;
							int num3 = 100;
							foreach (Thing ingredient in unfinishedThing.ingredients)
							{
								num2 += ingredient.stackCount;
							}
							unfinishedThing.ingredients.Clear();
							while (num2 > 0 && num3-- > 0)
							{
								Thing thing = ThingMaker.MakeThing(stuff);
								thing.stackCount = Mathf.Min(num2, stuff.stackLimit);
								unfinishedThing.ingredients.Add(thing);
								num2 -= thing.stackCount;
							}
						}
					}
				}
			}));
		}
		return debugActionNode;
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
	private static DebugActionNode SetQuality()
	{
		DebugActionNode debugActionNode = new DebugActionNode();
		foreach (QualityCategory value in Enum.GetValues(typeof(QualityCategory)))
		{
			QualityCategory qualityInner = value;
			debugActionNode.AddChild(new DebugActionNode(qualityInner.ToString(), DebugActionType.ToolMap, delegate
			{
				foreach (Thing thing in UI.MouseCell().GetThingList(Find.CurrentMap))
				{
					thing.TryGetComp<CompQuality>()?.SetQuality(qualityInner, ArtGenerationContext.Outsider);
				}
			}));
		}
		return debugActionNode;
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap)]
	public static void MeasureDrawSize()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 first = Vector3.zero;
		DebugTools.curMeasureTool = new DrawMeasureTool("first corner...", delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			first = UI.MouseMapPosition();
			DebugTools.curMeasureTool = new DrawMeasureTool("second corner...", delegate
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0137: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				Vector3 val = UI.MouseMapPosition();
				Rect val2 = default(Rect);
				((Rect)(ref val2)).xMin = Mathf.Min(first.x, val.x);
				((Rect)(ref val2)).yMin = Mathf.Min(first.z, val.z);
				((Rect)(ref val2)).xMax = Mathf.Max(first.x, val.x);
				((Rect)(ref val2)).yMax = Mathf.Max(first.z, val.z);
				string text = $"Center: ({((Rect)(ref val2)).center.x},{((Rect)(ref val2)).center.y})";
				text += $"\nSize: ({((Rect)(ref val2)).size.x},{((Rect)(ref val2)).size.y})";
				if (Find.Selector.SingleSelectedObject != null)
				{
					Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
					Vector3 drawPos = singleSelectedThing.DrawPos;
					Vector2 val3 = ((Rect)(ref val2)).center - new Vector2(drawPos.x, drawPos.z);
					text += $"\nOffset: ({val3.x},{val3.y})";
					Vector2 val4 = val3.RotatedBy(0f - singleSelectedThing.Rotation.AsAngle);
					text += $"\nUnrotated offset: ({val4.x},{val4.y})";
				}
				Log.Message(text);
				MeasureDrawSize();
			}, first);
		});
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap)]
	public static void MeasureWorldDistance()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 first = Vector3.zero;
		DebugTools.curMeasureTool = new MeasureWorldDistanceTool("First Point...", delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			first = UI.MouseMapPosition();
			DebugTools.curMeasureTool = new MeasureWorldDistanceTool("Second Point...", delegate
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				Vector3 val = UI.MouseMapPosition() - first;
				Log.Message($"Vector: {val}, Distance:{((Vector3)(ref val)).magnitude}");
				MeasureWorldDistance();
			}, first);
		});
	}

	[DebugAction("General", "Draw Attach Points", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap)]
	public static void DrawAttachPoints()
	{
		foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
		{
			if (!(item is ThingWithComps thingWithComps))
			{
				continue;
			}
			CompAttachPoints comp = thingWithComps.GetComp<CompAttachPoints>();
			if (comp == null)
			{
				continue;
			}
			int ttl = 500;
			DebugTools.curTool = new DebugTool("attach point drawer", delegate
			{
			}, delegate
			{
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				ttl--;
				if (ttl <= 0)
				{
					DebugTools.curTool = null;
				}
				foreach (AttachPointType item2 in comp.points.PointTypes())
				{
					Vector3 worldPos = comp.points.GetWorldPos(item2);
					GenMapUI.DrawText(new Vector2(worldPos.x, worldPos.z), item2.ToString(), Color.red);
					GenDraw.DrawCircleOutline(worldPos, 0.4f);
				}
			});
		}
	}

	[DebugAction("General", "Pollution +1%", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolWorld, allowedGameStates = AllowedGameStates.PlayingOnWorld, requiresBiotech = true)]
	private static void IncreasePollutionSmall()
	{
		PlanetTile root = GenWorld.MouseTile();
		if (root.Valid)
		{
			WorldPollutionUtility.PolluteWorldAtTile(root, 0.01f);
		}
	}

	[DebugAction("General", "Pollution +25%", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolWorld, allowedGameStates = AllowedGameStates.PlayingOnWorld, requiresBiotech = true)]
	private static void IncreasePollutionLarge()
	{
		PlanetTile root = GenWorld.MouseTile();
		if (root.Valid)
		{
			WorldPollutionUtility.PolluteWorldAtTile(root, 0.25f);
		}
	}

	[DebugAction("General", "Pollution -25%", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolWorld, allowedGameStates = AllowedGameStates.PlayingOnWorld, requiresBiotech = true)]
	private static void DecreasePollutionLarge()
	{
		PlanetTile root = GenWorld.MouseTile();
		if (root.Valid)
		{
			WorldPollutionUtility.PolluteWorldAtTile(root, -0.25f);
		}
	}

	[DebugAction("General", "Set biome", false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnWorld)]
	private static List<DebugActionNode> SetBiome()
	{
		List<DebugActionNode> list = new List<DebugActionNode>();
		foreach (BiomeDef item in DefDatabase<BiomeDef>.AllDefsListForReading)
		{
			BiomeDef local = item;
			if (!item.implemented || !item.generatesNaturally)
			{
				continue;
			}
			list.Add(new DebugActionNode(item.label, DebugActionType.ToolWorld, delegate
			{
				PlanetTile planetTile = GenWorld.MouseTile();
				if (planetTile.Valid && planetTile.Tile.PrimaryBiome != local)
				{
					planetTile.Tile.PrimaryBiome = local;
					Find.World.renderer.GetLayer<WorldDrawLayer_Terrain>(PlanetLayer.Selected).RegenerateNow();
				}
			}));
		}
		return list;
	}

	[DebugAction("General", "Set landmark", false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnWorld, requiresOdyssey = true)]
	private static List<DebugActionNode> SetLandmark()
	{
		List<DebugActionNode> list = new List<DebugActionNode>();
		foreach (LandmarkDef item in DefDatabase<LandmarkDef>.AllDefsListForReading)
		{
			LandmarkDef local = item;
			list.Add(new DebugActionNode(item.defName, DebugActionType.ToolWorld, delegate
			{
				PlanetTile tile = GenWorld.MouseTile();
				if (tile.Valid && local.IsValidTile(tile, tile.Layer))
				{
					Find.World.landmarks.AddLandmark(local, tile);
					Find.World.renderer.GetLayer<WorldDrawLayer_Terrain>(tile.Layer).RegenerateNow();
					Find.World.renderer.GetLayer<WorldDrawLayer_Landmarks>(Find.WorldGrid.Surface).RegenerateNow();
					Find.World.renderer.GetLayer<WorldDrawLayer_Hills>(Find.WorldGrid.Surface).RegenerateNow();
				}
			}));
		}
		return list;
	}

	[DebugAction("General", "Clear Landmark", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolWorld, allowedGameStates = AllowedGameStates.PlayingOnWorld, requiresOdyssey = true)]
	private static void ClearLandmark()
	{
		PlanetTile tile = GenWorld.MouseTile();
		if (tile.Valid)
		{
			Find.World.landmarks.RemoveLandmark(tile);
			Find.World.renderer.GetLayer<WorldDrawLayer_Terrain>(tile.Layer).RegenerateNow();
		}
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, requiresBiotech = true)]
	private static void ResetBossgroupCooldown()
	{
		Find.BossgroupManager.lastBossgroupCalled = Find.TickManager.TicksGame - 120000;
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, requiresBiotech = true)]
	private static void ResetBossgroupKilledPawns()
	{
		Find.BossgroupManager.DebugResetDefeatedPawns();
	}

	[DebugAction("Insect", "Spawn cocoon infestation", false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, hideInSubMenu = true, requiresBiotech = true)]
	private static List<DebugActionNode> SpawnCocoonInfestationWithPoints()
	{
		List<DebugActionNode> list = new List<DebugActionNode>();
		foreach (float item2 in DebugActionsUtility.PointsOptions(extended: false))
		{
			float localP = item2;
			DebugActionNode item = new DebugActionNode(localP + " points", DebugActionType.ToolMap, delegate
			{
				CocoonInfestationUtility.SpawnCocoonInfestation(UI.MouseCell(), Find.CurrentMap, localP);
			});
			list.Add(item);
		}
		return list;
	}

	[DebugAction("Anomaly", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolWorld, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, requiresAnomaly = true)]
	private static void SpawnPitGate()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 loc = UI.MouseMapPosition().ToIntVec3();
		GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.PitGateSpawner), loc, Find.CurrentMap);
	}

	[DebugAction("Anomaly", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolWorld, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, requiresAnomaly = true)]
	private static void SpawnPitBurrow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		FleshbeastUtility.SpawnFleshbeastsFromPitBurrowEmergence(UI.MouseMapPosition().ToIntVec3(), Find.CurrentMap, 200f, new IntRange(600, 600), new IntRange(60, 180));
	}

	[DebugAction("Anomaly", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolWorld, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, requiresAnomaly = true)]
	private static void SpawnFleshmassHeart()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 loc = UI.MouseMapPosition().ToIntVec3();
		GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.FleshmassHeartSpawner), loc, Find.CurrentMap);
	}

	[DebugAction("Anomaly", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresAnomaly = true)]
	private static void DiscoverAllEntities()
	{
		Find.EntityCodex.Debug_DiscoverAll();
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action)]
	private static void BenchmarkPerformance()
	{
		Messages.Message($"Running benchmark, results displayed in {30f} seconds", MessageTypeDefOf.NeutralEvent, historical: false);
		PerformanceBenchmarkUtility.StartBenchmark();
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
	private static void CompareLineOfSight(Pawn pawn)
	{
		foreach (IntVec3 item in GenRadial.RadialCellsAround(UI.MouseCell(), 50f, useCenter: true))
		{
			if (!item.InBounds(Find.CurrentMap))
			{
				continue;
			}
			Pawn firstPawn = item.GetFirstPawn(Find.CurrentMap);
			if (firstPawn != null)
			{
				bool num = pawn.CanSee(firstPawn);
				bool flag = firstPawn.CanSee(pawn);
				if (num != flag)
				{
					Find.CurrentMap.debugDrawer.FlashCell(item, 1f);
				}
			}
		}
	}

	[DebugAction("General", "Enable wound debug draw", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap, hideInSubMenu = true)]
	private static void WoundDebug()
	{
		IntVec3 c = UI.MouseCell();
		Pawn pawn = c.GetFirstPawn(Find.CurrentMap);
		if (pawn == null || pawn.def.race == null || pawn.def.race.body == null)
		{
			return;
		}
		List<DebugMenuOption> list = new List<DebugMenuOption>();
		list.Add(new DebugMenuOption("All", DebugMenuOptionMode.Action, delegate
		{
			pawn.Drawer.renderer.WoundOverlays.debugDrawAllParts = true;
			pawn.Drawer.renderer.WoundOverlays.ClearCache();
			PortraitsCache.SetDirty(pawn);
			GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
		}));
		List<BodyPartRecord> allParts = pawn.def.race.body.AllParts;
		for (int i = 0; i < allParts.Count; i++)
		{
			BodyPartRecord part = allParts[i];
			list.Add(new DebugMenuOption(part.LabelCap, DebugMenuOptionMode.Action, delegate
			{
				pawn.Drawer.renderer.WoundOverlays.debugDrawPart = part;
				pawn.Drawer.renderer.WoundOverlays.ClearCache();
				PortraitsCache.SetDirty(pawn);
				GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
			}));
		}
		Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
	}

	[DebugAction("General", "Wound debug export (non-humanlike)", false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap, hideInSubMenu = true)]
	private static void WoundDebugExport()
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		string text = Application.dataPath + "\\woundDump";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		HashSet<RaceProperties> hashSet = new HashSet<RaceProperties>();
		foreach (PawnKindDef item in DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef pkd) => !pkd.RaceProps.Humanlike))
		{
			if (!hashSet.Contains(item.RaceProps))
			{
				Pawn pawn = PawnGenerator.GeneratePawn(item);
				for (int i = 0; i < 4; i++)
				{
					Rot4 rot = new Rot4((byte)i);
					RenderTexture temporary = RenderTexture.GetTemporary(256, 256, 32, (RenderTextureFormat)0);
					((Object)temporary).name = "WoundDebugExport";
					pawn.Drawer.renderer.WoundOverlays.debugDrawAllParts = true;
					pawn.Drawer.renderer.WoundOverlays.ClearCache();
					Find.PawnCacheRenderer.RenderPawn(pawn, temporary, Vector3.zero, 1f, 0f, rot);
					pawn.Drawer.renderer.WoundOverlays.debugDrawAllParts = false;
					pawn.Drawer.renderer.WoundOverlays.ClearCache();
					Texture2D val = new Texture2D(((Texture)temporary).width, ((Texture)temporary).height, (TextureFormat)5, 0, false);
					RenderTexture.active = temporary;
					val.ReadPixels(new Rect(0f, 0f, (float)((Texture)temporary).width, (float)((Texture)temporary).height), 0, 0, true);
					RenderTexture.active = null;
					RenderTexture.ReleaseTemporary(temporary);
					string text2 = text + "\\" + pawn.def.LabelCap + "_";
					Rot4 rot2 = rot;
					File.WriteAllBytes(text2 + rot2.ToString() + ".png", ImageConversion.EncodeToPNG(val));
				}
				pawn.Destroy();
				hashSet.Add(item.RaceProps);
			}
		}
		Log.Message("Dumped to " + text);
	}

	[DebugAction("Anomaly", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, requiresAnomaly = true)]
	private static List<DebugActionNode> EmergeMetalhorrors()
	{
		List<DebugActionNode> list = new List<DebugActionNode>();
		for (int i = 1; i <= 10; i++)
		{
			int delaySeconds = i;
			list.Add(new DebugActionNode($"{i}s delay")
			{
				action = delegate
				{
					int delayTicks = delaySeconds * 60;
					DelayedMetalhorrorEmerger.Spawn(Find.CurrentMap, delayTicks);
				}
			});
		}
		return list;
	}

	[DebugAction("Mechanoid", null, false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.IsCurrentlyOnMap, requiresOdyssey = true)]
	private static void DisableMechs()
	{
		if (Faction.OfMechanoids != null)
		{
			Faction.OfMechanoids.deactivated = true;
		}
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresOdyssey = true)]
	private static void AddTraitToUniqueWeapon()
	{
		CompUniqueWeapon comp = Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).FirstOrDefault((Thing x) => x.HasComp<CompUniqueWeapon>()).TryGetComp<CompUniqueWeapon>();
		if (comp == null)
		{
			return;
		}
		tmpTraitDefs.Clear();
		tmpTraitDefs.AddRange(DefDatabase<WeaponTraitDef>.AllDefs.Where((WeaponTraitDef trait) => comp.CanAddTrait(trait)));
		if (!tmpTraitDefs.Any())
		{
			return;
		}
		List<DebugMenuOption> list = new List<DebugMenuOption>();
		foreach (WeaponTraitDef tmpTraitDef in tmpTraitDefs)
		{
			WeaponTraitDef localTrait = tmpTraitDef;
			list.Add(new DebugMenuOption(tmpTraitDef.LabelCap, DebugMenuOptionMode.Action, delegate
			{
				comp.AddTrait(localTrait);
				comp.Setup(fromSave: true);
			}));
		}
		Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
	}

	[DebugAction("General", null, false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresOdyssey = true)]
	private static void RemoveTraitFromUniqueWeapon()
	{
		CompUniqueWeapon comp = Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).FirstOrDefault((Thing x) => x.HasComp<CompUniqueWeapon>()).TryGetComp<CompUniqueWeapon>();
		if (comp == null)
		{
			return;
		}
		List<DebugMenuOption> list = new List<DebugMenuOption>();
		foreach (WeaponTraitDef item in comp.TraitsListForReading)
		{
			WeaponTraitDef localTrait = item;
			list.Add(new DebugMenuOption(item.LabelCap, DebugMenuOptionMode.Action, delegate
			{
				comp.TraitsListForReading.Remove(localTrait);
			}));
		}
		Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
	}
}
