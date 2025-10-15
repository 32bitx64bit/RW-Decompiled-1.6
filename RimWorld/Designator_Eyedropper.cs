using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Designator_Eyedropper : Designator
{
	private Action<ColorDef> selectAction;

	private string rejectMessage;

	public static readonly Texture2D EyeDropperTex = ContentFinder<Texture2D>.Get("UI/Icons/Eyedropper");

	public Designator_Eyedropper(Action<ColorDef> selectAction, string rejectMessage, string desc)
	{
		this.selectAction = selectAction;
		this.rejectMessage = rejectMessage;
		defaultLabel = "DesignatorEyedropper".Translate();
		defaultDesc = desc;
		icon = (Texture)(object)EyeDropperTex;
		useMouseIcon = true;
	}

	public override AcceptanceReport CanDesignateCell(IntVec3 c)
	{
		if (ColorDefAt(c) != null)
		{
			return AcceptanceReport.WasAccepted;
		}
		if (!rejectMessage.NullOrEmpty())
		{
			return rejectMessage;
		}
		return false;
	}

	public override void DesignateSingleCell(IntVec3 cell)
	{
		ColorDef colorDef = ColorDefAt(cell);
		if (colorDef != null)
		{
			selectAction?.Invoke(colorDef);
			Messages.Message("GrabbedColor".Translate() + ": " + colorDef.LabelCap, null, MessageTypeDefOf.NeutralEvent, historical: false);
		}
		else if (!rejectMessage.NullOrEmpty())
		{
			Messages.Message(rejectMessage, null, MessageTypeDefOf.RejectInput, historical: false);
		}
	}

	protected virtual ColorDef ColorDefAt(IntVec3 cell)
	{
		if (!cell.InBounds(base.Map) || cell.Fogged(base.Map))
		{
			return null;
		}
		List<Thing> thingList = cell.GetThingList(base.Map);
		for (int i = 0; i < thingList.Count; i++)
		{
			if (thingList[i] is Building building && building.def.building.paintable && building.PaintColorDef != null)
			{
				return building.PaintColorDef;
			}
		}
		return base.Map.terrainGrid.ColorAt(cell) ?? cell.GetTerrain(base.Map).colorDef;
	}

	public override void DrawMouseAttachments()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (useMouseIcon)
		{
			string text = string.Empty;
			ColorDef colorDef = ColorDefAt(UI.MouseCell());
			if (colorDef != null)
			{
				text = "Grab".Translate() + ": " + colorDef.LabelCap;
			}
			else if (!rejectMessage.NullOrEmpty())
			{
				text = rejectMessage;
			}
			GenUI.DrawMouseAttachment(icon, text, iconAngle, iconOffset);
		}
	}
}
