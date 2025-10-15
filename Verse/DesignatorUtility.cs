using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;

namespace Verse;

[StaticConstructorOnStartup]
public static class DesignatorUtility
{
	public static readonly Material DragHighlightCellMat = MaterialPool.MatFrom("UI/Overlays/DragHighlightCell", ShaderDatabase.MetaOverlay);

	public static readonly Material DragHighlightThingMat = MaterialPool.MatFrom("UI/Overlays/DragHighlightThing", ShaderDatabase.MetaOverlay);

	private static Dictionary<Type, Designator> StandaloneDesignators = new Dictionary<Type, Designator>();

	private static HashSet<Thing> selectedThings = new HashSet<Thing>();

	private const float RotButSize = 64f;

	private const float RotButSpacing = 10f;

	public static T FindAllowedDesignator<T>() where T : Designator
	{
		List<DesignationCategoryDef> allDefsListForReading = DefDatabase<DesignationCategoryDef>.AllDefsListForReading;
		GameRules rules = Current.Game.Rules;
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			List<Designator> allResolvedDesignators = allDefsListForReading[i].AllResolvedDesignators;
			for (int j = 0; j < allResolvedDesignators.Count; j++)
			{
				if ((rules == null || rules.DesignatorAllowed(allResolvedDesignators[j])) && allResolvedDesignators[j] is T result)
				{
					return result;
				}
			}
		}
		Designator designator = StandaloneDesignators.TryGetValue(typeof(T));
		if (designator == null)
		{
			designator = Activator.CreateInstance(typeof(T)) as Designator;
			StandaloneDesignators[typeof(T)] = designator;
		}
		return (T)designator;
	}

	public static void RenderHighlightOverSelectableCells(Designator designator, List<IntVec3> dragCells)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		foreach (IntVec3 dragCell in dragCells)
		{
			Vector3 val = dragCell.ToVector3Shifted();
			val.y = AltitudeLayer.MetaOverlays.AltitudeFor();
			Graphics.DrawMesh(MeshPool.plane10, val, Quaternion.identity, DragHighlightCellMat, 0);
		}
	}

	public static void RenderHighlightOverSelectableThings(Designator designator, List<IntVec3> dragCells)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		selectedThings.Clear();
		foreach (IntVec3 dragCell in dragCells)
		{
			List<Thing> thingList = dragCell.GetThingList(designator.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (designator.CanDesignateThing(thingList[i]).Accepted && !selectedThings.Contains(thingList[i]))
				{
					selectedThings.Add(thingList[i]);
					Vector3 drawPos = thingList[i].DrawPos;
					drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor();
					Graphics.DrawMesh(MeshPool.plane10, drawPos, Quaternion.identity, DragHighlightThingMat, 0);
				}
			}
		}
		selectedThings.Clear();
	}

	public static void GUIDoRotationControls(float leftX, float bottomY, Rot4 rot, Action<Rot4> rotSetter)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Rect winRect = new Rect(leftX, bottomY - 90f, 200f, 90f);
		Find.WindowStack.ImmediateWindow(73095, winRect, WindowLayer.GameUI, delegate
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			RotationDirection rotationDirection = RotationDirection.None;
			Text.Anchor = (TextAnchor)4;
			Text.Font = GameFont.Medium;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref winRect)).width / 2f - 64f - 5f, 15f, 64f, 64f);
			if (Widgets.ButtonImage(val, TexUI.RotLeftTex))
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
				rotationDirection = RotationDirection.Counterclockwise;
				Event.current.Use();
			}
			if (!SteamDeck.IsSteamDeck)
			{
				Widgets.Label(val, KeyBindingDefOf.Designator_RotateLeft.MainKeyLabel);
			}
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref winRect)).width / 2f + 5f, 15f, 64f, 64f);
			if (Widgets.ButtonImage(val2, TexUI.RotRightTex))
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
				rotationDirection = RotationDirection.Clockwise;
				Event.current.Use();
			}
			if (!SteamDeck.IsSteamDeck)
			{
				Widgets.Label(val2, KeyBindingDefOf.Designator_RotateRight.MainKeyLabel);
			}
			if (rotationDirection != 0)
			{
				rot.Rotate(rotationDirection);
				rotSetter(rot);
			}
			Text.Anchor = (TextAnchor)0;
			Text.Font = GameFont.Small;
		});
	}
}
