using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public abstract class Need : IExposable
{
	public NeedDef def;

	protected readonly Pawn pawn;

	protected float curLevelInt;

	protected List<float> threshPercents;

	private CompCanBeDormant intDormant;

	public const float MaxDrawHeight = 70f;

	private static readonly Texture2D BarInstantMarkerTex = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarker");

	private static readonly Texture2D NeedUnitDividerTex = ContentFinder<Texture2D>.Get("UI/Misc/NeedUnitDivider");

	private const float BarInstantMarkerSize = 12f;

	public string LabelCap => def.LabelCap;

	public float CurInstantLevelPercentage => CurInstantLevel / MaxLevel;

	public virtual int GUIChangeArrow => 0;

	public virtual float CurInstantLevel => -1f;

	public virtual float MaxLevel => 1f;

	public virtual float CurLevel
	{
		get
		{
			return curLevelInt;
		}
		set
		{
			curLevelInt = Mathf.Clamp(value, 0f, MaxLevel);
		}
	}

	public float CurLevelPercentage
	{
		get
		{
			return CurLevel / MaxLevel;
		}
		set
		{
			CurLevel = value * MaxLevel;
		}
	}

	protected virtual bool IsFrozen
	{
		get
		{
			if (pawn.Suspended)
			{
				return true;
			}
			if (def.freezeWhileSleeping && !pawn.Awake())
			{
				return true;
			}
			if (def.freezeInMentalState && pawn.InMentalState)
			{
				return true;
			}
			if (NeedFrozenFromDormanancy())
			{
				return true;
			}
			return !IsPawnInteractableOrVisible;
		}
	}

	private bool IsPawnInteractableOrVisible
	{
		get
		{
			if (pawn.SpawnedOrAnyParentSpawned)
			{
				return true;
			}
			if (pawn.IsCaravanMember())
			{
				return true;
			}
			if (PawnUtility.IsTravelingInTransportPodWorldObject(pawn))
			{
				return true;
			}
			return false;
		}
	}

	public virtual bool ShowOnNeedList => def.showOnNeedList;

	private bool NeedFrozenFromDormanancy()
	{
		if (intDormant == null)
		{
			return false;
		}
		if (intDormant.Awake)
		{
			return false;
		}
		return intDormant.Props.freezeNeeds.Contains(def);
	}

	public Need(Pawn newPawn)
	{
		pawn = newPawn;
		SetInitialLevel();
		intDormant = pawn.TryGetComp<CompCanBeDormant>();
	}

	public virtual void ExposeData()
	{
		Scribe_Defs.Look(ref def, "def");
		Scribe_Values.Look(ref curLevelInt, "curLevel", 0f);
	}

	public abstract void NeedInterval();

	public virtual string GetTipString()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		string text = (LabelCap + ": " + CurLevelPercentage.ToStringPercent()).Colorize(ColoredText.TipSectionTitleColor) + "\n" + def.description;
		Gene gene;
		Hediff hediff;
		if (pawn.story?.traits != null && pawn.story.traits.TryGetNeedEnablingTrait(def, out var trait))
		{
			text += string.Format("\n\n{0}: {1}", "ComesFromTrait".Translate(), trait.LabelCap);
		}
		else if (pawn.genes != null && pawn.genes.TryGetNeedEnablingGene(def, out gene))
		{
			text += string.Format("\n\n{0}: {1}", "ComesFromGene".Translate(), gene.LabelCap);
		}
		else if (pawn.Ideo != null && pawn.Ideo.EnablesNeed(def))
		{
			text += string.Format("\n\n{0}: {1}", "ComesFromIdeo".Translate(), pawn.Ideo.name.CapitalizeFirst());
		}
		else if (pawn.health != null && pawn.health.hediffSet.TryGetNeedEnablingHediff(def, out hediff))
		{
			text += string.Format("\n\n{0}: {1}", "ComesFromHediff".Translate(), hediff.LabelCap);
		}
		return text;
	}

	public virtual void SetInitialLevel()
	{
		CurLevelPercentage = 0.5f;
	}

	public virtual void OnNeedRemoved()
	{
	}

	protected virtual void OffsetDebugPercent(float offsetPercent)
	{
		CurLevelPercentage += offsetPercent;
	}

	public virtual void DrawOnGUI(Rect rect, int maxThresholdMarkers = int.MaxValue, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true, Rect? rectForTooltip = null, bool drawLabel = true)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		if (((Rect)(ref rect)).height > 70f)
		{
			float num = (((Rect)(ref rect)).height - 70f) / 2f;
			((Rect)(ref rect)).height = 70f;
			((Rect)(ref rect)).y = ((Rect)(ref rect)).y + num;
		}
		Rect rect2 = (Rect)(((_003F?)rectForTooltip) ?? rect);
		if (Mouse.IsOver(rect2))
		{
			Widgets.DrawHighlight(rect2);
		}
		if (doTooltip && Mouse.IsOver(rect2))
		{
			TooltipHandler.TipRegion(rect2, new TipSignal(() => GetTipString(), ((object)(Rect)(ref rect2)).GetHashCode()));
		}
		float num2 = 14f;
		float num3 = ((customMargin >= 0f) ? customMargin : (num2 + 15f));
		if (((Rect)(ref rect)).height < 50f)
		{
			num2 *= Mathf.InverseLerp(0f, 50f, ((Rect)(ref rect)).height);
		}
		if (drawLabel)
		{
			Text.Font = ((((Rect)(ref rect)).height > 55f) ? GameFont.Small : GameFont.Tiny);
			Text.Anchor = (TextAnchor)6;
			Widgets.Label(new Rect(((Rect)(ref rect)).x + num3 + ((Rect)(ref rect)).width * 0.1f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - num3 - ((Rect)(ref rect)).width * 0.1f, ((Rect)(ref rect)).height / 2f), LabelCap);
			Text.Anchor = (TextAnchor)0;
		}
		Rect val = rect;
		if (drawLabel)
		{
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref rect)).height / 2f;
			((Rect)(ref val)).height = ((Rect)(ref val)).height - ((Rect)(ref rect)).height / 2f;
		}
		((Rect)(ref val))._002Ector(((Rect)(ref val)).x + num3, ((Rect)(ref val)).y, ((Rect)(ref val)).width - num3 * 2f, ((Rect)(ref val)).height - num2);
		if (DebugSettings.ShowDevGizmos)
		{
			float lineHeight = Text.LineHeight;
			Rect rect3 = default(Rect);
			((Rect)(ref rect3))._002Ector(((Rect)(ref val)).xMax - lineHeight, ((Rect)(ref val)).y - lineHeight, lineHeight, lineHeight);
			if (Widgets.ButtonImage(rect3.ContractedBy(4f), TexButton.Plus))
			{
				OffsetDebugPercent(0.1f);
			}
			if (Mouse.IsOver(rect3))
			{
				TooltipHandler.TipRegion(rect3, "+ 10%");
			}
			Rect rect4 = default(Rect);
			((Rect)(ref rect4))._002Ector(((Rect)(ref rect3)).xMin - lineHeight, ((Rect)(ref val)).y - lineHeight, lineHeight, lineHeight);
			if (Widgets.ButtonImage(rect4.ContractedBy(4f), TexButton.Minus))
			{
				OffsetDebugPercent(-0.1f);
			}
			if (Mouse.IsOver(rect4))
			{
				TooltipHandler.TipRegion(rect4, "- 10%");
			}
		}
		Rect val2 = val;
		float num4 = 1f;
		if (def.scaleBar && MaxLevel < 1f)
		{
			num4 = MaxLevel;
		}
		((Rect)(ref val2)).width = ((Rect)(ref val2)).width * num4;
		Rect barRect = Widgets.FillableBar(val2, CurLevelPercentage);
		if (drawArrows)
		{
			Widgets.FillableBarChangeArrows(val2, GUIChangeArrow);
		}
		if (threshPercents != null)
		{
			for (int i = 0; i < Mathf.Min(threshPercents.Count, maxThresholdMarkers); i++)
			{
				DrawBarThreshold(barRect, threshPercents[i] * num4);
			}
		}
		if (def.showUnitTicks)
		{
			for (int j = 1; (float)j < MaxLevel; j++)
			{
				DrawBarDivision(barRect, (float)j / MaxLevel * num4);
			}
		}
		float curInstantLevelPercentage = CurInstantLevelPercentage;
		if (curInstantLevelPercentage >= 0f)
		{
			DrawBarInstantMarkerAt(val, curInstantLevelPercentage * num4);
		}
		if (!def.tutorHighlightTag.NullOrEmpty())
		{
			UIHighlighter.HighlightOpportunity(rect, def.tutorHighlightTag);
		}
		Text.Font = GameFont.Small;
	}

	protected void DrawBarInstantMarkerAt(Rect barRect, float pct)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (pct > 1f)
		{
			Log.ErrorOnce(def?.ToString() + " drawing bar percent > 1 : " + pct, 6932178);
		}
		float num = 12f;
		if (((Rect)(ref barRect)).width < 150f)
		{
			num /= 2f;
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref barRect)).x + ((Rect)(ref barRect)).width * pct, ((Rect)(ref barRect)).y + ((Rect)(ref barRect)).height);
		GUI.DrawTexture(new Rect(val.x - num / 2f, val.y, num, num), (Texture)(object)BarInstantMarkerTex);
	}

	protected void DrawBarThreshold(Rect barRect, float threshPct)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		float num = ((!(((Rect)(ref barRect)).width > 60f)) ? 1 : 2);
		Rect val = new Rect(((Rect)(ref barRect)).x + ((Rect)(ref barRect)).width * threshPct - (num - 1f), ((Rect)(ref barRect)).y + ((Rect)(ref barRect)).height / 2f, num, ((Rect)(ref barRect)).height / 2f);
		Texture2D val2;
		if (threshPct < CurLevelPercentage)
		{
			val2 = BaseContent.BlackTex;
			GUI.color = new Color(1f, 1f, 1f, 0.9f);
		}
		else
		{
			val2 = BaseContent.GreyTex;
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
		}
		GUI.DrawTexture(val, (Texture)(object)val2);
		GUI.color = Color.white;
	}

	private void DrawBarDivision(Rect barRect, float threshPct)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		float num = 5f;
		Rect val = new Rect(((Rect)(ref barRect)).x + ((Rect)(ref barRect)).width * threshPct - (num - 1f), ((Rect)(ref barRect)).y, num, ((Rect)(ref barRect)).height);
		if (threshPct < CurLevelPercentage)
		{
			GUI.color = new Color(0f, 0f, 0f, 0.9f);
		}
		else
		{
			GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		}
		Rect val2 = val;
		((Rect)(ref val2)).yMax = ((Rect)(ref val2)).yMin + 4f;
		GUI.DrawTextureWithTexCoords(val2, (Texture)(object)NeedUnitDividerTex, new Rect(0f, 0.5f, 1f, 0.5f));
		Rect val3 = val;
		((Rect)(ref val3)).yMin = ((Rect)(ref val3)).yMax - 4f;
		GUI.DrawTextureWithTexCoords(val3, (Texture)(object)NeedUnitDividerTex, new Rect(0f, 0f, 1f, 0.5f));
		Rect val4 = val;
		((Rect)(ref val4)).yMin = ((Rect)(ref val2)).yMax;
		((Rect)(ref val4)).yMax = ((Rect)(ref val3)).yMin;
		if (((Rect)(ref val4)).height > 0f)
		{
			GUI.DrawTextureWithTexCoords(val4, (Texture)(object)NeedUnitDividerTex, new Rect(0f, 0.4f, 1f, 0.2f));
		}
		GUI.color = Color.white;
	}
}
