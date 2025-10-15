using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Designator_Paint : DesignatorWithEyedropper
{
	protected ColorDef colorDef;

	private string cachedAttachmentString;

	protected abstract Texture2D IconTopTex { get; }

	public override Color IconDrawColor => colorDef.color;

	public override bool DragDrawMeasurements => true;

	public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Paint;

	protected virtual IEnumerable<ColorDef> Colors => from x in DefDatabase<ColorDef>.AllDefs
		where x.colorType == ColorType.Structure
		orderby x.displayOrder
		select x;

	private string AttachmentString
	{
		get
		{
			if (cachedAttachmentString == null)
			{
				cachedAttachmentString = "Paint".Translate() + ": " + colorDef.LabelCap + "\n" + KeyBindingDefOf.ShowEyedropper.MainKeyLabel + ": " + "GrabExistingColor".Translate();
			}
			return cachedAttachmentString;
		}
	}

	public Designator_Paint()
	{
		colorDef = Colors.FirstOrDefault();
		soundDragSustain = SoundDefOf.Designate_DragStandard;
		soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
		useMouseIcon = true;
		soundSucceeded = SoundDefOf.Designate_Paint;
		hotKey = KeyBindingDefOf.Misc6;
		eyedropper = new Designator_Eyedropper(delegate(ColorDef newCol)
		{
			colorDef = newCol;
			cachedAttachmentString = null;
			if (!eyedropMode)
			{
				Find.DesignatorManager.Select(this);
			}
		}, "SelectAPaintedBuilding".Translate(), "DesignatorEyeDropperDesc_Paint".Translate());
	}

	public override void ProcessInput(Event ev)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckCanInteract())
		{
			return;
		}
		List<FloatMenuGridOption> list = new List<FloatMenuGridOption>();
		Texture2D eyeDropperTex = Designator_Eyedropper.EyeDropperTex;
		Action action = delegate
		{
			base.ProcessInput(ev);
			Find.DesignatorManager.Select(eyedropper);
		};
		TipSignal? tooltip = "DesignatorEyeDropperDesc_Paint".Translate();
		list.Add(new FloatMenuGridOption(eyeDropperTex, action, null, tooltip));
		foreach (ColorDef color in Colors)
		{
			ColorDef newCol = color;
			list.Add(new FloatMenuGridOption(BaseContent.WhiteTex, delegate
			{
				base.ProcessInput(ev);
				Find.DesignatorManager.Select(this);
				colorDef = newCol;
				cachedAttachmentString = null;
			}, newCol.color, newCol.LabelCap));
		}
		Find.WindowStack.Add(new FloatMenuGrid(list));
		Find.DesignatorManager.Select(this);
	}

	public override void DrawMouseAttachments()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		eyedropMode = KeyBindingDefOf.ShowEyedropper.IsDown;
		if (eyedropMode)
		{
			eyedropper.DrawMouseAttachments();
			return;
		}
		if (useMouseIcon)
		{
			Texture iconTex = icon;
			string attachmentString = AttachmentString;
			float angle = iconAngle;
			Vector2 offset = iconOffset;
			Color? iconColor = colorDef.color;
			GenUI.DrawMouseAttachment(iconTex, attachmentString, angle, offset, null, null, drawTextBackground: false, default(Color), iconColor, delegate(Rect r)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				GUI.DrawTexture(r, (Texture)(object)IconTopTex);
			});
		}
		if (Find.DesignatorManager.Dragger.Dragging)
		{
			Vector2 val = Event.current.mousePosition + Designator_Place.PlaceMouseAttachmentDrawOffset;
			if (useMouseIcon)
			{
				val.y += 32f + Text.LineHeight * 2f;
			}
			Widgets.ThingIcon(new Rect(val.x, val.y, 27f, 27f), ThingDefOf.Dye);
			int num = NumHighlightedCells();
			string text = num.ToStringCached();
			if (base.Map.resourceCounter.GetCount(ThingDefOf.Dye) < num)
			{
				GUI.color = Color.red;
				text += " (" + "NotEnoughStoredLower".Translate() + ")";
			}
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(new Rect(val.x + 29f, val.y, 999f, 29f), text);
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
		}
	}

	public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base.DrawIcon(rect, buttonMat, parms);
		Widgets.DrawTextureFitted(rect, (Texture)(object)IconTopTex, iconDrawScale * 0.85f, iconProportions, iconTexCoords, iconAngle, buttonMat);
	}

	protected abstract int NumHighlightedCells();
}
