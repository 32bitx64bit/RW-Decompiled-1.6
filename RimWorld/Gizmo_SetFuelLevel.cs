using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld;

public class Gizmo_SetFuelLevel : Gizmo_Slider
{
	private CompRefuelable refuelable;

	private static bool draggingBar;

	protected override float Target
	{
		get
		{
			return refuelable.TargetFuelLevel / refuelable.Props.fuelCapacity;
		}
		set
		{
			refuelable.TargetFuelLevel = value * refuelable.Props.fuelCapacity;
		}
	}

	protected override float ValuePercent => refuelable.FuelPercentOfMax;

	protected override string Title => refuelable.Props.FuelGizmoLabel;

	protected override bool IsDraggable => refuelable.Props.targetFuelLevelConfigurable;

	protected override string BarLabel => refuelable.Fuel.ToStringDecimalIfSmall() + " / " + refuelable.Props.fuelCapacity.ToStringDecimalIfSmall();

	protected override bool DraggingBar
	{
		get
		{
			return draggingBar;
		}
		set
		{
			draggingBar = value;
		}
	}

	public Gizmo_SetFuelLevel(CompRefuelable refuelable)
	{
		this.refuelable = refuelable;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!refuelable.Props.showAllowAutoRefuelToggle)
		{
			return base.GizmoOnGUI(topLeft, maxWidth, parms);
		}
		if (SteamDeck.IsSteamDeckInNonKeyboardMode)
		{
			return base.GizmoOnGUI(topLeft, maxWidth, parms);
		}
		KeyCode val = (KeyCode)((KeyBindingDefOf.Command_ItemForbid != null) ? ((int)KeyBindingDefOf.Command_ItemForbid.MainKey) : 0);
		if ((int)val != 0 && !GizmoGridDrawer.drawnHotKeys.Contains(val) && KeyBindingDefOf.Command_ItemForbid.KeyDownEvent)
		{
			ToggleAutoRefuel();
			Event.current.Use();
		}
		return base.GizmoOnGUI(topLeft, maxWidth, parms);
	}

	protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (refuelable.Props.showAllowAutoRefuelToggle)
		{
			((Rect)(ref headerRect)).xMax = ((Rect)(ref headerRect)).xMax - 24f;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref headerRect)).xMax, ((Rect)(ref headerRect)).y, 24f, 24f);
			GUI.DrawTexture(val, (Texture)(object)refuelable.Props.FuelIcon);
			GUI.DrawTexture(new Rect(((Rect)(ref val)).center.x, ((Rect)(ref val)).y, ((Rect)(ref val)).width / 2f, ((Rect)(ref val)).height / 2f), (Texture)(object)(refuelable.allowAutoRefuel ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex));
			if (Widgets.ButtonInvisible(val))
			{
				ToggleAutoRefuel();
			}
			if (Mouse.IsOver(val))
			{
				Widgets.DrawHighlight(val);
				TooltipHandler.TipRegion(val, RefuelTip, 828267373);
				mouseOverElement = true;
			}
		}
		base.DrawHeader(headerRect, ref mouseOverElement);
	}

	private void ToggleAutoRefuel()
	{
		refuelable.allowAutoRefuel = !refuelable.allowAutoRefuel;
		if (refuelable.allowAutoRefuel)
		{
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
		else
		{
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
		}
	}

	private string RefuelTip()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Format("{0}", "CommandToggleAllowAutoRefuel".Translate()) + "\n\n";
		string str = (refuelable.allowAutoRefuel ? "On".Translate() : "Off".Translate());
		string text2 = refuelable.TargetFuelLevel.ToString("F0").Colorize(ColoredText.TipSectionTitleColor);
		string text3 = string.Concat(text + "CommandToggleAllowAutoRefuelDesc".Translate(text2, str.UncapitalizeFirst().Named("ONOFF")).Resolve(), "\n\n");
		string text4 = KeyPrefs.KeyPrefsData.GetBoundKeyCode(KeyBindingDefOf.Command_ItemForbid, KeyPrefs.BindingSlot.A).ToStringReadable();
		return text3 + ("HotKeyTip".Translate() + ": " + text4);
	}

	protected override string GetTooltip()
	{
		return "";
	}
}
