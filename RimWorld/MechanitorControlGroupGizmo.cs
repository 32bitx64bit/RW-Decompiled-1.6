using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class MechanitorControlGroupGizmo : Gizmo
{
	public const int InRectPadding = 6;

	private const float Width = 130f;

	private const int IconButtonSize = 26;

	private const float BaseSelectedTexJump = 20f;

	private const float BaseSelectedTextScale = 0.8f;

	private static readonly CachedTexture PowerIcon = new CachedTexture("UI/Icons/MechRechargeSettings");

	private static readonly Color UncontrolledMechBackgroundColor = Color32.op_Implicit(new Color32(byte.MaxValue, (byte)25, (byte)25, (byte)55));

	private MechanitorControlGroup controlGroup;

	private List<MechanitorControlGroup> mergedControlGroups;

	public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions => GetWorkModeOptions(controlGroup);

	public override bool Visible
	{
		get
		{
			if (controlGroup.MechsForReading.Count <= 0)
			{
				return Find.Selector.SelectedPawns.Count == 1;
			}
			return true;
		}
	}

	public override float Order
	{
		get
		{
			if (controlGroup.MechsForReading.Count > 0)
			{
				return -89f;
			}
			return -88f;
		}
	}

	public MechanitorControlGroupGizmo(MechanitorControlGroup controlGroup)
	{
		this.controlGroup = controlGroup;
		Order = -89f;
	}

	public static IEnumerable<FloatMenuOption> GetWorkModeOptions(MechanitorControlGroup controlGroup)
	{
		foreach (MechWorkModeDef wm in DefDatabase<MechWorkModeDef>.AllDefsListForReading.OrderBy((MechWorkModeDef d) => d.uiOrder))
		{
			FloatMenuOption floatMenuOption = new FloatMenuOption(wm.LabelCap, delegate
			{
				controlGroup.SetWorkMode(wm);
			}, wm.uiIcon, Color.white);
			floatMenuOption.tooltip = new TipSignal(wm.description, wm.index ^ 0xDFE8661);
			yield return floatMenuOption;
		}
	}

	public override void GizmoUpdateOnMouseover()
	{
		base.GizmoUpdateOnMouseover();
		controlGroup.WorkMode.Worker.DrawControlGroupMouseOverExtra(controlGroup);
	}

	public override bool GroupsWith(Gizmo other)
	{
		if (!(other is MechanitorControlGroupGizmo mechanitorControlGroupGizmo))
		{
			return false;
		}
		if (mechanitorControlGroupGizmo.controlGroup == controlGroup)
		{
			return true;
		}
		if (controlGroup.Tracker == mechanitorControlGroupGizmo.controlGroup.Tracker && controlGroup.MechsForReading.Count == 0 && mechanitorControlGroupGizmo.controlGroup.MechsForReading.Count == 0)
		{
			return true;
		}
		if (mergedControlGroups.NotNullAndContains(mechanitorControlGroupGizmo.controlGroup))
		{
			mergedControlGroups.Remove(mechanitorControlGroupGizmo.controlGroup);
		}
		return false;
	}

	public override void MergeWith(Gizmo gizmo)
	{
		if (!(gizmo is MechanitorControlGroupGizmo mechanitorControlGroupGizmo))
		{
			Log.ErrorOnce("Tried to merge MechanitorControlGroupGizmo with unexpected type", 345234235);
		}
		else if (mechanitorControlGroupGizmo.controlGroup != controlGroup)
		{
			if (mergedControlGroups == null)
			{
				mergedControlGroups = new List<MechanitorControlGroup>();
			}
			if (!mergedControlGroups.Contains(mechanitorControlGroupGizmo.controlGroup))
			{
				mergedControlGroups.Add(mechanitorControlGroupGizmo.controlGroup);
			}
		}
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckBiotech("Mechanitor control group gizmo"))
		{
			return new GizmoResult(GizmoState.Clear);
		}
		AcceptanceReport canControlMechs = controlGroup.Tracker.CanControlMechs;
		disabled = !canControlMechs;
		disabledReason = canControlMechs.Reason;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = rect.ContractedBy(6f);
		bool flag = Mouse.IsOver(val);
		List<Pawn> mechsForReading = controlGroup.MechsForReading;
		Color white = Color.white;
		Material material = ((disabled || parms.lowLight || mechsForReading.Count <= 0) ? TexUI.GrayscaleGUI : null);
		GUI.color = (parms.lowLight ? Command.LowLightBgColor : white);
		GenUI.DrawTextureWithMaterial(rect, (Texture)(object)(parms.shrunk ? Command.BGTexShrunk : Command.BGTex), material);
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		Rect val2 = val;
		TaggedString str = ((!mergedControlGroups.NullOrEmpty()) ? "Groups".Translate() : "Group".Translate());
		str += " " + controlGroup.Index;
		if (!mergedControlGroups.NullOrEmpty())
		{
			mergedControlGroups.SortBy((MechanitorControlGroup c) => c.Index);
			for (int i = 0; i < mergedControlGroups.Count; i++)
			{
				str += ", " + mergedControlGroups[i].Index;
			}
		}
		str = str.Truncate(((Rect)(ref val)).width);
		Vector2 val3 = Text.CalcSize(str);
		((Rect)(ref val2)).width = val3.x;
		((Rect)(ref val2)).height = val3.y;
		Widgets.Label(val2, str);
		if (mechsForReading.Count <= 0)
		{
			GUI.color = ColoredText.SubtleGrayColor;
			Text.Anchor = (TextAnchor)4;
			Widgets.Label(val, "(" + "NoMechs".Translate() + ")");
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
			return new GizmoResult(GizmoState.Clear);
		}
		if (Mouse.IsOver(val2))
		{
			Widgets.DrawHighlight(val2);
			if (Widgets.ButtonInvisible(val2))
			{
				Find.Selector.ClearSelection();
				for (int j = 0; j < mechsForReading.Count; j++)
				{
					Find.Selector.Select(mechsForReading[j]);
				}
			}
		}
		bool flag2 = false;
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 26f - 6f, ((Rect)(ref rect)).y + 6f, 26f, 26f);
		Widgets.DrawTextureFitted(val4, (Texture)(object)PowerIcon.Texture, 1f);
		if (!disabled && Mouse.IsOver(val4))
		{
			flag2 = true;
			Widgets.DrawHighlight(val4);
			if (Widgets.ButtonInvisible(val4))
			{
				Find.WindowStack.Add(new Dialog_RechargeSettings(controlGroup));
			}
		}
		bool flag3 = false;
		Rect val5 = default(Rect);
		((Rect)(ref val5))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 52f - 6f, ((Rect)(ref rect)).y + 6f, 26f, 26f);
		Widgets.DrawTextureFitted(val5, (Texture)(object)controlGroup.WorkMode.uiIcon, 1f);
		if (!disabled && Mouse.IsOver(val5))
		{
			flag3 = true;
			Widgets.DrawHighlight(val5);
		}
		Rect val6 = default(Rect);
		((Rect)(ref val6))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y + 26f + 4f, ((Rect)(ref val)).width, ((Rect)(ref val)).height - 26f - 4f);
		float num = ((Rect)(ref val6)).height;
		int num2 = 0;
		int num3 = 0;
		for (float num4 = num; num4 >= 0f; num4 -= 1f)
		{
			num2 = Mathf.FloorToInt(((Rect)(ref val6)).width / num4);
			num3 = Mathf.FloorToInt(((Rect)(ref val6)).height / num4);
			if (num2 * num3 >= mechsForReading.Count)
			{
				num = num4;
				break;
			}
		}
		float num5 = (((Rect)(ref val6)).width - (float)num2 * num) / 2f;
		float num6 = (((Rect)(ref val6)).height - (float)num3 * num) / 2f;
		int num7 = 0;
		Rect val7 = default(Rect);
		for (int k = 0; k < num2; k++)
		{
			for (int l = 0; l < num2; l++)
			{
				if (num7 >= mechsForReading.Count)
				{
					break;
				}
				((Rect)(ref val7))._002Ector(((Rect)(ref val6)).x + (float)l * num + num5, ((Rect)(ref val6)).y + (float)k * num + num6, num, num);
				Pawn pawn = mechsForReading[num7];
				Vector2 size = ((Rect)(ref val7)).size;
				Rot4 east = Rot4.East;
				float controlGroupPortraitZoom = mechsForReading[num7].kindDef.controlGroupPortraitZoom;
				RenderTexture val8 = PortraitsCache.Get(pawn, size, east, default(Vector3), controlGroupPortraitZoom);
				if (!controlGroup.Tracker.ControlledPawns.Contains(mechsForReading[num7]))
				{
					Widgets.DrawRectFast(val7, UncontrolledMechBackgroundColor);
				}
				GUI.DrawTexture(val7, (Texture)(object)val8);
				if (Mouse.IsOver(val7))
				{
					Widgets.DrawHighlight(val7);
					MouseoverSounds.DoRegion(val7, SoundDefOf.Mouseover_Command);
					if ((int)Event.current.type == 0)
					{
						if (Event.current.shift)
						{
							Find.Selector.Select(mechsForReading[num7]);
						}
						else
						{
							CameraJumper.TryJumpAndSelect(mechsForReading[num7]);
						}
					}
					TargetHighlighter.Highlight(mechsForReading[num7], arrow: true, colonistBar: false);
				}
				if (Find.Selector.IsSelected(mechsForReading[num7]))
				{
					SelectionDrawerUtility.DrawSelectionOverlayOnGUI(mechsForReading[num7], val7, 0.8f / (float)num2, 20f);
				}
				num7++;
			}
			if (num7 >= mechsForReading.Count)
			{
				break;
			}
		}
		if (Find.WindowStack.FloatMenu == null && !flag2)
		{
			TooltipHandler.TipRegion(rect, delegate
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				string text = string.Concat("ControlGroup".Translate() + " #", controlGroup.Index.ToString()).Colorize(ColoredText.TipSectionTitleColor) + "\n\n";
				text = text + ("CurrentMechWorkMode".Translate() + ": " + controlGroup.WorkMode.LabelCap).Colorize(ColoredText.TipSectionTitleColor) + "\n" + controlGroup.WorkMode.description + "\n\n";
				IEnumerable<string> entries = from x in controlGroup.MechsForReading
					where x.needs?.energy != null
					select x into m
					select (m.LabelCap + " (" + m.needs.energy.CurLevelPercentage.ToStringPercent() + " " + "EnergyLower".Translate() + ")").Resolve();
				text = text + "AssignedMechs".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n" + entries.ToLineList(" - ");
				if (disabled && !disabledReason.NullOrEmpty())
				{
					text += ("\n\n" + "DisabledCommand".Translate() + ": " + disabledReason).Colorize(ColorLibrary.RedReadable);
				}
				return text;
			}, 2545872);
		}
		if (flag3 && (int)Event.current.type == 0)
		{
			return new GizmoResult(GizmoState.OpenedFloatMenu, Event.current);
		}
		return new GizmoResult(flag ? GizmoState.Mouseover : GizmoState.Clear);
	}

	public override float GetWidth(float maxWidth)
	{
		return 130f;
	}
}
