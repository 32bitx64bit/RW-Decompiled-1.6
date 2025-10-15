using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld;

public abstract class Bill : IExposable, ILoadReferenceable
{
	[Unsaved(false)]
	public BillStack billStack;

	private int loadID = -1;

	public RecipeDef recipe;

	public Precept_ThingStyle precept;

	public ThingStyleDef style;

	public bool globalStyle = true;

	public int? graphicIndexOverride;

	public Xenogerm xenogerm;

	public bool suspended;

	public ThingFilter ingredientFilter;

	public float ingredientSearchRadius = 999f;

	public IntRange allowedSkillRange = new IntRange(0, 20);

	private Pawn pawnRestriction;

	private bool slavesOnly;

	private bool mechsOnly;

	private bool nonMechsOnly;

	public bool deleted;

	public int nextTickToSearchForIngredients;

	public const int MaxIngredientSearchRadius = 999;

	public const float ButSize = 24f;

	private const float InterfaceBaseHeight = 53f;

	private const float InterfaceStatusLineHeight = 17f;

	public Map Map
	{
		get
		{
			object obj = billStack?.billGiver?.Map;
			if (obj == null)
			{
				Thing obj2 = billStack?.billGiver as Thing;
				if (obj2 == null)
				{
					return null;
				}
				obj = obj2.MapHeld;
			}
			return (Map)obj;
		}
	}

	public virtual string Label
	{
		get
		{
			if (precept == null)
			{
				return recipe.label;
			}
			return GenText.UncapitalizeFirst("RecipeMake".Translate(precept.LabelCap));
		}
	}

	public virtual string LabelCap
	{
		get
		{
			if (precept == null)
			{
				return Label.CapitalizeFirst(recipe);
			}
			return GenText.CapitalizeFirst("RecipeMake".Translate(precept.LabelCap));
		}
	}

	public virtual bool CheckIngredientsIfSociallyProper => true;

	public virtual bool CompletableEver => true;

	protected virtual string StatusString => null;

	protected virtual float StatusLineMinHeight => 0f;

	protected virtual bool CanCopy => true;

	public virtual bool CanFinishNow => true;

	public bool DeletedOrDereferenced
	{
		get
		{
			if (deleted)
			{
				return true;
			}
			if (billStack.billGiver is Thing { Destroyed: not false })
			{
				return true;
			}
			return false;
		}
	}

	public Pawn PawnRestriction => pawnRestriction;

	public bool SlavesOnly => slavesOnly;

	public bool MechsOnly => mechsOnly;

	public bool NonMechsOnly => nonMechsOnly;

	protected virtual Color BaseColor
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (ShouldDoNow())
			{
				return Color.white;
			}
			return new Color(1f, 0.7f, 0.7f, 0.7f);
		}
	}

	public Bill()
	{
	}

	public Bill(RecipeDef recipe, Precept_ThingStyle precept = null)
	{
		this.recipe = recipe;
		this.precept = precept;
		ingredientFilter = new ThingFilter();
		ingredientFilter.CopyAllowancesFrom(recipe.defaultIngredientFilter);
		InitializeAfterClone();
	}

	public void InitializeAfterClone()
	{
		loadID = Find.UniqueIDsManager.GetNextBillID();
	}

	public void SetPawnRestriction(Pawn pawn)
	{
		pawnRestriction = pawn;
		slavesOnly = false;
		mechsOnly = false;
		nonMechsOnly = false;
	}

	public void SetAnySlaveRestriction()
	{
		pawnRestriction = null;
		slavesOnly = true;
		mechsOnly = false;
		nonMechsOnly = false;
	}

	public void SetAnyPawnRestriction()
	{
		slavesOnly = false;
		pawnRestriction = null;
		mechsOnly = false;
		nonMechsOnly = false;
	}

	public void SetAnyMechRestriction()
	{
		slavesOnly = false;
		pawnRestriction = null;
		mechsOnly = true;
		nonMechsOnly = false;
	}

	public void SetAnyNonMechRestriction()
	{
		slavesOnly = false;
		pawnRestriction = null;
		mechsOnly = false;
		nonMechsOnly = true;
	}

	public virtual void ExposeData()
	{
		Scribe_Values.Look(ref loadID, "loadID", 0);
		Scribe_Defs.Look(ref recipe, "recipe");
		Scribe_Values.Look(ref suspended, "suspended", defaultValue: false);
		Scribe_Values.Look(ref ingredientSearchRadius, "ingredientSearchRadius", 999f);
		Scribe_Values.Look(ref allowedSkillRange, "allowedSkillRange");
		Scribe_References.Look(ref pawnRestriction, "pawnRestriction");
		Scribe_References.Look(ref precept, "precept");
		Scribe_References.Look(ref xenogerm, "xenogerm");
		Scribe_Values.Look(ref slavesOnly, "slavesOnly", defaultValue: false);
		Scribe_Values.Look(ref mechsOnly, "mechsOnly", defaultValue: false);
		Scribe_Values.Look(ref nonMechsOnly, "nonMechsOnly", defaultValue: false);
		Scribe_Defs.Look(ref style, "style");
		Scribe_Values.Look(ref globalStyle, "globalStyle", defaultValue: true);
		Scribe_Values.Look(ref graphicIndexOverride, "graphicIndexOverride");
		if (Scribe.mode == LoadSaveMode.Saving && recipe.fixedIngredientFilter != null)
		{
			foreach (ThingDef allDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (!recipe.fixedIngredientFilter.Allows(allDef))
				{
					ingredientFilter.SetAllow(allDef, allow: false);
				}
			}
		}
		Scribe_Deep.Look(ref ingredientFilter, "ingredientFilter");
	}

	public virtual bool PawnAllowedToStartAnew(Pawn p)
	{
		if (pawnRestriction != null)
		{
			return pawnRestriction == p;
		}
		if (slavesOnly && !p.IsSlave)
		{
			return false;
		}
		if (mechsOnly && !p.IsColonyMechPlayerControlled)
		{
			return false;
		}
		if (nonMechsOnly && p.IsColonyMechPlayerControlled)
		{
			return false;
		}
		if (recipe.workSkill != null && (p.skills != null || p.IsColonyMech))
		{
			int num = ((p.skills != null) ? p.skills.GetSkill(recipe.workSkill).Level : p.RaceProps.mechFixedSkillLevel);
			if (num < allowedSkillRange.min)
			{
				JobFailReason.Is("UnderAllowedSkill".Translate(allowedSkillRange.min), Label);
				return false;
			}
			if (num > allowedSkillRange.max)
			{
				JobFailReason.Is("AboveAllowedSkill".Translate(allowedSkillRange.max), Label);
				return false;
			}
		}
		if (ModsConfig.BiotechActive && recipe.mechanitorOnlyRecipe && !MechanitorUtility.IsMechanitor(p))
		{
			JobFailReason.Is("NotAMechanitor".Translate());
			return false;
		}
		return true;
	}

	public virtual void Notify_PawnDidWork(Pawn p)
	{
	}

	public virtual void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
	{
	}

	public abstract bool ShouldDoNow();

	public virtual void Notify_DoBillStarted(Pawn billDoer)
	{
	}

	public virtual void Notify_BillWorkStarted(Pawn billDoer)
	{
	}

	public virtual void Notify_BillWorkFinished(Pawn billDoer)
	{
	}

	protected virtual void DoConfigInterface(Rect rect, Color baseColor)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 29f;
		float y = ((Rect)(ref rect)).center.y;
		Widgets.InfoCardButton(((Rect)(ref rect)).xMax - (((Rect)(ref rect)).yMax - y) - 12f, y - 12f, recipe);
	}

	public virtual void DoStatusLineInterface(Rect rect)
	{
	}

	public Rect DoInterface(float x, float y, float width, int index)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(x, y, width, 53f);
		float num = 0f;
		if (!StatusString.NullOrEmpty())
		{
			num = Mathf.Max(Text.TinyFontSupported ? 17f : 21f, StatusLineMinHeight);
		}
		((Rect)(ref val)).height = ((Rect)(ref val)).height + num;
		Color val2 = (GUI.color = BaseColor);
		Text.Font = GameFont.Small;
		if (index % 2 == 0)
		{
			Widgets.DrawAltRect(val);
		}
		Widgets.BeginGroup(val);
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(0f, 0f, 24f, 24f);
		if (billStack.IndexOf(this) > 0)
		{
			if (Widgets.ButtonImage(val3, TexButton.ReorderUp, val2))
			{
				billStack.Reorder(this, -1);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			TooltipHandler.TipRegionByKey(val3, "ReorderBillUpTip");
		}
		if (billStack.IndexOf(this) < billStack.Count - 1)
		{
			Rect val4 = new Rect(0f, 24f, 24f, 24f);
			if (Widgets.ButtonImage(val4, TexButton.ReorderDown, val2))
			{
				billStack.Reorder(this, 1);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
			TooltipHandler.TipRegionByKey(val4, "ReorderBillDownTip");
		}
		GUI.color = val2;
		Widgets.Label(new Rect(28f, 0f, ((Rect)(ref val)).width - 48f - 20f, ((Rect)(ref val)).height + 5f), LabelCap);
		DoConfigInterface(val.AtZero(), val2);
		Rect val5 = default(Rect);
		((Rect)(ref val5))._002Ector(((Rect)(ref val)).width - 24f, 0f, 24f, 24f);
		if (Widgets.ButtonImage(val5, TexButton.Delete, val2, val2 * GenUI.SubtleMouseoverColor))
		{
			billStack.Delete(this);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		TooltipHandler.TipRegionByKey(val5, "DeleteBillTip");
		Rect val7 = default(Rect);
		if (CanCopy)
		{
			Rect val6 = default(Rect);
			((Rect)(ref val6))._002Ector(val5);
			((Rect)(ref val6)).x = ((Rect)(ref val6)).x - (((Rect)(ref val6)).width + 4f);
			if (Widgets.ButtonImageFitted(val6, TexButton.Copy, val2))
			{
				BillUtility.Clipboard = Clone();
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			TooltipHandler.TipRegionByKey(val6, "CopyBillTip");
			((Rect)(ref val7))._002Ector(val6);
		}
		else
		{
			((Rect)(ref val7))._002Ector(val5);
		}
		((Rect)(ref val7)).x = ((Rect)(ref val7)).x - ((Rect)(ref val7)).width;
		if (Widgets.ButtonImage(val7, TexButton.Suspend, val2))
		{
			suspended = !suspended;
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		TooltipHandler.TipRegionByKey(val7, "SuspendBillTip");
		if (!StatusString.NullOrEmpty())
		{
			Text.Font = GameFont.Tiny;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(24f, ((Rect)(ref val)).height - num, ((Rect)(ref val)).width - 24f, num);
			Widgets.Label(rect, StatusString);
			DoStatusLineInterface(rect);
		}
		Widgets.EndGroup();
		if (suspended)
		{
			Text.Font = GameFont.Medium;
			Text.Anchor = (TextAnchor)4;
			Rect val8 = new Rect(((Rect)(ref val)).x + ((Rect)(ref val)).width / 2f - 70f, ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f - 20f, 140f, 40f);
			GUI.DrawTexture(val8, (Texture)(object)TexUI.GrayTextBG);
			Widgets.Label(val8, "SuspendedCaps".Translate());
			Text.Anchor = (TextAnchor)0;
			Text.Font = GameFont.Small;
		}
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		return val;
	}

	public virtual bool IsFixedOrAllowedIngredient(Thing thing)
	{
		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			IngredientCount ingredientCount = recipe.ingredients[i];
			if (ingredientCount.IsFixedIngredient && ingredientCount.filter.Allows(thing))
			{
				return true;
			}
		}
		if (recipe.fixedIngredientFilter.Allows(thing))
		{
			return ingredientFilter.Allows(thing);
		}
		return false;
	}

	public bool IsFixedOrAllowedIngredient(ThingDef def)
	{
		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			IngredientCount ingredientCount = recipe.ingredients[i];
			if (ingredientCount.IsFixedIngredient && ingredientCount.filter.Allows(def))
			{
				return true;
			}
		}
		if (recipe.fixedIngredientFilter.Allows(def))
		{
			return ingredientFilter.Allows(def);
		}
		return false;
	}

	public static void CreateNoPawnsWithSkillDialog(RecipeDef recipe)
	{
		string text = "RecipeRequiresSkills".Translate(recipe.LabelCap);
		text += "\n\n";
		text += recipe.MinSkillString;
		Find.WindowStack.Add(new Dialog_MessageBox(text));
	}

	public virtual BillStoreModeDef GetStoreMode()
	{
		return BillStoreModeDefOf.BestStockpile;
	}

	public virtual ISlotGroup GetSlotGroup()
	{
		return null;
	}

	public virtual void SetStoreMode(BillStoreModeDef mode, ISlotGroup group = null)
	{
		Log.ErrorOnce("Tried to set store mode of a non-production bill", 27190980);
	}

	public virtual float GetWorkAmount(Thing thing = null)
	{
		return recipe.WorkAmountTotal(thing);
	}

	public virtual Bill Clone()
	{
		Bill obj = (Bill)Activator.CreateInstance(GetType());
		obj.recipe = recipe;
		obj.precept = precept;
		obj.style = style;
		obj.globalStyle = globalStyle;
		obj.suspended = suspended;
		obj.ingredientFilter = new ThingFilter();
		obj.ingredientFilter.CopyAllowancesFrom(ingredientFilter);
		obj.ingredientSearchRadius = ingredientSearchRadius;
		obj.allowedSkillRange = allowedSkillRange;
		obj.pawnRestriction = pawnRestriction;
		obj.slavesOnly = slavesOnly;
		obj.xenogerm = xenogerm;
		obj.mechsOnly = mechsOnly;
		obj.nonMechsOnly = nonMechsOnly;
		return obj;
	}

	public virtual void ValidateSettings()
	{
		if (pawnRestriction != null && (pawnRestriction.Dead || pawnRestriction.Faction != Faction.OfPlayer || pawnRestriction.IsKidnapped()))
		{
			if (this != BillUtility.Clipboard)
			{
				Messages.Message("MessageBillValidationPawnUnavailable".Translate(pawnRestriction.LabelShortCap, Label, billStack.billGiver.LabelShort), billStack.billGiver as Thing, MessageTypeDefOf.NegativeEvent);
			}
			pawnRestriction = null;
		}
	}

	public string GetUniqueLoadID()
	{
		return "Bill_" + recipe.defName + "_" + loadID;
	}

	public override string ToString()
	{
		return GetUniqueLoadID();
	}
}
