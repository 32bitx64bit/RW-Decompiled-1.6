using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;

namespace Verse;

public abstract class Designator : Command
{
	protected bool useMouseIcon;

	public bool isOrder;

	public SoundDef soundDragSustain;

	public SoundDef soundDragChanged;

	public SoundDef soundSucceeded;

	protected SoundDef soundFailed = SoundDefOf.Designate_Failed;

	protected bool hasDesignateAllFloatMenuOption;

	protected string designateAllLabel;

	protected bool showReverseDesignatorDisabledReason;

	protected DesignationDef[] removeAllOtherDesignationDefs;

	private string cachedTutorTagSelect;

	private string cachedTutorTagDesignate;

	protected string cachedHighlightTag;

	private List<Designation> tmpAllDesignations = new List<Designation>();

	private const float IconButtonSize = 48f;

	private const float IconButtonGap = 16f;

	private const float StyleButtonSize = 60f;

	private const float StyleButtonLabelOffset = 27f;

	public Map Map => Find.CurrentMap;

	public virtual bool DragDrawMeasurements => false;

	public virtual bool DrawHighlight => true;

	protected override bool DoTooltip => false;

	public virtual bool AlwaysDoGuiControls => false;

	protected virtual DesignationDef Designation => null;

	public virtual float PanelReadoutTitleExtraRightMargin => 0f;

	public virtual DrawStyleCategoryDef DrawStyleCategory => null;

	public override string TutorTagSelect
	{
		get
		{
			if (tutorTag == null)
			{
				return null;
			}
			return cachedTutorTagSelect ?? (cachedTutorTagSelect = "SelectDesignator-" + tutorTag);
		}
	}

	public string TutorTagDesignate
	{
		get
		{
			if (tutorTag == null)
			{
				return null;
			}
			return cachedTutorTagDesignate ?? (cachedTutorTagDesignate = "Designate-" + tutorTag);
		}
	}

	public override string HighlightTag
	{
		get
		{
			if (cachedHighlightTag == null && tutorTag != null)
			{
				cachedHighlightTag = "Designator-" + tutorTag;
			}
			return cachedHighlightTag;
		}
	}

	public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
	{
		get
		{
			foreach (FloatMenuOption rightClickFloatMenuOption in base.RightClickFloatMenuOptions)
			{
				yield return rightClickFloatMenuOption;
			}
			if (hasDesignateAllFloatMenuOption)
			{
				int num = 0;
				List<Thing> things = Map.listerThings.AllThings;
				for (int i = 0; i < things.Count; i++)
				{
					Thing t = things[i];
					if (!t.Fogged() && CanDesignateThing(t).Accepted)
					{
						num++;
					}
				}
				if (num > 0)
				{
					yield return new FloatMenuOption(designateAllLabel + " (" + "CountToDesignate".Translate(num) + ")", delegate
					{
						for (int k = 0; k < things.Count; k++)
						{
							Thing t2 = things[k];
							if (!t2.Fogged() && CanDesignateThing(t2).Accepted)
							{
								DesignateThing(things[k]);
							}
						}
					});
				}
				else
				{
					yield return new FloatMenuOption(designateAllLabel + " (" + "NoneLower".Translate() + ")", null);
				}
			}
			DesignationDef designation = Designation;
			if (Designation == null)
			{
				yield break;
			}
			tmpAllDesignations.Clear();
			tmpAllDesignations.AddRange(Map.designationManager.designationsByDef[designation]);
			if (removeAllOtherDesignationDefs != null)
			{
				DesignationDef[] array = removeAllOtherDesignationDefs;
				foreach (DesignationDef def in array)
				{
					tmpAllDesignations.AddRange(Map.designationManager.designationsByDef[def]);
				}
			}
			int num2 = 0;
			foreach (Designation tmpAllDesignation in tmpAllDesignations)
			{
				if (RemoveAllDesignationsAffects(tmpAllDesignation.target))
				{
					num2++;
				}
			}
			if (num2 > 0)
			{
				yield return new FloatMenuOption(string.Concat("RemoveAllDesignations".Translate() + " (", num2.ToString(), ")"), delegate
				{
					for (int num3 = tmpAllDesignations.Count - 1; num3 >= 0; num3--)
					{
						Map.designationManager.RemoveDesignation(tmpAllDesignations[num3]);
					}
				});
			}
			else
			{
				yield return new FloatMenuOption("RemoveAllDesignations".Translate() + " (" + "NoneLower".Translate() + ")", null);
			}
		}
	}

	public Designator()
	{
		activateSound = SoundDefOf.Tick_Tiny;
		designateAllLabel = "DesignateAll".Translate();
	}

	protected bool CheckCanInteract()
	{
		if (TutorSystem.TutorialMode && !TutorSystem.AllowAction(TutorTagSelect))
		{
			return false;
		}
		return true;
	}

	public override void ProcessInput(Event ev)
	{
		if (CheckCanInteract())
		{
			base.ProcessInput(ev);
			Find.DesignatorManager.Select(this);
		}
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
		if (DebugViewSettings.showArchitectMenuOrder)
		{
			Text.Anchor = (TextAnchor)4;
			Text.Font = GameFont.Tiny;
			Widgets.Label(new Rect(topLeft.x, topLeft.y + 5f, GetWidth(maxWidth), 15f), Order.ToString());
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)0;
		}
		return result;
	}

	public Command_Action CreateReverseDesignationGizmo(Thing t)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		AcceptanceReport acceptanceReport = CanDesignateThing(t);
		float angle;
		Vector2 offset;
		if (acceptanceReport.Accepted || (showReverseDesignatorDisabledReason && !acceptanceReport.Reason.NullOrEmpty()))
		{
			return new Command_Action
			{
				defaultLabel = LabelCapReverseDesignating(t),
				icon = (Texture)(object)IconReverseDesignating(t, out angle, out offset),
				iconAngle = angle,
				iconOffset = offset,
				defaultDesc = (acceptanceReport.Reason.NullOrEmpty() ? DescReverseDesignating(t) : acceptanceReport.Reason),
				Order = ((this is Designator_Uninstall) ? (-11f) : (-20f)),
				Disabled = !acceptanceReport.Accepted,
				action = delegate
				{
					if (TutorSystem.AllowAction(TutorTagDesignate) && (Designation == null || Designation.targetType != 0 || Map.designationManager.DesignationOn(t, Designation) == null))
					{
						DesignateThing(t);
						Finalize(somethingSucceeded: true);
					}
				},
				hotKey = hotKey,
				groupKeyIgnoreContent = groupKeyIgnoreContent,
				groupKey = groupKey
			};
		}
		return null;
	}

	public virtual AcceptanceReport CanDesignateThing(Thing t)
	{
		return AcceptanceReport.WasRejected;
	}

	public virtual void DesignateThing(Thing t)
	{
		throw new NotImplementedException();
	}

	public abstract AcceptanceReport CanDesignateCell(IntVec3 loc);

	public virtual void DesignateMultiCell(IEnumerable<IntVec3> cells)
	{
		if (TutorSystem.TutorialMode && !TutorSystem.AllowAction(new EventPack(TutorTagDesignate, cells)))
		{
			return;
		}
		bool somethingSucceeded = false;
		bool flag = false;
		foreach (IntVec3 cell in cells)
		{
			if (CanDesignateCell(cell).Accepted)
			{
				DesignateSingleCell(cell);
				somethingSucceeded = true;
				if (!flag)
				{
					flag = ShowWarningForCell(cell);
				}
			}
		}
		Finalize(somethingSucceeded);
		if (TutorSystem.TutorialMode)
		{
			TutorSystem.Notify_Event(new EventPack(TutorTagDesignate, cells));
		}
	}

	public virtual void DesignateSingleCell(IntVec3 c)
	{
		throw new NotImplementedException();
	}

	public virtual bool ShowWarningForCell(IntVec3 c)
	{
		return false;
	}

	public void Finalize(bool somethingSucceeded)
	{
		if (somethingSucceeded)
		{
			FinalizeDesignationSucceeded();
		}
		else
		{
			FinalizeDesignationFailed();
		}
	}

	protected virtual void FinalizeDesignationSucceeded()
	{
		if (soundSucceeded != null)
		{
			soundSucceeded.PlayOneShotOnCamera();
		}
	}

	protected virtual void FinalizeDesignationFailed()
	{
		if (soundFailed != null)
		{
			soundFailed.PlayOneShotOnCamera();
		}
		if (Find.DesignatorManager.Dragger.FailureReason != null)
		{
			Messages.Message(Find.DesignatorManager.Dragger.FailureReason, MessageTypeDefOf.RejectInput, historical: false);
		}
	}

	public virtual string LabelCapReverseDesignating(Thing t)
	{
		return LabelCap;
	}

	public virtual string DescReverseDesignating(Thing t)
	{
		return Desc;
	}

	public virtual Texture2D IconReverseDesignating(Thing t, out float angle, out Vector2 offset)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		angle = iconAngle;
		offset = iconOffset;
		return (Texture2D)icon;
	}

	protected virtual bool RemoveAllDesignationsAffects(LocalTargetInfo target)
	{
		return true;
	}

	public virtual void DrawMouseAttachments()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (useMouseIcon)
		{
			GenUI.DrawMouseAttachment(hideMouseIcon ? null : icon, mouseText, iconAngle, iconOffset);
		}
	}

	public virtual void DrawPanelReadout(ref float curY, float width)
	{
	}

	public virtual void DoExtraGuiControls(float leftX, float bottomY)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Rect winRect = new Rect(leftX, bottomY - 90f, 200f, 90f);
		DrawStyleDef style = Find.DesignatorManager.SelectedStyle;
		List<DrawStyleDef> list = DrawStyleCategory?.styles;
		if (style == null || list == null || list.Count <= 1)
		{
			return;
		}
		Find.WindowStack.ImmediateWindow(415111, winRect, WindowLayer.GameUI, delegate
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			using (new TextBlock(GameFont.Medium, (TextAnchor)4))
			{
				Rect rect = winRect;
				((Rect)(ref rect)).x = 0f;
				((Rect)(ref rect)).y = 0f;
				Rect val = rect.MiddlePartPixels(48f, 48f);
				if (Widgets.ButtonImage(val, style.uiIcon))
				{
					List<FloatMenuOption> list2 = new List<FloatMenuOption>();
					foreach (DrawStyleDef style2 in DrawStyleCategory.styles)
					{
						DrawStyleDef lDef = style2;
						list2.Add(new FloatMenuOption(style2.LabelCap, delegate
						{
							Find.DesignatorManager.SelectedStyle = lDef;
						}));
					}
					Find.WindowStack.Add(new FloatMenu(list2));
				}
				Rect val2 = rect.MiddlePartPixels(60f, 60f);
				((Rect)(ref val2)).x = ((Rect)(ref val)).xMin - 16f - ((Rect)(ref val2)).width;
				Rect rect2 = val;
				((Rect)(ref rect2)).x = ((Rect)(ref val2)).xMin + 27f;
				((Rect)(ref rect2)).xMax = ((Rect)(ref val2)).xMax + 27f;
				Rect butRect = val2;
				((Rect)(ref butRect)).x = ((Rect)(ref val)).xMax + 16f;
				Rect rect3 = val;
				((Rect)(ref rect3)).x = ((Rect)(ref butRect)).xMin - 27f;
				((Rect)(ref rect3)).xMax = ((Rect)(ref butRect)).xMax - 27f;
				if (Widgets.ButtonImage(val2, TexUI.ConcaveArrowTexLeft))
				{
					SoundDefOf.DragSlider.PlayOneShotOnCamera();
					Find.DesignatorManager.PreviousDrawStyle();
					Event.current.Use();
				}
				if (!SteamDeck.IsSteamDeck)
				{
					Widgets.Label(rect2, KeyBindingDefOf.Designator_PreviousDrawStyle.MainKeyLabel);
				}
				if (Widgets.ButtonImage(butRect, TexUI.ConcaveArrowTexRight))
				{
					SoundDefOf.DragSlider.PlayOneShotOnCamera();
					Find.DesignatorManager.AdvanceDrawStyle();
					Event.current.Use();
				}
				if (!SteamDeck.IsSteamDeck)
				{
					Widgets.Label(rect3, KeyBindingDefOf.Designator_NextDrawStyle.MainKeyLabel);
				}
			}
		});
	}

	public virtual void SelectedUpdate()
	{
	}

	public virtual void SelectedProcessInput(Event ev)
	{
		if (KeyBindingDefOf.Designator_NextDrawStyle.KeyDownEvent)
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			Find.DesignatorManager.AdvanceDrawStyle();
		}
		if (KeyBindingDefOf.Designator_PreviousDrawStyle.KeyDownEvent)
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			Find.DesignatorManager.PreviousDrawStyle();
		}
	}

	public virtual bool CanRemainSelected()
	{
		return true;
	}

	public virtual void Selected()
	{
	}

	public virtual void Deselected()
	{
	}

	public virtual void RenderHighlight(List<IntVec3> dragCells)
	{
		DesignatorUtility.RenderHighlightOverSelectableThings(this, dragCells);
	}
}
