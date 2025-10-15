using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public abstract class Designator_Place : DesignatorWithEyedropper
{
	protected Rot4 placingRot = Rot4.North;

	protected static float middleMouseDownTime;

	public static readonly Color CanPlaceColor = new Color(0.5f, 1f, 0.6f, 0.4f);

	public static readonly Color CannotPlaceColor = new Color(1f, 0f, 0f, 0.4f);

	public static readonly Vector2 PlaceMouseAttachmentDrawOffset = new Vector2(19f, 17f);

	private static List<Thing> tmpThings = new List<Thing>();

	public abstract BuildableDef PlacingDef { get; }

	public abstract ThingStyleDef ThingStyleDefForPreview { get; }

	public abstract ThingDef StuffDef { get; }

	public override DrawStyleCategoryDef DrawStyleCategory => PlacingDef.drawStyleCategory;

	public Designator_Place()
	{
		soundDragSustain = SoundDefOf.Designate_DragBuilding;
		soundDragChanged = null;
		soundSucceeded = SoundDefOf.Designate_PlaceBuilding;
	}

	public override void DrawMouseAttachments()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		base.DrawMouseAttachments();
		Map currentMap = Find.CurrentMap;
		currentMap.deepResourceGrid.DrawPlacingMouseAttachments(PlacingDef);
		Vector2 val = Event.current.mousePosition + PlaceMouseAttachmentDrawOffset;
		float x = val.x;
		float curY = val.y;
		DrawPlaceMouseAttachments(x, ref curY);
		if (PlacingDef.PlaceWorkers != null)
		{
			foreach (PlaceWorker placeWorker in PlacingDef.PlaceWorkers)
			{
				placeWorker.DrawPlaceMouseAttachments(x, ref curY, PlacingDef, UI.MouseCell(), placingRot);
			}
			foreach (PlaceWorker placeWorker2 in PlacingDef.PlaceWorkers)
			{
				placeWorker2.DrawMouseAttachments(PlacingDef);
			}
		}
		if (currentMap == null || !(PlacingDef is ThingDef thingDef) || thingDef.displayNumbersBetweenSameDefDistRange.max <= 0f)
		{
			return;
		}
		IntVec3 intVec = UI.MouseCell();
		tmpThings.Clear();
		tmpThings.AddRange(currentMap.listerThings.ThingsOfDef(thingDef));
		tmpThings.AddRange(currentMap.listerThings.ThingsInGroup(ThingRequestGroup.Blueprint));
		foreach (Thing tmpThing in tmpThings)
		{
			if ((tmpThing.def == thingDef || tmpThing.def.entityDefToBuild == thingDef) && (tmpThing.Position.x == intVec.x || tmpThing.Position.z == intVec.z) && CanDrawNumbersBetween(tmpThing, thingDef, intVec, tmpThing.Position, currentMap))
			{
				IntVec3 intVec2 = tmpThing.Position - intVec;
				intVec2.x = Mathf.Abs(intVec2.x) + 1;
				intVec2.z = Mathf.Abs(intVec2.z) + 1;
				if (intVec2.x >= 3)
				{
					Vector2 screenPos = (tmpThing.Position.ToUIPosition() + intVec.ToUIPosition()) / 2f;
					screenPos.y = tmpThing.Position.ToUIPosition().y;
					Color textColor = (thingDef.displayNumbersBetweenSameDefDistRange.Includes(intVec2.x) ? Color.white : Color.red);
					Widgets.DrawNumberOnMap(screenPos, intVec2.x, textColor);
				}
				if (intVec2.z >= 3)
				{
					Vector2 screenPos2 = (tmpThing.Position.ToUIPosition() + intVec.ToUIPosition()) / 2f;
					screenPos2.x = tmpThing.Position.ToUIPosition().x;
					Color textColor2 = (thingDef.displayNumbersBetweenSameDefDistRange.Includes(intVec2.z) ? Color.white : Color.red);
					Widgets.DrawNumberOnMap(screenPos2, intVec2.z, textColor2);
				}
			}
		}
		tmpThings.Clear();
	}

	protected virtual void DrawPlaceMouseAttachments(float curX, ref float curY)
	{
	}

	protected virtual bool CanDrawNumbersBetween(Thing thing, ThingDef def, IntVec3 a, IntVec3 b, Map map)
	{
		return !GenThing.CloserThingBetween(def, a, b, map);
	}

	public override void DoExtraGuiControls(float leftX, float bottomY)
	{
		if (PlacingDef.PlaceWorkers != null)
		{
			foreach (PlaceWorker placeWorker in PlacingDef.PlaceWorkers)
			{
				placeWorker.DrawOnGUIExtra(PlacingDef);
			}
		}
		if (PlacingDef is ThingDef { rotatable: not false })
		{
			DesignatorUtility.GUIDoRotationControls(leftX, bottomY, placingRot, delegate(Rot4 rot)
			{
				placingRot = rot;
			});
		}
		else
		{
			base.DoExtraGuiControls(leftX, bottomY);
		}
	}

	public override void SelectedProcessInput(Event ev)
	{
		if (PlacingDef is ThingDef { rotatable: not false })
		{
			HandleRotationShortcuts();
		}
		else
		{
			base.SelectedProcessInput(ev);
		}
	}

	public override void SelectedUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Map.IsPocketMap)
		{
			GenDraw.DrawNoBuildEdgeLines();
		}
		IntVec3 intVec = UI.MouseCell();
		Rect infoRect = ArchitectCategoryTab.InfoRect;
		if (((Rect)(ref infoRect)).Contains(UI.MousePositionOnUIInverted) || !intVec.InBounds(base.Map))
		{
			return;
		}
		if (PlacingDef is TerrainDef)
		{
			GenUI.RenderMouseoverBracket();
			DrawPlaceWorkers();
			return;
		}
		DrawBeforeGhost();
		Color ghostCol = ((!CanDesignateCell(intVec).Accepted) ? CannotPlaceColor : CanPlaceColor);
		DrawGhost(ghostCol);
		if (CanDesignateCell(intVec).Accepted && PlacingDef.specialDisplayRadius > 0.01f)
		{
			GenDraw.DrawRadiusRing(intVec, PlacingDef.specialDisplayRadius);
		}
		GenDraw.DrawInteractionCells((ThingDef)PlacingDef, intVec, placingRot);
		if (PlacingDef is ThingDef { building: not null } thingDef && thingDef.building.isAttachment && GenConstruct.GetWallAttachedTo(intVec, placingRot, Find.CurrentMap) == null && HasPotentialAttachment(intVec))
		{
			HandleRotation(RotationDirection.Clockwise);
		}
	}

	protected virtual void DrawBeforeGhost()
	{
		if (PlacingDef is ThingDef def)
		{
			MeditationUtility.DrawMeditationFociAffectedByBuildingOverlay(base.Map, def, Faction.OfPlayer, UI.MouseCell(), placingRot);
			GauranlenUtility.DrawConnectionsAffectedByBuildingOverlay(base.Map, def, Faction.OfPlayer, UI.MouseCell(), placingRot);
			PsychicRitualUtility.DrawPsychicRitualSpotsAffectedByThingOverlay(base.Map, def, UI.MouseCell(), placingRot);
		}
	}

	protected virtual void DrawGhost(Color ghostCol)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		GhostDrawer.DrawGhostThing(UI.MouseCell(), placingRot, (ThingDef)PlacingDef, ThingStyleDefForPreview?.Graphic, ghostCol, AltitudeLayer.Blueprint, null, drawPlaceWorkers: true, StuffDef);
	}

	protected virtual void DrawPlaceWorkers()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (PlacingDef.PlaceWorkers == null)
		{
			return;
		}
		foreach (PlaceWorker placeWorker in PlacingDef.PlaceWorkers)
		{
			placeWorker.DrawGhost(null, UI.MouseCell(), placingRot, CanPlaceColor);
		}
	}

	private void HandleRotationShortcuts()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		RotationDirection rotationDirection = RotationDirection.None;
		if (Event.current.button == 2)
		{
			if ((int)Event.current.type == 0)
			{
				Event.current.Use();
				middleMouseDownTime = Time.realtimeSinceStartup;
			}
			if ((int)Event.current.type == 1 && Time.realtimeSinceStartup - middleMouseDownTime < 0.15f)
			{
				rotationDirection = RotationDirection.Clockwise;
			}
		}
		if (KeyBindingDefOf.Designator_RotateRight.KeyDownEvent)
		{
			rotationDirection = RotationDirection.Clockwise;
		}
		if (KeyBindingDefOf.Designator_RotateLeft.KeyDownEvent)
		{
			rotationDirection = RotationDirection.Counterclockwise;
		}
		if (rotationDirection != 0)
		{
			HandleRotation(rotationDirection);
		}
	}

	private void HandleRotation(RotationDirection dir)
	{
		IntVec3 intVec = UI.MouseCell();
		ThingDef obj = PlacingDef as ThingDef;
		if (obj != null && obj.building.isAttachment && HasPotentialAttachment(intVec))
		{
			placingRot.Rotate(dir);
			while (GenConstruct.GetWallAttachedTo(intVec, placingRot, Find.CurrentMap) == null)
			{
				placingRot.Rotate(dir);
			}
		}
		else
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			placingRot.Rotate(dir);
		}
	}

	private bool HasPotentialAttachment(IntVec3 cell)
	{
		if (!(PlacingDef is ThingDef thingDef) || !thingDef.building.isAttachment)
		{
			return false;
		}
		foreach (Rot4 allRotation in Rot4.AllRotations)
		{
			if (GenConstruct.GetWallAttachedTo(cell, allRotation, Find.CurrentMap) != null)
			{
				return true;
			}
		}
		return false;
	}

	public override void Selected()
	{
		placingRot = PlacingDef.defaultPlacingRot;
	}
}
