using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Command_Ritual : Command
{
	private Precept_Ritual ritual;

	private RitualObligation obligation;

	private TargetInfo targetInfo;

	private Dictionary<string, Pawn> forcedForRole;

	private readonly IntVec2 PenaltyIconSize = new IntVec2(16, 16);

	private static readonly Texture2D CooldownBarTex = SolidColorMaterials.NewSolidColorTexture(Color32.op_Implicit(new Color32((byte)170, (byte)150, (byte)0, (byte)60)));

	private static Texture2D penaltyArrowTex;

	private static Texture2D PenaltyArrowTex
	{
		get
		{
			if ((Object)(object)penaltyArrowTex == (Object)null)
			{
				penaltyArrowTex = ContentFinder<Texture2D>.Get("UI/Icons/Rituals/QualityPenalty");
			}
			return penaltyArrowTex;
		}
	}

	public override string Desc => ritual.TipLabel.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + ritual.TipMainPart();

	public override string DescPostfix => ritual.TipExtraPart();

	public Command_Ritual(Precept_Ritual ritual, TargetInfo targetInfo, RitualObligation forObligation = null, Dictionary<string, Pawn> forcedForRole = null)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		this.ritual = ritual;
		this.targetInfo = targetInfo;
		this.forcedForRole = forcedForRole;
		obligation = forObligation;
		defaultLabel = ritual.GetBeginRitualText(obligation);
		defaultDesc = ritual.def.description;
		foreach (RitualObligationTrigger obligationTrigger in ritual.obligationTriggers)
		{
			if (obligationTrigger.TriggerExtraDesc != null)
			{
				defaultDesc = defaultDesc + "\n\n" + obligationTrigger.TriggerExtraDesc;
			}
		}
		groupKey = (ritual.canMergeGizmosFromDifferentIdeos ? (-1) : ritual.ideo.id);
		icon = (Texture)(object)ritual.Icon;
		if (!ritual.def.mergeRitualGizmosFromAllIdeos && !ritual.def.iconIgnoresIdeoColor)
		{
			defaultIconColor = ritual.ideo.Color;
		}
		if (!disabled)
		{
			ValidateDisabledState();
		}
	}

	public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		base.DrawIcon(rect, buttonMat, parms);
		if (ritual.RepeatPenaltyActive)
		{
			float num = Mathf.InverseLerp(1200000f, 0f, (float)ritual.TicksSinceLastPerformed);
			Widgets.FillableBar(rect.ContractedBy(1f), Mathf.Clamp01(num), CooldownBarTex, null, doBorder: false);
			Text.Font = GameFont.Tiny;
			Text.Anchor = (TextAnchor)1;
			float num2 = (float)(1200000 - ritual.TicksSinceLastPerformed) / 60000f;
			Widgets.Label(label: "PeriodDays".Translate((!(num2 >= 1f)) ? ((float)(int)(num2 * 10f) / 10f) : ((float)Mathf.RoundToInt(num2))), rect: rect);
			Text.Anchor = (TextAnchor)0;
			GUI.DrawTexture(new Rect(((Rect)(ref rect)).xMax - (float)PenaltyIconSize.x, ((Rect)(ref rect)).yMin + 4f, (float)PenaltyIconSize.x, (float)PenaltyIconSize.z), (Texture)(object)PenaltyArrowTex);
		}
	}

	protected override GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!disabled)
		{
			ValidateDisabledState();
		}
		return base.GizmoOnGUIInt(butRect, parms);
	}

	private void ValidateDisabledState()
	{
		string str = ritual.behavior.CanStartRitualNow(targetInfo, ritual);
		if (!str.NullOrEmpty())
		{
			disabled = true;
			disabledReason = str;
		}
		else if (ritual.abilityOnCooldownUntilTick > Find.TickManager.TicksGame)
		{
			disabled = true;
			disabledReason = "AbilityOnCooldown".Translate((ritual.abilityOnCooldownUntilTick - Find.TickManager.TicksGame).ToStringTicksToPeriod()).Resolve();
		}
		else
		{
			if (ritual.def.sourcePawnRoleDef == null || ritual.def.sourceAbilityDef == null)
			{
				return;
			}
			Precept_Role precept_Role = ritual.ideo.RolesListForReading.FirstOrDefault((Precept_Role r) => r.def == ritual.def.sourcePawnRoleDef);
			if (precept_Role != null && precept_Role is Precept_RoleSingle precept_RoleSingle && precept_RoleSingle.ChosenPawnSingle() != null)
			{
				Ability ability = precept_RoleSingle.AbilitiesForReading.FirstOrDefault((Ability a) => a.def == ritual.def.sourceAbilityDef);
				if (ability != null)
				{
					disabled = ability.GizmoDisabled(out disabledReason);
				}
			}
		}
	}

	public override void GizmoUpdateOnMouseover()
	{
		base.GizmoUpdateOnMouseover();
		ritual.behavior?.DrawPreviewOnTarget(targetInfo);
	}

	public override void ProcessInput(Event ev)
	{
		base.ProcessInput(ev);
		ritual.ShowRitualBeginWindow(targetInfo, null, null, forcedForRole);
		SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
	}
}
