using System;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Command_Toggle : Command
{
	public Func<bool> isActive;

	public Action toggleAction;

	public SoundDef turnOnSound = SoundDefOf.Checkbox_TurnedOn;

	public SoundDef turnOffSound = SoundDefOf.Checkbox_TurnedOff;

	public bool activateIfAmbiguous = true;

	public bool hideIconIfDisabled;

	public override SoundDef CurActivateSound
	{
		get
		{
			if (isActive())
			{
				return turnOffSound;
			}
			return turnOnSound;
		}
	}

	public override void ProcessInput(Event ev)
	{
		base.ProcessInput(ev);
		toggleAction();
	}

	public override GizmoResult GizmoOnGUI(Vector2 loc, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		GizmoResult result = base.GizmoOnGUI(loc, maxWidth, parms);
		if (!disabled || !hideIconIfDisabled)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(loc.x, loc.y, GetWidth(maxWidth), 75f);
			Rect val2 = new Rect(((Rect)(ref val)).x + ((Rect)(ref val)).width - 24f, ((Rect)(ref val)).y, 24f, 24f);
			Texture2D val3 = (isActive() ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
			GUI.DrawTexture(val2, (Texture)(object)val3);
		}
		return result;
	}

	public override bool InheritInteractionsFrom(Gizmo other)
	{
		if (other is Command_Toggle command_Toggle)
		{
			return command_Toggle.isActive() == isActive();
		}
		return false;
	}
}
