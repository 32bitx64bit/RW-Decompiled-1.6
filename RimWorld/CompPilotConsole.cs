using System.Collections.Generic;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompPilotConsole : CompGravshipFacility
{
	[Unsaved(false)]
	private CompBreakdownable breakdownableComp;

	public static readonly Material ThrusterRadiusMat = MaterialPool.MatFrom(GenDraw.OneSidedLineTexPath, ShaderDatabase.WorldOverlayTransparent, ColorLibrary.White, 3590);

	public static readonly Material FuelRadiusMat = MaterialPool.MatFrom(GenDraw.OneSidedLineTexPath, ShaderDatabase.WorldOverlayTransparent, ColorLibrary.Yellow, 3590);

	public static readonly Material ThrusterRadiusMatHighVis = MaterialPool.MatFrom(GenDraw.OneSidedLineOpaqueTexPath, ShaderDatabase.WorldOverlayAdditiveTwoSided, ColorLibrary.White, 3590);

	public static readonly Material FuelRadiusMatHighVis = MaterialPool.MatFrom(GenDraw.OneSidedLineOpaqueTexPath, ShaderDatabase.WorldOverlayAdditiveTwoSided, ColorLibrary.Yellow, 3590);

	private static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment");

	private static readonly StringBuilder cannotPlaceTileReason = new StringBuilder();

	private static readonly CachedTexture ViewRangeTex = new CachedTexture("UI/Commands/ViewRange");

	private CompBreakdownable Breakdownable => breakdownableComp ?? (breakdownableComp = parent.GetComp<CompBreakdownable>());

	private int GetMaxLaunchDistance(PlanetLayer layer)
	{
		return Mathf.FloorToInt(((float?)engine?.MaxLaunchDistance / layer.Def.rangeDistanceFactor).GetValueOrDefault());
	}

	public static Material GetThrusterRadiusMat(PlanetTile tile)
	{
		if (!tile.LayerDef.isSpace)
		{
			return ThrusterRadiusMat;
		}
		return ThrusterRadiusMatHighVis;
	}

	public static Material GetFuelRadiusMat(PlanetTile tile)
	{
		if (!tile.LayerDef.isSpace)
		{
			return FuelRadiusMat;
		}
		return FuelRadiusMatHighVis;
	}

	public void StartChoosingDestination()
	{
		StartChoosingDestination_NewTemp();
	}

	public void StartChoosingDestination_NewTemp(bool launching = true)
	{
		if (engine == null)
		{
			return;
		}
		if (launching && DebugSettings.skipGravshipTileSelection)
		{
			Find.GravshipController.InitiateTakeoff(engine, default(PlanetTile));
			return;
		}
		CameraJumper.TryJump(CameraJumper.GetWorldTarget(parent));
		Find.WorldSelector.ClearSelection();
		PlanetTile curTile = parent.Map.Tile;
		PlanetLayer curLayer = parent.Map.Tile.Layer;
		PlanetTile cachedClosestLayerTile = PlanetTile.Invalid;
		Find.TilePicker.StartTargeting_NewTemp(delegate(PlanetTile tile)
		{
			cannotPlaceTileReason.Clear();
			if (!GravshipUtility.TryGetPathFuelCost(curTile, tile, out var cost2, out var distance2, 10f, engine.FuelUseageFactor) && !DebugSettings.ignoreGravshipRange)
			{
				Messages.Message("CannotLaunchDestination".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (!engine.HasSignalJammer && Find.WorldObjects.TryGetWorldObjectAt<MapParent>(tile, out var wo) && wo.RequiresSignalJammerToReach)
			{
				Messages.Message("TransportPodDestinationRequiresSignalJammer".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (cost2 > engine.TotalFuel && !DebugSettings.ignoreGravshipRange)
			{
				Messages.Message("CannotLaunchNotEnoughFuel".Translate().CapitalizeFirst(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (distance2 > GetMaxLaunchDistance(tile.Layer) && !DebugSettings.ignoreGravshipRange)
			{
				Messages.Message("TransportPodDestinationBeyondMaximumRange".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (tile == parent.Tile && !parent.Map.listerThings.AnyThingWithDef(ThingDefOf.GravAnchor))
			{
				Messages.Message("CannotLandOnSameTile".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			MapParent mapParent = Find.World.worldObjects.MapParentAt(tile);
			if (mapParent != null && mapParent.HasMap)
			{
				return true;
			}
			if (!TileFinder.IsValidTileForNewSettlement(tile, cannotPlaceTileReason, forGravship: true))
			{
				Messages.Message(cannotPlaceTileReason.ToString(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return true;
		}, delegate(PlanetTile tile)
		{
			if (launching)
			{
				SettlementProximityGoodwillUtility.CheckConfirmSettle(tile, delegate
				{
					WorldComponent_GravshipController.DestroyTreesAroundSubstructure(engine.Map, engine.ValidSubstructure);
					Find.World.renderer.wantedMode = WorldRenderMode.None;
					engine.ConsumeFuel(tile);
					Find.GravshipController.InitiateTakeoff(engine, tile);
					SoundDefOf.Gravship_Launch.PlayOneShotOnCamera();
				}, delegate
				{
					StartChoosingDestination_NewTemp();
				}, engine);
			}
		}, delegate
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			WorldObject singleSelectedObject = Find.WorldSelector.SingleSelectedObject;
			PlanetTile planetTile2 = GenWorld.MouseTile();
			PlanetTile planetTile3 = ((!planetTile2.Valid && singleSelectedObject != null) ? singleSelectedObject.Tile : planetTile2);
			Vector2 mousePosition = Event.current.mousePosition;
			GUI.DrawTexture(new Rect(mousePosition.x + 8f, mousePosition.y + 8f, 32f, 32f), (Texture)(object)TargeterMouseAttachment);
			if (planetTile3.Valid)
			{
				bool flag = false;
				int maxLaunchDistance2 = GetMaxLaunchDistance(PlanetLayer.Selected);
				PlanetTile tileA = ((curTile.Layer == planetTile3.Layer) ? curTile : planetTile3.Layer.GetClosestTile_NewTemp(curTile));
				string text;
				float cost;
				int distance;
				if (Find.WorldGrid.ApproxDistanceInTiles(tileA, planetTile3) > (float)(maxLaunchDistance2 + 10))
				{
					text = string.Format(" ({0})", "TransportPodDestinationBeyondMaximumRange".Translate());
				}
				else if (GravshipUtility.TryGetPathFuelCost(curTile, planetTile3, out cost, out distance, 10f, engine.FuelUseageFactor))
				{
					flag = cost <= engine.TotalFuel && distance <= maxLaunchDistance2;
					text = string.Format("{0}: {1}", "Cost".Translate().CapitalizeFirst(), "FuelAmount".Translate(cost, ThingDefOf.Chemfuel));
					if (distance > maxLaunchDistance2)
					{
						text += string.Format(" ({0})", "TransportPodDestinationBeyondMaximumRange".Translate());
					}
					else if (!flag)
					{
						text += string.Format(" ({0})", "CannotLaunchNotEnoughFuel".Translate().CapitalizeFirst());
					}
					else if (!engine.HasSignalJammer && singleSelectedObject is MapParent && singleSelectedObject.RequiresSignalJammerToReach)
					{
						flag = false;
						text += string.Format(" ({0})", "TransportPodDestinationRequiresSignalJammer".Translate());
					}
				}
				else
				{
					text = "CannotLaunchDestination".Translate();
				}
				if (singleSelectedObject != null && !planetTile2.Valid)
				{
					Widgets.WorldAttachedLabel(singleSelectedObject.DrawPos, text, 0f, 0f, flag ? Color.white : ColorLibrary.RedReadable);
				}
				else
				{
					Widgets.MouseAttachedLabel(text, 0f, 0f, flag ? Color.white : ColorLibrary.RedReadable);
				}
			}
		}, delegate
		{
			int maxLaunchDistance = GetMaxLaunchDistance(PlanetLayer.Selected);
			int num = GravshipUtility.MaxDistForFuel(engine.TotalFuel, curLayer, PlanetLayer.Selected, 10f, engine.FuelUseageFactor);
			PlanetTile planetTile = curTile;
			if (curTile.Layer != Find.WorldSelector.SelectedLayer)
			{
				if (cachedClosestLayerTile.Layer != Find.WorldSelector.SelectedLayer || !cachedClosestLayerTile.Valid)
				{
					cachedClosestLayerTile = Find.WorldSelector.SelectedLayer.GetClosestTile_NewTemp(curTile);
				}
				planetTile = cachedClosestLayerTile;
			}
			GenDraw.DrawWorldRadiusRing(planetTile, maxLaunchDistance, GetThrusterRadiusMat(planetTile));
			if (num < maxLaunchDistance)
			{
				GenDraw.DrawWorldRadiusRing(planetTile, num, GetFuelRadiusMat(planetTile));
			}
		}, allowEscape: true, delegate
		{
			CameraJumper.TryJump(parent, CameraJumper.MovementMode.Cut);
		}, "ChooseWhereToLand".Translate(), showRandomButton: false, selectTileBehindObject: true, hideFormCaravanGizmo: true, noTileChosenMessage: "MessageNoLandingSiteSelected".Translate(), showNextButton: launching, canCancel: true);
	}

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		if (!parent.Spawned || engine == null)
		{
			yield break;
		}
		yield return new Command_Action
		{
			defaultLabel = "ViewRange".Translate(),
			defaultDesc = "ViewRangeDesc".Translate(),
			icon = (Texture)(object)ViewRangeTex.Texture,
			action = delegate
			{
				StartChoosingDestination_NewTemp(launching: false);
			}
		};
		if (!DebugSettings.ShowDevGizmos)
		{
			yield break;
		}
		yield return new Command_Action
		{
			defaultLabel = "DEV: Launch instantly",
			action = delegate
			{
				engine.launchInfo = new LaunchInfo
				{
					quality = 1f,
					doNegativeOutcome = false
				};
				StartChoosingDestination_NewTemp();
			}
		};
		if (engine.cooldownCompleteTick > GenTicks.TicksGame)
		{
			yield return new Command_Action
			{
				defaultLabel = "DEV: Reset cooldown",
				action = delegate
				{
					engine.cooldownCompleteTick = -1;
				}
			};
		}
	}

	public AcceptanceReport CanUseNow()
	{
		if (engine == null)
		{
			return new AcceptanceReport("CannotLaunchNoEngine".Translate().CapitalizeFirst());
		}
		if (Breakdownable.BrokenDown)
		{
			return new AcceptanceReport("BrokenDown".Translate().CapitalizeFirst());
		}
		if (!engine.ValidSubstructureAt(parent.InteractionCell))
		{
			return new AcceptanceReport("PilotConsoleInaccessible".Translate().CapitalizeFirst());
		}
		AcceptanceReport result = engine.CanLaunch(this);
		if (!result.Accepted)
		{
			return result;
		}
		return AcceptanceReport.WasAccepted;
	}

	public override string CompInspectStringExtra()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(base.CompInspectStringExtra());
		if (engine != null)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append("GravshipName".Translate().CapitalizeFirst()).Append(": ").Append(engine.RenamableLabel);
			stringBuilder.AppendInNewLine("GravshipRange".Translate().CapitalizeFirst()).Append(": ").Append(GetMaxLaunchDistance(parent.Map.Tile.Layer).ToString("F0"));
			foreach (CompGravshipFacility gravshipComponent in engine.GravshipComponents)
			{
				if (gravshipComponent is CompGravshipThruster { Blocked: not false })
				{
					stringBuilder.Append(" (").Append("ThrusterBlocked".Translate()).Append(")");
					break;
				}
			}
			stringBuilder.AppendInNewLine("StoredChemfuel".Translate().CapitalizeFirst()).Append(": ").Append(engine.TotalFuel.ToString("F0"))
				.Append(" / ")
				.Append(engine.MaxFuel.ToString("F0"));
			stringBuilder.AppendInNewLine("FuelConsumption".Translate().CapitalizeFirst()).Append(": ").Append("FuelPerTile".Translate(engine.FuelPerTile.ToString("0.#")).CapitalizeFirst());
			if (GenTicks.TicksGame < engine.cooldownCompleteTick)
			{
				stringBuilder.AppendInNewLine("CannotLaunchOnCooldown".Translate((engine.cooldownCompleteTick - GenTicks.TicksGame).ToStringTicksToPeriod()).CapitalizeFirst());
			}
		}
		return stringBuilder.ToString();
	}

	private AcceptanceReport ValidateNavigator(LocalTargetInfo target)
	{
		if (!(target.Thing is Pawn pawn))
		{
			return false;
		}
		if (!pawn.IsColonistPlayerControlled)
		{
			return false;
		}
		if (pawn.Downed || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
		{
			return "Incapable".Translate();
		}
		if (pawn.skills == null || pawn.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
		{
			return "Incapable".Translate();
		}
		return true;
	}

	public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
	{
		AcceptanceReport acceptanceReport = CanUseNow();
		if (!acceptanceReport.Accepted)
		{
			yield return new FloatMenuOption("CannotChooseNavigator".Translate() + ": " + acceptanceReport.Reason.CapitalizeFirst(), null);
		}
		else if (selPawn.skills == null || selPawn.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
		{
			yield return new FloatMenuOption("CannotChooseNavigator".Translate() + ": " + "IncapableOfCapacity".Translate(SkillDefOf.Intellectual.label).CapitalizeFirst(), null);
		}
		else if (!selPawn.CanReach(parent, PathEndMode.InteractionCell, Danger.Deadly))
		{
			yield return new FloatMenuOption("CannotChooseNavigator".Translate() + ": " + "NoPath".Translate(), null);
		}
		else if ((bool)ValidateNavigator(selPawn))
		{
			yield return new FloatMenuOption("PilotGravship".Translate(), delegate
			{
				selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.PilotConsole, parent), JobTag.Misc);
			});
		}
	}
}
