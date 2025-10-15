using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class MainButtonWorker
{
	public MainButtonDef def;

	private const float CompactModeMargin = 2f;

	private const float IconSize = 32f;

	public virtual float ButtonBarPercent => 0f;

	public virtual bool Disabled
	{
		get
		{
			if (Find.CurrentMap == null && (!def.validWithoutMap || def == MainButtonDefOf.World))
			{
				return true;
			}
			if (Find.WorldRoutePlanner.Active && Find.WorldRoutePlanner.FormingCaravan && (!def.validWithoutMap || def == MainButtonDefOf.World))
			{
				return true;
			}
			if (Find.TilePicker.Active && !Find.TilePicker.AllowEscape && (!def.validWithoutMap || def == MainButtonDefOf.World))
			{
				return true;
			}
			return false;
		}
	}

	public virtual bool Visible
	{
		get
		{
			if (ModsConfig.IdeologyActive && !def.validWithClassicIdeo && Find.IdeoManager.classicMode)
			{
				return false;
			}
			return def.buttonVisible;
		}
	}

	public abstract void Activate();

	public virtual void InterfaceTryActivate()
	{
		if (!TutorSystem.TutorialMode || !def.canBeTutorDenied || Find.MainTabsRoot.OpenTab == def || TutorSystem.AllowAction("MainTab-" + def.defName + "-Open"))
		{
			if (def.closesWorldView && Find.TilePicker.Active && !Find.TilePicker.AllowEscape)
			{
				Messages.Message("MessagePlayerMustSelectTile".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				Activate();
			}
		}
	}

	public virtual void DoButton(Rect rect)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		string text = def.LabelCap;
		float num = def.LabelCapWidth;
		if (num > ((Rect)(ref rect)).width - 2f)
		{
			text = def.ShortenedLabelCap;
			num = def.ShortenedLabelCapWidth;
		}
		if (Disabled)
		{
			Widgets.DrawAtlas(rect, Widgets.ButtonSubtleAtlas);
			if ((int)Event.current.type == 0 && Mouse.IsOver(rect))
			{
				Event.current.Use();
			}
			return;
		}
		bool flag = num > 0.85f * ((Rect)(ref rect)).width - 1f;
		Rect rect2 = rect;
		string label = (((Object)(object)def.Icon == (Object)null) ? text : "");
		float textLeftMargin = (flag ? 2f : (-1f));
		if (Widgets.ButtonTextSubtle(rect2, label, ButtonBarPercent, textLeftMargin, SoundDefOf.Mouseover_Category))
		{
			InterfaceTryActivate();
		}
		if ((Object)(object)def.Icon != (Object)null)
		{
			Vector2 val = ((Rect)(ref rect)).center;
			float num2 = 16f;
			if (Mouse.IsOver(rect))
			{
				val += new Vector2(2f, -2f);
			}
			GUI.DrawTexture(new Rect(val.x - num2, val.y - num2, 32f, 32f), (Texture)(object)def.Icon);
		}
		if (Find.MainTabsRoot.OpenTab != def && !Find.WindowStack.NonImmediateDialogWindowOpen)
		{
			UIHighlighter.HighlightOpportunity(rect, def.cachedHighlightTagClosed);
		}
		if (Mouse.IsOver(rect) && !def.description.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, def.LabelCap.Colorize(ColorLibrary.Yellow) + "\n\n" + def.description);
		}
	}
}
