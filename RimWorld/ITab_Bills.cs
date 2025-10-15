using System.Collections.Generic;
using LudeonTK;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class ITab_Bills : ITab
{
	private float viewHeight = 1000f;

	private Vector2 scrollPosition;

	private Bill mouseoverBill;

	private static readonly Vector2 WinSize = new Vector2(420f, 480f);

	[TweakValue("Interface", 0f, 128f)]
	private static float PasteX = 48f;

	[TweakValue("Interface", 0f, 128f)]
	private static float PasteY = 3f;

	[TweakValue("Interface", 0f, 32f)]
	private static float PasteSize = 24f;

	protected Building_WorkTable SelTable => (Building_WorkTable)base.SelThing;

	public ITab_Bills()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabBills";
		tutorTag = "Bills";
	}

	protected override void FillTab()
	{
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(WinSize.x - PasteX, PasteY, PasteSize, PasteSize);
		if (BillUtility.Clipboard != null)
		{
			if (!SelTable.def.AllRecipes.Contains(BillUtility.Clipboard.recipe) || !BillUtility.Clipboard.recipe.AvailableNow || !BillUtility.Clipboard.recipe.AvailableOnNow(SelTable))
			{
				GUI.color = Color.gray;
				Widgets.DrawTextureFitted(val, (Texture)(object)TexButton.Paste, 1f);
				GUI.color = Color.white;
				if (Mouse.IsOver(val))
				{
					TooltipHandler.TipRegion(val, "ClipboardBillNotAvailableHere".Translate() + ": " + BillUtility.Clipboard.LabelCap);
				}
			}
			else if (SelTable.billStack.Count >= 15)
			{
				GUI.color = Color.gray;
				Widgets.DrawTextureFitted(val, (Texture)(object)TexButton.Paste, 1f);
				GUI.color = Color.white;
				if (Mouse.IsOver(val))
				{
					TooltipHandler.TipRegion(val, "PasteBillTip".Translate() + " (" + "PasteBillTip_LimitReached".Translate() + "): " + BillUtility.Clipboard.LabelCap);
				}
			}
			else
			{
				if (Widgets.ButtonImageFitted(val, TexButton.Paste, Color.white))
				{
					Bill bill = BillUtility.Clipboard.Clone();
					bill.InitializeAfterClone();
					SelTable.billStack.AddBill(bill);
					SoundDefOf.Tick_Low.PlayOneShotOnCamera();
				}
				if (Mouse.IsOver(val))
				{
					TooltipHandler.TipRegion(val, "PasteBillTip".Translate() + ": " + BillUtility.Clipboard.LabelCap);
				}
			}
		}
		Rect rect2 = GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f);
		mouseoverBill = SelTable.billStack.DoListing(rect2, OptionsMaker, ref scrollPosition, ref viewHeight);
		List<FloatMenuOption> OptionsMaker()
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			for (int i = 0; i < SelTable.def.AllRecipes.Count; i++)
			{
				RecipeDef recipe;
				if (SelTable.def.AllRecipes[i].AvailableNow && SelTable.def.AllRecipes[i].AvailableOnNow(SelTable))
				{
					recipe = SelTable.def.AllRecipes[i];
					Add();
					foreach (Ideo allIdeo in Faction.OfPlayer.ideos.AllIdeos)
					{
						foreach (Precept_Building cachedPossibleBuilding in allIdeo.cachedPossibleBuildings)
						{
							if (cachedPossibleBuilding.ThingDef == recipe.ProducedThingDef)
							{
								Add(cachedPossibleBuilding);
							}
						}
					}
				}
				void Add(Precept_ThingStyle precept = null)
				{
					string label = ((precept != null) ? "RecipeMake".Translate(precept.LabelCap).CapitalizeFirst() : recipe.LabelCap);
					opts.Add(new FloatMenuOption(label, delegate
					{
						if (ModsConfig.BiotechActive && recipe.mechanitorOnlyRecipe && !SelTable.Map.mapPawns.FreeColonists.Any(MechanitorUtility.IsMechanitor))
						{
							Find.WindowStack.Add(new Dialog_MessageBox("RecipeRequiresMechanitor".Translate(recipe.LabelCap)));
						}
						else if (!SelTable.Map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
						{
							Bill.CreateNoPawnsWithSkillDialog(recipe);
						}
						Bill bill2 = recipe.MakeNewBill(precept);
						SelTable.billStack.AddBill(bill2);
						if (recipe.conceptLearned != null)
						{
							PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
						}
						if (TutorSystem.TutorialMode)
						{
							TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap.Resolve());
						}
					}, iconTex: recipe.UIIcon, shownItemForIcon: recipe.UIIconThing, thingStyle: null, forceBasicStyle: false, priority: MenuOptionPriority.Default, mouseoverGuiAction: delegate(Rect rect)
					{
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						BillUtility.DoBillInfoWindow(i, label, rect, recipe);
					}, revalidateClickTarget: null, extraPartWidth: 29f, extraPartOnGUI: (Rect rect) => Widgets.InfoCardButton(((Rect)(ref rect)).x + 5f, ((Rect)(ref rect)).y + (((Rect)(ref rect)).height - 24f) / 2f, recipe, precept), revalidateWorldClickTarget: null, playSelectionSound: true, orderInPriority: -recipe.displayPriority));
				}
			}
			if (!opts.Any())
			{
				opts.Add(new FloatMenuOption("NoneBrackets".Translate(), null));
			}
			return opts;
		}
	}

	public override void TabUpdate()
	{
		if (mouseoverBill != null)
		{
			mouseoverBill.TryDrawIngredientSearchRadiusOnMap(SelTable.Position);
			mouseoverBill = null;
		}
	}
}
