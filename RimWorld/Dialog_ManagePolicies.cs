using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Dialog_ManagePolicies<T> : Window where T : Policy
{
	private readonly ThingFilterUI.UIState thingFilterState = new ThingFilterUI.UIState();

	private readonly QuickSearchWidget quickSearch = new QuickSearchWidget();

	private Vector2 scroll;

	private T policyInt;

	private const float LeftPanelWidth = 200f;

	private const float PolicyHeight = 32f;

	protected T SelectedPolicy
	{
		get
		{
			return policyInt;
		}
		set
		{
			ValidateName();
			policyInt = value;
		}
	}

	protected abstract string TitleKey { get; }

	protected abstract string TipKey { get; }

	public override Vector2 InitialSize => new Vector2(700f, 700f);

	protected virtual float OffsetHeaderY => 0f;

	private void ValidateName()
	{
		if (SelectedPolicy != null && SelectedPolicy.label.NullOrEmpty())
		{
			SelectedPolicy.label = "UnnamedPolicy".Translate();
		}
	}

	protected Dialog_ManagePolicies(T policy)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		forcePause = true;
		doCloseX = true;
		doCloseButton = true;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
		scroll = Vector2.zero;
		quickSearch.Reset();
		SelectedPolicy = policy;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		thingFilterState.quickSearch.Reset();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		Rect val = inRect;
		((Rect)(ref val)).height = 32f;
		Rect val2 = val;
		((Rect)(ref val2)).y = ((Rect)(ref val)).yMax;
		Rect val3 = val2;
		((Rect)(ref val3)).y = ((Rect)(ref val2)).yMax + 10f;
		((Rect)(ref val3)).height = 32f;
		((Rect)(ref val3)).width = 200f;
		Rect val4 = val3;
		((Rect)(ref val4)).x = ((Rect)(ref val3)).xMax + 10f;
		((Rect)(ref val4)).xMax = ((Rect)(ref inRect)).xMax;
		Rect rect = val4;
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 100f;
		Rect val5 = val4;
		((Rect)(ref val5)).x = ((Rect)(ref val4)).xMax - ((Rect)(ref val4)).height;
		((Rect)(ref val5)).width = ((Rect)(ref val4)).height;
		Rect val6 = val5;
		((Rect)(ref val6)).x = ((Rect)(ref val5)).x - ((Rect)(ref val4)).height - 10f;
		Rect val7 = val6;
		((Rect)(ref val7)).x = ((Rect)(ref val6)).x - ((Rect)(ref val4)).height - 10f;
		Rect val8 = val7;
		((Rect)(ref val8)).x = ((Rect)(ref val7)).x - ((Rect)(ref val4)).height - 10f;
		Rect leftRect = val3;
		((Rect)(ref leftRect)).y = ((Rect)(ref val3)).yMax + OffsetHeaderY + 4f;
		((Rect)(ref leftRect)).yMax = ((Rect)(ref inRect)).yMax - Window.CloseButSize.y - 10f;
		Rect rect2 = val4;
		((Rect)(ref rect2)).y = ((Rect)(ref leftRect)).y;
		((Rect)(ref rect2)).yMax = ((Rect)(ref leftRect)).yMax;
		Rect rect3 = val4;
		((Rect)(ref rect3)).yMax = ((Rect)(ref rect2)).yMax;
		using (new TextBlock(GameFont.Medium))
		{
			Widgets.Label(val, TitleKey.Translate());
		}
		Text.Font = GameFont.Small;
		Widgets.Label(val2, TipKey.Translate());
		using (new TextBlock(GameFont.Medium))
		{
			Widgets.Label(val3, "AvailablePolicies".Translate());
		}
		DoPolicyListing(leftRect);
		if (SelectedPolicy != null)
		{
			DoContentsRect(rect2);
			bool flag = GetDefaultPolicy() == SelectedPolicy;
			string text = SelectedPolicy.label;
			if (flag)
			{
				text += string.Format(" ({0})", "default".Translate()).Colorize(Color.gray);
			}
			using (new TextBlock(GameFont.Medium))
			{
				Widgets.LabelEllipses(rect, text);
			}
			TooltipHandler.TipRegionByKey(val5, "DeletePolicyTip");
			TooltipHandler.TipRegionByKey(val6, "DuplicatePolicyTip");
			TooltipHandler.TipRegionByKey(val7, "RenamePolicyTip");
			if (!flag)
			{
				TooltipHandler.TipRegionByKey(val8, "DefaultPolicyTip");
			}
			if (Widgets.ButtonImage(val5, TexUI.DismissTex))
			{
				if (Input.GetKey((KeyCode)306))
				{
					DeletePolicy();
				}
				else
				{
					TaggedString taggedString = "DeletePolicyConfirm".Translate(SelectedPolicy.label);
					TaggedString taggedString2 = "DeletePolicyConfirmButton".Translate();
					Find.WindowStack.Add(new Dialog_Confirm(taggedString, taggedString2, DeletePolicy));
				}
			}
			if (Widgets.ButtonImage(val6, TexUI.CopyTex))
			{
				T val9 = CreateNewPolicy();
				val9.CopyFrom(SelectedPolicy);
				SelectedPolicy = val9;
			}
			if (Widgets.ButtonImage(val7, TexUI.RenameTex))
			{
				Find.WindowStack.Add(new Dialog_RenamePolicy(SelectedPolicy));
			}
			if (!flag && Widgets.ButtonImage(val8, TexUI.MakeDefault))
			{
				SetDefaultPolicy(SelectedPolicy);
			}
			return;
		}
		using (new TextBlock(GameFont.Medium, (TextAnchor)4, Color.gray))
		{
			Widgets.Label(rect3, "NoPolicySelected".Translate());
		}
	}

	private void DeletePolicy()
	{
		AcceptanceReport acceptanceReport = TryDeletePolicy(SelectedPolicy);
		if (!acceptanceReport.Accepted)
		{
			Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, historical: false);
		}
		else
		{
			SelectedPolicy = null;
		}
	}

	private void DoPolicyListing(Rect leftRect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = leftRect;
		((Rect)(ref rect)).y = ((Rect)(ref leftRect)).yMax - 24f;
		((Rect)(ref rect)).height = 24f;
		Rect val = leftRect;
		((Rect)(ref val)).yMax = ((Rect)(ref rect)).y - 10f;
		Rect rect2 = val;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 10f;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 10f;
		((Rect)(ref rect2)).y = ((Rect)(ref val)).yMax - Window.CloseButSize.y - 10f;
		((Rect)(ref rect2)).height = Window.CloseButSize.y;
		Rect outRect = val;
		((Rect)(ref outRect)).yMax = ((Rect)(ref rect2)).y - 10f;
		quickSearch.OnGUI(rect);
		Widgets.DrawMenuSection(val);
		if (Widgets.ButtonText(rect2, "NewPolicy".Translate()))
		{
			SelectedPolicy = CreateNewPolicy();
		}
		int num = 0;
		foreach (T policy in GetPolicies())
		{
			if (quickSearch.filter.Matches(policy.label))
			{
				num++;
			}
		}
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width, (float)num * 32f);
		Widgets.AdjustRectsForScrollView(val, ref outRect, ref viewRect);
		Widgets.BeginScrollView(outRect, ref scroll, viewRect);
		float num2 = 0f;
		int num3 = 0;
		T defaultPolicy = GetDefaultPolicy();
		Rect val2 = default(Rect);
		foreach (T item in from x in GetPolicies()
			orderby defaultPolicy != x, x.label
			select x)
		{
			if (quickSearch.filter.Matches(item.label))
			{
				((Rect)(ref val2))._002Ector(0f, num2, ((Rect)(ref outRect)).width, 32f);
				Rect rect3 = val2;
				((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x + 10f;
				num2 += 32f;
				if (SelectedPolicy == item)
				{
					Widgets.DrawHighlightSelected(val2);
				}
				else if (Mouse.IsOver(val2))
				{
					Widgets.DrawHighlight(val2);
				}
				else if (num3 % 2 == 1)
				{
					Widgets.DrawLightHighlight(val2);
				}
				num3++;
				string text = item.label;
				if (defaultPolicy == item)
				{
					text += "*".Colorize(Color.gray);
				}
				using (new TextBlock((TextAnchor)3))
				{
					Widgets.Label(rect3, text);
				}
				if (Widgets.ButtonInvisible(val2))
				{
					SelectedPolicy = item;
				}
			}
		}
		Widgets.EndScrollView();
	}

	protected abstract T CreateNewPolicy();

	protected abstract T GetDefaultPolicy();

	protected abstract void SetDefaultPolicy(T policy);

	protected abstract AcceptanceReport TryDeletePolicy(T policy);

	protected abstract List<T> GetPolicies();

	protected abstract void DoContentsRect(Rect rect);

	public override void PreClose()
	{
		base.PreClose();
		ValidateName();
	}
}
