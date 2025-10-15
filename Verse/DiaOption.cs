using System;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class DiaOption
{
	public Window dialog;

	protected string text;

	public DiaNode link;

	public Func<DiaNode> linkLateBind;

	public bool resolveTree;

	public Action action;

	public bool disabled;

	public string disabledReason;

	public SoundDef clickSound = SoundDefOf.PageChange;

	public Dialog_InfoCard.Hyperlink hyperlink;

	protected readonly Color DisabledOptionColor = new Color(0.5f, 0.5f, 0.5f);

	public static DiaOption DefaultOK => new DiaOption("OK".Translate())
	{
		resolveTree = true
	};

	protected Dialog_NodeTree OwningDialog => (Dialog_NodeTree)dialog;

	public DiaOption()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		text = "OK".Translate();
	}

	public DiaOption(string text)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		this.text = text;
	}

	public DiaOption(Dialog_InfoCard.Hyperlink hyperlink)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		this.hyperlink = hyperlink;
		text = "ViewHyperlink".Translate(hyperlink.Label);
	}

	public DiaOption(DiaOptionMold def)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		text = def.Text;
		DiaNodeMold diaNodeMold = def.RandomLinkNode();
		if (diaNodeMold != null)
		{
			link = new DiaNode(diaNodeMold);
		}
	}

	public void Disable(string newDisabledReason)
	{
		disabled = true;
		disabledReason = newDisabledReason;
	}

	public void SetText(string newText)
	{
		text = newText;
	}

	public float OptOnGUI(Rect rect, bool active = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Color textColor = Widgets.NormalOptionColor;
		string text = this.text;
		if (disabled)
		{
			textColor = DisabledOptionColor;
			if (disabledReason != null)
			{
				text = text + " (" + disabledReason + ")";
			}
		}
		((Rect)(ref rect)).height = Text.CalcHeight(text, ((Rect)(ref rect)).width);
		if (hyperlink.def != null)
		{
			Widgets.HyperlinkWithIcon(rect, hyperlink, text);
		}
		else if (Widgets.ButtonText(rect, text, drawBackground: false, !disabled, textColor, active && !disabled))
		{
			Activate();
		}
		return ((Rect)(ref rect)).height;
	}

	protected void Activate()
	{
		if (clickSound != null && !resolveTree)
		{
			clickSound.PlayOneShotOnCamera();
		}
		if (resolveTree)
		{
			OwningDialog.Close();
		}
		if (action != null)
		{
			action();
		}
		if (linkLateBind != null)
		{
			OwningDialog.GotoNode(linkLateBind());
		}
		else if (link != null)
		{
			OwningDialog.GotoNode(link);
		}
	}
}
