using System;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class ITab_ContentsBase : ITab
{
	private Vector2 scrollPosition;

	private float lastDrawnHeight;

	private List<Thing> thingsToSelect = new List<Thing>();

	public bool canRemoveThings = true;

	protected static List<Thing> tmpSingleThing = new List<Thing>();

	protected const float TopPadding = 20f;

	protected const float SpaceBetweenItemsLists = 10f;

	protected const float ThingRowHeight = 28f;

	protected const float ThingIconSize = 28f;

	protected const float ThingLeftX = 36f;

	protected static readonly Color ThingLabelColor = ITab_Pawn_Gear.ThingLabelColor;

	protected static readonly Color ThingHighlightColor = ITab_Pawn_Gear.HighlightColor;

	public string containedItemsKey;

	public abstract IList<Thing> container { get; }

	public override bool IsVisible => base.SelThing.Faction == Faction.OfPlayer;

	public virtual IntVec3 DropOffset => IntVec3.Zero;

	public virtual bool UseDiscardMessage => true;

	public ITab_ContentsBase()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(460f, 450f);
	}

	protected override void FillTab()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		thingsToSelect.Clear();
		Rect outRect = GenUI.ContractedBy(new Rect(default(Vector2), size), 10f);
		((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + 20f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, Mathf.Max(lastDrawnHeight, ((Rect)(ref outRect)).height));
		Text.Font = GameFont.Small;
		Widgets.BeginScrollView(outRect, ref scrollPosition, val);
		float curY = 0f;
		DoItemsLists(val, ref curY);
		lastDrawnHeight = curY;
		Widgets.EndScrollView();
		if (thingsToSelect.Any())
		{
			ITab_Pawn_FormingCaravan.SelectNow(thingsToSelect);
			thingsToSelect.Clear();
		}
	}

	protected virtual void DoItemsLists(Rect inRect, ref float curY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(inRect);
		Widgets.ListSeparator(ref curY, ((Rect)(ref inRect)).width, containedItemsKey.Translate());
		IList<Thing> list = container;
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			Thing t = list[i];
			if (t != null)
			{
				flag = true;
				tmpSingleThing.Clear();
				tmpSingleThing.Add(t);
				DoThingRow(t.def, t.stackCount, tmpSingleThing, ((Rect)(ref inRect)).width, ref curY, delegate(int x)
				{
					OnDropThing(t, x);
				});
				tmpSingleThing.Clear();
			}
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref inRect)).width);
		}
		Widgets.EndGroup();
	}

	protected virtual void OnDropThing(Thing t, int count)
	{
		GenDrop.TryDropSpawn(t.SplitOff(count), base.SelThing.Position + DropOffset, base.SelThing.Map, ThingPlaceMode.Near, out var _);
	}

	protected void DoThingRow(ThingDef thingDef, int count, List<Thing> things, float width, ref float curY, Action<int> discardAction)
	{
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, width, 28f);
		if (canRemoveThings)
		{
			if (count != 1 && Widgets.ButtonImage(new Rect(((Rect)(ref val)).x + ((Rect)(ref val)).width - 24f, ((Rect)(ref val)).y + (((Rect)(ref val)).height - 24f) / 2f, 24f, 24f), CaravanThingsTabUtility.AbandonSpecificCountButtonTex))
			{
				Find.WindowStack.Add(new Dialog_Slider("RemoveSliderText".Translate(thingDef.label), 1, count, discardAction));
			}
			((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
			if (Widgets.ButtonImage(new Rect(((Rect)(ref val)).x + ((Rect)(ref val)).width - 24f, ((Rect)(ref val)).y + (((Rect)(ref val)).height - 24f) / 2f, 24f, 24f), CaravanThingsTabUtility.AbandonButtonTex))
			{
				if (UseDiscardMessage)
				{
					string text = thingDef.label;
					if (things.Count == 1 && things[0] is Pawn)
					{
						text = ((Pawn)things[0]).LabelShortCap;
					}
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmRemoveItemDialog".Translate(text), delegate
					{
						discardAction(count);
					}));
				}
				else
				{
					discardAction(count);
				}
			}
			((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		}
		if (things.Count == 1)
		{
			Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, curY, things[0]);
		}
		else
		{
			Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, curY, thingDef);
		}
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		if (Mouse.IsOver(val))
		{
			GUI.color = ThingHighlightColor;
			GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
		}
		if ((Object)(object)thingDef.DrawMatSingle != (Object)null && (Object)(object)thingDef.DrawMatSingle.mainTexture != (Object)null)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(4f, curY, 28f, 28f);
			if (things.Count == 1)
			{
				Widgets.ThingIcon(rect, things[0]);
			}
			else
			{
				Widgets.ThingIcon(rect, thingDef);
			}
		}
		Text.Anchor = (TextAnchor)3;
		GUI.color = ThingLabelColor;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(36f, curY, ((Rect)(ref val)).width - 36f, ((Rect)(ref val)).height);
		string text2 = ((things.Count == 1 && count == things[0].stackCount) ? things[0].LabelCap : ((thingDef != ThingDefOf.MinifiedThing) ? GenLabel.ThingLabel(thingDef, null, count).CapitalizeFirst() : GenLabel.ThingLabel(things[0].GetInnerIfMinified(), count).CapitalizeFirst()));
		Text.WordWrap = false;
		Widgets.Label(rect2, text2.StripTags().Truncate(((Rect)(ref rect2)).width));
		Text.WordWrap = true;
		Text.Anchor = (TextAnchor)0;
		TooltipHandler.TipRegion(val, text2);
		if (Widgets.ButtonInvisible(val))
		{
			SelectLater(things);
		}
		if (Mouse.IsOver(val))
		{
			for (int i = 0; i < things.Count; i++)
			{
				TargetHighlighter.Highlight(things[i]);
			}
		}
		curY += 28f;
	}

	private void SelectLater(List<Thing> things)
	{
		thingsToSelect.Clear();
		thingsToSelect.AddRange(things);
	}
}
