using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet;

public class WITab_Caravan_Social : WITab
{
	private Vector2 scrollPosition;

	private float scrollViewHeight;

	private Pawn specificSocialTabForPawn;

	private const float RowHeight = 34f;

	private const float ScrollViewTopMargin = 15f;

	private const float PawnLabelHeight = 18f;

	private const float PawnLabelColumnWidth = 100f;

	private const float SpaceAroundIcon = 4f;

	private List<Pawn> Pawns => base.SelCaravan.PawnsListForReading;

	private float SpecificSocialTabWidth
	{
		get
		{
			EnsureSpecificSocialTabForPawnValid();
			if (specificSocialTabForPawn.DestroyedOrNull())
			{
				return 0f;
			}
			return 540f;
		}
	}

	public WITab_Caravan_Social()
	{
		labelKey = "TabCaravanSocial";
	}

	protected override void FillTab()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		EnsureSpecificSocialTabForPawnValid();
		Text.Font = GameFont.Small;
		Rect val = GenUI.ContractedBy(new Rect(0f, 15f, size.x, size.y - 15f), 10f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, scrollViewHeight);
		float curY = 0f;
		Widgets.BeginScrollView(val, ref scrollPosition, val2);
		DoRows(ref curY, val2, val);
		if ((int)Event.current.type == 8)
		{
			scrollViewHeight = curY + 30f;
		}
		Widgets.EndScrollView();
	}

	protected override void UpdateSize()
	{
		EnsureSpecificSocialTabForPawnValid();
		base.UpdateSize();
		size.x = 243f;
		size.y = Mathf.Min(550f, PaneTopY - 30f);
	}

	protected override void ExtraOnGUI()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		EnsureSpecificSocialTabForPawnValid();
		base.ExtraOnGUI();
		Pawn localSpecificSocialTabForPawn = specificSocialTabForPawn;
		if (localSpecificSocialTabForPawn == null)
		{
			return;
		}
		Rect tabRect = base.TabRect;
		float specificSocialTabWidth = SpecificSocialTabWidth;
		Rect rect = new Rect(((Rect)(ref tabRect)).xMax - 1f, ((Rect)(ref tabRect)).yMin, specificSocialTabWidth, ((Rect)(ref tabRect)).height);
		Find.WindowStack.ImmediateWindow(1439870015, rect, WindowLayer.GameUI, delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (!localSpecificSocialTabForPawn.DestroyedOrNull())
			{
				SocialCardUtility.DrawSocialCard(rect.AtZero(), localSpecificSocialTabForPawn);
				if (Widgets.CloseButtonFor(rect.AtZero()))
				{
					specificSocialTabForPawn = null;
					SoundDefOf.TabClose.PlayOneShotOnCamera();
				}
			}
		});
	}

	public override void OnOpen()
	{
		base.OnOpen();
		if ((specificSocialTabForPawn == null || !Pawns.Contains(specificSocialTabForPawn)) && Pawns.Any())
		{
			specificSocialTabForPawn = Pawns[0];
		}
	}

	private void DoRows(ref float curY, Rect scrollViewRect, Rect scrollOutRect)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		List<Pawn> pawns = Pawns;
		Pawn pawn = BestCaravanPawnUtility.FindBestNegotiator(base.SelCaravan);
		GUI.color = new Color(0.8f, 0.8f, 0.8f, 1f);
		Widgets.Label(new Rect(0f, curY, ((Rect)(ref scrollViewRect)).width, 24f), string.Format("{0}: {1}", "Negotiator".TranslateSimple(), (pawn != null) ? pawn.LabelShort : "NoneCapable".Translate().ToString()));
		curY += 24f;
		if (specificSocialTabForPawn != null && !pawns.Contains(specificSocialTabForPawn))
		{
			specificSocialTabForPawn = null;
		}
		bool flag = false;
		for (int i = 0; i < pawns.Count; i++)
		{
			Pawn pawn2 = pawns[i];
			if (pawn2.RaceProps.IsFlesh && pawn2.IsColonist)
			{
				if (!flag)
				{
					Widgets.ListSeparator(ref curY, ((Rect)(ref scrollViewRect)).width, "CaravanColonists".Translate());
					flag = true;
				}
				DoRow(ref curY, scrollViewRect, scrollOutRect, pawn2);
			}
		}
		bool flag2 = false;
		for (int j = 0; j < pawns.Count; j++)
		{
			Pawn pawn3 = pawns[j];
			if (pawn3.RaceProps.IsFlesh && !pawn3.IsColonist)
			{
				if (!flag2)
				{
					Widgets.ListSeparator(ref curY, ((Rect)(ref scrollViewRect)).width, "CaravanPrisonersAndAnimals".Translate());
					flag2 = true;
				}
				DoRow(ref curY, scrollViewRect, scrollOutRect, pawn3);
			}
		}
	}

	private void DoRow(ref float curY, Rect viewRect, Rect scrollOutRect, Pawn p)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float num = scrollPosition.y - 34f;
		float num2 = scrollPosition.y + ((Rect)(ref scrollOutRect)).height;
		if (curY > num && curY < num2)
		{
			DoRow(new Rect(0f, curY, ((Rect)(ref viewRect)).width, 34f), p);
		}
		curY += 34f;
	}

	private void DoRow(Rect rect, Pawn p)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect val = rect.AtZero();
		CaravanThingsTabUtility.DoAbandonButton(val, p, base.SelCaravan);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, (((Rect)(ref rect)).height - 24f) / 2f, p);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		CaravanThingsTabUtility.DoOpenSpecificTabButton(val, p, ref specificSocialTabForPawn);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		CaravanThingsTabUtility.DoOpenSpecificTabButtonInvisible(val, p, ref specificSocialTabForPawn);
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(4f, (((Rect)(ref rect)).height - 27f) / 2f, 27f, 27f);
		Widgets.ThingIcon(rect2, p);
		Rect bgRect = default(Rect);
		((Rect)(ref bgRect))._002Ector(((Rect)(ref rect2)).xMax + 4f, 8f, 100f, 18f);
		GenMapUI.DrawPawnLabel(p, bgRect, 1f, 100f, null, GameFont.Small, alwaysDrawBg: false, alignCenter: false);
		if (p.Downed && !p.ageTracker.CurLifeStage.alwaysDowned)
		{
			GUI.color = new Color(1f, 0f, 0f, 0.5f);
			Widgets.DrawLineHorizontal(0f, ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width);
			GUI.color = Color.white;
		}
		Widgets.EndGroup();
	}

	public override void Notify_ClearingAllMapsMemory()
	{
		base.Notify_ClearingAllMapsMemory();
		specificSocialTabForPawn = null;
	}

	private void EnsureSpecificSocialTabForPawnValid()
	{
		if (specificSocialTabForPawn != null && (specificSocialTabForPawn.Destroyed || !base.SelCaravan.ContainsPawn(specificSocialTabForPawn)))
		{
			specificSocialTabForPawn = null;
		}
	}
}
